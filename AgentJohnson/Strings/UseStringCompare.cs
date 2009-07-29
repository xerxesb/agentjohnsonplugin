// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseStringCompare.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The equality operator to equals action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
  using JetBrains.ReSharper.Intentions.Util;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// Defines the equality operator to equals action class.
  /// </summary>
  [ContextAction(Group = "C#", Name = "Convert '==' to string.Compare", Description = "Converts usage of equality operator ('==') to a call to string.Compare method.")]
  public class EqualityOperatorToEqualsAction : OneItemContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityOperatorToEqualsAction"/> class.
    /// </summary>
    /// <param name="dataProvider">
    /// The data provider.
    /// </param>
    public EqualityOperatorToEqualsAction(ICSharpContextActionDataProvider dataProvider) : base(dataProvider)
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
        return "Use string.Compare [Agent Johnson]";
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
      var expression = this.FindEqualityExpression();
      var expressionNode = expression.ToTreeNode();

      var operands = new object[]
      {
        Deparenthesize<ICSharpExpression>(expression.LeftOperand), 
        Deparenthesize<ICSharpExpression>(expression.RightOperand)
      };

      var newExpression = expression.ReplaceBy(this.Provider.ElementFactory.CreateExpression("string.Compare($0, $1) " + expressionNode.OperatorSign.GetText() + " 0", operands));

      ContextActionUtils.FormatWithDefaultProfile(newExpression);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == <c>null</c>
    /// </summary>
    /// <returns>
    /// <c>true</c>, if available.
    /// </returns>
    protected override bool IsAvailableInternal()
    {
      if (!this.Provider.PsiManager.CachesIdle)
      {
        return false;
      }

      var expression = this.FindEqualityExpression();
      if (expression == null)
      {
        return false;
      }

      var expressionNode = expression.ToTreeNode();
      if (expressionNode.OperatorSign == null)
      {
        return false;
      }

      if (expression.RightOperand == null)
      {
        return false;
      }

      if (!expressionNode.OperatorSign.GetTreeTextRange().Contains(this.Provider.CaretOffset))
      {
        return false;
      }

      return expression.LeftOperand.Type().Equals(this.SystemString) && expression.RightOperand.Type().Equals(this.SystemString);
    }

    /// <summary>
    /// The find equality expression.
    /// </summary>
    /// <returns>
    /// Returns the equality expression.
    /// </returns>
    private IEqualityExpression FindEqualityExpression()
    {
      return this.GetSelectedElement<IEqualityExpression>(false);
    }

    #endregion
  }
}