// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatementUtil.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the statement utility class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the statement utility class.
  /// </summary>
  public static class StatementUtil
  {
    #region Public Methods

    /// <summary>
    /// Gets the new statement position.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// The new statement position.
    /// </returns>
    public static TextRange GetNewStatementPosition(IElement element)
    {
      var block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return TextRange.InvalidRange;
      }

      var statement = element.GetContainingElement(typeof(IStatement), true) as IStatement;
      if (statement != null && statement != block && block.Contains(statement))
      {
        var range = statement.GetTreeTextRange();

        return new TextRange(range.EndOffset + 1);
      }

      return TextRange.InvalidRange;
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// </returns>
    [CanBeNull]
    public static IStatement GetPreviousStatement(IElement element)
    {
      var block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return null;
      }

      var statement = element.GetContainingElement(typeof(IStatement), true);
      if (statement != null && !block.Contains(statement))
      {
        return null;
      }

      return GetPreviousStatement(block, element);
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="block">
    /// The block.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    public static IStatement GetPreviousStatement(IBlock block, IElement element)
    {
      IStatement result = null;

      var caret = element.GetTreeStartOffset();

      foreach (var statement in block.Statements)
      {
        if (statement.GetTreeStartOffset() > caret)
        {
          break;
        }

        result = statement;
      }

      return result;
    }

    /// <summary>
    /// Determines whether [is after last statement] [the specified element].
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is after last statement] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAfterLastStatement(IElement element)
    {
      var block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return false;
      }

      if (block.Statements.Count <= 0)
      {
        return true;
      }

      var statement = block.Statements[block.Statements.Count - 1];
      var range = statement.GetDocumentRange();

      var end = range.TextRange.StartOffset + range.TextRange.Length;
      if (end > element.GetTreeTextRange().StartOffset)
      {
        return false;
      }

      return true;
    }

    #endregion
  }
}