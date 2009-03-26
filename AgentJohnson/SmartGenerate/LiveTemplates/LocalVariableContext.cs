namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Tree;
  using Scopes;

  /// <summary>
  /// </summary>
  [LiveTemplate("Local Variable Context", "Generate local Variable Context")]
  public class LocalVariableContext : ILiveTemplate
  {
    #region Private methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The items.</returns>
    IEnumerable<LiveTemplateItem> ILiveTemplate.GetItems(SmartGenerateParameters parameters)
    {
      IElement element = parameters.Element;

      List<ScopeEntry> scope = parameters.Scope;
      if (scope.Count == 0)
      {
        return null;
      }
      string name = scope[parameters.ScopeIndex].Name;
      IType type = scope[parameters.ScopeIndex].Type;

      List<LiveTemplateItem> result = new List<LiveTemplateItem>();

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem
      {
        MenuText = string.Format("After declaration of variable of type '{0}'", type.GetPresentableName(element.Language)),
        Description = string.Format("After declaration of variable of type '{0}'", type.GetPresentableName(element.Language)),
        Shortcut = string.Format("After declaration of variable of type '{0}'", type.GetLongPresentableName(element.Language))
      };

      liveTemplateItem.Variables["Name"] = name;
      liveTemplateItem.Variables["Type"] = type.GetPresentableName(element.Language);

      result.Add(liveTemplateItem);

      return result;
    }

    #endregion
  }
}