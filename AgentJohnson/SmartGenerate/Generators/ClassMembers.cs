namespace AgentJohnson.SmartGenerate.Generators
{
  using System;
  using System.Collections.Generic;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.ReSharper.Feature.Services.Generate;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// </summary>
  [SmartGenerate("Generate class members", "Generates a new property or method on a class.", Priority = 0)]
  public class ClassMembers : SmartGenerateHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      IElement element = smartGenerateParameters.Element;

      IClassDeclaration classDeclaration = element.GetContainingElement(typeof(IClassDeclaration), true) as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IClassDeclaration))
      {
        return;
      }

      string modifier = ModifierUtil.GetModifier(element, classDeclaration);

      this.AddAction("Auto property", "166BE49C-D068-476D-BC9C-2B5C3AF21B06", modifier);
      this.AddAction("Property", "a684b217-f179-431b-a485-e3d76dbe57fd", modifier);
      this.AddAction("Method", "85BBC654-4EE4-4932-BB0C-E0670FA1BB82", modifier);

      IList<IGeneratorItem> items = GeneratorManager.GetInstance(smartGenerateParameters.Solution).GetGeneratorItems(smartGenerateParameters.Context);

      foreach (IGeneratorItem item in items)
      {
        if (!item.Available(smartGenerateParameters.Context))
        {
          continue;
        }

        ClassMemberAction action = new ClassMemberAction
        {
          GeneratorItem = item,
          DataContext = smartGenerateParameters.Context,
          SelectionRange = TextRange.InvalidRange,
          Text = item.Text.Text
        };

        this.AddAction(action);
      }
    }

    /// <summary>
    /// Defines the class member action class.
    /// </summary>
    public class ClassMemberAction: ISmartGenerateAction
    {
      public IGeneratorItem GeneratorItem  { get; set; }

      public TextRange SelectionRange { get; set; }

      public string Template { get; set; }

      public string Text { get; set; }

      public IDataContext DataContext { get; set; }

      /// <summary>
      /// Called when the item is clicked.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      /// <returns>
      /// 	<c>true</c>, if handled, otherwise <c>false</c>.
      /// </returns>
      public bool HandleClick(object sender, EventArgs e)
      {
        Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue("Create Live Template", delegate
        {
          using (ReadLockCookie.Create())
          {
            using (CommandCookie.Create("Context Action Create Live Template"))
            {
              GeneratorItem.Execute(DataContext);
            }
          }
        });

        return true;
      }
    }

    #endregion
  }
}