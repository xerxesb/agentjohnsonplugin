// <copyright file="MakeVirtual.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Feature.Services.CSharp.Generate.MemberBody;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the make abstract class.
  /// </summary>
  [ContextAction(Description = "Converts an abstract class to a normal class with virtual members.", Name = "Make abstract class virtual", Priority = -1, Group = "C#")]
  public class MakeVirtual : ContextActionBase
  {
    #region Constructor

    public MakeVirtual(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element)
    {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      ITreeNode parent = element.ToTreeNode().Parent;
      if (parent == null)
      {
        return;
      }

      IClassDeclaration classDeclaration = parent.Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      classDeclaration.SetAbstract(false);

      IList<IMethodDeclaration> methodDeclarations = classDeclaration.MethodDeclarations;

      foreach (IMethodDeclaration methodDeclaration in methodDeclarations)
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

        IBlock block = factory.CreateEmptyBlock();

        methodDeclaration.SetBody(block);

        CSharpMemberBodyUtil.Instance.SetBodyToDefault(methodDeclaration);
      }

      IList<IPropertyDeclaration> propertyDeclarations = classDeclaration.PropertyDeclarations;

      foreach (IPropertyDeclaration propertyDeclaration in propertyDeclarations)
      {
        if (!propertyDeclaration.IsAbstract)
        {
          continue;
        }

        propertyDeclaration.SetAbstract(false);
        propertyDeclaration.SetVirtual(true);

        foreach (IAccessorDeclaration accessorDeclaration in propertyDeclaration.AccessorDeclarations)
        {
          if (accessorDeclaration.Body != null)
          {
            continue;
          }

          IBlock block = factory.CreateEmptyBlock();

          accessorDeclaration.SetBody(block);

          CSharpPropertyBodyUtil.SetDefaultBody(accessorDeclaration);
        }
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text in the context menu.</returns>
    protected override string GetText()
    {
      return "Make virtual";
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
      string text = element.GetText();

      if (text != "abstract")
      {
        return false;
      }

      ITreeNode parent = element.ToTreeNode().Parent;
      if (parent == null)
      {
        return false;
      }

      ITreeNode classNode = parent.Parent;

      return classNode is IClassDeclaration;
    }

    #endregion
  }
}