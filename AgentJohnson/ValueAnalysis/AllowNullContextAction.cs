namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates the parameter under the caret with the Allow Null attribute.", Name = "Annotate with AllowNull attribute for the current parameter", Priority = -1, Group = "C#")]
  public class AllowNullContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public AllowNullContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    protected override string GetText()
    {
      string attribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

      int n = attribute.LastIndexOf('.');
      if (n >= 0)
      {
        attribute = attribute.Substring(n + 1);
      }

      return string.Format("Annotate with '{0}'", attribute);
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void Execute(IElement element)
    {
      if (string.IsNullOrEmpty(ValueAnalysisSettings.Instance.AllowNullAttribute))
      {
        return;
      }

      if (!this.IsAvailableInternal())
      {
        return;
      }

      IParameterDeclaration parameterDeclaration = this.Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if (parameterDeclaration == null)
      {
        return;
      }

      IParameter parameter = parameterDeclaration as IParameter;
      if (parameter == null)
      {
        return;
      }

      ITypeMemberDeclaration typeMemberDeclaration = parameterDeclaration.GetContainingElement(typeof(ITypeMemberDeclaration), true) as ITypeMemberDeclaration;
      if (typeMemberDeclaration == null)
      {
        return;
      }

      IAttributesOwnerDeclaration attributesOwnerDeclaration = typeMemberDeclaration as IAttributesOwnerDeclaration;
      if (attributesOwnerDeclaration == null)
      {
        return;
      }

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      CodeFormatter codeFormatter = languageService.CodeFormatter;
      if (codeFormatter == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(typeMemberDeclaration.GetPsiModule());

      IAttribute attribute = factory.CreateTypeMemberDeclaration("[" + ValueAnalysisSettings.Instance.AllowNullAttribute + "(\"" + parameter.ShortName + "\")]void Foo(){}", new object[]
      {
      }).Attributes[0];

      attribute = attributesOwnerDeclaration.AddAttributeAfter(attribute, null);

      string name = attribute.TypeReference.GetName();
      if (!name.EndsWith("Attribute"))
      {
        return;
      }

      /*
      IReferenceName referenceName = factory.CreateReferenceName(name.Substring(0, name.Length - "Attribute".Length), new object[0]);
      referenceName = attribute.Name.ReplaceBy(referenceName);

      if (referenceName.Reference.Resolve().DeclaredElement != typeElement) {
        referenceName.Reference.BindTo(typeElement);
      }
      */

      DocumentRange range = attribute.GetDocumentRange();
      IPsiRangeMarker marker = (attribute as IElement).GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(attribute.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if PsiManager, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailable(IElement element)
    {
      if (string.IsNullOrEmpty(ValueAnalysisSettings.Instance.AllowNullAttribute))
      {
        return false;
      }

      IParameterDeclaration parameterDeclaration = this.Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if (parameterDeclaration == null)
      {
        return false;
      }

      IParameter parameter = parameterDeclaration.DeclaredElement;
      if (parameter == null)
      {
        return false;
      }

      if (parameter.Kind == ParameterKind.OUTPUT)
      {
        return false;
      }

      IFunctionDeclaration functionDeclaration = this.Provider.GetSelectedElement<IFunctionDeclaration>(true, true);
      if (functionDeclaration == null)
      {
        return false;
      }

      IFunction function = functionDeclaration.DeclaredElement;
      if (function == null)
      {
        return false;
      }

      IInterface interfaceType = parameter.GetContainingType() as IInterface;
      if (interfaceType != null)
      {
        return false;
      }

      CLRTypeName clrTypeName = new CLRTypeName(ValueAnalysisSettings.Instance.AllowNullAttribute);

      IList<IAttributeInstance> attributeInstances = function.GetAttributeInstances(clrTypeName, true);

      foreach (IAttributeInstance instance in attributeInstances)
      {
        ConstantValue2 allowNull = instance.PositionParameter(0).ConstantValue;

        if (allowNull.Value == null)
        {
          continue;
        }

        string allowNullName = allowNull.Value as string;

        if (allowNullName == parameter.ShortName)
        {
          return false;
        }

        if (allowNullName == "*")
        {
          return false;
        }
      }

      return true;
    }

    #endregion
  }
}