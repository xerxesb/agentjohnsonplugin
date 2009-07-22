// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMembers.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The enum members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The enum members.
  /// </summary>
  [SmartGenerate("Add enum member", "Adds a new constant to the enumeration.", Priority = 0)]
  public class EnumMembers : SmartGenerateHandlerBase
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

      var enumDeclaration = element.GetContainingElement(typeof(IEnumDeclaration), false) as IEnumDeclaration;
      if (enumDeclaration == null)
      {
        return;
      }

      this.AddAction("Add enum member", "587F88E2-6876-41F2-885C-58AD93BBC8B4");
    }

    #endregion
  }
}