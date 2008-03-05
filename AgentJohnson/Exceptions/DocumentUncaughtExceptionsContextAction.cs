using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Editor;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TextControl;
using JetBrains.Shell;
using JetBrains.Util;

namespace AgentJohnson.Exceptions {
  /// <summary>
  /// </summary>
  [ContextAction(Description="Document uncaught exceptions that are thrown in called functions", Name="Add xml-docs comments for uncaught exceptions", Priority=-1, Group="C#")]
  public class DocumentUncaughtExceptionsContextAction : IContextAction, IBulbItem {
    #region Fields

    readonly ISolution _solution;
    readonly ITextControl _textControl;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CatchExceptionsContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public DocumentUncaughtExceptionsContextAction(ISolution solution, ITextControl textControl) {
      _solution = solution;
      _textControl = textControl;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public void Execute(ISolution solution, ITextControl textControl) {
      if(_solution != solution || _textControl != textControl){
        throw new InvalidOperationException();
      }

      Shell.Instance.AssertReadAccessAllowed();

      IElement element = GetElementAtCaret();
      if(element == null){
        return;
      }

      ITreeNode node = element.ToTreeNode();
      if(node == null) {
        return;
      }

      IInvocationExpression invocationExpression = null;

      while(node != null) {
        invocationExpression = node as IInvocationExpression;

        if(invocationExpression != null) {
          break;
        }

        node = node.Parent;
      }

      if(invocationExpression == null) {
        return;
      }

      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null){
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()){
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS){
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", Text))){
          psiManager.DoTransaction(delegate {
                                     Execute(solution, invocationExpression);
                                   });
        }
      }
    }

