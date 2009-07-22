// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodWithSignature.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The method with signature.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The method with signature.
  /// </summary>
  [LiveTemplate("Method with signature", "Generate body in a method with a signature")]
  public class MethodWithSignature : ILiveTemplate
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

      var signatureBuilder = new StringBuilder();
      var first = true;

      foreach (var parameter in method.Parameters)
      {
        if (!first)
        {
          signatureBuilder.Append(", ");
        }

        first = false;

        signatureBuilder.Append(parameter.Type.GetLongPresentableName(element.Language));
      }

      var signature = signatureBuilder.ToString();

      return new List<LiveTemplateItem>
      {
        new LiveTemplateItem
        {
          MenuText = string.Format("Body in method with signature ({0})", signature),
          Description = string.Format("Body in method with signature ({0})", signature),
          Shortcut = string.Format("Body in method with signature ({0})", signature)
        }
      };
    }

    #endregion

    #endregion
  }
}