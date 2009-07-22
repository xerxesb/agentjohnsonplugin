// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisRefactoring.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the value analysis refactoring class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using JetBrains.Application.Progress;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Parsing;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.ExtensionsAPI;
  using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.Text;

  /// <summary>
  /// Defines the value analysis refactoring class.
  /// </summary>
  internal class ValueAnalysisRefactoring
  {
    #region Constants and Fields

    /// <summary>
    /// The context action data provider.
    /// </summary>
    [NotNull]
    private readonly ICSharpContextActionDataProvider contextActionDataProvider;

    /// <summary>
    /// The type member declaration.
    /// </summary>
    [NotNull]
    private readonly ITypeMemberDeclaration typeMemberDeclaration;

    /// <summary>
    /// The can be null attribute clr name.
    /// </summary>
    private CLRTypeName canBeNullAttributeCLRName;

    /// <summary>
    /// The can be null type element.
    /// </summary>
    private ITypeElement canBeNullTypeElement;

    /// <summary>
    /// The not nullable attribute clr name.
    /// </summary>
    private CLRTypeName notNullableAttributeCLRName;

    /// <summary>
    /// The not null type element.
    /// </summary>
    private ITypeElement notNullTypeElement;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisRefactoring"/> class.
    /// </summary>
    /// <param name="typeMemberDeclaration">
    /// The type member declaration.
    /// </param>
    /// <param name="contextActionDataProvider">
    /// The context action data provider.
    /// </param>
    public ValueAnalysisRefactoring([NotNull] ITypeMemberDeclaration typeMemberDeclaration, [NotNull] ICSharpContextActionDataProvider contextActionDataProvider)
    {
      this.typeMemberDeclaration = typeMemberDeclaration;
      this.contextActionDataProvider = contextActionDataProvider;

      var codeAnnotationsCache = CodeAnnotationsCache.GetInstance(this.Solution);

      this.notNullTypeElement = codeAnnotationsCache.GetAttributeTypeForElement(this.TypeMemberDeclaration, CodeAnnotationsCache.NotNullAttributeShortName);
      this.canBeNullTypeElement = codeAnnotationsCache.GetAttributeTypeForElement(this.TypeMemberDeclaration, CodeAnnotationsCache.CanBeNullAttributeShortName);

      if (this.notNullTypeElement == null || this.canBeNullTypeElement == null)
      {
        return;
      }

      this.notNullableAttributeCLRName = new CLRTypeName(this.notNullTypeElement.CLRName);
      this.canBeNullAttributeCLRName = new CLRTypeName(this.canBeNullTypeElement.CLRName);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    [NotNull]
    private ISolution Solution
    {
      get
      {
        return this.TypeMemberDeclaration.GetManager().Solution;
      }
    }

    /// <summary>
    /// Gets the type member declaration.
    /// </summary>
    /// <value>The type member declaration.</value>
    [NotNull]
    private ITypeMemberDeclaration TypeMemberDeclaration
    {
      get
      {
        return this.typeMemberDeclaration;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Executes the specified element.
    /// </summary>
    public void Execute()
    {
      if (!this.IsAttributesDefined())
      {
        return;
      }

      var propertyDeclaration = this.TypeMemberDeclaration as IPropertyDeclaration;
      if (propertyDeclaration != null)
      {
        this.ExecuteProperty(propertyDeclaration);
        return;
      }

      var indexerDeclaration = this.TypeMemberDeclaration as IIndexerDeclaration;
      if (indexerDeclaration != null)
      {
        this.ExecuteIndexer(indexerDeclaration);
        return;
      }

      var functionDeclaration = this.TypeMemberDeclaration as ICSharpFunctionDeclaration;
      if (functionDeclaration != null)
      {
        this.ExecuteFunction(functionDeclaration);
        return;
      }
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAvailable()
    {
      if (!this.IsAttributesDefined())
      {
        return false;
      }

      var propertyDeclaration = this.TypeMemberDeclaration as IPropertyDeclaration;
      if (propertyDeclaration != null)
      {
        return this.IsAvailableProperty(propertyDeclaration);
      }

      var indexerDeclaration = this.TypeMemberDeclaration as IIndexerDeclaration;
      if (indexerDeclaration != null)
      {
        return this.IsAvailableIndexer(indexerDeclaration);
      }

      var functionDeclaration = this.TypeMemberDeclaration as ICSharpFunctionDeclaration;
      if (functionDeclaration != null)
      {
        return this.IsAvailableFunction(functionDeclaration, functionDeclaration);
      }

      return false;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Finds the attribute.
    /// </summary>
    /// <param name="attributeName">
    /// Name of the attribute.
    /// </param>
    /// <param name="attributesOwner">
    /// The attributes owner.
    /// </param>
    /// <returns>
    /// The attribute.
    /// </returns>
    [CanBeNull]
    private static IAttributeInstance FindAttribute([NotNull] string attributeName, [NotNull] IAttributesOwner attributesOwner)
    {
      var typeName = new CLRTypeName(attributeName);
      var instances = attributesOwner.GetAttributeInstances(typeName, true);

      if (instances != null && instances.Count > 0)
      {
        return instances[0];
      }

      return null;
    }

    /// <summary>
    /// Gets the access rights.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    /// <returns>
    /// The access rights.
    /// </returns>
    private static AccessRights GetAccessRights([NotNull] ICSharpFunctionDeclaration functionDeclaration)
    {
      var accessRights = functionDeclaration.GetAccessRights();
      if (accessRights != AccessRights.PUBLIC)
      {
        return accessRights;
      }

      var containingTypeDeclaration = functionDeclaration.GetContainingTypeDeclaration();
      if (containingTypeDeclaration == null)
      {
        return accessRights;
      }

      return containingTypeDeclaration.GetAccessRights();
    }

    /// <summary>
    /// Gets the anchor.
    /// </summary>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <returns>
    /// The anchor.
    /// </returns>
    [CanBeNull]
    private static IStatement GetAnchor([NotNull] IBlock body)
    {
      var list = body.Statements;

      if (list != null && list.Count > 0)
      {
        return list[0];
      }

      return null;
    }

    /// <summary>
    /// Gets the code.
    /// </summary>
    /// <param name="assertion">
    /// The assertion.
    /// </param>
    /// <param name="accessRights">
    /// The access rights.
    /// </param>
    /// <returns>
    /// The code.
    /// </returns>
    private static string GetCode([NotNull] ParameterStatement assertion, AccessRights accessRights)
    {
      var rule = Rule.GetRule(assertion.Parameter.Type, assertion.Parameter.Language);

      if (rule == null)
      {
        rule = Rule.GetDefaultRule();
      }

      if (rule == null)
      {
        return null;
      }

      var code = accessRights == AccessRights.PUBLIC ? rule.PublicParameterAssertion : rule.NonPublicParameterAssertion;

      if (string.IsNullOrEmpty(code))
      {
        rule = Rule.GetDefaultRule();

        if (rule != null)
        {
          code = accessRights == AccessRights.PUBLIC ? rule.PublicParameterAssertion : rule.NonPublicParameterAssertion;
        }
      }

      return string.IsNullOrEmpty(code) ? null : code;
    }

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns>
    /// The code formatter.
    /// </returns>
    [CanBeNull]
    private static CodeFormatter GetCodeFormatter()
    {
      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return null;
      }

      return languageService.CodeFormatter;
    }

    /// <summary>
    /// Inserts the blank line.
    /// </summary>
    /// <param name="anchor">
    /// The anchor.
    /// </param>
    /// <param name="formatter">
    /// The formatter.
    /// </param>
    private static void InsertBlankLine([NotNull] IStatement anchor, [NotNull] CodeFormatter formatter)
    {
      var anchorTreeNode = anchor.ToTreeNode();
      if (anchorTreeNode == null)
      {
        return;
      }

      StringBuffer newLineText;

      var whitespace = anchor.ToTreeNode().PrevSibling as IWhitespaceNode;
      if (whitespace != null)
      {
        newLineText = new StringBuffer("\r\n" + whitespace.GetText());
      }
      else
      {
        newLineText = new StringBuffer("\r\n");
      }

      ITreeNode element = TreeElementFactory.CreateLeafElement(CSharpTokenType.NEW_LINE, newLineText, 0, newLineText.Length);

      LowLevelModificationUtil.AddChildBefore(anchorTreeNode, element);

      var range = element.GetDocumentRange();
      var marker = element.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(element.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Executes the parameters.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    private void ExecuteFunction([NotNull] ICSharpFunctionDeclaration functionDeclaration)
    {
      if (this.IsAvailableGetterFunction(functionDeclaration))
      {
        this.ExecuteReturnValueAttribute(functionDeclaration);
      }

      var assertions = this.GetAssertions(functionDeclaration, true);
      if (assertions.Count == 0)
      {
        return;
      }

      var codeFormatter = GetCodeFormatter();
      if (codeFormatter == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(functionDeclaration.GetPsiModule());
      var body = functionDeclaration.Body;

      var accessRights = GetAccessRights(functionDeclaration);

      foreach (var assertion in assertions)
      {
        if (assertion.Nullable)
        {
          continue;
        }

        if (!assertion.NeedsStatement)
        {
          continue;
        }

        if (assertion.Statement != null)
        {
          body.RemoveStatement(assertion.Statement);

          assertion.Statement = factory.CreateStatement(assertion.Statement.GetText());

          continue;
        }

        var code = GetCode(assertion, accessRights);
        if (string.IsNullOrEmpty(code))
        {
          continue;
        }

        code = string.Format(code, assertion.Parameter.ShortName);

        assertion.Statement = factory.CreateStatement(code);
      }

      IStatement anchor = null;
      if (body != null)
      {
        anchor = GetAnchor(body);
      }

      var hasAsserts = false;
      foreach (var assertion in assertions)
      {
        if (assertion.Nullable)
        {
          continue;
        }

        this.MarkParameterWithAttribute(assertion);

        if (body == null || assertion.Statement == null)
        {
          continue;
        }

        var result = body.AddStatementBefore(assertion.Statement, anchor);

        var range = result.GetDocumentRange();
        var marker = result.GetManager().CreatePsiRangeMarker(range);
        codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);

        hasAsserts = true;
      }

      if (anchor != null && hasAsserts)
      {
        InsertBlankLine(anchor, codeFormatter);
      }
    }

    /// <summary>
    /// Executes the indexer.
    /// </summary>
    /// <param name="indexerDeclaration">
    /// The property declaration.
    /// </param>
    private void ExecuteIndexer([NotNull] IIndexerDeclaration indexerDeclaration)
    {
      foreach (var accessorDeclaration in indexerDeclaration.AccessorDeclarations)
      {
        if (accessorDeclaration.ToTreeNode().AccessorName == null)
        {
          continue;
        }

        var accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if (accessorName == "get")
        {
          if (this.IsAvailableGetterFunction(accessorDeclaration))
          {
            this.ExecuteReturnValueAttribute(accessorDeclaration);
          }

          var parametersOwner = accessorDeclaration.DeclaredElement as IParametersOwner;

          if (parametersOwner != null && parametersOwner.Parameters.Count > 0)
          {
            this.ExecuteFunction(accessorDeclaration);
          }
        }
        else if (accessorName == "set")
        {
          this.ExecuteFunction(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Executes the property.
    /// </summary>
    /// <param name="propertyDeclaration">
    /// The declaration.
    /// </param>
    private void ExecuteProperty([NotNull] IPropertyDeclaration propertyDeclaration)
    {
      foreach (var accessorDeclaration in propertyDeclaration.AccessorDeclarations)
      {
        if (accessorDeclaration.ToTreeNode().AccessorName == null)
        {
          continue;
        }

        var accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if (accessorName == "get")
        {
          if (this.IsAvailableGetterFunction(accessorDeclaration))
          {
            this.ExecuteReturnValueAttribute(accessorDeclaration);
          }
        }
        else if (accessorName == "set")
        {
          this.ExecuteFunction(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Executes the attribute.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    private void ExecuteReturnValueAttribute([NotNull] ICSharpFunctionDeclaration functionDeclaration)
    {
      if (this.HasAnnotation())
      {
        return;
      }

      if (this.notNullTypeElement == null || this.canBeNullTypeElement == null)
      {
        return;
      }

      var graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      var inspect = graf.Inspect(true);

      var state = inspect.SuggestReturnValueAnnotationAttribute;

      switch (state)
      {
        case CSharpControlFlowNullReferenceState.UNKNOWN:
          break;
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          this.MarkWithAttribute(this.notNullTypeElement.CLRName);
          break;
        case CSharpControlFlowNullReferenceState.NULL:
          this.MarkWithAttribute(this.canBeNullTypeElement.CLRName);
          break;
        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          this.MarkWithAttribute(this.canBeNullTypeElement.CLRName);
          break;
      }
    }

    /// <summary>
    /// Finds the attributes.
    /// </summary>
    /// <param name="parameterStatement">
    /// The parameter statement.
    /// </param>
    private void FindAttributes([NotNull] ParameterStatement parameterStatement)
    {
      if (this.notNullTypeElement == null || this.canBeNullTypeElement == null)
      {
        return;
      }

      var instances = parameterStatement.Parameter.GetAttributeInstances(this.notNullableAttributeCLRName, false);
      if (instances != null && instances.Count > 0)
      {
        parameterStatement.Nullable = false;
        parameterStatement.AttributeInstance = instances[0];
        return;
      }

      instances = parameterStatement.Parameter.GetAttributeInstances(this.canBeNullAttributeCLRName, false);
      if (instances != null && instances.Count > 0)
      {
        parameterStatement.Nullable = true;
        parameterStatement.AttributeInstance = instances[0];
        return;
      }

      instances = parameterStatement.Parameter.GetAttributeInstances(this.notNullableAttributeCLRName, true);
      if (instances != null && instances.Count > 0)
      {
        parameterStatement.Nullable = false;
        parameterStatement.AttributeInstance = instances[0];
        return;
      }

      instances = parameterStatement.Parameter.GetAttributeInstances(this.canBeNullAttributeCLRName, true);
      if (instances != null && instances.Count > 0)
      {
        parameterStatement.Nullable = true;
        parameterStatement.AttributeInstance = instances[0];
      }
    }

    /// <summary>
    /// Gets the assertion parameters.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    /// <param name="result">
    /// The result.
    /// </param>
    private void GetAssertionParameters([NotNull] ICSharpFunctionDeclaration functionDeclaration, [NotNull] List<ParameterStatement> result)
    {
      IParametersOwner parametersOwner = functionDeclaration.DeclaredElement;
      if (parametersOwner == null || parametersOwner.Parameters.Count <= 0)
      {
        return;
      }

      IAttributesOwner attributesOwner;
      if (functionDeclaration is IAccessorDeclaration)
      {
        attributesOwner = functionDeclaration.GetContainingTypeMemberDeclaration().DeclaredElement;
      }
      else
      {
        attributesOwner = parametersOwner as IAttributesOwner;
      }

      if (attributesOwner == null)
      {
        return;
      }

      var allowNullParameters = new List<string>();

      var allowNullAttribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

      if (!string.IsNullOrEmpty(allowNullAttribute))
      {
        var typeName = new CLRTypeName(allowNullAttribute);

        var instances = attributesOwner.GetAttributeInstances(typeName, true);

        if (instances != null && instances.Count > 0)
        {
          foreach (var instance in instances)
          {
            var positionParameter = instance.PositionParameter(0).ConstantValue;
            if (!positionParameter.IsString())
            {
              continue;
            }

            var name = positionParameter.Value as string;

            if (name == "*")
            {
              return;
            }

            if (!string.IsNullOrEmpty(name))
            {
              allowNullParameters.Add(name);
            }
          }
        }
      }

      var accessRights = GetAccessRights(functionDeclaration);

      foreach (var parameter in parametersOwner.Parameters)
      {
        if (parameter == null)
        {
          continue;
        }

        if (allowNullParameters.Contains(parameter.ShortName))
        {
          continue;
        }

        if (!parameter.Type.IsReferenceType())
        {
          continue;
        }

        var parameterStatement = new ParameterStatement
        {
          Parameter = parameter
        };

        this.FindAttributes(parameterStatement);

        if (parameter.Kind == ParameterKind.OUTPUT)
        {
          parameterStatement.NeedsStatement = false;
        }
        else
        {
          var code = GetCode(parameterStatement, accessRights);

          if (string.IsNullOrEmpty(code))
          {
            parameterStatement.NeedsStatement = false;
          }
        }

        result.Add(parameterStatement);
      }
    }

    /// <summary>
    /// Gets the assertions.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    /// <param name="findStatements">
    /// if set to <c>true</c> [find statements].
    /// </param>
    /// <returns>
    /// The assertions.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <c>functionDeclaration</c> is null.
    /// </exception>
    [NotNull]
    private List<ParameterStatement> GetAssertions([NotNull] ICSharpFunctionDeclaration functionDeclaration, bool findStatements)
    {
      var result = new List<ParameterStatement>();

      this.GetAssertionParameters(functionDeclaration, result);

      if (findStatements)
      {
        this.GetAssertionStatements(functionDeclaration, result);
      }

      return result;
    }

    /// <summary>
    /// Gets the assertion statements.
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    /// <param name="result">
    /// The result.
    /// </param>
    private void GetAssertionStatements([NotNull] ICSharpFunctionDeclaration functionDeclaration, [NotNull] List<ParameterStatement> result)
    {
      var function = functionDeclaration.DeclaredElement;
      if (function == null)
      {
        return;
      }

      if (function.Parameters.Count <= 0)
      {
        return;
      }

      var codeAnnotationsCache = CodeAnnotationsCache.GetInstance(this.Solution);

      var body = functionDeclaration.Body;
      if (body == null)
      {
        foreach (var statement in result)
        {
          statement.NeedsStatement = false;
        }

        return;
      }

      var statements = body.Statements;
      if (statements == null || statements.Count == 0)
      {
        return;
      }

      foreach (var statement in statements)
      {
        var expressionStatement = statement as IExpressionStatement;
        if (expressionStatement == null)
        {
          continue;
        }

        var invocationExpression = expressionStatement.Expression as IInvocationExpression;
        if (invocationExpression == null)
        {
          continue;
        }

        var reference = invocationExpression.InvokedExpression as IReferenceExpression;
        if (reference == null)
        {
          continue;
        }

        var resolveResult = reference.Reference.Resolve();

        var method = resolveResult.DeclaredElement as IMethod;
        if (method == null)
        {
          continue;
        }

        if (!codeAnnotationsCache.IsAssertionMethod(method))
        {
          continue;
        }

        var parameterIndex = -1;

        for (var index = 0; index < method.Parameters.Count; index++)
        {
          var parameter = method.Parameters[index];

          var assertionConditionType = codeAnnotationsCache.GetParameterAssertionCondition(parameter);
          if (assertionConditionType == null)
          {
            continue;
          }

          parameterIndex = index;

          break;
        }

        if (parameterIndex < 0)
        {
          continue;
        }

        IArgumentsOwner argumentsOwner = invocationExpression;
        if (argumentsOwner.Arguments.Count <= parameterIndex)
        {
          continue;
        }

        var argument = argumentsOwner.Arguments[parameterIndex];
        var argumentText = argument.GetText();

        if (argumentText.StartsWith("@"))
        {
          argumentText = argumentText.Substring(1);
        }

        foreach (var parameterStatement in result)
        {
          if (parameterStatement.Parameter.ShortName == argumentText)
          {
            parameterStatement.Statement = statement;
          }
        }
      }
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="attributeName">
    /// Name of the attribute.
    /// </param>
    /// <returns>
    /// The attribute.
    /// </returns>
    [CanBeNull]
    private ITypeElement GetAttribute([NotNull] string attributeName)
    {
      var scope = DeclarationsScopeFactory.SolutionScope(this.Solution, true);
      var cache = PsiManager.GetInstance(this.Solution).GetDeclarationsCache(scope, true);

      var typeElement = cache.GetTypeElementByCLRName(attributeName);

      if (typeElement == null)
      {
        return null;
      }

      return typeElement;
    }

    /// <summary>
    /// Determines whether this instance has annotation.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this instance has annotation; otherwise, <c>false</c>.
    /// </returns>
    private bool HasAnnotation()
    {
      var attributes = this.TypeMemberDeclaration.DeclaredElement.GetAttributeInstances(true);

      var codeAnnotationsCache = CodeAnnotationsCache.GetInstance(this.Solution);

      foreach (var attribute in attributes)
      {
        if (codeAnnotationsCache.IsAnnotationAttribute(attribute))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether the value analysis attributes are defined.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the value analysis attributes are defined; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAttributesDefined()
    {
      return this.notNullTypeElement != null && this.canBeNullTypeElement != null;
    }

    /// <summary>
    /// Determines whether [is available function] [the specified declaration].
    /// </summary>
    /// <param name="getterFunctionDeclaration">
    /// The function declaration.
    /// </param>
    /// <param name="setterFunctionDeclaration">
    /// The setter function declaration.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is available function] [the specified declaration]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAvailableFunction([CanBeNull] ICSharpFunctionDeclaration getterFunctionDeclaration, [CanBeNull] ICSharpFunctionDeclaration setterFunctionDeclaration)
    {
      if (getterFunctionDeclaration != null)
      {
        var result = this.IsAvailableGetterFunction(getterFunctionDeclaration);
        if (result)
        {
          return true;
        }
      }

      if (setterFunctionDeclaration != null)
      {
        var result = this.IsAvailableSetterFunction(setterFunctionDeclaration);
        if (result)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether [is available getter function] [the specified getter function declaration].
    /// </summary>
    /// <param name="getterFunctionDeclaration">
    /// The getter function declaration.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is available getter function] [the specified getter function declaration]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAvailableGetterFunction([NotNull] IFunctionDeclaration getterFunctionDeclaration)
    {
      var method = getterFunctionDeclaration.DeclaredElement as IMethod;
      if (method == null)
      {
        return false;
      }

      var returnType = method.ReturnType;
      if (!returnType.IsReferenceType())
      {
        return false;
      }

      return !this.HasAnnotation();
    }

    /// <summary>
    /// Determines whether [is available indexer] [the specified indexer declaration].
    /// </summary>
    /// <param name="indexerDeclaration">
    /// The indexer declaration.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is available indexer] [the specified indexer declaration]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAvailableIndexer([NotNull] IIndexerDeclaration indexerDeclaration)
    {
      ICSharpFunctionDeclaration getterFunctionDeclaration = null;
      ICSharpFunctionDeclaration setterFunctionDeclaration = null;

      foreach (var accessorDeclaration in indexerDeclaration.AccessorDeclarations)
      {
        if (accessorDeclaration.ToTreeNode().AccessorName == null)
        {
          continue;
        }

        var accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        if (accessorName == "get")
        {
          getterFunctionDeclaration = accessorDeclaration;
        }
        else if (accessorName == "set")
        {
          setterFunctionDeclaration = accessorDeclaration;
        }
      }

      return this.IsAvailableFunction(getterFunctionDeclaration, setterFunctionDeclaration);
    }

    /// <summary>
    /// Determines whether [is available property] [the specified property declaration].
    /// </summary>
    /// <param name="propertyDeclaration">
    /// The property declaration.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is available property] [the specified property declaration]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAvailableProperty([NotNull] IPropertyDeclaration propertyDeclaration)
    {
      ICSharpFunctionDeclaration getterFunctionDeclaration = null;
      ICSharpFunctionDeclaration setterFunctionDeclaration = null;

      foreach (var accessorDeclaration in propertyDeclaration.AccessorDeclarations)
      {
        if (accessorDeclaration.ToTreeNode().AccessorName == null)
        {
          continue;
        }

        if (accessorDeclaration.Body == null)
        {
          continue;
        }

        var accessorName = accessorDeclaration.ToTreeNode().AccessorName.GetText();

        switch (accessorName)
        {
          case "get":
            getterFunctionDeclaration = accessorDeclaration;
            break;
          case "set":
            setterFunctionDeclaration = accessorDeclaration;
            break;
        }
      }

      return this.IsAvailableFunction(getterFunctionDeclaration, setterFunctionDeclaration);
    }

    /// <summary>
    /// Determines whether [is available parameters] [the specified function declaration].
    /// </summary>
    /// <param name="functionDeclaration">
    /// The function declaration.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is available parameters] [the specified function declaration]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsAvailableSetterFunction([NotNull] ICSharpFunctionDeclaration functionDeclaration)
    {
      if (functionDeclaration.IsAbstract || functionDeclaration.IsExtern)
      {
        return false;
      }

      if (functionDeclaration.Body == null)
      {
        return false;
      }

      var assertions = this.GetAssertions(functionDeclaration, true);

      foreach (var parameterStatement in assertions)
      {
        if (parameterStatement.Nullable)
        {
          continue;
        }

        if (parameterStatement.Statement == null && parameterStatement.NeedsStatement)
        {
          return true;
        }

        if (!parameterStatement.Parameter.IsValueVariable && parameterStatement.AttributeInstance == null)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Marks the parameter with attribute.
    /// </summary>
    /// <param name="assertion">
    /// The assertion.
    /// </param>
    private void MarkParameterWithAttribute([NotNull] ParameterStatement assertion)
    {
      var rule = Rule.GetRule(assertion.Parameter.Type, assertion.Parameter.Language);
      if (rule != null && !rule.NotNull && !rule.CanBeNull)
      {
        rule = null;
      }

      if (rule == null)
      {
        rule = Rule.GetDefaultRule();
      }

      if (rule == null)
      {
        return;
      }

      string name = null;
      ITypeElement typeElement = null;

      if (rule.NotNull)
      {
        name = CodeAnnotationsCache.NotNullAttributeShortName;
        typeElement = this.notNullTypeElement;
      }
      else if (rule.CanBeNull)
      {
        name = CodeAnnotationsCache.CanBeNullAttributeShortName;
        typeElement = this.canBeNullTypeElement;
      }

      if (string.IsNullOrEmpty(name))
      {
        return;
      }

      if (typeElement == null)
      {
        return;
      }

      var valueAttribute = typeElement.CLRName;
      if (string.IsNullOrEmpty(valueAttribute))
      {
        return;
      }

      var attributeInstance = FindAttribute(valueAttribute, assertion.Parameter);
      if (attributeInstance != null)
      {
        return;
      }

      var regularParameterDeclaration = assertion.Parameter as IRegularParameterDeclaration;
      if (regularParameterDeclaration == null)
      {
        return;
      }

      this.MarkWithAttribute(valueAttribute, regularParameterDeclaration);
    }

    /// <summary>
    /// Marks the with attribute.
    /// </summary>
    /// <param name="attributeName">
    /// Name of the attribute.
    /// </param>
    private void MarkWithAttribute([NotNull] string attributeName)
    {
      var attributesOwner = this.TypeMemberDeclaration as IAttributesOwnerDeclaration;
      if (attributesOwner == null)
      {
        return;
      }

      this.MarkWithAttribute(attributeName, attributesOwner);
    }

    /// <summary>
    /// Marks the with attribute.
    /// </summary>
    /// <param name="attributeName">
    /// Name of the attribute.
    /// </param>
    /// <param name="attributesOwnerDeclaration">
    /// The meta info target declaration.
    /// </param>
    private void MarkWithAttribute([NotNull] string attributeName, [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration)
    {
      var typeElement = this.GetAttribute(attributeName);
      if (typeElement == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration.GetPsiModule());

      var objects = new object[]
      {
        typeElement
      };
      var attribute = factory.CreateTypeMemberDeclaration("[$0]void Foo(){}", objects).Attributes[0];

      attribute = attributesOwnerDeclaration.AddAttributeAfter(attribute, null);

      var name = attribute.TypeReference.GetName();
      if (!name.EndsWith("Attribute"))
      {
        return;
      }

      var referenceName = factory.CreateReferenceName(name.Substring(0, name.Length - "Attribute".Length), new object[0]);
      referenceName = attribute.Name.ReplaceBy(referenceName);

      var declaredElement = referenceName.Reference.Resolve().DeclaredElement;
      if (declaredElement == null)
      {
        return;
      }

      if (declaredElement.Equals(typeElement))
      {
        referenceName.Reference.BindTo(typeElement);
      }
    }

    #endregion
  }
}