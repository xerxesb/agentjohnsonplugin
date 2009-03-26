// <copyright file="DocumentUncaughtExceptionsContextAction.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Exceptions
{
  using System.Collections.Generic;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Xml;
  using JetBrains.Application;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.ExtensionsAPI;
  using JetBrains.ReSharper.Psi.Resolve;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the document uncaught exceptions context action class.
  /// </summary>
  [ContextAction(Description = "Document uncaught exceptions that are thrown in called functions", Name = "Add xml-docs comments for uncaught exceptions", Priority = -1, Group = "C#")]
  public class DocumentUncaughtExceptionsContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentUncaughtExceptionsContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public DocumentUncaughtExceptionsContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
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

      Execute(invocationExpression);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The context action text.</value>
    protected override string GetText()
    {
      return "Add xml-docs comments for uncaught exceptions";
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

      List<string[]> exceptions = new List<string[]>();

      GetExceptions(invocationExpression, exceptions);

      return exceptions.Count > 0;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Examines the catches.
    /// </summary>
    /// <param name="tryStatement">The try statement.</param>
    /// <param name="exceptions">The exceptions.</param>
    private static void ExamineCatches(ITryStatement tryStatement, IList<string[]> exceptions)
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
        string typeName = exceptions[n][0];

        if (catches.Contains(typeName))
        {
          exceptions.RemoveAt(n);
        }
      }
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    private static void Execute(IInvocationExpression invocationExpression)
    {
      ITypeMemberDeclaration typeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return;
      }

      IDocCommentBlockOwnerNode docCommentBlockOwnerNode = typeMemberDeclaration as IDocCommentBlockOwnerNode;
      if (docCommentBlockOwnerNode == null)
      {
        return;
      }

      ITreeNode anchor = typeMemberDeclaration.ToTreeNode();
      if (anchor == null)
      {
        return;
      }

      List<string[]> exceptions = new List<string[]>();

      GetExceptions(invocationExpression, exceptions);

      StringBuilder text = new StringBuilder();

      foreach (string[] exception in exceptions)
      {
        string t = exception[1];

        t = Regex.Replace(t, "<paramref name=\"([^\"]*)\" />", "$1");

        text.Append("\r\n <exception cref=\"" + exception[0] + "\">" + t + "</exception>");
      }

      string indent = GetIndent(anchor);

      InsertSlashes(text, indent);

      IDocCommentBlockNode docCommentBlockNode = docCommentBlockOwnerNode.GetDocCommentBlockNode();
      if (docCommentBlockNode != null)
      {
        string docCommentText = GetDocCommentText(docCommentBlockNode);

        text.Insert(0, docCommentText);
      }
      else
      {
        text.Remove(0, 1);
      }

      text.Append("\nvoid foo(){}");

      ICSharpTypeMemberDeclaration declaration = CSharpElementFactory.GetInstance(typeMemberDeclaration.GetPsiModule()).CreateTypeMemberDeclaration(text.ToString());
      if (declaration == null)
      {
        return;
      }

      IDocCommentBlockNode node = SharedImplUtil.GetDocCommentBlockNode(declaration.ToTreeNode());
      if (node == null)
      {
        return;
      }

      docCommentBlockOwnerNode.SetDocCommentBlockNode(node);
    }

    /// <summary>
    /// Gets the doc comment text.
    /// </summary>
    /// <param name="docCommentBlockNode">The doc comment block node.</param>
    /// <returns>The doc comment text.</returns>
    private static string GetDocCommentText(IElement docCommentBlockNode)
    {
      return docCommentBlockNode.GetText();
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <param name="exceptions">The exceptions.</param>
    private static void GetExceptions(IInvocationExpression invocationExpression, List<string[]> exceptions)
    {
      ProcessInvocation(invocationExpression, exceptions);

      if (exceptions.Count == 0)
      {
        return;
      }

      RemoveDocumented(invocationExpression, exceptions);
    }

    /// <summary>
    /// Gets the exceptions.
    /// </summary>
    /// <param name="exceptionList">The exception list.</param>
    /// <returns>The exceptions.</returns>
    private static List<string[]> GetExceptions(XmlNodeList exceptionList)
    {
      List<string[]> result = new List<string[]>();

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

          string[] entry = new[]
          {
            typeName, exceptionNode.InnerXml
          };

          result.Add(entry);
        }
      }

      return result;
    }

    /// <summary>
    /// Gets the indent.
    /// </summary>
    /// <param name="anchor">The anchor.</param>
    /// <returns>The indent.</returns>
    private static string GetIndent(IElement anchor)
    {
      string indent = string.Empty;

      IWhitespaceNode whitespace = anchor.ToTreeNode().PrevSibling as IWhitespaceNode;
      if (whitespace != null)
      {
        indent = whitespace.GetText();
      }

      return indent;
    }

    /// <summary>
    /// Inserts the slashes.
    /// </summary>
    /// <param name="text">The text to insert slashes into.</param>
    /// <param name="indent">The indent.</param>
    private static void InsertSlashes(StringBuilder text, string indent)
    {
      string slashes = indent + "///";

      for (int i = 0; i < text.Length; i++)
      {
        if (text[i] == '\n')
        {
          text.Insert(i + 1, slashes);
        }
      }
    }

    /// <summary>
    /// Determines whether the specified element is visible.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <param name="exceptions">The exceptions.</param>
    private static void ProcessInvocation(IInvocationExpression invocationExpression, List<string[]> exceptions)
    {
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

      ITreeNode node = invocationExpression as ITreeNode;
      if (node == null)
      {
        return;
      }

      List<string[]> ex = GetExceptions(exceptionList);

      RemoveCaught(node, ex);

      foreach (string[] exception in ex)
      {
        bool found = false;

        foreach (string[] e in exceptions)
        {
          if (e[0] == exception[0])
          {
            found = true;
            break;
          }
        }

        if (!found)
        {
          exceptions.Add(exception);
        }
      }
    }

    /// <summary>
    /// Removes the caught.
    /// </summary>
    /// <param name="node">The tree node.</param>
    /// <param name="exceptions">The exceptions.</param>
    private static void RemoveCaught(ITreeNode node, List<string[]> exceptions)
    {
      while (node != null)
      {
        ITryStatement tryStatement = node as ITryStatement;

        if (tryStatement != null)
        {
          ExamineCatches(tryStatement, exceptions);

          if (exceptions.Count == 0)
          {
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
    private static void RemoveDocumented(IInvocationExpression invocationExpression, List<string[]> exceptions)
    {
      ITypeMemberDeclaration typeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return;
      }

      IDeclaredElement declaredElement = typeMemberDeclaration.DeclaredElement;
      if (declaredElement == null)
      {
        return;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(false);
      if (xmlNode == null)
      {
        return;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return;
      }

      foreach (XmlNode node in exceptionList)
      {
        XmlAttribute attribute = node.Attributes["cref"];
        if (attribute == null)
        {
          continue;
        }

        string cref = attribute.Value;
        if (string.IsNullOrEmpty(cref))
        {
          continue;
        }

        if (cref.StartsWith("T:"))
        {
          cref = cref.Substring(2);
        }

        foreach (string[] exception in exceptions)
        {
          if (exception[0] == cref)
          {
            exceptions.Remove(exception);
            break;
          }
        }
      }
    }

    #endregion
  }
}