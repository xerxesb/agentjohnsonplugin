// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceMembers.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The interface members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The interface members.
  /// </summary>
  [LiveTemplate("Interface member", "Generate interface member")]
  public class InterfaceMembers : ILiveTemplate
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

      var interfaceDeclaration = element.GetContainingElement(typeof(IInterfaceDeclaration), true);
      if (interfaceDeclaration == null)
      {
        return null;
      }

      var memberDeclaration = element.GetContainingElement(typeof(ITypeMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IInterfaceDeclaration))
      {
        return null;
      }

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Interface member",
        Description = "Interface member",
        Shortcut = "Interface member"
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