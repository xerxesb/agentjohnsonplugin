using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Scopes {
  /// <summary>
  /// Defines the scope class.
  /// </summary>
  public class Scope {
    #region Public methods

    /// <summary>
    /// Gets the nearest variable.
    /// </summary>
    /// <param name="element">The element.</param>
    public static List<ScopeEntry> Populate(IElement element) {
      List<ScopeEntry> result = new List<ScopeEntry>();
      ITreeNode node = element.ToTreeNode();

      while(node != null) {
        GetLocalVariable(result, node);

        ITreeNode prevSibling = node.PrevSibling;

        if(prevSibling == null) {
          node = node.Parent;

          GetForEach(result, node);
          GetFor(result, node);
          GetFunctionParameters(result, node);
        }
        else {
          node = prevSibling;
        }
      }

      return result;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets for.
    /// </summary>
    /// <param name="entries">The entries.</param>
    /// <param name="node">The node.</param>
    static void GetFor(List<ScopeEntry> entries, ITreeNode node) {
      IForStatement forStatement = node as IForStatement;
      if(forStatement == null) {
        return;
      }

      IList<ILocalVariableDeclaration> initializerDeclarations = forStatement.InitializerDeclarations;
      if(initializerDeclarations.Count != 1) {
        return;
      }

      ILocalVariableDeclaration localVariableDeclaration = initializerDeclarations[0];
      if(localVariableDeclaration == null) {
        return;
      }

      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if(localVariable == null) {
        return;
      }

      ScopeEntry entry = new ScopeEntry {
        Element = forStatement,
        Name = localVariable.ShortName,
        Type = localVariable.Type,
        IsAssigned = true
      };

      entries.Add(entry);
    }

    /// <summary>
    /// Gets for each.
    /// </summary>
    /// <param name="entries">The entries.</param>
    /// <param name="node">The node.</param>
    static void GetForEach(List<ScopeEntry> entries, ITreeNode node) {
      IForeachStatement foreachStatement = node as IForeachStatement;
      if(foreachStatement == null) {
        return;
      }

      IForeachVariableDeclaration foreachVariableDeclaration = foreachStatement.IteratorDeclaration;
      if(foreachVariableDeclaration == null) {
        return;
      }

      IForeachVariableDeclarationNode foreachVariableDeclarationNode = foreachVariableDeclaration.ToTreeNode();
      if(foreachVariableDeclarationNode == null) {
        return;
      }

      ScopeEntry entry = new ScopeEntry {
        Element = foreachStatement,
        Name = foreachStatement.IteratorName,
        Type = CSharpTypeFactory.CreateType(foreachVariableDeclarationNode.TypeUsage),
        IsAssigned = true
      };

      entries.Add(entry);
    }

    /// <summary>
    /// Gets the function parameters.
    /// </summary>
    /// <param name="entries">The entries.</param>
    /// <param name="node">The node.</param>
    static void GetFunctionParameters(List<ScopeEntry> entries, ITreeNode node) {
      IParametersOwner parametersOwner = node as IParametersOwner;
      if(parametersOwner == null) {
        return;
      }

      if(parametersOwner.Parameters.Count <= 0) {
        return;
      }

      foreach(IParameter parameter in parametersOwner.Parameters) {
        ScopeEntry entry = new ScopeEntry {
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
    /// <param name="node">The node.</param>
    static void GetLocalVariable(List<ScopeEntry> entries, ITreeNode node) {
      IDeclarationStatement declarationStatement = node as IDeclarationStatement;
      if(declarationStatement == null) {
        return;
      }

      IList<ILocalVariableDeclaration> localVariableDeclarations = declarationStatement.VariableDeclarations;
      if(localVariableDeclarations.Count != 1) {
        return;
      }

      ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];
      if(localVariableDeclaration == null) {
        return;
      }

      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if(localVariable == null) {
        return;
      }

      ScopeEntry entry = new ScopeEntry() {
        Element = localVariableDeclaration,
        Name = localVariable.ShortName,
        Type = localVariable.Type,
        IsAssigned = localVariableDeclaration.Initial != null
      };

      entries.Add(entry);
    }

    #endregion
  }
}