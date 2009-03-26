// <copyright file="SortCaseStatements.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>
  /// Defines the make abstract class.
  /// </summary>
  [ContextAction(Description = "Sort case statements.", Name = "Sort case statements", Priority = -1, Group = "C#")]
  public class SortCaseStatements : ContextActionBase, IComparer<SortCaseStatements.KeyCode>
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SortCaseStatements"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public SortCaseStatements(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// Value
    /// Condition
    /// Less than zero
    /// <paramref name="x"/> is less than <paramref name="y"/>.
    /// Zero
    /// <paramref name="x"/> equals <paramref name="y"/>.
    /// Greater than zero
    /// <paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    public int Compare(KeyCode x, KeyCode y)
    {
      if (x.Case == null && y.Case == null)
      {
        return 0;
      }

      if (x.Case == null)
      {
        return 1;
      }

      if (y.Case == null)
      {
        return -1;
      }

      if (x.Case is string)
      {
        return string.Compare(x.Case as string, y.Case as string);
      }

      return (int)x.Case - (int)y.Case;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element)
    {
      ISwitchStatement switchStatement = element.ToTreeNode().Parent as ISwitchStatement;
      if (switchStatement == null)
      {
        return;
      }

      IBlock block = switchStatement.Block;
      if (block == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      IList<IStatement> statements = block.Statements;
      if (statements == null)
      {
        return;
      }

      var list = new List<KeyCode>();

      GetCases(list, statements);

      list.Sort(this);

      StringBuilder code = new StringBuilder("{\r\n");

      foreach (KeyCode keyCode in list)
      {
        code.Append(keyCode.Code);
      }

      code.Append("\r\n}");

      switchStatement.SetBlock(factory.CreateBlock(code.ToString()));
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text in the context menu.</returns>
    protected override string GetText()
    {
      return "Sort cases";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      ISwitchStatement switchStatement = element.ToTreeNode().Parent as ISwitchStatement;
      if (switchStatement == null)
      {
        return false;
      }

      IExpression condition = switchStatement.Condition;
      if (condition == null)
      {
        return false;
      }

      IBlock block = switchStatement.Block;
      if (block == null)
      {
        return false;
      }

      IList<IStatement> statements = block.Statements;
      if (statements == null)
      {
        return false;
      }

      bool found = false;

      foreach (IStatement statement in statements)
      {
        ISwitchLabelStatement switchLabelStatement = statement as ISwitchLabelStatement;
        if (switchLabelStatement != null && !switchLabelStatement.IsDefault)
        {
          found = true;
          break;
        }
      }

      if (!found)
      {
        return false;
      }

      IType type = condition.Type();
      string typeName = type.GetPresentableName(element.Language);

      if (typeName == "string")
      {
        return true;
      }

      bool isEnumType = TypesUtil.IsEnumType(type);
      if (isEnumType)
      {
        return true;
      }

      bool isInt = PredefinedType.IsInt(type) || PredefinedType.IsLong(type);
      if (isInt)
      {
        return true;
      }

      return false;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the cases.
    /// </summary>
    /// <param name="list">The list of statements.</param>
    /// <param name="statements">The statements.</param>
    private static void GetCases(List<KeyCode> list, IList<IStatement> statements)
    {
      object caseValue = null;
      StringBuilder code = null;

      foreach (IStatement statement in statements)
      {
        ISwitchLabelStatement switchLabelStatement = statement as ISwitchLabelStatement;
        if (switchLabelStatement != null)
        {
          if (code != null)
          {
            list.Add(new KeyCode
            {
              Case = caseValue,
              Code = code.ToString()
            });
          }

          caseValue = null;

          ICSharpExpression valueExpression = switchLabelStatement.ValueExpression;
          if (valueExpression != null)
          {
            caseValue = valueExpression.ConstantValue.Value;
          }

          code = new StringBuilder();
        }

        statement.GetText(code);
      }

      if (code != null)
      {
        list.Add(new KeyCode
        {
          Case = caseValue,
          Code = code.ToString()
        });
      }
    }

    #endregion

    #region Nested type: KeyCode

    /// <summary>
    /// Defines the key code class.
    /// </summary>
    public class KeyCode
    {
      #region Public properties

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      /// <value>The key of the case.</value>
      public object Case { get; set; }

      /// <summary>
      /// Gets or sets the code.
      /// </summary>
      /// <value>The case code.</value>
      public string Code { get; set; }

      #endregion
    }

    #endregion
  }
}