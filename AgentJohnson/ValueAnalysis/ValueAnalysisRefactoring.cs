using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  internal class ValueAnalysisRefactoring {
    #region Fields

    [CanBeNull]
    readonly ICSharpContextActionDataProvider _contextActionDataProvider;
    [NotNull]
    readonly ITypeMemberDeclaration _typeMemberDeclaration;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisRefactoring"/> class.
    /// </summary>
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    public ValueAnalysisRefactoring(ITypeMemberDeclaration typeMemberDeclaration) {
      _typeMemberDeclaration = typeMemberDeclaration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisRefactoring"/> class.
    /// </summary>
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    /// <param name="contextActionDataProvider">The context action data provider.</param>
    public ValueAnalysisRefactoring(ITypeMemberDeclaration typeMemberDeclaration, ICSharpContextActionDataProvider contextActionDataProvider) {
      _typeMemberDeclaration = typeMemberDeclaration;
      _contextActionDataProvider = contextActionDataProvider;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution {
      get {
        return TypeMemberDeclaration.GetManager().Solution;
      }
    }
    /// <summary>
    /// Gets the type member declaration.
    /// </summary>
    /// <value>The type member declaration.</value>
    public ITypeMemberDeclaration TypeMemberDeclaration {
      get {
        return _typeMemberDeclaration;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the specified element.
    /// </summary>
    public void Execute() {
      IPropertyDeclaration propertyDeclaration = TypeMemberDeclaration as IPropertyDeclaration;
      if(propertyDeclaration != null) {
        ExecuteProperty(propertyDeclaration);
        return;
      }

      IIndexerDeclaration indexerDeclaration = TypeMemberDeclaration as IIndexerDeclaration;
      if(indexerDeclaration != null) {
        ExecuteIndexer(indexerDeclaration);
        return;
      }

      IFunctionDeclaration functionDeclaration = TypeMemberDeclaration as IFunctionDeclaration;
      if(functionDeclaration != null) {
        ExecuteFunction(functionDeclaration);
        return;
      }
    }

    /// <summary>
    /// Finds the attribute.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns>The attribute.</returns>
    public IAttributeInstance FindAttribute([CanBeNull] string attributeName) {
      IMetaInfoTargetDeclaration metaInfoTargetDeclaration = TypeMemberDeclaration as IMetaInfoTargetDeclaration;
      if(metaInfoTargetDeclaration == null) {
        return null;
      }

      return FindAttribute(attributeName, metaInfoTargetDeclaration);
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAvailable() {
      IPropertyDeclaration propertyDeclaration = TypeMemberDeclaration as IPropertyDeclaration;
      if(propertyDeclaration != null) {
        return IsAvailableProperty(propertyDeclaration);
      }

      IIndexerDeclaration indexerDeclaration = TypeMemberDeclaration as IIndexerDeclaration;
      if(indexerDeclaration != null) {
        return IsAvailableIndexer(indexerDeclaration);
      }

      IFunctionDeclaration functionDeclaration = TypeMemberDeclaration as IFunctionDeclaration;
      if(functionDeclaration != null) {
        return IsAvailableFunction(functionDeclaration, functionDeclaration);
      }

      return false;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Executes the attribute.
    /// </summary>
    void ExecuteAttribute() {
      string notNullableValueAttribute = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.NotNullAttributeShortName);
      string canBeNullValueAttribute = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.CanBeNullAttributeShortName);
      if(string.IsNullOrEmpty(notNullableValueAttribute) && string.IsNullOrEmpty(canBeNullValueAttribute)) {
        return;
      }

      IAttributeInstance notNull = FindAttribute(notNullableValueAttribute);
      IAttributeInstance canBeNull = FindAttribute(canBeNullValueAttribute);
      if(notNull != null || canBeNull != null) {
        return;
      }

      ICSharpControlFlowGraf controlFlowGraf = GetControlFlow();

      if(controlFlowGraf == null) {
        return;
      }

      CSharpControlFlowNullReferenceState attribute = controlFlowGraf.SuggestReturnValueAnnotationAttribute;

      switch(attribute) {
        case CSharpControlFlowNullReferenceState.UNKNOWN:
          break;
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          MarkWithAttribute(notNullableValueAttribute);
          break;
        case CSharpControlFlowNullReferenceState.NULL:
          MarkWithAttribute(canBeNullValueAttribute);
          break;
        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          MarkWithAttribute(canBeNullValueAttribute);
          break;
      }
    }

    /// <summary>
    /// Executes the parameters.
    /// </summary>
    void ExecuteFunction(IFunctionDeclaration functionDeclaration) {
      if(IsAvailableGetterFunction(functionDeclaration)) {
        ExecuteAttribute();
      }

      List<ParameterStatement> assertions = GetAssertions(functionDeclaration, true);
      if(assertions.Count == 0) {
        return;
      }

      CodeFormatter codeFormatter = GetCodeFormatter();
      if(codeFormatter == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(functionDeclaration.GetProject());

      IBlock body = functionDeclaration.Body;

      AccessRights accessRights = functionDeclaration.GetAccessRights();

      foreach(ParameterStatement assertion in assertions) {
        if(assertion.Nullable) {
          continue;
        }

        if(!assertion.NeedsStatement) {
          continue;
        }

        if(assertion.Statement != null) {
          body.RemoveStatement(assertion.Statement);

          assertion.Statement = factory.CreateStatement(assertion.Statement.GetText());

          continue;
        }

        string code = GetCode(assertion, accessRights);
        if(string.IsNullOrEmpty(code)) {
          continue;
        }

        code = string.Format(code, assertion.Parameter.ShortName);

        assertion.Statement = factory.CreateStatement(code);
      }

      IStatement anchor = GetAnchor(body);
      bool hasAsserts = false;

      foreach(ParameterStatement assertion in assertions) {
        if(assertion.Nullable) {
          continue;
        }

        MarkParameterWithAttribute(assertion);

        if(assertion.Statement == null) {
          continue;
        }

        IStatement result = body.AddStatementBefore(assertion.Statement, anchor);

        DocumentRange range = result.GetDocumentRange();
        IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
        codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);

        hasAsserts = true;
      }

      if(anchor != null && hasAsserts) {
        InsertBlankLine(anchor, codeFormatter);
      }
    }

    /// <summary>
    /// Executes the indexer.
    /// </summary>
    /// <param name="indexerDeclaration">The property declaration.</param>
    void ExecuteIndexer(IIndexerDeclaration indexerDeclaration) {
      foreach(IAccessorDeclaration accessorDeclaration in indexerDeclaration.AccessorDeclarations) {
        if(accessorDeclaration.ToTreeNode().AccessorName == null) {
          continue;
        }

        string accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if(accessorName == "get") {
          if(IsAvailableGetterFunction(accessorDeclaration)) {
            ExecuteAttribute();
          }

          IParametersOwner parametersOwner = accessorDeclaration as IParametersOwner;

          if(parametersOwner != null && parametersOwner.Parameters.Count > 0) {
            ExecuteFunction(accessorDeclaration);
          }
        }
        else if(accessorName == "set") {
          ExecuteFunction(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Executes the property.
    /// </summary>
    /// <param name="propertyDeclaration">The declaration.</param>
    void ExecuteProperty(IPropertyDeclaration propertyDeclaration) {
      foreach(IAccessorDeclaration accessorDeclaration in propertyDeclaration.AccessorDeclarations) {
        if(accessorDeclaration.ToTreeNode().AccessorName == null) {
          continue;
        }

        string accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if(accessorName == "get") {
          if(IsAvailableGetterFunction(accessorDeclaration)) {
            ExecuteAttribute();
          }
        }
        else if(accessorName == "set") {
          ExecuteFunction(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Finds the attribute.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="metaInfoTargetDeclaration">The meta info target declaration.</param>
    /// <returns>The attribute.</returns>
    [CanBeNull]
    static IAttributeInstance FindAttribute(string attributeName, IMetaInfoTargetDeclaration metaInfoTargetDeclaration) {
      IAttributesOwner attributesOwner = metaInfoTargetDeclaration as IAttributesOwner;
      if(attributesOwner == null) {
        return null;
      }

      CLRTypeName typeName = new CLRTypeName(attributeName);
      IList<IAttributeInstance> instances = attributesOwner.GetAttributeInstances(typeName, true);

      if(instances != null && instances.Count > 0) {
        return instances[0];
      }

      return null;
    }

    /// <summary>
    /// Finds the attributes.
    /// </summary>
    /// <param name="parameterStatement">The parameter statement.</param>
    void FindAttributes(ParameterStatement parameterStatement) {
      string notNullableValueAttributeTypeName = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.NotNullAttributeShortName);
      if(string.IsNullOrEmpty(notNullableValueAttributeTypeName)) {
        return;
      }

      CLRTypeName notNullableAttributeName = new CLRTypeName(notNullableValueAttributeTypeName);

      IList<IAttributeInstance> instances = parameterStatement.Parameter.GetAttributeInstances(notNullableAttributeName, true);
      if(instances != null && instances.Count > 0) {
        parameterStatement.Nullable = false;
        parameterStatement.AttributeInstance = instances[0];
        return;
      }

      string canBeNullValueAttributeTypeName = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.CanBeNullAttributeShortName);
      if(string.IsNullOrEmpty(canBeNullValueAttributeTypeName)) {
        return;
      }

      CLRTypeName canBeNullAttributeName = new CLRTypeName(canBeNullValueAttributeTypeName);
      instances = parameterStatement.Parameter.GetAttributeInstances(canBeNullAttributeName, true);
      if(instances != null && instances.Count > 0) {
        parameterStatement.Nullable = true;
        parameterStatement.AttributeInstance = instances[0];
        return;
      }
    }

    /// <summary>
    /// Gets the anchor.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <returns>The anchor.</returns>
    [CanBeNull]
    static IStatement GetAnchor(IBlock body) {
      IList<IStatement> list = body.Statements;

      if(list != null && list.Count > 0) {
        return list[0];
      }
      return null;
    }

    /// <summary>
    /// Gets the assertion parameters.
    /// </summary>
    /// <param name="functionDeclaration">The function declaration.</param>
    /// <param name="result">The result.</param>
    void GetAssertionParameters(IFunctionDeclaration functionDeclaration, List<ParameterStatement> result) {
      IParametersOwner parametersOwner = functionDeclaration as IParametersOwner;
      if(parametersOwner == null || parametersOwner.Parameters.Count <= 0) {
        return;
      }

      IAttributesOwner attributesOwner;
      if(functionDeclaration is IAccessorDeclaration) {
        attributesOwner = functionDeclaration.GetContainingTypeMemberDeclaration() as IAttributesOwner;
      }
      else {
        attributesOwner = parametersOwner as IAttributesOwner;
      }

      if(attributesOwner == null) {
        return;
      }

      List<string> allowNullParameters = new List<string>();

      string allowNullAttribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

      if(!string.IsNullOrEmpty(allowNullAttribute)) {
        CLRTypeName typeName = new CLRTypeName(allowNullAttribute);

        IList<IAttributeInstance> instances = attributesOwner.GetAttributeInstances(typeName, true);

        if(instances != null && instances.Count > 0) {
          foreach(IAttributeInstance instance in instances) {
            ConstantValue2 positionParameter = instance.PositionParameter(0).ConstantValue;
            if(!positionParameter.IsString()) {
              continue;
            }

            string name = positionParameter.Value as string;

            if(name == "*") {
              return;
            }

            if(!string.IsNullOrEmpty(name)) {
              allowNullParameters.Add(name);
            }
          }
        }
      }

      AccessRights accessRights = functionDeclaration.GetAccessRights();

      foreach(IParameter parameter in parametersOwner.Parameters) {
        if(parameter == null) {
          continue;
        }

        if(allowNullParameters.Contains(parameter.ShortName)) {
          continue;
        }

        if(!parameter.Type.IsReferenceType()) {
          continue;
        }

        ParameterStatement parameterStatement = new ParameterStatement();

        parameterStatement.Parameter = parameter;

        FindAttributes(parameterStatement);

        if(parameter.Kind == ParameterKind.OUTPUT) {
          parameterStatement.NeedsStatement = false;
        }
        else {
          string code = GetCode(parameterStatement, accessRights);

          if(string.IsNullOrEmpty(code)) {
            parameterStatement.NeedsStatement = false;
          }
        }

        result.Add(parameterStatement);
      }
    }

    /// <summary>
    /// Gets the assertions.
    /// </summary>
    /// <returns></returns>
    List<ParameterStatement> GetAssertions(IFunctionDeclaration functionDeclaration, bool findStatements) {
      List<ParameterStatement> result = new List<ParameterStatement>();

      GetAssertionParameters(functionDeclaration, result);
      if(findStatements) {
        GetAssertionStatements(functionDeclaration, result);
      }

      return result;
    }

    /// <summary>
    /// Gets the assertion statements.
    /// </summary>
    /// <param name="functionDeclaration">The function declaration.</param>
    /// <param name="result">The result.</param>
    void GetAssertionStatements(IFunctionDeclaration functionDeclaration, List<ParameterStatement> result) {
      IParametersOwner parametersOwner = functionDeclaration as IParametersOwner;
      if(parametersOwner == null || parametersOwner.Parameters.Count <= 0) {
        return;
      }

      CodeAnnotationsCache codeAnnotationsCache = CodeAnnotationsCache.GetInstance(Solution);

      foreach(IStatement statement in functionDeclaration.Body.Statements) {
        IExpressionStatement expressionStatement = statement as IExpressionStatement;
        if(expressionStatement == null) {
          continue;
        }

        IInvocationExpression invocationExpression = expressionStatement.Expression as IInvocationExpression;
        if(invocationExpression == null) {
          continue;
        }

        IReferenceExpression reference = invocationExpression.InvokedExpression as IReferenceExpression;
        if(reference == null) {
          continue;
        }

        ResolveResult resolveResult = reference.Reference.Resolve();

        IMethod method = null;

        IMethodDeclaration methodDeclaration = resolveResult.DeclaredElement as IMethodDeclaration;
        if(methodDeclaration != null) {
          method = methodDeclaration as IMethod;
        }

        if(method == null) {
          method = resolveResult.DeclaredElement as IMethod;
        }

        if(method == null) {
          continue;
        }

        if(!codeAnnotationsCache.IsAssertionMethod(method)) {
          continue;
        }

        int parameterIndex = -1;

        for(int index = 0; index < method.Parameters.Count; index++) {
          IParameter parameter = method.Parameters[index];

          AssertionConditionType? assertionConditionType = codeAnnotationsCache.GetParameterAssertionCondition(parameter);
          if(assertionConditionType == null) {
            continue;
          }

          parameterIndex = index;

          break;
        }

        if(parameterIndex < 0) {
          continue;
        }

        IArgumentsOwner argumentsOwner = invocationExpression;
        if(argumentsOwner.Arguments.Count <= parameterIndex) {
          continue;
        }

        IArgument argument = argumentsOwner.Arguments[parameterIndex];
        string argumentText = argument.GetText();

        if(argumentText.StartsWith("@")) {
          argumentText = argumentText.Substring(1);
        }

        foreach(ParameterStatement parameterStatement in result) {
          if(parameterStatement.Parameter.ShortName == argumentText) {
            parameterStatement.Statement = statement;
          }
        }
      }
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    ITypeElement GetAttribute(string attributeName) {
      DeclarationsCacheScope declarationsCacheScope = DeclarationsCacheScope.SolutionScope(Solution, true);

      IDeclarationsCache declarationsCache = PsiManager.GetInstance(Solution).GetDeclarationsCache(declarationsCacheScope, true);

      ITypeElement typeElement = declarationsCache[attributeName] as ITypeElement;

      if(typeElement == null) {
        return null;
      }

      /*
      PredefinedType predefinedType = new PredefinedType(Solution.SolutionProject);

      if(!TypeFactory.CreateType(typeElement).IsSubtypeOf(predefinedType.Attribute)) {
        return null;
      }
      */

      return typeElement;
    }

    /// <summary>
    /// Gets the code.
    /// </summary>
    /// <param name="assertion">The assertion.</param>
    /// <param name="accessRights">The access rights.</param>
    /// <returns>The code.</returns>
    static string GetCode(ParameterStatement assertion, AccessRights accessRights) {
      Rule rule = Rule.GetRule(assertion.Parameter.Type);

      if(rule == null) {
        rule = Rule.GetDefaultRule();
      }

      if(rule == null) {
        return null;
      }

      string code = accessRights == AccessRights.PUBLIC ? rule.PublicParameterAssertion : rule.NonPublicParameterAssertion;

      if(string.IsNullOrEmpty(code)) {
        rule = Rule.GetDefaultRule();

        if(rule != null) {
          code = accessRights == AccessRights.PUBLIC ? rule.PublicParameterAssertion : rule.NonPublicParameterAssertion;
        }
      }

      return string.IsNullOrEmpty(code) ? null : code;
    }

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns>The code formatter.</returns>
    [CanBeNull]
    static CodeFormatter GetCodeFormatter() {
      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null) {
        return null;
      }

      return languageService.CodeFormatter;
    }

    /// <summary>
    /// Gets the control flow.
    /// </summary>
    /// <returns></returns>
    ICSharpControlFlowGraf GetControlFlow() {
      if(_contextActionDataProvider == null) {
        return null;
      }

      ICSharpControlFlowGraf graf = _contextActionDataProvider.GetControlFlowGraf();
      if(graf == null) {
        return null;
      }

      try {
        if (_contextActionDataProvider.InspectControlFlowGraf()) {
          return null;
        }
      }
      catch {
        return null;
      }

      return graf;
    }

    /// <summary>
    /// Inserts the blank line.
    /// </summary>
    /// <param name="anchor">The anchor.</param>
    /// <param name="formatter">The formatter.</param>
    static void InsertBlankLine(IStatement anchor, CodeFormatter formatter) {
      IStatementNode anchorTreeNode = anchor.ToTreeNode();
      if(anchorTreeNode == null) {
        return;
      }

      StringBuffer newLineText;

      IWhitespaceNode whitespace = anchor.ToTreeNode().PrevSibling as IWhitespaceNode;
      if(whitespace != null) {
        newLineText = new StringBuffer("\r\n" + whitespace.GetText());
      }
      else {
        newLineText = new StringBuffer("\r\n");
      }

      LeafElement element = TreeElementFactory.CreateLeafElement(CSharpTokenType.NEW_LINE, newLineText, 0, newLineText.Length);

      LowLevelModificationUtil.AddChildBefore(anchorTreeNode, element);

      DocumentRange range = element.GetDocumentRange();
      IPsiRangeMarker marker = element.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(element.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Determines whether [is available internal].
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if [is available internal]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableAttribute() {
      string notNullableValueAttribute = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.NotNullAttributeShortName);
      string canBeNullValueAttribute = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(CodeAnnotationsCache.CanBeNullAttributeShortName);

      if(string.IsNullOrEmpty(notNullableValueAttribute) && string.IsNullOrEmpty(canBeNullValueAttribute)) {
        return false;
      }

      IAttributeInstance notNull = FindAttribute(notNullableValueAttribute);
      IAttributeInstance canBeNull = FindAttribute(canBeNullValueAttribute);

      if(notNull == null && canBeNull == null) {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Determines whether [is available function] [the specified declaration].
    /// </summary>
    /// <param name="getterFunctionDeclaration">The function declaration.</param>
    /// <param name="setterFunctionDeclaration">The setter function declaration.</param>
    /// <returns>
    /// 	<c>true</c> if [is available function] [the specified declaration]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableFunction(IFunctionDeclaration getterFunctionDeclaration, IFunctionDeclaration setterFunctionDeclaration) {
      if(getterFunctionDeclaration != null) {
        bool result = IsAvailableGetterFunction(getterFunctionDeclaration);
        if(result) {
          return true;
        }
      }

      if(setterFunctionDeclaration != null) {
        bool result = IsAvailableSetterFunction(setterFunctionDeclaration);
        if(result) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether [is available getter function] [the specified getter function declaration].
    /// </summary>
    /// <param name="getterFunctionDeclaration">The getter function declaration.</param>
    /// <returns>
    /// 	<c>true</c> if [is available getter function] [the specified getter function declaration]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableGetterFunction(IFunctionDeclaration getterFunctionDeclaration) {
      IMethod method = getterFunctionDeclaration as IMethod;
      if(method == null) {
        return false;
      }

      IType returnType = method.ReturnType;

      if(!returnType.IsReferenceType()) {
        return false;
      }

      return IsAvailableAttribute();
    }

    /// <summary>
    /// Determines whether [is available indexer] [the specified indexer declaration].
    /// </summary>
    /// <param name="indexerDeclaration">The indexer declaration.</param>
    /// <returns>
    /// 	<c>true</c> if [is available indexer] [the specified indexer declaration]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableIndexer(IIndexerDeclaration indexerDeclaration) {
      IFunctionDeclaration getterFunctionDeclaration = null;
      IFunctionDeclaration setterFunctionDeclaration = null;

      foreach(IAccessorDeclaration accessorDeclaration in indexerDeclaration.AccessorDeclarations) {
        if(accessorDeclaration.ToTreeNode().AccessorName == null) {
          continue;
        }

        string accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if(accessorName == "get") {
          getterFunctionDeclaration = accessorDeclaration;
        }
        else if(accessorName == "set") {
          setterFunctionDeclaration = accessorDeclaration;
        }
      }

      return IsAvailableFunction(getterFunctionDeclaration, setterFunctionDeclaration);
    }

    /// <summary>
    /// Determines whether [is available property] [the specified property declaration].
    /// </summary>
    /// <param name="propertyDeclaration">The property declaration.</param>
    /// <returns>
    /// 	<c>true</c> if [is available property] [the specified property declaration]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableProperty(IPropertyDeclaration propertyDeclaration) {
      IFunctionDeclaration getterFunctionDeclaration = null;
      IFunctionDeclaration setterFunctionDeclaration = null;

      foreach(IAccessorDeclaration accessorDeclaration in propertyDeclaration.AccessorDeclarations) {
        if(accessorDeclaration.ToTreeNode().AccessorName == null) {
          continue;
        }

        if (accessorDeclaration.Body == null) {
          continue;
        }

        string accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        switch(accessorName) {
          case "get":
            getterFunctionDeclaration = accessorDeclaration;
            break;
          case "set":
            setterFunctionDeclaration = accessorDeclaration;
            break;
        }
      }

      return IsAvailableFunction(getterFunctionDeclaration, setterFunctionDeclaration);
    }

    /// <summary>
    /// Determines whether [is available parameters] [the specified function declaration].
    /// </summary>
    /// <param name="functionDeclaration">The function declaration.</param>
    /// <returns>
    /// 	<c>true</c> if [is available parameters] [the specified function declaration]; otherwise, <c>false</c>.
    /// </returns>
    bool IsAvailableSetterFunction(IFunctionDeclaration functionDeclaration) {
      if(functionDeclaration.IsAbstract || functionDeclaration.IsExtern) {
        return false;
      }

      if(functionDeclaration.Body == null) {
        return false;
      }

      List<ParameterStatement> assertions = GetAssertions(functionDeclaration, true);

      foreach(ParameterStatement parameterStatement in assertions) {
        if(parameterStatement.Nullable) {
          continue;
        }

        if(parameterStatement.Statement == null && parameterStatement.NeedsStatement) {
          return true;
        }

        if(!parameterStatement.Parameter.IsValueVariable && parameterStatement.AttributeInstance == null) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Marks the parameter with attribute.
    /// </summary>
    /// <param name="assertion">The assertion.</param>
    void MarkParameterWithAttribute(ParameterStatement assertion) {
      IRegularParameterDeclaration regularParameterDeclaration = assertion.Parameter as IRegularParameterDeclaration;
      if(regularParameterDeclaration == null) {
        return;
      }

      Rule rule = Rule.GetRule(assertion.Parameter.Type);
      if (rule != null && !rule.NotNull && !rule.CanBeNull) {
        rule = null;
      }

      if(rule == null) {
        rule = Rule.GetDefaultRule();
      }

      if(rule == null) {
        return;
      }

      string name = null;

      if(rule.NotNull) {
        name = CodeAnnotationsCache.NotNullAttributeShortName;
      }
      else if(rule.CanBeNull) {
        name = CodeAnnotationsCache.CanBeNullAttributeShortName;
      }

      if(string.IsNullOrEmpty(name)) {
        return;
      }

      string valueAttribute = CodeAnnotationsCache.GetInstance(Solution).GetDefaultAnnotationAttribute(name);
      if(string.IsNullOrEmpty(valueAttribute)) {
        return;
      }

      IAttributeInstance attributeInstance = FindAttribute(valueAttribute, regularParameterDeclaration);
      if(attributeInstance != null) {
        return;
      }

      MarkWithAttribute(valueAttribute, regularParameterDeclaration);
    }

    /// <summary>
    /// Marks the with attribute.
    /// </summary>
    void MarkWithAttribute(string attributeName) {
      IMetaInfoTargetDeclaration metaInfoTargetDeclaration = TypeMemberDeclaration as IMetaInfoTargetDeclaration;
      if(metaInfoTargetDeclaration == null) {
        return;
      }

      MarkWithAttribute(attributeName, metaInfoTargetDeclaration);
    }

    /// <summary>
    /// Marks the with attribute.
    /// </summary>
    /// <param name="metaInfoTargetDeclaration">The meta info target declaration.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    void MarkWithAttribute(string attributeName, IMetaInfoTargetDeclaration metaInfoTargetDeclaration) {
      ITypeElement typeElement = GetAttribute(attributeName);
      if(typeElement == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(metaInfoTargetDeclaration.GetProject());

      IAttribute attribute = factory.CreateTypeMemberDeclaration("[$0]void Foo(){}", new object[] {typeElement}).Attributes[0];

      attribute = metaInfoTargetDeclaration.AddAttributeAfter(attribute, null);

      string name = attribute.TypeReference.GetName();
      if(!name.EndsWith("Attribute")) {
        return;
      }

      IReferenceName referenceName = factory.CreateReferenceName(name.Substring(0, name.Length - "Attribute".Length), new object[0]);
      referenceName = attribute.Name.ReplaceBy(referenceName);

      IDeclaredElement declaredElement = referenceName.Reference.Resolve().DeclaredElement;
      if(declaredElement == null) {
        return;
      }

      if(declaredElement.Equals(typeElement)) {
        referenceName.Reference.BindTo(typeElement);
      }
    }

    #endregion
  }
}