    /// <summary>
    /// Determines whether the specified cache is available.
    /// </summary>
    /// <param name="cache">The cache.</param>
    /// <returns>
    /// 	<c>true</c> if the specified cache is available; otherwise, <c>false</c>.
    /// </returns>
    public bool IsAvailable(IUserDataHolder cache) {
      IElement element = GetElementAtCaret();
      if(element == null) {
        return false;
      }

      ITreeNode node = element.ToTreeNode();
      if(node == null) {
        return false;
      }

      IInvocationExpression invocationExpression = null;

      while(node != null) {
        invocationExpression = node as IInvocationExpression;

        if(invocationExpression != null) {
          break;
        }

        if (node is IStatement){
          break;
        }

        node = node.Parent;
      }

      if(invocationExpression == null) {
        return false;
      }

      List<string[]> exceptions = new List<string[]>();

      GetExceptions(invocationExpression, exceptions);

      return exceptions.Count > 0;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <returns>The element.</returns>
    protected IElement GetElementAtCaret() {
      IProjectFile projectFile = DocumentManager.GetInstance(_solution).GetProjectFile(_textControl.Document);
      if(projectFile == null){
        return null;
      }

      PsiManager psiManager = PsiManager.GetInstance(_solution);
      if(psiManager == null){
        return null;
      }

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if(file == null){
        return null;
      }

      return file.FindTokenAt(_textControl.CaretModel.Offset);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    static void Execute(ISolution solution, IInvocationExpression invocationExpression) {
      ITypeMemberDeclaration typeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclaration();
      if(typeMemberDeclaration == null){
        return;
      }

      IDocCommentBlockOwnerNode docCommentBlockOwnerNode = typeMemberDeclaration as IDocCommentBlockOwnerNode;
      if(docCommentBlockOwnerNode == null){
        return;
      }

      ITreeNode anchor = typeMemberDeclaration.ToTreeNode();
      if(anchor == null){
        return;
      }

      List<string[]> exceptions = new List<string[]>();

      GetExceptions(invocationExpression, exceptions);

      StringBuilder text = new StringBuilder();

      foreach(string[] exception in exceptions){
        string t = exception[1];

        t = Regex.Replace(t, "<paramref name=\"([^\"]*)\" />", "$1");

        text.Append("\r\n <exception cref=\"" + exception[0] + "\">" + t + "</exception>");
      }

      string indent = GetIndent(anchor);

      InsertSlashes(text, indent);

      IDocCommentBlockNode docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
      if(docCommentBlockNode != null){
        string docCommentText = GetDocCommentText(docCommentBlockNode);

        text.Insert(0, docCommentText);
      }
      else{
        text.Remove(0, 1);
      }

      text.Append("\nvoid foo(){}");

      ICSharpTypeMemberDeclaration declaration = CSharpElementFactory.GetInstance(solution).CreateTypeMemberDeclaration(text.ToString());
      if(declaration == null){
        return;
      }

      IDocCommentBlockNode node = SharedImplUtil.GetDocCommentBlockNode(declaration.ToTreeNode());
      if(node == null){
        return;
      }

      docCommentBlockOwnerNode.SetDocCommentBlockNode(node);
    }

    /// <summary>
    /// Gets the doc comment text.
    /// </summary>
    /// <param name="docCommentBlockNode">The doc comment block node.</param>
    /// <returns>The doc comment text.</returns>
    static string GetDocCommentText(IElement docCommentBlockNode) {
      return docCommentBlockNode.GetText();
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <param name="exceptions">The exceptions.</param>
    static void GetExceptions(IInvocationExpression invocationExpression, List<string[]> exceptions) {
      ProcessInvocation(invocationExpression, exceptions);

      if(exceptions.Count == 0){
        return;
      }

      RemoveDocumented(invocationExpression, exceptions);
    }

    /// <summary>
    /// Gets the indent.
    /// </summary>
    /// <param name="anchor">The anchor.</param>
    /// <returns>The indent.</returns>
    static string GetIndent(IElement anchor) {
      string indent = string.Empty;

      IWhitespaceNode whitespace = anchor.ToTreeNode().PrevSibling as IWhitespaceNode;
      if(whitespace != null){
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

      for(int i = 0; i < text.Length; i++){
        if(text[i] == '\n'){
          text.Insert(i + 1, slashes);
        }
      }
    }

    #endregion

    #region IBulbItem Members

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
      get {
        return "Add xml-docs comments for uncaught exceptions";
      }
    }

    #endregion

    #region IContextAction Members

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items {
      get {
        return new IBulbItem[]{this};
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Examines the catches.
    /// </summary>
    /// <param name="tryStatement">The try statement.</param>
    /// <param name="exceptions">The exceptions.</param>
    /// <returns>The catches.</returns>
    static void ExamineCatches(ITryStatement tryStatement, IList<string[]> exceptions) {
      IList<ICatchClause> list = tryStatement.Catches;
      List<string> catches = new List<string>();

      foreach(ICatchClause clause in list) {
        IDeclaredType declaredType = clause.ExceptionType;

        if(declaredType == null) {
          break;
        }

        string clrName = declaredType.GetCLRName();

        if(!string.IsNullOrEmpty(clrName)) {
          catches.Add(clrName);
        }
      }

      for(int n = exceptions.Count - 1; n >= 0; n--) {
        string typeName = exceptions[n][0];

        if(catches.Contains(typeName)) {
          exceptions.RemoveAt(n);
        }
      }
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    static List<string[]> GetExceptions(XmlNodeList exceptionList) {
      List<string[]> result = new List<string[]>();

      foreach(XmlNode exceptionNode in exceptionList) {
        XmlAttribute attribute = exceptionNode.Attributes["cref"];

        if(attribute == null) {
          continue;
        }

        string typeName = attribute.Value;

        if(!string.IsNullOrEmpty(typeName)) {
          if(typeName.StartsWith("T:")) {
            typeName = typeName.Substring(2);
          }

          string[] entry = new string[] { typeName, exceptionNode.InnerXml };

          result.Add(entry);
        }
      }

      return result;
    }

    /// <summary>
    /// Determines whether the specified element is visible.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <param name="exceptions">The exceptions.</param>
    static void ProcessInvocation(IInvocationExpression invocationExpression, List<string[]> exceptions) {
      IReference reference = invocationExpression.InvokedExpression as IReference;
      if(reference == null) {
        return;
      }

      ResolveResult resolveResult = reference.Resolve();

      IDeclaredElement declaredElement = resolveResult.DeclaredElement;
      if(declaredElement == null) {
        return;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(true);
      if(xmlNode == null) {
        return;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if(exceptionList == null || exceptionList.Count == 0) {
        return;
      }

      ITreeNode node = invocationExpression as ITreeNode;
      if(node == null) {
        return;
      }

      List<string[]> ex = GetExceptions(exceptionList);

      RemoveCaught(node, ex);

      foreach(string[] exception in ex) {
        bool found = false;

        foreach(string[] e in exceptions) {
          if(e[0] == exception[0]) {
            found = true;
            break;
          }
        }

        if(!found) {
          exceptions.Add(exception);
        }
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Removes the caught.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="exceptions">The exceptions.</param>
    static void RemoveCaught(ITreeNode node, List<string[]> exceptions) {
      while(node != null) {
        ITryStatement tryStatement = node as ITryStatement;

        if(tryStatement != null) {
          ExamineCatches(tryStatement, exceptions);

          if(exceptions.Count == 0) {
            return;
          }
        }

        node = node.Parent;
      }
    }

    /// <summary>
    /// Determines whether this instance is documented.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <param name="exceptions">The exceptions.</param>
    public static void RemoveDocumented(IInvocationExpression invocationExpression, List<string[]> exceptions) {
      ITypeMemberDeclaration typeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclaration();
      if(typeMemberDeclaration == null) {
        return;
      }

      IDeclaredElement declaredElement = typeMemberDeclaration.DeclaredElement;
      if(declaredElement == null) {
        return;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(false);
      if(xmlNode == null) {
        return;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if(exceptionList == null || exceptionList.Count == 0) {
        return;
      }

      foreach(XmlNode node in exceptionList) {
        XmlAttribute attribute = node.Attributes["cref"];
        if(attribute == null) {
          continue;
        }

        string cref = attribute.Value;
        if(string.IsNullOrEmpty(cref)) {
          continue;
        }

        if(cref.StartsWith("T:")) {
          cref = cref.Substring(2);
        }

        foreach(string[] exception in exceptions) {
          if(exception[0] == cref) {
            exceptions.Remove(exception);
            break;
          }
        }
      }
    }

    #endregion

  }
}