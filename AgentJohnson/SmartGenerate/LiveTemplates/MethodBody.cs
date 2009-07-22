// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBody.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The method body.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The method body.
  /// </summary>
  [LiveTemplate("Body in method that returns a type", "Generates a body in method that returns a type")]
  public class MethodBody : ILiveTemplate
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

      var methodDeclaration = element.GetContainingElement(typeof(IMethodDeclaration), true) as IMethodDeclaration;
      if (methodDeclaration == null)
      {
        return null;
      }

      var body = methodDeclaration.Body;
      if (body == null || body.Statements.Count > 0)
      {
        return null;
      }

      var name = methodDeclaration.DeclaredName;
      if (string.IsNullOrEmpty(name))
      {
        return null;
      }

      var method = methodDeclaration.DeclaredElement;
      if (method == null)
      {
        return null;
      }

      var returnType = method.ReturnType;
      var returnTypeName = returnType.GetPresentableName(element.Language);
      if (returnTypeName == "void" || string.IsNullOrEmpty(returnTypeName))
      {
        return null;
      }

      var liveTemplateItem = new LiveTemplateItem
      {
        MenuText = string.Format("Body in method that returns '{0}'", returnTypeName),
        Description = string.Format("Body in method that returns '{0}'", returnTypeName),
        Shortcut = string.Format("Body in method that returns '{0}'", returnTypeName)
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