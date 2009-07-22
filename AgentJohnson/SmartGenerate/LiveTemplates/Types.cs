// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Types.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The types.
  /// </summary>
  [LiveTemplate("Type", "Generate type")]
  public class Types : ILiveTemplate
  {
    #region Implemented Interfaces

    #region ILiveTemplate

    /// <summary>
    /// Gets the items.
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

      var classLikeDeclaration = element.GetContainingElement(typeof(IClassLikeDeclaration), true);
      if (classLikeDeclaration != null)
      {
        return null;
      }

      var enumDecl = element.GetContainingElement(typeof(IEnumDeclaration), true) as IEnumDeclaration;
      if (enumDecl != null)
      {
        return null;
      }

      var namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if (namespaceDeclaration == null)
      {
        return null;
      }

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Type",
        Description = "Type",
        Shortcut = "Type"
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