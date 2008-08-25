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
  [LiveTemplate("After invocation", "Executes a Live Template after the invocation of a method.")]
  public class AfterInvocation : ILiveTemplate {
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
      IExpressionStatement expressionStatement = previousStatement as IExpressionStatement;
      if(expressionStatement == null) {
        return null;
      }

      IInvocationExpression invocationExpression = expressionStatement.Expression as IInvocationExpression;
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
        MenuText = string.Format("After call to '{0}'", text),
        Description = string.Format("After call to '{0}'", text),
        Shortcut = string.Format("After call to {0}", shortcut)
      };

      liveTemplateItem.Variables["Name"] = method.ShortName;
      liveTemplateItem.Variables["Type"] = containingType != null ? containingType.ShortName : string.Empty;

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}