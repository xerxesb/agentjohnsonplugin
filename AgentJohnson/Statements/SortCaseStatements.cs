// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortCaseStatements.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the make abstract class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.DataProviders;
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
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SortCaseStatements"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public SortCaseStatements(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Implemented Interfaces

    #region IComparer<KeyCode>

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

    #endregion

    #region Methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute(IElement element)
    {
      var switchStatement = element.ToTreeNode().Parent as ISwitchStatement;
      if (switchStatement == null)
      {
        return;
      }

      var block = switchStatement.Block;
      if (block == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var statements = block.Statements;
      if (statements == null)
      {
        return;
      }

      var list = new List<KeyCode>();

      GetCases(list, statements);

      list.Sort(this);

      var code = new StringBuilder("{\r\n");

      foreach (var keyCode in list)
      {
        code.Append(keyCode.Code);
      }

      code.Append("\r\n}");

      switchStatement.SetBlock(factory.CreateBlock(code.ToString()));
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The text in the context menu.
    /// </returns>
    protected override string GetText()
    {
      return "Sort cases [Agent Johnson]";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      var switchStatement = element.ToTreeNode().Parent as ISwitchStatement;
      if (switchStatement == null)
      {
        return false;
      }

      IExpression condition = switchStatement.Condition;
      if (condition == null)
      {
        return false;
      }

      var block = switchStatement.Block;
      if (block == null)
      {
        return false;
      }

      var statements = block.Statements;
      if (statements == null)
      {
        return false;
      }

      var found = false;

      foreach (var statement in statements)
      {
        var switchLabelStatement = statement as ISwitchLabelStatement;
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

      var type = condition.Type();
      var typeName = type.GetPresentableName(element.Language);

      if (typeName == "string")
      {
        return true;
      }

      var isEnumType = TypesUtil.IsEnumType(type);
      if (isEnumType)
      {
        return true;
      }

      var isInt = PredefinedType.IsInt(type) || PredefinedType.IsLong(type);
      if (isInt)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Gets the cases.
    /// </summary>
    /// <param name="list">
    /// The list of statements.
    /// </param>
    /// <param name="statements">
    /// The statements.
    /// </param>
    private static void GetCases(List<KeyCode> list, IList<IStatement> statements)
    {
      object caseValue = null;
      StringBuilder code = null;

      foreach (var statement in statements)
      {
        var switchLabelStatement = statement as ISwitchLabelStatement;
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

          var valueExpression = switchLabelStatement.ValueExpression;
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

    /// <summary>
    /// Defines the key code class.
    /// </summary>
    public class KeyCode
    {
      #region Properties

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
  }
}