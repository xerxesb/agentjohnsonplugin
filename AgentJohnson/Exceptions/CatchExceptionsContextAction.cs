// <copyright file="CatchExceptionsContextAction.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Exceptions
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using System.Xml;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Resolve;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the catch exceptions context action class.
  /// </summary>
  [ContextAction(Description = "Generates try/catch clauses surrounding expressions", Name = "Catch exceptions", Priority = -1, Group = "C#")]
  public partial class CatchExceptionsContextAction : ContextActionBase, IComparer<CatchExceptionsContextAction.Pair<string, Type>>
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CatchExceptionsContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public CatchExceptionsContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Public methods

    ///<summary>
    ///Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    ///</summary>
    ///<param name="y">The second object to compare.</param>
    ///<param name="x">The first object to compare.</param>
    public int Compare(Pair<string, Type> x, Pair<string, Type> y)
    {
      if (x.Value == null)
      {
        return -1;
      }

      if (y.Value == null)
      {
        return 1;
      }

      if (x.Value.IsSubclassOf(y.Value))
      {
        return -1;
      }

      if (y.Value.IsSubclassOf(x.Value))
      {
        return 1;
      }

      return string.Compare(x.Key, y.Key);
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      ITreeNode node = element.ToTreeNode();
      if (node == null)
      {
        return;
      }

      IInvocationExpression invocationExpression = null;

      while (node != null)
      {
        invocationExpression = node as IInvocationExpression;
        if (invocationExpression != null)
        {
          break;
        }

        node = node.Parent;
      }

      if (invocationExpression == null)
      {
        return;
      }

      this.CatchExceptions(invocationExpression);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    protected override string GetText()
    {
      return "Catch exceptions [Agent Johnson]";
    }

    /// <summary>
    /// Determines whether the specified cache is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified cache is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      ITreeNode node = element.ToTreeNode();
      if (node == null)
      {
        return false;
      }

      IInvocationExpression invocationExpression = null;

      while (node != null)
      {
        invocationExpression = node as IInvocationExpression;

        if (invocationExpression != null)
        {
          break;
        }

        if (node is IStatement)
        {
          break;
        }

        node = node.Parent;
      }

      if (invocationExpression == null)
      {
        return false;
      }

      return IsVisible(invocationExpression);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Examines the catches.
    /// </summary>
    /// <param name="tryStatement">The try statement.</param>
    /// <param name="exceptions">The exceptions.</param>
    /// <returns>The catches.</returns>
    private static void ExamineCatches(ITryStatement tryStatement, IList<string> exceptions)
    {
      IList<ICatchClause> list = tryStatement.Catches;
      List<string> catches = new List<string>();

      foreach (ICatchClause clause in list)
      {
        IDeclaredType declaredType = clause.ExceptionType;

        if (declaredType == null)
        {
          break;
        }

        string clrName = declaredType.GetCLRName();

        if (!string.IsNullOrEmpty(clrName))
        {
          catches.Add(clrName);
        }
      }

      for (int n = exceptions.Count - 1; n >= 0; n--)
      {
        string type = exceptions[n];

        if (catches.Contains(type))
        {
          exceptions.RemoveAt(n);
        }
      }
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    private static List<string> GetExceptions(XmlNodeList exceptionList)
    {
      List<string> result = new List<string>();

      foreach (XmlNode exceptionNode in exceptionList)
      {
        XmlAttribute attribute = exceptionNode.Attributes["cref"];

        if (attribute == null)
        {
          continue;
        }

        string typeName = attribute.Value;

        if (!string.IsNullOrEmpty(typeName))
        {
          if (typeName.StartsWith("T:"))
          {
            typeName = typeName.Substring(2);
          }

          result.Add(typeName);
        }
      }

      return result;
    }

    /// <summary>
    /// Determines whether the specified element is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified element is available; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsVisible(IElement element)
    {
      IInvocationExpression invocationExpression = element as IInvocationExpression;
      if (invocationExpression == null)
      {
        return false;
      }

      IReferenceExpression reference = invocationExpression.InvokedExpression as IReferenceExpression;
      if (reference == null)
      {
        return false;
      }

      IResolveResult resolveResult = reference.Reference.Resolve();

      IDeclaredElement declaredElement = resolveResult.DeclaredElement;
      if (declaredElement == null)
      {
        return false;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(true);
      if (xmlNode == null)
      {
        return false;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return false;
      }

      ITreeNode node = element as ITreeNode;
      if (node == null)
      {
        return false;
      }

      List<string> exceptions = GetExceptions(exceptionList);

      while (node != null)
      {
        ITryStatement tryStatement = node as ITryStatement;

        if (tryStatement != null)
        {
          ExamineCatches(tryStatement, exceptions);

          if (exceptions.Count == 0)
          {
            return false;
          }
        }

        node = node.Parent;
      }

      return exceptions.Count > 0;
    }

    /// <summary>
    /// Generates the function assert statements.
    /// </summary>
    /// <param name="element">The element.</param>
    private void CatchExceptions(IElement element)
    {
      IInvocationExpression invocationExpression = element as IInvocationExpression;
      if (invocationExpression == null)
      {
        return;
      }

      IStatement statement = invocationExpression.GetContainingStatement();
      if (statement == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(statement.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      IReferenceExpression reference = invocationExpression.InvokedExpression as IReferenceExpression;
      if (reference == null)
      {
        return;
      }

      IResolveResult resolveResult = reference.Reference.Resolve();

      IDeclaredElement declaredElement = resolveResult.DeclaredElement;
      if (declaredElement == null)
      {
        return;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(true);
      if (xmlNode == null)
      {
        return;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return;
      }

      List<Pair<string, Type>> exceptions = this.GetSortedExceptions(exceptionList);

      string code = "try { " + statement.GetText() + " } ";

      foreach (Pair<string, Type> exception in exceptions)
      {
        code += "catch(" + exception.Key + ") { } ";
      }

      ITryStatement tryStatement = factory.CreateStatement(code) as ITryStatement;

      if (tryStatement == null)
      {
        MessageBox.Show("Failed to create code.", "Agent Johnson");
        return;
      }

      IStatement result = statement.ReplaceBy(tryStatement);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    private List<Pair<string, Type>> GetSortedExceptions(XmlNodeList exceptionList)
    {
      List<Pair<string, Type>> exceptions = new List<Pair<string, Type>>(exceptionList.Count);

      foreach (XmlNode exceptionNode in exceptionList)
      {
        XmlAttribute attribute = exceptionNode.Attributes["cref"];
        if (attribute == null)
        {
          continue;
        }

        string typeName = attribute.Value;
        if (string.IsNullOrEmpty(typeName))
        {
          continue;
        }

        if (typeName.StartsWith("T:"))
        {
          typeName = typeName.Substring(2);
        }

        Type exceptionType = Type.GetType(typeName, false, false);

        Pair<string, Type> pair = new Pair<string, Type>
        {
          Key = typeName,
          Value = exceptionType
        };

        exceptions.Add(pair);
      }

      exceptions.Sort(this);

      return exceptions;
    }

    #endregion
  }
}