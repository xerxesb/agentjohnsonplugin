using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Assign property", "Assigns another property on the object.", Priority=0)]
  public class AfterPropertyAssignment : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return;
      }
      IElement statement = element.GetContainingElement(typeof(IStatement), true);
      if(statement != null && !block.Contains(statement)) {
        return;
      }

      IExpressionStatement expressionStatement = StatementUtil.GetPreviousStatement(block, element) as IExpressionStatement;
      if(expressionStatement == null) {
        return;
      }

      IAssignmentExpression assignmentExpression = expressionStatement.Expression as IAssignmentExpression;
      if(assignmentExpression == null) {
        return;
      }

      IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if(referenceExpression == null) {
        return;
      }

      IReference reference = referenceExpression.Reference;
      if(reference == null) {
        return;
      }

      ResolveResult resolve = reference.Resolve();

      IPropertyDeclaration propertyDeclaration = resolve.DeclaredElement as IPropertyDeclaration;
      if(propertyDeclaration == null) {
        return;
      }

      ICSharpExpression qualifierExpression = referenceExpression.QualifierExpression;
      if(qualifierExpression == null) {
        return;
      }

      string qualifier = qualifierExpression.GetText();

      AddMenuItem("Assign another property on '{0}'", "AA6EFC53-174B-4EFD-A137-8115C22666C7", qualifier);
    }

    #endregion
  }
}