namespace AgentJohnson.Strings
{
  using System.Collections.Generic;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Define the string empty quick fix class.
  /// </summary>
  [QuickFix]
  public class StringEmptyQuickFix : IQuickFix, IBulbItem
  {
    #region Fields

    /// <summary>The Suggestion.</summary>
    private readonly StringEmptySuggestion _suggestion;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyQuickFix"/> class.
    /// </summary>
    /// <param name="suggestion">The suggestion.</param>
    public StringEmptyQuickFix(StringEmptySuggestion suggestion)
    {
      this._suggestion = suggestion;
    }

    #endregion

    #region Public methods

    #region IBulbItem Members

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public void Execute(ISolution solution, ITextControl textControl)
    {
      PsiManager psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return;
      }

      using (ModificationCookie cookie = textControl.Document.EnsureWritable())
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

    #region IQuickFix Members

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool IsAvailable(IUserDataHolder cache)
    {
      return true;
    }

    #endregion

    #endregion

    #region Private methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    private void Execute()
    {
      ITreeNode treeNode = this._suggestion.Node.ToTreeNode();
      if (treeNode == null)
      {
        return;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(this._suggestion.Node.GetProject());
      if (factory == null)
      {
        return;
      }

      ICSharpExpression stringEmptyExpression = factory.CreateExpression("string.Empty");

      expression.ReplaceBy(stringEmptyExpression);
    }

    #endregion

    #region IBulbItem Members

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

    #region IQuickFix Members

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items
    {
      get
      {
        List<IBulbItem> items = new List<IBulbItem>
        {
          this
        };

        return items.ToArray();
      }
    }

    #endregion
  }
}