// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutsideNamespace.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The outside namespace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The outside namespace.
  /// </summary>
  [LiveTemplate("Outside namespace", "Executes a Live Template outside a namespace.")]
  public class OutsideNamespace : ILiveTemplate
  {
    #region Implemented Interfaces

    #region ILiveTemplate

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The items.
    /// </returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters parameters)
    {
      var element = parameters.Element;

      var namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if (namespaceDeclaration != null)
      {
        return null;
      }

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Outside namespace",
        Description = "Outside namespace",
        Shortcut = "Outside namespace"
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