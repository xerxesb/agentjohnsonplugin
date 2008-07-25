using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  [LiveTemplate("After local variable of type", "Executes a Live Template after the declaration of a local variable of a type.")]
  public class LocalVariableDeclarationLiveTemplate : ILiveTemplate {
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

      ILocalVariable localVariable = localVariableDeclarations[0] as ILocalVariable;
      if(localVariable == null) {
        return null;
      }

      IType type = localVariable.Type;

      string presentableName = type.GetPresentableName(element.Language);
      string longPresentableName = type.GetLongPresentableName(element.Language);

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = string.Format("After local variable declaration of type '{0}'", presentableName),
        Description = string.Format("After local variable of type '{0}'", presentableName),
        Shortcut = string.Format("After local variable of type {0}", longPresentableName)
      };

      liveTemplateItem.Variables["Name"] = localVariable.ShortName;
      liveTemplateItem.Variables["Type"] = presentableName;

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}