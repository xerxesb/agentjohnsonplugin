// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PullParameters.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the suggest property class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.LiveMacros
{
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
  using JetBrains.ReSharper.Feature.Services.Lookup;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Services;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the suggest property class.
  /// </summary>
  [Macro("LiveMacros.PullParameters", ShortDescription = "Pull parameters", LongDescription = "Pulls the list of parameters from the containing method.")]
  public class PullParameters : IMacro
  {
    #region Properties

    /// <summary>
    /// Gets array of parameter descriptions
    /// </summary>
    /// <value></value>
    public ParameterInfo[] Parameters
    {
      get
      {
        return EmptyArray<ParameterInfo>.Instance;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IMacro

    /// <summary>
    /// Evaluates "quick result" for this macro.
    /// Unlike the result returned by <see cref="M:JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.IMacro.GetLookupItems(JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots.IHotspotContext,System.Collections.Generic.IList{System.String})"/> method,
    /// quick result is re-evaluated on each typing and so its implementation should be very quick.
    /// If the macro cannot provide any result that can be evaluated very quickly, it should return null.
    /// </summary>
    /// <param name="context">
    /// </param>
    /// <param name="arguments">
    /// Values
    /// </param>
    /// <returns>
    /// The evaluate quick result.
    /// </returns>
    public string EvaluateQuickResult(IHotspotContext context, IList<string> arguments)
    {
      return null;
    }

    /// <summary>
    /// The get lookup items.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="arguments">
    /// The arguments.
    /// </param>
    /// <returns>
    /// </returns>
    public HotspotItems GetLookupItems(IHotspotContext context, IList<string> arguments)
    {
      var solution = context.SessionContext.Solution;
      var textControl = context.SessionContext.TextControl;

      var element = TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);

      var text = GetText(element);
      if (text == null)
      {
        return null;
      }

      var item = new TextLookupItem(text);

      var result = new HotspotItems(item);

      return result;
    }

    /// <summary>
    /// The get placeholder.
    /// </summary>
    /// <returns>
    /// The get placeholder.
    /// </returns>
    public string GetPlaceholder()
    {
      return "a";
    }

    /// <summary>
    /// The handle expansion.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="arguments">
    /// The arguments.
    /// </param>
    /// <returns>
    /// The handle expansion.
    /// </returns>
    public bool HandleExpansion(IHotspotContext context, IList<string> arguments)
    {
      return true;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// Returns the string.
    /// </returns>
    private static string GetText(IElement element)
    {
      var typeMemberDeclaration = element.GetContainingElement<ITypeMemberDeclaration>(true);
      if (typeMemberDeclaration == null)
      {
        return null;
      }

      var parametersOwner = typeMemberDeclaration.DeclaredElement as IParametersOwner;
      if (parametersOwner == null)
      {
        return null;
      }

      if (parametersOwner.Parameters.Count == 0)
      {
        return null;
      }

      var first = true;
      var parametersBuilder = new StringBuilder();

      foreach (var parameter in parametersOwner.Parameters)
      {
        if (!first)
        {
          parametersBuilder.Append(", ");
        }

        first = false;

        parametersBuilder.Append(parameter.ShortName);
      }

      return parametersBuilder.ToString();
    }

    #endregion
  }
}