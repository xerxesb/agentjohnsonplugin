// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassMembers.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The class members.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using System;
  using JetBrains.ActionManagement;
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.ReSharper.Feature.Services.Generate;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.Util;

  /// <summary>
  /// The class members.
  /// </summary>
  [SmartGenerate("Generate class members", "Generates a new property or method on a class.", Priority = 0), UsedImplicitly]
  public class ClassMembers : SmartGenerateHandlerBase
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

      var classDeclaration = element.GetContainingElement(typeof(IClassDeclaration), true) as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      var memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IClassDeclaration))
      {
        return;
      }

      var modifier = ModifierUtil.GetModifier(element, classDeclaration);

      this.AddAction("Auto property", "166BE49C-D068-476D-BC9C-2B5C3AF21B06", modifier);
      this.AddAction("Property", "a684b217-f179-431b-a485-e3d76dbe57fd", modifier);
      this.AddAction("Method", "85BBC654-4EE4-4932-BB0C-E0670FA1BB82", modifier);

      var items = GeneratorManager.GetInstance(smartGenerateParameters.Solution).GetGeneratorItems(smartGenerateParameters.Context);

      foreach (var item in items)
      {
        if (!item.Available(smartGenerateParameters.Context))
        {
          continue;
        }

        var action = new ClassMemberAction
        {
          GeneratorItem = item,
          DataContext = smartGenerateParameters.Context,
          SelectionRange = TextRange.InvalidRange,
          Text = item.Text.Text
        };

        this.AddAction(action);
      }
    }

    #endregion

    /// <summary>
    /// Defines the class member action class.
    /// </summary>
    public class ClassMemberAction : ISmartGenerateAction
    {
      #region Properties

      /// <summary>
      /// Gets or sets DataContext.
      /// </summary>
      public IDataContext DataContext { get; set; }

      /// <summary>
      /// Gets or sets GeneratorItem.
      /// </summary>
      public IGeneratorItem GeneratorItem { get; set; }

      /// <summary>
      /// Gets or sets SelectionRange.
      /// </summary>
      public TextRange SelectionRange { get; set; }

      /// <summary>
      /// Gets or sets Template.
      /// </summary>
      public string Template { get; set; }

      /// <summary>
      /// Gets or sets Text.
      /// </summary>
      public string Text { get; set; }

      #endregion

      #region Implemented Interfaces

      #region ISmartGenerateAction

      /// <summary>
      /// Called when the item is clicked.
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The <see cref="System.EventArgs"/> instance containing the event data.
      /// </param>
      /// <returns>
      /// <c>true</c>, if handled, otherwise <c>false</c>.
      /// </returns>
      public bool HandleClick(object sender, EventArgs e)
      {
        Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
          "Create Live Template", 
          delegate
        {
          using (ReadLockCookie.Create())
          {
            using (CommandCookie.Create("Context Action Create Live Template"))
            {
              this.GeneratorItem.Execute(this.DataContext);
            }
          }
        });

        return true;
      }

      #endregion

      #endregion
    }
  }
}