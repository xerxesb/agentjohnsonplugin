// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakeVirtual.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the make abstract class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Statements
{
  using JetBrains.ReSharper.Feature.Services.CSharp.Generate.MemberBody;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.DataProviders;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the make abstract class.
  /// </summary>
  [ContextAction(Description = "Converts an abstract class to a normal class with virtual members.", Name = "Make abstract class virtual", Priority = -1, Group = "C#")]
  public class MakeVirtual : ContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeVirtual"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public MakeVirtual(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

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
      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var parent = element.ToTreeNode().Parent;
      if (parent == null)
      {
        return;
      }

      var classDeclaration = parent.Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      classDeclaration.SetAbstract(false);

      var methodDeclarations = classDeclaration.MethodDeclarations;

      foreach (var methodDeclaration in methodDeclarations)
      {
        if (!methodDeclaration.IsAbstract)
        {
          continue;
        }

        methodDeclaration.SetAbstract(false);
        methodDeclaration.SetVirtual(true);

        if (methodDeclaration.Body != null)
        {
          continue;
        }

        var block = factory.CreateEmptyBlock();

        methodDeclaration.SetBody(block);

        CSharpMemberBodyUtil.Instance.SetBodyToDefault(methodDeclaration);
      }

      var propertyDeclarations = classDeclaration.PropertyDeclarations;

      foreach (var propertyDeclaration in propertyDeclarations)
      {
        if (!propertyDeclaration.IsAbstract)
        {
          continue;
        }

        propertyDeclaration.SetAbstract(false);
        propertyDeclaration.SetVirtual(true);

        foreach (var accessorDeclaration in propertyDeclaration.AccessorDeclarations)
        {
          if (accessorDeclaration.Body != null)
          {
            continue;
          }

          var block = factory.CreateEmptyBlock();

          accessorDeclaration.SetBody(block);

          CSharpPropertyBodyUtil.SetDefaultBody(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The text in the context menu.
    /// </returns>
    protected override string GetText()
    {
      return "Make virtual [Agent Johnson]";
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
      var text = element.GetText();

      if (text != "abstract")
      {
        return false;
      }

      var parent = element.ToTreeNode().Parent;
      if (parent == null)
      {
        return false;
      }

      var classNode = parent.Parent;

      return classNode is IClassDeclaration;
    }

    #endregion
  }
}