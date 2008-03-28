using System.Collections.Generic;
using AgentJohnson.Exceptions;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.Strings {
  /// <summary>
  /// 
  /// </summary>
  [QuickFix]
  public class StringEmptyQuickFix : IQuickFix, IBulbItem {
    #region Fields

    readonly StringEmptySuggestion _suggestion;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionQuickFix"/> class.
    /// </summary>
    /// <param name="suggestion">The suggestion.</param>
    public StringEmptyQuickFix(StringEmptySuggestion suggestion) {
      _suggestion = suggestion;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public void Execute(ISolution solution, ITextControl textControl) {
      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null){
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()){
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS){
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", Text))){
          psiManager.DoTransaction(delegate { Execute(); });
        }
      }
    }

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool IsAvailable(IUserDataHolder cache) {
      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    void Execute() {
      ITreeNode treeNode = _suggestion.Node.ToTreeNode();
      if(treeNode == null){
        return;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if(expression == null){
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(_suggestion.Node.GetProject());
      if(factory == null){
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
    public string Text {
      get {
        return "Replace \"\" with string.Empty";
      }
    }

    #endregion

    #region IQuickFix Members

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items {
      get {
        List<IBulbItem> items = new List<IBulbItem>();

        items.Add(this);

        return items.ToArray();
      }
    }

    #endregion
  }
}