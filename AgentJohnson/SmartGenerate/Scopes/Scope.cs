// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scope.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the scope class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Scopes
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the scope class.
  /// </summary>
  public class Scope
  {
    #region Public Methods

    /// <summary>
    /// Gets the nearest variable.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// The populated list.
    /// </returns>
    public static List<ScopeEntry> Populate(IElement element)
    {
      var result = new List<ScopeEntry>();
      var node = element.ToTreeNode();

      while (node != null)
      {
        GetLocalVariable(result, node);

        var prevSibling = node.PrevSibling;

        if (prevSibling == null)
        {
          node = node.Parent;

          GetForEach(result, node);
          GetFor(result, node);
          GetFunctionParameters(result, node);
          GetUsing(result, node);
        }
        else
        {
          node = prevSibling;
        }
      }

      return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the 'for' loop.
    /// </summary>
    /// <param name="entries">
    /// The entries.
    /// </param>
    /// <param name="node">
    /// The tree node.
    /// </param>
    private static void GetFor(List<ScopeEntry> entries, ITreeNode node)
    {
      var forStatement = node as IForStatement;
      if (forStatement == null)
      {
        return;
      }

      foreach (var localVariableDeclaration in forStatement.InitializerDeclarations)
      {
        var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        var entry = new ScopeEntry
        {
          Element = forStatement,
          Name = localVariable.ShortName,
          Type = localVariable.Type,
          IsAssigned = true
        };

        entries.Add(entry);
      }
    }

    /// <summary>
    /// Gets for each.
    /// </summary>
    /// <param name="entries">
    /// The entries.
    /// </param>
    /// <param name="treeNode">
    /// The tree node.
    /// </param>
    private static void GetForEach(List<ScopeEntry> entries, ITreeNode treeNode)
    {
      var foreachStatement = treeNode as IForeachStatement;
      if (foreachStatement == null)
      {
        return;
      }

      var foreachVariableDeclaration = foreachStatement.IteratorDeclaration;
      if (foreachVariableDeclaration == null)
      {
        return;
      }

      var foreachVariableDeclarationNode = foreachVariableDeclaration.ToTreeNode();
      if (foreachVariableDeclarationNode == null)
      {
        return;
      }

      IType type = null;

      if (foreachVariableDeclaration.IsVar)
      {
        var typeOwner = foreachVariableDeclaration as ITypeOwner;
        if (typeOwner != null)
        {
          type = typeOwner.Type;
        }
      }
      else
      {
        type = CSharpTypeFactory.CreateType(foreachVariableDeclarationNode.TypeUsage);
      }

      var entry = new ScopeEntry
      {
        Element = foreachStatement,
        Name = foreachStatement.IteratorName,
        Type = type,
        IsAssigned = true
      };

      entries.Add(entry);
    }

    /// <summary>
    /// Gets the function parameters.
    /// </summary>
    /// <param name="entries">
    /// The entries.
    /// </param>
    /// <param name="node">
    /// The tree node.
    /// </param>
    private static void GetFunctionParameters(List<ScopeEntry> entries, ITreeNode node)
    {
      var parametersOwner = node as IParametersOwner;
      if (parametersOwner == null)
      {
        return;
      }

      if (parametersOwner.Parameters.Count <= 0)
      {
        return;
      }

      foreach (var parameter in parametersOwner.Parameters)
      {
        var entry = new ScopeEntry
        {
          Element = parameter as IParameterDeclaration,
          Name = parameter.ShortName,
          Type = parameter.Type,
          IsAssigned = true
        };

        entries.Add(entry);
      }
    }

    /// <summary>
    /// Gets the local variable.
    /// </summary>
    /// <param name="entries">
    /// The entries.
    /// </param>
    /// <param name="node">
    /// The tree node.
    /// </param>
    private static void GetLocalVariable(List<ScopeEntry> entries, ITreeNode node)
    {
      var declarationStatement = node as IDeclarationStatement;
      if (declarationStatement == null)
      {
        return;
      }

      foreach (var localVariableDeclaration in declarationStatement.VariableDeclarations)
      {
        var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        var entry = new ScopeEntry
        {
          Element = localVariableDeclaration,
          Name = localVariable.ShortName,
          Type = localVariable.Type,
          IsAssigned = localVariableDeclaration.Initial != null
        };

        entries.Add(entry);
      }
    }

    /// <summary>
    /// Gets 'using' statement.
    /// </summary>
    /// <param name="entries">
    /// The entries.
    /// </param>
    /// <param name="node">
    /// The tree node.
    /// </param>
    private static void GetUsing(List<ScopeEntry> entries, ITreeNode node)
    {
      var usingStatement = node as IUsingStatement;
      if (usingStatement == null)
      {
        return;
      }

      var localVariableDeclarations = usingStatement.VariableDeclarations;

      foreach (var localVariableDeclaration in localVariableDeclarations)
      {
        var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        var entry = new ScopeEntry
        {
          Element = localVariableDeclaration,
          Name = localVariable.ShortName,
          Type = localVariable.Type,
          IsAssigned = localVariableDeclaration.Initial != null
        };

        entries.Add(entry);
      }
    }

    #endregion
  }
}