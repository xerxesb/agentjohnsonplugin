using System.Collections.Generic;
using AgentJohnson.SmartGenerate.Scopes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// Defines the generate assignment check class.
  /// </summary>
  [SmartGenerate("Generate check if variable is null", "Generates statements that check for null or empty string.", Priority = 0)]
  public class AssignmentCheck : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;
      List<ScopeEntry> scope = smartGenerateParameters.Scope;
      if(scope.Count == 0) {
        return;
      }

      string name = scope[smartGenerateParameters.ScopeIndex].Name;
      IType type = scope[smartGenerateParameters.ScopeIndex].Type;
      TextRange range = StatementUtil.GetNewStatementPosition(element);

      if(type.GetPresentableName(element.Language) == "string") {
        AddMenuItem("Check if '{0}' is null or empty", "514313A0-91F4-4AE5-B4EB-2BB53736A023", range, name);
      }
      else {
        if(type.IsReferenceType()) {
          AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
        }
      }
    }

    #endregion
  }
}