// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseStringBuilderContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The string concatenation action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The string concatenation action.
  /// </summary>
  [ContextAction(Group = "C#", Name = "Use StringBuilder", Description = "Converts concatenation of a few strings and other objects to the use of StringBuilder class.")]
  public class UseStringBuilderAction : OneItemContextActionBase
  {
    #region Constants and Fields

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UseStringBuilderAction"/> class. 
    /// Initializes a new instance of the <see cref="StringConcatenationAction"/> class.
    /// </summary>
    /// <param name="dataProvider">
    /// The data provider.
    /// </param>
    public UseStringBuilderAction(ICSharpContextActionDataProvider dataProvider) : base(dataProvider)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets Text.
    /// </summary>
    public override string Text
    {
      get
      {
        return "Use StringBuilder [Agent Johnson]";
      }
    }

    /// <summary>
    /// Gets SystemString.
    /// </summary>
    private IDeclaredType SystemString
    {
      get
      {
        return this.Provider.PsiModule.GetPredefinedType().String;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The execute internal.
    /// </summary>
    protected override void ExecuteInternal()
    {
      var concatenatedStrings = new List<ICSharpExpression>();
      this.BuildAdditionList(this.GetConcatenation(), concatenatedStrings);

      this.ReplaceWithStringFormat(concatenatedStrings);
    }

    /// <summary>
    /// The is available internal.
    /// </summary>
    /// <returns>If available internal.</returns>
    protected override bool IsAvailableInternal()
    {
      var additiveExpression = this.GetConcatenation();
      if (additiveExpression != null)
      {
        var concatenatedStrings = new List<ICSharpExpression>();
        if (!this.BuildAdditionList(additiveExpression, concatenatedStrings))
        {
          return false;
        }

        foreach (var expression in concatenatedStrings)
        {
          var literalExpression = expression as ILiteralExpression;
          if ((literalExpression == null) || !literalExpression.Type().Equals(this.SystemString))
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Builds the addition list.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="concatenatedStrings">The concatenated strings.</param>
    /// <returns>The build addition list.</returns>
    private bool BuildAdditionList(ICSharpExpression expression, ICollection<ICSharpExpression> concatenatedStrings)
    {
      if (expression.Type().Equals(this.SystemString))
      {
        if (expression is ILiteralExpression)
        {
          concatenatedStrings.Add(expression);
          return true;
        }

        if (expression is IAdditiveExpression)
        {
          var additiveExpression = (IAdditiveExpression)expression;
          var left = this.BuildAdditionList(additiveExpression.LeftOperand, concatenatedStrings);
          var right = this.BuildAdditionList(additiveExpression.RightOperand, concatenatedStrings);
          if (!left)
          {
            return right;
          }

          return true;
        }
      }

      concatenatedStrings.Add(expression);

      return false;
    }

    /// <summary>
    /// The get concatenation.
    /// </summary>
    /// <returns>
    /// Returns the concatenation.
    /// </returns>
    private IAdditiveExpression GetConcatenation()
    {
      IAdditiveExpression additiveElement;
      var element = this.Provider.SelectedElement;
      if (element == null)
      {
        return null;
      }

      var tmp = element.GetContainingElement<IAdditiveExpression>(true);
      if (tmp == null)
      {
        return null;
      }

      do
      {
        additiveElement = tmp;
        tmp = additiveElement.GetContainingElement<IAdditiveExpression>(false);
      }
      while (tmp != null);

      if (!additiveElement.Type().Equals(this.SystemString))
      {
        return null;
      }

      return additiveElement;
    }

    /// <summary>
    /// Replaces the with string format.
    /// </summary>
    /// <param name="concatenatedStrings">
    /// The concatenated strings.
    /// </param>
    private void ReplaceWithStringFormat(IEnumerable<ICSharpExpression> concatenatedStrings)
    {
      var element = this.Provider.SelectedElement;
      if (element == null)
      {
        return;
      }

      var anchor = element.GetContainingElement<IStatement>(true);
      if (anchor == null)
      {
        return;
      }

      var body = anchor.GetContainingElement<IBlock>(true);
      if (body == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      var statement = factory.CreateStatement("StringBuilder stringBuilder = new StringBuilder();");

      body.AddStatementBefore(statement, anchor);

      foreach (var expression in concatenatedStrings)
      {
        statement = factory.CreateStatement("stringBuilder.Append(" + expression.GetText() + ");");

        body.AddStatementBefore(statement, anchor);
      }

      var formatStringCall = this.Provider.ElementFactory.CreateExpression("stringBuilder.ToString()");

      this.GetConcatenation().ReplaceBy(formatStringCall);
    }

    #endregion
  }
}