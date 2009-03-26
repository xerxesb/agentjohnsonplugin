namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates a function with the Allow Null attribute.", Name = "Annotate with Allow Null attributes for all parameters", Priority = -1, Group = "C#")]
  public class AllowNullsContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullsContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public AllowNullsContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
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

      ITypeMemberDeclaration typeMemberDeclaration = this.GetTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return;
      }

      IAttributesOwnerDeclaration attributesOwnerDeclaration = typeMemberDeclaration as IAttributesOwnerDeclaration;
      if (attributesOwnerDeclaration == null)
      {
        return;
      }

      ITypeElement typeElement = GetAttribute(typeMemberDeclaration, ValueAnalysisSettings.Instance.AllowNullAttribute);
      if (typeElement == null)
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

      IAttribute attribute = factory.CreateTypeMemberDeclaration("[" + ValueAnalysisSettings.Instance.AllowNullAttribute + "(\"*\")]void Foo(){}", new object[]
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

      if (referenceName.Reference.Resolve().DeclaredElement != typeElement)
      {
        referenceName.Reference.BindTo(typeElement);
      }
      */

      DocumentRange range = attribute.GetDocumentRange();
      IPsiRangeMarker marker = (attribute as IElement).GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(attribute.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

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

      ITypeMemberDeclaration typeMemberDeclaration = this.GetTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return false;
      }

      ITypeElement typeElement = GetAttribute(typeMemberDeclaration, ValueAnalysisSettings.Instance.AllowNullAttribute);
      if (typeElement == null)
      {
        return false;
      }

      IParametersOwner parametersOwner = typeMemberDeclaration as IParametersOwner;
      if (parametersOwner == null || parametersOwner.Parameters.Count == 0)
      {
        return false;
      }

      IAttributesOwner attributesOwner = typeMemberDeclaration as IAttributesOwner;
      if (attributesOwner == null)
      {
        return false;
      }

      CLRTypeName clrTypeName = new CLRTypeName(ValueAnalysisSettings.Instance.AllowNullAttribute);

      IList<IAttributeInstance> attributeInstances = attributesOwner.GetAttributeInstances(clrTypeName, true);

      foreach (IAttributeInstance instance in attributeInstances)
      {
        ConstantValue2 allowNull = instance.PositionParameter(0).ConstantValue;

        if (allowNull.Value == null)
        {
          continue;
        }

        string allowNullName = allowNull.Value as string;

        if (allowNullName == "*")
        {
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
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    private static ITypeElement GetAttribute(IElement typeMemberDeclaration, string attributeName)
    {
      ISolution solution = typeMemberDeclaration.GetManager().Solution;

      IDeclarationsScope scope = DeclarationsScopeFactory.SolutionScope(solution, true);
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName(attributeName);

      if (typeElement == null)
      {
        return null;
      }

      /*
      PredefinedType predefinedType = new PredefinedType(solution.SolutionProject);
      if(!TypeFactory.CreateType(typeElement).IsSubtypeOf(predefinedType.Attribute)) {
        return null;
      }
      */

      return typeElement;
    }

    /// <summary>
    /// Gets the type member declaration.
    /// </summary>
    /// <returns>The type member declaration.</returns>
    private ITypeMemberDeclaration GetTypeMemberDeclaration()
    {
      IElement element = this.Provider.SelectedElement;
      if (element == null)
      {
        return null;
      }

      ITypeMemberDeclaration typeMemberDeclaration = null;

      ITreeNode treeNode = element as ITreeNode;
      if (treeNode != null)
      {
        typeMemberDeclaration = treeNode.Parent as ITypeMemberDeclaration;
      }

      if (typeMemberDeclaration == null)
      {
        IIdentifierNode identifierNode = element as IIdentifierNode;

        if (identifierNode != null)
        {
          typeMemberDeclaration = identifierNode.Parent as ITypeMemberDeclaration;
        }
      }

      return typeMemberDeclaration;
    }

    #endregion
  }
}