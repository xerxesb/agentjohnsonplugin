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
  using System.Linq;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.DataProviders;
  using JetBrains.ReSharper.Intentions.Util;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

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
      if (!this.IsAvailableInternal())
      {
        return;
      }

      var attributesOwnerDeclaration = this.Provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, true);
      if (attributesOwnerDeclaration == null)
      {
        return;
      }

      var psiModule = attributesOwnerDeclaration.GetPsiModule();
      var scope = DeclarationsScopeFactory.ModuleScope(psiModule, true);
      string allowNullAttribute = ValueAnalysisSettings.Instance.AllowNullAttribute;

      if (!allowNullAttribute.EndsWith("Attribute"))
      {
        allowNullAttribute += "Attribute";
      }

      var typeElement = PsiManager.GetInstance(psiModule.GetSolution()).GetDeclarationsCache(scope, true).GetTypeElementsByCLRName(new CLRTypeName(allowNullAttribute)).FirstOrDefault();

      Logger.Assert(typeElement != null, "typeElement != null");

      var attribute = this.Provider.ElementFactory.CreateAttribute(typeElement);

      ContextActionUtils.FormatWithDefaultProfile(attributesOwnerDeclaration.AddAttributeAfter(attribute, null));
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