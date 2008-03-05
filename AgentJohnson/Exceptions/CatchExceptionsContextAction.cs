using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Editor;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TextControl;
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Util;

namespace AgentJohnson.Exceptions {
  /// <summary>
  /// </summary>
  [ContextAction(Description="Generates try/catch clauses surrounding expressions", Name="Catch exceptions", Priority=-1, Group="C#")]
  public partial class CatchExceptionsContextAction : IContextAction, IBulbItem, IComparer<CatchExceptionsContextAction.Pair<string, Type>> {
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
    public CatchExceptionsContextAction(ISolution solution, ITextControl textControl) {
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

      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null){
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()){
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS){
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", Text))){
          psiManager.DoTransaction(delegate { Execute(solution, element); });
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
      Shell.Instance.AssertReadAccessAllowed();

      IElement element = GetElementAtCaret();
      if(element == null){
        return false;
      }

      ITreeNode node = element.ToTreeNode();
      if(node == null){
        return false;
      }

      IInvocationExpression invocationExpression = null;

      while(node != null){
        invocationExpression = node as IInvocationExpression;

        if(invocationExpression != null){
          break;
        }

        if(node is IStatement) {
          break;
        }

        node = node.Parent;
      }

      if(invocationExpression == null){
        return false;
      }

      return IsVisible(invocationExpression);
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
    /// Generates the function assert statements.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="element">The element.</param>
    void CatchExceptions(ISolution solution, IElement element) {
      IInvocationExpression invocationExpression = element as IInvocationExpression;
      if(invocationExpression == null){
        return;
      }

      IStatement statement = invocationExpression.GetContainingStatement();
      if(statement == null){
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(solution);
      if(factory == null){
        return;
      }

      IReference reference = invocationExpression.InvokedExpression as IReference;
      if(reference == null){
        return;
      }

      ResolveResult resolveResult = reference.Resolve();
      if(resolveResult == null){
        return;
      }

      IDeclaredElement declaredElement = resolveResult.DeclaredElement;
      if(declaredElement == null){
        return;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(true);
      if(xmlNode == null){
        return;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if(exceptionList == null || exceptionList.Count == 0){
        return;
      }

      List<Pair<string, Type>> exceptions = GetSortedExceptions(exceptionList);

      string code = "try { " + statement.GetText() + " } ";

      foreach(Pair<string, Type> exception in exceptions){
        code += "catch(" + exception.Key + ") { } ";
      }

      ITryStatement tryStatement = factory.CreateStatement(code) as ITryStatement;

      if(tryStatement == null){
        MessageBox.Show("Failed to create code.", "Agent Johnson");
        return;
      }

      IStatement result = statement.ReplaceBy(tryStatement);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null){
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if(formatter == null){
        return;
      }

      formatter.Optimize(result.GetContainingFile(), result.GetDocumentRange(), false, true, NullProgressIndicator.INSTANCE);
    }

    ///<summary>
    ///Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    ///</summary>
    ///<param name="y">The second object to compare.</param>
    ///<param name="x">The first object to compare.</param>
    int IComparer<Pair<string, Type>>.Compare(Pair<string, Type> x, Pair<string, Type> y) {
      if(x.Value == null){
        return -1;
      }

      if(y.Value == null){
        return 1;
      }

      if(x.Value.IsSubclassOf(y.Value)){
        return -1;
      }

      if(y.Value.IsSubclassOf(x.Value)){
        return 1;
      }

      return string.Compare(x.Key, y.Key);
    }

    /// <summary>
    /// Examines the catches.
    /// </summary>
    /// <param name="tryStatement">The try statement.</param>
    /// <param name="exceptions">The exceptions.</param>
    /// <returns>The catches.</returns>
    static void ExamineCatches(ITryStatement tryStatement, IList<string> exceptions) {
      IList<ICatchClause> list = tryStatement.Catches;
      List<string> catches = new List<string>();

      foreach(ICatchClause clause in list){
        IDeclaredType declaredType = clause.ExceptionType;

        if(declaredType == null){
          break;
        }

        string clrName = declaredType.GetCLRName();

        if(!string.IsNullOrEmpty(clrName)){
          catches.Add(clrName);
        }
      }

      for(int n = exceptions.Count - 1; n >= 0; n--){
        string type = exceptions[n];

        if(catches.Contains(type)){
          exceptions.RemoveAt(n);
        }
      }
    }

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="element">The element.</param>
    void Execute(ISolution solution, IElement element) {
      ITreeNode node = element.ToTreeNode();
      if(node == null){
        return;
      }

      IInvocationExpression invocationExpression = null;

      while(node != null){
        invocationExpression = node as IInvocationExpression;
        if(invocationExpression != null){
          break;
        }

        node = node.Parent;
      }

      if(invocationExpression == null){
        return;
      }

      CatchExceptions(solution, invocationExpression);
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    static List<string> GetExceptions(XmlNodeList exceptionList) {
      List<string> result = new List<string>();

      foreach(XmlNode exceptionNode in exceptionList){
        XmlAttribute attribute = exceptionNode.Attributes["cref"];

        if(attribute == null){
          continue;
        }

        string typeName = attribute.Value;

        if(!string.IsNullOrEmpty(typeName)){
          if(typeName.StartsWith("T:")){
            typeName = typeName.Substring(2);
          }

          result.Add(typeName);
        }
      }

      return result;
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    List<Pair<string, Type>> GetSortedExceptions(XmlNodeList exceptionList) {
      List<Pair<string, Type>> exceptions = new List<Pair<string, Type>>(exceptionList.Count);

      foreach(XmlNode exceptionNode in exceptionList){
        XmlAttribute attribute = exceptionNode.Attributes["cref"];
        if(attribute == null){
          continue;
        }

        string typeName = attribute.Value;
        if(string.IsNullOrEmpty(typeName)){
          continue;
        }

        if(typeName.StartsWith("T:")){
          typeName = typeName.Substring(2);
        }

        Type exceptionType = Type.GetType(typeName, false, false);

        Pair<string, Type> pair = new Pair<string, Type>();

        pair.Key = typeName;
        pair.Value = exceptionType;

        exceptions.Add(pair);
      }

      exceptions.Sort(this);

      return exceptions;
    }

    /// <summary>
    /// Determines whether the specified element is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified element is available; otherwise, <c>false</c>.
    /// </returns>
    static bool IsVisible(IElement element) {
      IInvocationExpression invocationExpression = element as IInvocationExpression;
      if(invocationExpression == null){
        return false;
      }

      IReference reference = invocationExpression.InvokedExpression as IReference;
      if(reference == null){
        return false;
      }

      ResolveResult resolveResult = reference.Resolve();

      IDeclaredElement declaredElement = resolveResult.DeclaredElement;
      if(declaredElement == null){
        return false;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(true);
      if(xmlNode == null){
        return false;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if(exceptionList == null || exceptionList.Count == 0){
        return false;
      }

      ITreeNode node = element as ITreeNode;
      if(node == null){
        return false;
      }

      List<string> exceptions = GetExceptions(exceptionList);

      while(node != null){
        ITryStatement tryStatement = node as ITryStatement;

        if(tryStatement != null){
          ExamineCatches(tryStatement, exceptions);

          if(exceptions.Count == 0){
            return false;
          }
        }

        node = node.Parent;
      }

      return exceptions.Count > 0;
    }

    #endregion

    #region IBulbItem Members

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
      get {
        return "Catch exceptions";
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
  }
}