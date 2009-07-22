// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExpression.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The string expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The string expression.
  /// </summary>
  [LiveTemplate("Surround expression", "Surrounds the expression.", Priority = -20)]
  public class StringExpression : ILiveTemplate
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
    IEnumerable<LiveTemplateItem> ILiveTemplate.GetItems(SmartGenerateParameters parameters)
    {
      var element = parameters.Element;

      var result = new List<LiveTemplateItem>();

      var hasString = false;
      var hasInt = false;
      var hasBool = false;

      var expression = element.GetContainingElement(typeof(IExpression), false) as IExpression;
      while (expression != null)
      {
        var type = expression.Type();

        var typeName = type.GetPresentableName(element.Language);

        if (typeName == "string" && !hasString)
        {
          result.Add(new LiveTemplateItem
          {
            MenuText = "Surround string expression",
            Description = "Surround string expression",
            Shortcut = "Surround string expression",
            Range = expression.GetTreeTextRange()
          });

          hasString = true;
        }

        if (typeName == "int" && !hasInt)
        {
          result.Add(new LiveTemplateItem
          {
            MenuText = "Surround integer expression",
            Description = "Surround integer expression",
            Shortcut = "Surround integer expression",
            Range = expression.GetTreeTextRange()
          });

          hasInt = true;
        }

        if (typeName == "bool" && !hasBool)
        {
          result.Add(new LiveTemplateItem
          {
            MenuText = "Surround boolean expression",
            Description = "Surround boolean expression",
            Shortcut = "Surround boolean expression",
            Range = expression.GetTreeTextRange()
          });

          hasBool = true;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }

      return result;
    }

    #endregion

    #endregion
  }
}