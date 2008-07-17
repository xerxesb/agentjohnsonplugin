using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public abstract class SmartGenerateBase : ISmartGenerate {
    #region Fields

    List<ISmartGenerateMenuItem> _items;

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    public virtual IEnumerable<ISmartGenerateMenuItem> GetMenuItems(ISolution solution, IDataContext context, IElement element) {
      _items = new List<ISmartGenerateMenuItem>();

      GetItems(solution, context, element);

      return _items;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="template">The template.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu item.</returns>
    [CanBeNull]
    protected ISmartGenerateMenuItem AddMenuItem([NotNull] string text, [NotNull] string template, params string[] parameters) {
      return AddMenuItem(text, template, TextRange.InvalidRange, parameters);
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="template">The template.</param>
    /// <param name="selectionRange">The selection range.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu item.</returns>
    [CanBeNull]
    protected ISmartGenerateMenuItem AddMenuItem([NotNull] string text, [NotNull] string template, TextRange selectionRange, params string[] parameters) {
      string expandedTemplate = template;

      if(!expandedTemplate.StartsWith("<Template")) {
        expandedTemplate = SmartGenerateManager.Instance.GetTemplate(expandedTemplate);
      }

      if(string.IsNullOrEmpty(expandedTemplate)) {
        return null;
      }

      SmartGenerateMenuItem menuItem = new SmartGenerateMenuItem();

      if(parameters.Length > 0) {
        text = string.Format(text, parameters);
        expandedTemplate = string.Format(expandedTemplate, parameters);
      }

      menuItem.Text = text;
      menuItem.Template = expandedTemplate;
      menuItem.SelectionRange = selectionRange;

      AddMenuItem(menuItem);

      return menuItem;
    }

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="menuItem">The menu item.</param>
    protected void AddMenuItem([NotNull] ISmartGenerateMenuItem menuItem) {
      _items.Add(menuItem);
    }

    /// <summary>
    /// Gets the smart generate items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    protected abstract void GetItems(ISolution solution, IDataContext context, IElement element);

    /// <summary>
    /// Gets the modifier.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns></returns>
    protected static string GetModifier(IElement element, IClassLikeDeclaration classDeclaration) {
      ITypeMemberDeclaration classMember = null;

      int caret = element.GetTreeStartOffset();

      foreach(ICSharpTypeMemberDeclaration typeMemberDeclaration in classDeclaration.MemberDeclarations) {
        if(typeMemberDeclaration.GetTreeStartOffset() > caret) {
          break;
        }

        classMember = typeMemberDeclaration;
      }

      string modifier = "public";

      IAccessRightsOwner accessRightsOwner = classMember as IAccessRightsOwner;
      if(accessRightsOwner != null) {
        AccessRights rights = accessRightsOwner.GetAccessRights();
        switch(rights) {
          case AccessRights.PUBLIC:
            modifier = "public";
            break;
          case AccessRights.INTERNAL:
            modifier = "internal";
            break;
          case AccessRights.PROTECTED:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_OR_INTERNAL:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_AND_INTERNAL:
            modifier = "protected internal";
            break;
          case AccessRights.PRIVATE:
            modifier = string.Empty;
            break;
          case AccessRights.NONE:
            modifier = string.Empty;
            break;
        }
      }

      if(!string.IsNullOrEmpty(modifier)) {
        modifier += ' ';
      }

      return modifier;
    }

    /// <summary>
    /// Gets the nearest variable.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="isAssigned">if set to <c>true</c> [is assigned].</param>
    protected static void GetNearestVariable(IElement element, out string name, out IType type, out bool isAssigned) {
      name = null;
      type = null;
      isAssigned = false;

      ITreeNode node = element.ToTreeNode();

      while(node != null) {
        // local variable
        IDeclarationStatement declarationStatement = node as IDeclarationStatement;
        if(declarationStatement != null) {
          IList<ILocalVariableDeclaration> localVariableDeclarations = declarationStatement.VariableDeclarations;

          if(localVariableDeclarations.Count == 1) {
            ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];

            if(localVariableDeclaration != null) {
              ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;

              if(localVariable != null) {
                name = localVariable.ShortName;
                type = localVariable.Type;
                isAssigned = localVariableDeclaration.Initial != null;
                return;
              }
            }
          }
        }

        ITreeNode prevSibling = node.PrevSibling;

        if(prevSibling == null) {
          node = node.Parent;

          // foreach
          IForeachStatement foreachStatement = node as IForeachStatement;
          if(foreachStatement != null) {
            IForeachVariableDeclaration foreachVariableDeclaration = foreachStatement.IteratorDeclaration;

            if(foreachVariableDeclaration != null) {
              IForeachVariableDeclarationNode foreachVariableDeclarationNode = foreachVariableDeclaration.ToTreeNode();

              if(foreachVariableDeclarationNode != null) {
                name = foreachStatement.IteratorName;
                type = CSharpTypeFactory.CreateType(foreachVariableDeclarationNode.TypeUsage);
                isAssigned = true;
                return;
              }
            }
          }

          // for
          IForStatement forStatement = node as IForStatement;
          if(forStatement != null) {
            IList<ILocalVariableDeclaration> initializerDeclarations = forStatement.InitializerDeclarations;
            if(initializerDeclarations.Count == 1) {
              ILocalVariableDeclaration localVariableDeclaration = initializerDeclarations[0];
              if(localVariableDeclaration != null) {
                ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;

                if(localVariable != null) {
                  name = localVariable.ShortName;
                  type = localVariable.Type;
                  isAssigned = true;
                  return;
                }
              }
            }
          }

          // parameter
          IParametersOwner parametersOwner = node as IParametersOwner;
          if(parametersOwner != null) {
            if(parametersOwner.Parameters.Count > 0) {
              IParameter parameter = parametersOwner.Parameters[0];
              name = parameter.ShortName;
              type = parameter.Type;
              isAssigned = true;
              return;
            }
          }
        }
        else {
          node = prevSibling;
        }
      }
    }

    /// <summary>
    /// Gets the new statement position.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The new statement position.</returns>
    protected static TextRange GetNewStatementPosition(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return TextRange.InvalidRange;
      }

      TextRange range = new TextRange();

      IStatement statement = element.GetContainingElement(typeof(IStatement), true) as IStatement;
      if(statement != null && block.Contains(statement)) {
        TextRange range1 = statement.GetTreeTextRange();
        range = new TextRange(range1.EndOffset + 1);
      }
      return range;
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="block">The block.</param>
    protected static IStatement GetPreviousStatement(IBlock block, IElement element) {
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

    /// <summary>
    /// Determines whether [is after last statement] [the specified element].
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if [is after last statement] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsAfterLastStatement(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return false;
      }

      if(block.Statements.Count <= 0) {
        return true;
      }

      IStatement statement = block.Statements[block.Statements.Count - 1];
      DocumentRange range = statement.GetDocumentRange();

      int end = range.TextRange.StartOffset + range.TextRange.Length;
      if(end > element.GetTreeTextRange().StartOffset) {
        return false;
      }

      return true;
    }

    #endregion
  }
}