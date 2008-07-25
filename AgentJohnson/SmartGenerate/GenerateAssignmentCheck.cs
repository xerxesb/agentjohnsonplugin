using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the generate assignment check class.
  /// </summary>
  [SmartGenerate("Generate check if variable is null", "Generates statements that check for null or empty string.", Priority=0)]
  public class GenerateAssignmentCheck: SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IDeclarationStatement declarationStatement = StatementUtil.GetPreviousStatement(element) as IDeclarationStatement;
      if(declarationStatement == null) {
        return;
      }

      TextRange range = GetNewStatementPosition(element);

      foreach(ILocalVariableDeclaration localVariableDeclaration in declarationStatement.VariableDeclarations) {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if(localVariable == null) {
          continue;
        }

        string name = localVariable.ShortName;
        IType type = localVariable.Type;

        if(type.GetPresentableName(element.Language) == "string") {
          AddMenuItem("Check if '{0}' is null or empty", "514313A0-91F4-4AE5-B4EB-2BB53736A023", range, name);
        } else {
          if(type.IsReferenceType()) {
            AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
          }
        }
      }
    }

    #endregion
  }
}