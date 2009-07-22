// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassMembers.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The class members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The class members.
  /// </summary>
  [LiveTemplate("Class member", "Generate class member")]
  public class ClassMembers : ILiveTemplate
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

      var classDeclaration = element.GetContainingElement(typeof(IClassDeclaration), true) as IClassDeclaration;
      if (classDeclaration == null)
      {
        return null;
      }

      var memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IClassDeclaration))
      {
        return null;
      }

      var modifier = ModifierUtil.GetModifier(element, classDeclaration);

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Class member",
        Description = "Class member",
        Shortcut = "Class member"
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