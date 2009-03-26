namespace AgentJohnson.LiveMacros
{
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
  using JetBrains.ReSharper.Feature.Services.Lookup;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Services;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the suggest property class.
  /// </summary>
  [Macro("LiveMacros.PullParameters", ShortDescription = "Pull parameters", LongDescription = "Pulls the list of parameters from the containing method.")]
  public class PullParameters : IMacro
  {
    #region Public methods

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    private static string GetText(IElement element)
    {
      ITypeMemberDeclaration typeMemberDeclaration = element.GetContainingElement<ITypeMemberDeclaration>(true);
      if (typeMemberDeclaration == null)
      {
        return null;
      }

      IParametersOwner parametersOwner = typeMemberDeclaration as IParametersOwner;
      if (parametersOwner == null)
      {
        return null;
      }

      if (parametersOwner.Parameters.Count == 0)
      {
        return null;
      }

      bool first = true;
      StringBuilder parametersBuilder = new StringBuilder();

      foreach (IParameter parameter in parametersOwner.Parameters)
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

    #region IMacro Members

    public string GetPlaceholder()
    {
      return "a";
    }

    public bool HandleExpansion(IHotspotContext context, IList<string> arguments)
    {
      return true;
    }

    public HotspotItems GetLookupItems(IHotspotContext context, IList<string> arguments)
    {
      ISolution solution = context.SessionContext.Solution;
      ITextControl textControl = context.SessionContext.TextControl;

      IElement element = TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);

      string text = GetText(element);
      if (text == null)
      {
        return null;
      }

      TextLookupItem item = new TextLookupItem(text);

      HotspotItems result = new HotspotItems(item);

      return result;
    }

    /// <summary>
    /// Evaluates "quick result" for this macro.
    /// Unlike the result returned by <see cref="M:JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.IMacro.GetLookupItems(JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots.IHotspotContext,System.Collections.Generic.IList{System.String})"/> method,
    /// quick result is re-evaluated on each typing and so its implementation should be very quick.
    /// If the macro cannot provide any result that can be evaluated very quickly, it should return null.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="arguments">Values</param>
    /// <returns></returns>
    public string EvaluateQuickResult(IHotspotContext context, IList<string> arguments)
    {
      return null;
    }

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
  }
}