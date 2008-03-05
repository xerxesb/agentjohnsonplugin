using System;
using System.Text;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TextControl;
using JetBrains.Shell;
using JetBrains.Util;

namespace AgentJohnson.Exceptions {
  /// <summary>
  /// 
  /// </summary>
  public class DocumentThrownExceptionBulbItem : IBulbItem {
    #region Fields

    readonly DocumentThrownExceptionWarning _warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionBulbItem"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public DocumentThrownExceptionBulbItem(DocumentThrownExceptionWarning warning) {
      _warning = warning;
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
      if(psiManager == null) {
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", Text))) {
          psiManager.DoTransaction(delegate { Execute(); });
        }
      } 
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    void Execute() {
      IThrowStatement throwStatement = _warning.ThrowStatement as IThrowStatement;
      if(throwStatement == null){
        return;
      }

      ITypeMemberDeclaration typeMemberDeclaration = throwStatement.GetContainingTypeMemberDeclaration();
      if(typeMemberDeclaration == null) {
        return;
      }

      IDocCommentBlockOwnerNode docCommentBlockOwnerNode = typeMemberDeclaration as IDocCommentBlockOwnerNode;
      if(docCommentBlockOwnerNode == null) {
        return;
      }

      ITreeNode anchor = typeMemberDeclaration.ToTreeNode();
      if(anchor == null) {
        return;
      }

      string exceptionTypeName = throwStatement.Exception.Type().GetPresentableName(throwStatement.Language);

      StringBuilder text = new StringBuilder("\r\n <exception cref=\"" + exceptionTypeName + "\"><c>" + exceptionTypeName + "</c>.</exception>");

      string indent = GetIndent(anchor);

      InsertSlashes(text, indent);

      IDocCommentBlockNode docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
      if(docCommentBlockNode != null) {
        string docCommentText = GetDocCommentText(docCommentBlockNode);

        text.Insert(0, docCommentText);
      }
      else {
        text.Remove(0, 1);
      }

      text.Append("\nvoid foo(){}");

      ICSharpTypeMemberDeclaration declaration = CSharpElementFactory.GetInstance(_warning.Solution).CreateTypeMemberDeclaration(text.ToString());
      if(declaration == null) {
        return;
      }

      IDocCommentBlockNode node = SharedImplUtil.GetDocCommentBlockNode(declaration.ToTreeNode());
      if(node == null) {
        return;
      }

      docCommentBlockOwnerNode.SetDocCommentBlockNode(node);
    }

    #endregion

    #region IBulbItem Members

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
      get {
        IThrowStatement throwStatement = _warning.ThrowStatement as IThrowStatement;
        if(throwStatement == null) {
          return string.Empty;
        }

        string exceptionTypeName = throwStatement.Exception.Type().GetPresentableName(throwStatement.Language);

        return String.Format("Add xml-docs comment for exception '{0}'", exceptionTypeName);
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the doc comment text.
    /// </summary>
    /// <param name="docCommentBlockNode">The doc comment block node.</param>
    /// <returns>The doc comment text.</returns>
    static string GetDocCommentText(IElement docCommentBlockNode) {
      return docCommentBlockNode.GetText();
    }

    /// <summary>
    /// Gets the indent.
    /// </summary>
    /// <param name="anchor">The anchor.</param>
    /// <returns>The indent.</returns>
    static string GetIndent(IElement anchor) {
      string indent = string.Empty;

      IWhitespaceNode whitespace = anchor.ToTreeNode().PrevSibling as IWhitespaceNode;
      if(whitespace != null) {
        indent = whitespace.GetText();
      }

      return indent;
    }

    /// <summary>
    /// Inserts the slashes.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="indent">The indent.</param>
    static void InsertSlashes(StringBuilder text, string indent) {
      string slashes = indent + "///";

      for(int i = 0; i < text.Length; i++) {
        if(text[i] == '\n') {
          text.Insert(i + 1, slashes);
        }
      }
    }

    #endregion
  }
}