// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllowNullContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates the parameter under the caret with the Allow Null attribute.", Name = "Annotate with AllowNull attribute for the current parameter", Priority = -1, Group = "C#")]
  public class AllowNullContextAction : ContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public AllowNullContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
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

      var parameterDeclaration = this.Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if (parameterDeclaration == null)
      {
        return;
      }

      var parameter = parameterDeclaration.DeclaredElement;
      if (parameter == null)
      {
        return;
      }

      var typeMemberDeclaration = parameterDeclaration.GetContainingElement(typeof(ITypeMemberDeclaration), true) as ITypeMemberDeclaration;
      if (typeMemberDeclaration == null)
      {
        return;
      }

      var attributesOwnerDeclaration = typeMemberDeclaration as IAttributesOwnerDeclaration;
      if (attributesOwnerDeclaration == null)
      {
        return;
      }

      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      var codeFormatter = languageService.CodeFormatter;
      if (codeFormatter == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(typeMemberDeclaration.GetPsiModule());

      var attribute = factory.CreateTypeMemberDeclaration("[" + ValueAnalysisSettings.Instance.AllowNullAttribute + "(\"" + parameter.ShortName + "\")]void Foo(){}", new object[]
      {
      }).Attributes[0];

      attribute = attributesOwnerDeclaration.AddAttributeAfter(attribute, null);

      var name = attribute.TypeReference.GetName();
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
      var range = attribute.GetDocumentRange();
      var marker = (attribute as IElement).GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(attribute.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    /// <returns>
    /// The get text.
    /// </returns>
    protected override string GetText()
    {
      var attribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

      var n = attribute.LastIndexOf('.');
      if (n >= 0)
      {
        attribute = attribute.Substring(n + 1);
      }

      return string.Format("Annotate with '{0}' [Agent Johnson]", attribute);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if PsiManager, ProjectFile of Solution == null
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// The is available.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      if (string.IsNullOrEmpty(ValueAnalysisSettings.Instance.AllowNullAttribute))
      {
        return false;
      }

      var parameterDeclaration = this.Provider.GetSelectedElement<IParameterDeclaration>(true, true);
      if (parameterDeclaration == null)
      {
        return false;
      }

      var parameter = parameterDeclaration.DeclaredElement;
      if (parameter == null)
      {
        return false;
      }

      if (parameter.Kind == ParameterKind.OUTPUT)
      {
        return false;
      }

      var functionDeclaration = this.Provider.GetSelectedElement<IFunctionDeclaration>(true, true);
      if (functionDeclaration == null)
      {
        return false;
      }

      var function = functionDeclaration.DeclaredElement;
      if (function == null)
      {
        return false;
      }

      var interfaceType = parameter.GetContainingType() as IInterface;
      if (interfaceType != null)
      {
        return false;
      }

      var clrTypeName = new CLRTypeName(ValueAnalysisSettings.Instance.AllowNullAttribute);

      var attributeInstances = function.GetAttributeInstances(clrTypeName, true);

      foreach (var instance in attributeInstances)
      {
        var allowNull = instance.PositionParameter(0).ConstantValue;

        if (allowNull.Value == null)
        {
          continue;
        }

        var allowNullName = allowNull.Value as string;

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