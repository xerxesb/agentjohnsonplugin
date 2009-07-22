// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBody.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The property body.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Naming.Settings;

  /// <summary>
  /// The property body.
  /// </summary>
  [SmartGenerate("Generate property accessor body", "Generates the body of either a getter or a setter property accessor.", Priority = 0)]
  public class PropertyBody : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      var propertyDeclaration = element.GetContainingElement(typeof(IPropertyDeclaration), true) as IProperty;
      if (propertyDeclaration == null)
      {
        return;
      }

      var accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;
      if (accessorDeclaration == null)
      {
        return;
      }

      var body = accessorDeclaration.Body;
      if (body == null || body.Statements.Count > 0)
      {
        return;
      }

      var name = propertyDeclaration.ShortName;
      if (string.IsNullOrEmpty(name))
      {
        return;
      }

      var charArray = name.ToCharArray();
      charArray[0] = char.ToLower(charArray[0]);
      name = new string(charArray);

      var namingPolicy = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings2().PredefinedNamingRules[NamedElementKinds.PrivateInstanceFields];
      var prefix = namingPolicy.NamingRule.Prefix;

      var typeName = propertyDeclaration.Type.GetPresentableName(element.Language);

      if (accessorDeclaration.Kind == AccessorKind.GETTER)
      {
        this.AddAction("Return '{0}{1}'", "0E03C1D4-A5DF-4011-86AE-4E561E419CD0", prefix, name);

        if (typeName == "string")
        {
          this.AddAction("Return '{0}{1} ?? string.Empty'", "BDC2EEB0-F626-4D6A-AB06-DD5C7C80BB30", prefix, name);
        }
      }

      if (accessorDeclaration.Kind == AccessorKind.SETTER)
      {
        this.AddAction("Assign value to '{0}{1}'", "A97E734B-C24B-44A3-A914-C67BBB3FAC65", prefix, name);
      }
    }

    #endregion
  }
}