using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the statement utility class.
  /// </summary>
  public static class StatementUtil {
    #region Public methods

    /// <summary>
    /// Gets the new statement position.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The new statement position.</returns>
    public static TextRange GetNewStatementPosition(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return TextRange.InvalidRange;
      }

      IStatement statement = element.GetContainingElement(typeof(IStatement), true) as IStatement;
      if(statement != null && statement != block && block.Contains(statement)) {
        TextRange range = statement.GetTreeTextRange();

        return new TextRange(range.EndOffset + 1);
      }

      return TextRange.InvalidRange;
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public static IStatement GetPreviousStatement(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return null;
      }

      IElement statement = element.GetContainingElement(typeof(IStatement), true);
      if(statement != null && !block.Contains(statement)) {
        return null;
      }

      return GetPreviousStatement(block, element);
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="block">The block.</param>
    public static IStatement GetPreviousStatement(IBlock block, IElement element) {
      IStatement result = null;

      int caret = element.GetTreeStartOffset();

      foreach(IStatement statement in block.Statements) {
        if(statement.GetTreeStartOffset() > caret) {
          break;
        }

        result = statement;
      }

      return result;
    }

    #endregion
  }
}