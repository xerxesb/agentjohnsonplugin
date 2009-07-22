// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlowControl.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The flow control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The flow control.
  /// </summary>
  [SmartGenerate("Generate 'continue' or 'break'", "Generates 'continue' or 'break'.", Priority = 200)]
  public class FlowControl : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      var hasLoop = element.GetContainingElement(typeof(IForeachStatement), false) as IForeachStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IForStatement), false) as IForStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IWhileStatement), false) as IWhileStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IDoStatement), false) as IDoStatement != null;

      if (!hasLoop)
      {
        return;
      }

      if (!StatementUtil.IsAfterLastStatement(element))
      {
        return;
      }

      this.AddAction("'continue'", "F849A86C-A93E-4805-B8E1-4B02CA8807CC");
      this.AddAction("'break'", "42DA21AD-1F9F-4ECE-B5F6-E7AFC5EAAE14");
    }

    #endregion
  }
}