using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// 
  /// </summary>
  [LiveTemplate("After initialization with call to a method", "Executes a Live Template after initialization with call to a method.")]
  public class AfterLocalVariableInitializer : ILiveTemplate {
    #region Public methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="dataContext">The data context.</param>
    /// <param name="previousStatement">The previous statement.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public IEnumerable<LiveTemplateItem> GetItems(ISolution solution, IDataContext dataContext, IStatement previousStatement, IElement element) {
      IDeclarationStatement declarationStatement = previousStatement as IDeclarationStatement;
      if(declarationStatement == null) {
        return null;
      }

      IList<ILocalVariableDeclaration> localVariableDeclarations = declarationStatement.VariableDeclarations;
      if(localVariableDeclarations == null || localVariableDeclarations.Count != 1) {
        return null;
      }

      ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];
      if(localVariableDeclaration == null) {
        return null;
      }

      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if(localVariable == null) {
        return null;
      }

      IExpressionInitializer initial = localVariableDeclaration.Initial as IExpressionInitializer;
      if(initial == null) {
        return null;
      }

      IInvocationExpression invocationExpression = initial.Value as IInvocationExpression;
      if(invocationExpression == null) {
        return null;
      }

      IReferenceExpression invokedExpression = invocationExpression.InvokedExpression as IReferenceExpression;
      if(invokedExpression == null) {
        return null;
      }

      ResolveResult resolveResult = invokedExpression.Reference.Resolve();

      IMethod method = null;

      IMethodDeclaration methodDeclaration = resolveResult.DeclaredElement as IMethodDeclaration;
      if(methodDeclaration != null) {
        method = methodDeclaration as IMethod;
      }

      if(method == null) {
        method = resolveResult.DeclaredElement as IMethod;
      }

      if(method == null) {
        return null;
      }

      string variableName = localVariable.ShortName;
      string text = method.ShortName;
      string shortcut = method.ShortName;

      ITypeElement containingType = method.GetContainingType();
      if(containingType != null) {
        text = containingType.ShortName + "." + text;
        shortcut = containingType.ShortName + "." + shortcut;

        INamespace ns = containingType.GetContainingNamespace();
        if(!string.IsNullOrEmpty(ns.ShortName)) {
          shortcut = ns.ShortName + "." + shortcut;
        }
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = string.Format("After initialization with call to '{0}'", text),
        Description = string.Format("After initialization with call to '{0}'", text),
        Shortcut = string.Format("After initialization with call to {0}", shortcut)
      };

      liveTemplateItem.Variables["Variable"] = variableName;
      liveTemplateItem.Variables["Name"] = method.ShortName;
      liveTemplateItem.Variables["Type"] = containingType != null ? containingType.ShortName : string.Empty;

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}