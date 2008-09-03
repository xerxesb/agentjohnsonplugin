using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Surround expression", "Surrounds the expression.", Priority = -20)]
  public class StringExpression : ILiveTemplate {
    #region Private methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The items.</returns>
    IEnumerable<LiveTemplateItem> ILiveTemplate.GetItems(SmartGenerateParameters parameters) {
      IElement element = parameters.Element;

      List<LiveTemplateItem> result = new List<LiveTemplateItem>();

      bool hasString = false;
      bool hasInt = false;
      bool hasBool = false;

      IExpression expression = element.GetContainingElement(typeof(IExpression), false) as IExpression;
      while(expression != null) {
        IType type = expression.Type();

        string typeName = type.GetPresentableName(element.Language);

        if(typeName == "string" && !hasString) {
          result.Add(new LiveTemplateItem {
            MenuText = "Surround string expression",
            Description = "Surround string expression",
            Shortcut = "Surround string expression",
            Range = expression.GetTreeTextRange()
          });

          hasString = true;
        }

        if(typeName == "int" && !hasInt) {
          result.Add(new LiveTemplateItem {
            MenuText = "Surround integer expression",
            Description = "Surround integer expression",
            Shortcut = "Surround integer expression",
            Range = expression.GetTreeTextRange()
          });

          hasInt = true;
        }

        if(typeName == "bool" && !hasBool) {
          result.Add(new LiveTemplateItem {
            MenuText = "Surround boolean expression",
            Description = "Surround boolean expression",
            Shortcut = "Surround boolean expression",
            Range = expression.GetTreeTextRange()
          });

          hasBool = true;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }

      return result;
    }

    #endregion
  }
}