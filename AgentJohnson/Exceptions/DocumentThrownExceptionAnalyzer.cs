// <copyright file="DocumentThrownExceptionAnalyzer.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Exceptions
{
  using System.Collections.Generic;
  using System.Xml;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the document thrown exception analyzer class.
  /// </summary>
  public class DocumentThrownExceptionAnalyzer : IStatementAnalyzer
  {
    #region Fields

    /// <summary>
    /// The solution.
    /// </summary>
    private readonly ISolution solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public DocumentThrownExceptionAnalyzer(ISolution solution)
    {
      this.solution = solution;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution
    {
      get
      {
        return this.solution;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>Returns the suggestion bases.</returns>
    public SuggestionBase[] Analyze(IStatement statement)
    {
      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      IThrowStatement throwStatement = statement as IThrowStatement;
      if (throwStatement != null)
      {
        suggestions.AddRange(this.AnalyzeThrowStatement(throwStatement));
      }

      return suggestions.ToArray();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Determines whether [is catch statement] [the specified throw statement].
    /// </summary>
    /// <param name="throwStatement">The throw statement.</param>
    /// <param name="tryStatement">The try statement.</param>
    /// <returns>
    /// 	<c>true</c> if [is catch statement] [the specified throw statement]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsCatchStatement(IThrowStatement throwStatement, ITryStatement tryStatement)
    {
      IList<ICatchClause> catchClauses = tryStatement.Catches;

      foreach (ICatchClause catchClause in catchClauses)
      {
        if (throwStatement.Exception == catchClause.ExceptionType)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether the specified throw statement is caught.
    /// </summary>
    /// <param name="throwStatement">The throw statement.</param>
    /// <returns>
    /// 	<c>true</c> if the specified throw statement is caught; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsExceptionCaught(IThrowStatement throwStatement)
    {
      ITreeNode node = throwStatement.ToTreeNode();
      if (node == null)
      {
        return true;
      }

      while (node != null)
      {
        ITryStatement tryStatement = node as ITryStatement;

        if (tryStatement != null)
        {
          if (IsCatchStatement(throwStatement, tryStatement))
          {
            return true;
          }
        }

        node = node.Parent;
      }

      return false;
    }

    /// <summary>
    /// Determines whether this instance is documented.
    /// </summary>
    /// <param name="throwStatement">The throw statement.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is documented; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsThrowStatementDocumented(IThrowStatement throwStatement)
    {
      ITypeMemberDeclaration typeMemberDeclaration = throwStatement.GetContainingTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return true;
      }

      IDeclaredElement declaredElement = typeMemberDeclaration.DeclaredElement;
      if (declaredElement == null)
      {
        return true;
      }

      XmlNode xmlNode = declaredElement.GetXMLDoc(false);
      if (xmlNode == null)
      {
        return false;
      }

      XmlNodeList exceptionList = xmlNode.SelectNodes("exception");
      if (exceptionList == null || exceptionList.Count == 0)
      {
        return false;
      }

      ICSharpExpression exception = throwStatement.Exception;
      if (exception == null)
      {
        return true;
      }

      IType type = exception.Type();

      string exceptionTypeName = type.GetLongPresentableName(throwStatement.Language);

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

        if (cref == exceptionTypeName)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Analyzes the documented.
    /// </summary>
    /// <param name="throwStatement">The statement.</param>
    /// <returns>Returns a list of suggestion bases.</returns>
    private IEnumerable<SuggestionBase> AnalyzeThrowStatement(IThrowStatement throwStatement)
    {
      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      if (IsExceptionCaught(throwStatement))
      {
        return suggestions;
      }

      if (IsThrowStatementDocumented(throwStatement))
      {
        return suggestions;
      }

      suggestions.Add(new DocumentThrownExceptionWarning(this.solution, throwStatement));

      return suggestions;
    }

    #endregion
  }
}