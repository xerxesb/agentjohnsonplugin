// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructMembers.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The struct members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The struct members.
  /// </summary>
  [LiveTemplate("Struct member", "Generate struct member")]
  public class StructMembers : ILiveTemplate
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

      var structDeclaration = element.GetContainingElement(typeof(IStructDeclaration), true) as IStructDeclaration;
      if (structDeclaration == null)
      {
        return null;
      }

      var memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IStructDeclaration))
      {
        return null;
      }

      var modifier = ModifierUtil.GetModifier(element, structDeclaration);

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Struct member",
        Description = "Struct member",
        Shortcut = "Struct member"
      };

      liveTemplateItem.Variables["Modifier"] = modifier;

      return new List<LiveTemplateItem>
      {
        liveTemplateItem
      };
    }

    #endregion

    #endregion
  }
}