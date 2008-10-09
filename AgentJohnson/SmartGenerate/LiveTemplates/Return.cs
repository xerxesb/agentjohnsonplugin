using System.Collections.Generic;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("At the end of a block", "Executes a Live Template at the end of a block.")]
  public class Return : ILiveTemplate {
    #region Public methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="smartGenerateParameters">The smart generate parameters.</param>
    /// <returns>The items.</returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      if(!StatementUtil.IsAfterLastStatement(element)) {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "At the end of a block",
        Description = "At the end of a block",
        Shortcut = "At the end of a block"
      };

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}