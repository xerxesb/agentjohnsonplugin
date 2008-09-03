using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate property accessor body", "Generates the body of either a getter or a setter property accessor.", Priority = 0)]
  public class PropertyBody : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      IProperty propertyDeclaration = element.GetContainingElement(typeof(IPropertyDeclaration), true) as IProperty;
      if(propertyDeclaration == null) {
        return;
      }

      IAccessorDeclaration accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;
      if(accessorDeclaration == null) {
        return;
      }

      IBlock body = accessorDeclaration.Body;
      if(body == null || body.Statements.Count > 0) {
        return;
      }

      string name = propertyDeclaration.ShortName;
      if(string.IsNullOrEmpty(name)) {
        return;
      }

      char[] charArray = name.ToCharArray();
      charArray[0] = char.ToLower(charArray[0]);
      name = new string(charArray);

      string prefix = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings().FieldNameSettings.Prefix;

      string typeName = propertyDeclaration.Type.GetPresentableName(element.Language);

      if(accessorDeclaration.Kind == AccessorKind.GETTER) {
        AddMenuItem("Return '{0}{1}'", "0E03C1D4-A5DF-4011-86AE-4E561E419CD0", prefix, name);

        if(typeName == "string") {
          AddMenuItem("Return '{0}{1} ?? string.Empty'", "BDC2EEB0-F626-4D6A-AB06-DD5C7C80BB30", prefix, name);
        }
      }

      if(accessorDeclaration.Kind == AccessorKind.SETTER) {
        AddMenuItem("Assign value to '{0}{1}'", "A97E734B-C24B-44A3-A914-C67BBB3FAC65", prefix, name);
      }
    }

    #endregion
  }
}