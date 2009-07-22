// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Return.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The return.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;

  /// <summary>
  /// The return.
  /// </summary>
  [LiveTemplate("At the end of a block", "Executes a Live Template at the end of a block.")]
  public class Return : ILiveTemplate
  {
    #region Implemented Interfaces

    #region ILiveTemplate

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The smart generate parameters.
    /// </param>
    /// <returns>
    /// The items.
    /// </returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      if (!StatementUtil.IsAfterLastStatement(element))
      {
        return null;
      }

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "At the end of a block",
        Description = "At the end of a block",
        Shortcut = "At the end of a block"
      };

      return new List<LiveTemplateItem>
      {
        liveTemplateItem
      };
    }

    #endregion

    #endregion
  }
}