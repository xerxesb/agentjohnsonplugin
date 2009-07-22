// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatchExceptionsContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the catch exceptions context action class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Exceptions
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using System.Xml;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the catch exceptions context action class.
  /// </summary>
  [ContextAction(Description = "Generates try/catch clauses surrounding expressions", Name = "Catch exceptions", Priority = -1, Group = "C#")]
  public partial class CatchExceptionsContextAction : ContextActionBase, IComparer<CatchExceptionsContextAction.Pair<string, Type>>
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CatchExceptionsContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public CatchExceptionsContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Implemented Interfaces

    #region IComparer<Pair<string,Type>>

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">
    /// The first object to compare.
    /// </param>
    /// <param name="y">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// The compare.
    /// </returns>
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

    #endregion

    #region Methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      var node = element.ToTreeNode();
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
    /// <value>
    /// The text.
    /// </value>
    /// <returns>
    /// The get text.
    /// </returns>
    protected override string GetText()
    {
      return "Catch exceptions [Agent Johnson]";
    }

    /// <summary>
    /// Determines whether the specified cache is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified cache is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      var node = element.ToTreeNode();
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

    /// <summary>
    /// Examines the catches.
    /// </summary>
    /// <param name="tryStatement">
    /// The try statement.
    /// </param>
    /// <param name="exceptions">
    /// The exceptions.
    /// </param>
    private static void ExamineCatches(ITryStatement tryStatement, IList<string> exceptions)
    {
      var list = tryStatement.Catches;
      var catches = new List<string>();

      foreach (var clause in list)
      {
        var declaredType = clause.ExceptionType;

        if (declaredType == null)
        {
          break;
        }

        var clrName = declaredType.GetCLRName();

        if (!string.IsNullOrEmpty(clrName))
        {
          catches.Add(clrName);
        }
      }

      for (var n = exceptions.Count - 1; n >= 0; n--)
      {
        var type = exceptions[n];

        if (catches.Contains(type))
        {
          exceptions.RemoveAt(n);
        }
      }
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">
    /// The exception list.
    /// </param>
    /// <returns>
    /// The exceptions.
    /// </returns>
    private static List<string> GetExceptions(XmlNodeList exceptionList)
    {
      var result = new List<string>();

      foreach (XmlNode exceptionNode in exceptionList)
      {
        var attribute = exceptionNode.Attributes["cref"];

        if (attribute == null)
        {
          continue;
        }

        var typeName = attribute.Value;

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
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified element is available; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsVisible(IElement element)
    {
      var invocationExpression = element as IInvocationExpression;
      if (invocationExpression == null)
      {
        return false;
      }

      var reference = invocationExpression.InvokedExpression as IReferenceExpression;
      if (reference == null)
      {
        return false;
      }

      var resolveResult = reference.Reference.Resolve();

      var declaredElement = resolveResult.DeclaredElement;
      if (declaredElement == null)
      {
        return false;
      }

      var xmlNode = declaredElement.GetXMLDoc(true);
      if (xmlNode == null)
      {
        return false;
      }

      var exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return false;
      }

      var node = element as ITreeNode;
      if (node == null)
      {
        return false;
      }

      var exceptions = GetExceptions(exceptionList);

      while (node != null)
      {
        var tryStatement = node as ITryStatement;

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
    /// <param name="element">
    /// The element.
    /// </param>
    private void CatchExceptions(IElement element)
    {
      var invocationExpression = element as IInvocationExpression;
      if (invocationExpression == null)
      {
        return;
      }

      var statement = invocationExpression.GetContainingStatement();
      if (statement == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(statement.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var reference = invocationExpression.InvokedExpression as IReferenceExpression;
      if (reference == null)
      {
        return;
      }

      var resolveResult = reference.Reference.Resolve();

      var declaredElement = resolveResult.DeclaredElement;
      if (declaredElement == null)
      {
        return;
      }

      var xmlNode = declaredElement.GetXMLDoc(true);
      if (xmlNode == null)
      {
        return;
      }

      var exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return;
      }

      var exceptions = this.GetSortedExceptions(exceptionList);

      var code = "try { " + statement.GetText() + " } ";

      foreach (var exception in exceptions)
      {
        code += "catch(" + exception.Key + ") { } ";
      }

      var tryStatement = factory.CreateStatement(code) as ITryStatement;

      if (tryStatement == null)
      {
        MessageBox.Show("Failed to create code.", "Agent Johnson");
        return;
      }

      IStatement result = statement.ReplaceBy(tryStatement);

      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      var formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      var range = result.GetDocumentRange();
      var marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">
    /// The exception list.
    /// </param>
    /// <returns>
    /// The exceptions.
    /// </returns>
    private List<Pair<string, Type>> GetSortedExceptions(XmlNodeList exceptionList)
    {
      var exceptions = new List<Pair<string, Type>>(exceptionList.Count);

      foreach (XmlNode exceptionNode in exceptionList)
      {
        var attribute = exceptionNode.Attributes["cref"];
        if (attribute == null)
        {
          continue;
        }

        var typeName = attribute.Value;
        if (string.IsNullOrEmpty(typeName))
        {
          continue;
        }

        if (typeName.StartsWith("T:"))
        {
          typeName = typeName.Substring(2);
        }

        var exceptionType = Type.GetType(typeName, false, false);

        var pair = new Pair<string, Type>
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