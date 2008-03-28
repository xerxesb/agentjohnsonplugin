using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Shell.Progress;
using JetBrains.TextControl;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates a parameter with the Allow Null attribute.", Name = "Allow Null Parameter", Priority = -1, Group = "C#")]
  public class AllowNullContextAction : OneItemContextActionBase {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public AllowNullContextAction(ISolution solution, ITextControl textControl): base(solution, textControl) {
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public override string Text {
      get {
        string attribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

        int n = attribute.LastIndexOf('.');
        if(n >= 0) {
          attribute = attribute.Substring(n + 1);
        }

        return string.Format("Annotate with '{0}'", attribute);
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void ExecuteInternal() {
      if (string.IsNullOrEmpty(ValueAnalysisSettings.Instance.AllowNullAttribute)){
        return;
      }

      if (!IsAvailableInternal()) {
        return;
      }

      IParameterDeclaration parameterDeclaration = Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if (parameterDeclaration == null) {
        return;
      }

      IParameter parameter = parameterDeclaration as IParameter;
      if (parameter == null) {
        return;
      }

      ITypeMemberDeclaration typeMemberDeclaration = parameterDeclaration.GetContainingTypeMemberDeclaration();
      if(typeMemberDeclaration == null){
        return;
      }

      IMetaInfoTargetDeclaration metaInfoTargetDeclaration = typeMemberDeclaration as IMetaInfoTargetDeclaration;
      if (metaInfoTargetDeclaration == null) {
        return;
      }

      ITypeElement typeElement = GetAttribute(typeMemberDeclaration, ValueAnalysisSettings.Instance.AllowNullAttribute);
      if (typeElement == null) {
        return;
      }

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null) {
        return;
      }

      CodeFormatter codeFormatter = languageService.CodeFormatter;
      if (codeFormatter == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(typeMemberDeclaration.GetProject());

      IAttribute attribute = factory.CreateTypeMemberDeclaration("[" + ValueAnalysisSettings.Instance.AllowNullAttribute + "(\"" + parameter.ShortName + "\")]void Foo(){}", new object[] { }).Attributes[0];

      attribute = metaInfoTargetDeclaration.AddAttributeAfter(attribute, null);

      string name = attribute.TypeReference.GetName();
      if (!name.EndsWith("Attribute")) {
        return;
      }

      IReferenceName referenceName = factory.CreateReferenceName(name.Substring(0, name.Length - "Attribute".Length), new object[0]);
      referenceName = attribute.Name.ReplaceBy(referenceName);

      /*
      if (referenceName.Reference.Resolve().DeclaredElement != typeElement) {
        referenceName.Reference.BindTo(typeElement);
      }
      */

      DocumentRange range = attribute.GetDocumentRange();
      IPsiRangeMarker marker = (attribute as IElement).GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(attribute.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if PsiManager, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailableInternal() {
      if (string.IsNullOrEmpty(ValueAnalysisSettings.Instance.AllowNullAttribute)) {
        return false;
      }

      IParameterDeclaration parameterDeclaration = Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if(parameterDeclaration == null){
        return false;
      }

      ITypeElement typeElement = GetAttribute(parameterDeclaration, ValueAnalysisSettings.Instance.AllowNullAttribute);
      if(typeElement == null) {
        return false;
      }

      IAttributesOwner attributesOwner = parameterDeclaration.GetContainingTypeMemberDeclaration() as IAttributesOwner;
      if (attributesOwner == null) {
        return false;
      }

      IParameter parameter = parameterDeclaration as IParameter;
      if(parameter == null){
        return false;
      }

      if (parameter.Kind == ParameterKind.OUTPUT) {
        return false;
      }

      IInterface interfaceType = parameter.GetContainingType() as IInterface;
      if (interfaceType != null) {
        return false;
      }

      IFunction function = parameter.GetContainingTypeMember() as IFunction;
      if (function == null) {
        return false;
      }

      CLRTypeName clrTypeName = new CLRTypeName(ValueAnalysisSettings.Instance.AllowNullAttribute);

      IList<IAttributeInstance> attributeInstances = attributesOwner.GetAttributeInstances(clrTypeName, true);

      foreach (IAttributeInstance instance in attributeInstances) {
        ConstantValue2 allowNull = instance.PositionParameter(0).ConstantValue;

        if (allowNull.Value == null) {
          continue;
        }

        string allowNullName = allowNull.Value as string;

        if (allowNullName == parameter.ShortName){
          return false;
        }

        if (allowNullName == "*"){
          return false;
        }
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="element">The type member declaration.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    static ITypeElement GetAttribute(IElement element, string attributeName) {
      ISolution solution = element.GetManager().Solution;
      
      DeclarationsCacheScope declarationsCacheScope = DeclarationsCacheScope.SolutionScope(solution, true);

      IDeclarationsCache declarationsCache = PsiManager.GetInstance(solution).GetDeclarationsCache(declarationsCacheScope, true);

      ITypeElement typeElement = declarationsCache[attributeName] as ITypeElement;

      if (typeElement == null) {
        return null;
      }

      /*
      PredefinedType predefinedType = new PredefinedType(element.GetProject());

      if (!TypeFactory.CreateType(typeElement).IsSubtypeOf(predefinedType.Attribute)) {
        return null;
      }
      */

      return typeElement;
    }

    #endregion
  }
}