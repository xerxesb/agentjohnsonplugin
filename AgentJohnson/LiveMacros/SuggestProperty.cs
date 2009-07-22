// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuggestProperty.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the suggest property class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.LiveMacros
{
  using System.Collections.Generic;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the suggest property class.
  /// </summary>
  [Macro("LiveMacros.SuggestProperty", ShortDescription = "Suggest property name", LongDescription = "Suggests a property name on an object")]
  public class SuggestProperty : IMacro
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
    /// Evaluates list of lookup items to show
    /// </summary>
    /// <param name="context">
    /// </param>
    /// <param name="arguments">
    /// </param>
    /// <returns>
    /// List of lookup items to show in order of preference. That is,
    /// </returns>
    public HotspotItems GetLookupItems(IHotspotContext context, IList<string> arguments)
    {
      return null;
    }

    /// <summary>
    /// <para>
    /// Placeholder value is inserted into the text on the very initial step of template expansion
    /// and is needed for proper template text reformatting when real values cannot be calculated yet.
    /// </para>
    /// <para>
    /// More precisely, the following steps are performed:
    /// <list type="bullet">
    /// <item>
    /// placeholder values for all template fields are inserted into the text
    /// </item>
    /// <item>
    /// the resulting text is reformatted
    /// </item>
    /// <item>
    /// <see cref="M:JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.IMacro.GetLookupItems(JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots.IHotspotContext,System.Collections.Generic.IList{System.String})"/> is used to evaluate and insert values for all fields.
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <returns>
    /// The get placeholder.
    /// </returns>
    public string GetPlaceholder()
    {
      return "a";
    }

    /// <summary>
    /// Execute custom action on expanding this macro
    /// </summary>
    /// <param name="context">
    /// </param>
    /// <param name="arguments">
    /// </param>
    /// <returns>
    /// <c>true</c> if all neccessary actions have been taken or <c>false</c> to proceed with normal <see cref="M:JetBrains.ReSharper.Feature.Services.LiveTemplates.Macros.IMacro.GetLookupItems(JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots.IHotspotContext,System.Collections.Generic.IList{System.String})"/> procedure
    /// </returns>
    public bool HandleExpansion(IHotspotContext context, IList<string> arguments)
    {
      var solution = context.SessionContext.Solution;
      var textControl = context.SessionContext.TextControl;

      var projectFile = DocumentManager.GetInstance(solution).GetProjectFile(textControl.Document);
      if (projectFile == null)
      {
        return false;
      }

      var psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return false;
      }

      var file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return false;
      }

      var element = file.FindTokenAt(textControl.CaretModel.Offset);
      if (element == null)
      {
        return false;
      }

      var referenceExpression = element.GetContainingElement(typeof(IExpression), true) as IExpression;
      if (referenceExpression == null)
      {
        return false;
      }

      var expressionType = referenceExpression.GetExpressionType();

      var declaredType = expressionType as IDeclaredType;
      if (declaredType == null)
      {
        return false;
      }

      var typeElement = declaredType.GetTypeElement();
      if (typeElement == null)
      {
        return false;
      }

      var properties = typeElement.Properties;

      foreach (var property in properties)
      {
        arguments.Add(property.ShortName);
      }

      return true;
    }

    #endregion

    #endregion
  }
}