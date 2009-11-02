// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnBulbItem.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the return bulb item class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.Bulbs;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.TextControl;
  using JetBrains.ReSharper.Psi.Tree;
  using AgentJohnson.Psi.CodeStyle;

  /// <summary>
  /// Defines the return bulb item class.
  /// </summary>
  public class ReturnBulbItem : IBulbItem
  {
    #region Constants and Fields

    /// <summary>
    /// The _warning.
    /// </summary>
    private readonly ReturnWarning warning;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnBulbItem"/> class.
    /// </summary>
    /// <param name="warning">
    /// The suggestion.
    /// </param>
    public ReturnBulbItem(ReturnWarning warning)
    {
      this.warning = warning;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get
      {
        return "Assert return value";
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBulbItem

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="textControl">
    /// The text control.
    /// </param>
    public void Execute(ISolution solution, ITextControl textControl)
    {
      var psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return;
      }

      using (var cookie = textControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != global::JetBrains.Util.EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create(string.Format("Context Action {0}", this.Text)))
        {
          psiManager.DoTransaction(delegate { this.Execute(solution); });
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="solution">The solution.</param>
    private void Execute(ISolution solution)
    {
      var returnStatement = this.warning.ReturnStatement;

      var function = returnStatement.GetContainingTypeMemberDeclaration() as IFunction;
      if (function == null)
      {
        return;
      }

      var type = function.ReturnType;

      var rule = Rule.GetRule(type, function.Language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return;
      }

      var code = rule.ReturnAssertion;

      var expression = returnStatement.Value.GetText();
      var typeName = type.GetLongPresentableName(returnStatement.Language);

      code = "return " + string.Format(code, expression, typeName) + ";";

      var factory = CSharpElementFactory.GetInstance(returnStatement.GetPsiModule());

      var statement = factory.CreateStatement(code);

      var result = returnStatement.ReplaceBy(statement);

      var range = result.GetDocumentRange();
      CodeFormatter codeFormatter = new CodeFormatter();
      codeFormatter.Format(solution, range);
    }

    #endregion
  }
}