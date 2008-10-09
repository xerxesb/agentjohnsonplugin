// <copyright file="Scope.cs" company="Jakob Christensen">
//   Copyright (c) Jakob Christensen. All rights reserved.
// </copyright>

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
    #region Public methods

    /// <summary>
    /// Gets the nearest variable.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The populated list.</returns>
    public static List<ScopeEntry> Populate(IElement element)
    {
      List<ScopeEntry> result = new List<ScopeEntry>();
      ITreeNode node = element.ToTreeNode();

      while (node != null)
      {
        GetLocalVariable(result, node);

        ITreeNode prevSibling = node.PrevSibling;

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

    #region Private methods

    /// <summary>
    /// Gets the 'for' loop.
    /// </summary>
    /// <param name="entries">The entries.</param>
    /// <param name="node">The tree node.</param>
    private static void GetFor(List<ScopeEntry> entries, ITreeNode node)
    {
      IForStatement forStatement = node as IForStatement;
      if (forStatement == null)
      {
        return;
      }

      foreach (ILocalVariableDeclaration localVariableDeclaration in forStatement.InitializerDeclarations)
      {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        ScopeEntry entry = new ScopeEntry
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
    /// <param name="entries">The entries.</param>
    /// <param name="treeNode">The tree node.</param>
    private static void GetForEach(List<ScopeEntry> entries, ITreeNode treeNode)
    {
      IForeachStatement foreachStatement = treeNode as IForeachStatement;
      if (foreachStatement == null)
      {
        return;
      }

      IForeachVariableDeclaration foreachVariableDeclaration = foreachStatement.IteratorDeclaration;
      if (foreachVariableDeclaration == null)
      {
        return;
      }

      IForeachVariableDeclarationNode foreachVariableDeclarationNode = foreachVariableDeclaration.ToTreeNode();
      if (foreachVariableDeclarationNode == null)
      {
        return;
      }

      IType type = null;

      if (foreachVariableDeclaration.IsVar)
      {
        ITypeOwner typeOwner = foreachVariableDeclaration as ITypeOwner;
        if (typeOwner != null)
        {
          type = typeOwner.Type;
        }
      }
      else
      {
        type = CSharpTypeFactory.CreateType(foreachVariableDeclarationNode.TypeUsage);
      }

      ScopeEntry entry = new ScopeEntry
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
    /// <param name="entries">The entries.</param>
    /// <param name="node">The tree node.</param>
    private static void GetFunctionParameters(List<ScopeEntry> entries, ITreeNode node)
    {
      IParametersOwner parametersOwner = node as IParametersOwner;
      if (parametersOwner == null)
      {
        return;
      }

      if (parametersOwner.Parameters.Count <= 0)
      {
        return;
      }

      foreach (IParameter parameter in parametersOwner.Parameters)
      {
        ScopeEntry entry = new ScopeEntry
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
    /// <param name="entries">The entries.</param>
    /// <param name="node">The tree node.</param>
    private static void GetLocalVariable(List<ScopeEntry> entries, ITreeNode node)
    {
      IDeclarationStatement declarationStatement = node as IDeclarationStatement;
      if (declarationStatement == null)
      {
        return;
      }

      foreach (ILocalVariableDeclaration localVariableDeclaration in declarationStatement.VariableDeclarations)
      {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        ScopeEntry entry = new ScopeEntry
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
    /// <param name="entries">The entries.</param>
    /// <param name="node">The tree node.</param>
    private static void GetUsing(List<ScopeEntry> entries, ITreeNode node)
    {
      IUsingStatement usingStatement = node as IUsingStatement;
      if (usingStatement == null)
      {
        return;
      }

      IList<ILocalVariableDeclaration> localVariableDeclarations = usingStatement.VariableDeclarations;

      foreach (ILocalVariableDeclaration localVariableDeclaration in localVariableDeclarations)
      {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        ScopeEntry entry = new ScopeEntry
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