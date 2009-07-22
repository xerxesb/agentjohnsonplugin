// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringEmptyQuickFix.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Define the string empty quick fix class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using System.Collections.Generic;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.Bulbs;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Define the string empty quick fix class.
  /// </summary>
  [QuickFix]
  public class StringEmptyQuickFix : IQuickFix, IBulbItem
  {
    #region Constants and Fields

    /// <summary>The Suggestion.</summary>
    private readonly StringEmptySuggestion _suggestion;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyQuickFix"/> class.
    /// </summary>
    /// <param name="suggestion">
    /// The suggestion.
    /// </param>
    public StringEmptyQuickFix(StringEmptySuggestion suggestion)
    {
      this._suggestion = suggestion;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items
    {
      get
      {
        var items = new List<IBulbItem>
        {
          this
        };

        return items.ToArray();
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get
      {
        return "Replace \"\" with string.Empty";
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBulbAction

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache">
    /// </param>
    /// <returns>
    /// The is available.
    /// </returns>
    public bool IsAvailable(IUserDataHolder cache)
    {
      return true;
    }

    #endregion

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
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create(string.Format("Context Action {0}", this.Text)))
        {
          psiManager.DoTransaction(delegate { this.Execute(); });
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    private void Execute()
    {
      var treeNode = this._suggestion.Node.ToTreeNode();
      if (treeNode == null)
      {
        return;
      }

      var expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(this._suggestion.Node.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var stringEmptyExpression = factory.CreateExpression("string.Empty");

      expression.ReplaceBy(stringEmptyExpression);
    }

    #endregion
  }
}