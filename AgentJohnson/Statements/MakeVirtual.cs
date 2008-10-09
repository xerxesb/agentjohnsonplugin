// <copyright file="MakeVirtual.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.CodeInsight.Services.CSharp.Generate.Util;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the make abstract class.
  /// </summary>
  [ContextAction(Description = "Converts an abstract class to a normal class with virtual members.", Name = "Make abstract class virtual", Priority = -1, Group = "C#")]
  public class MakeVirtual : ContextActionBase
  {
    /// <summary>
    /// The MakeVirtual.cs field.
    /// </summary>
    private int myFieldInteger;

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeVirtual"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public MakeVirtual(ISolution solution, ITextControl textControl) : base(solution, textControl)
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
      using (ModificationCookie cookie = this.TextControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Action Make virtual"))
        {
          PsiManager.GetInstance(this.Solution).DoTransaction(delegate { this.ConvertToAbstract(element); });
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

      if (classNode is IClassDeclaration)
      {
        return true;
      }

      return false;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Converts to abstract.
    /// </summary>
    /// <param name="element">The element.</param>
    private void ConvertToAbstract(IElement element)
    {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());
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

      GenerateTemplateManager manager = new GenerateTemplateManager(element.GetProject(), () => this.TextControl);

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

        manager.AddDefaultBody(methodDeclaration);
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

          manager.AddDefaultBody(accessorDeclaration);
        }
      }
    }

    #endregion
  }
}