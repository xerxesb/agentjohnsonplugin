// <copyright file="MakeAbstract.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the make abstract class.
  /// </summary>
  [ContextAction(Description = "Converts a virtual method to an abstract method.", Name = "Make virtual member abstract", Priority = -1, Group = "C#")]
  public class MakeAbstract : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeAbstract"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public MakeAbstract(ISolution solution, ITextControl textControl) : base(solution, textControl)
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

        using (CommandCookie.Create("Context Action Make abstract"))
        {
          PsiManager.GetInstance(this.Solution).DoTransaction(delegate { ConvertToAbstract(element); });
        }
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text in the context menu.</returns>
    protected override string GetText()
    {
      return "Make abstract";
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

      if (text != "virtual")
      {
        return false;
      }

      IFunctionDeclaration functionDeclaration = element.GetContainingElement<IFunctionDeclaration>(true);
      if (functionDeclaration != null)
      {
        return true;
      }

      IPropertyDeclaration propertyDeclaration = element.GetContainingElement<IPropertyDeclaration>(true);
      if (propertyDeclaration != null)
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
    private static void ConvertToAbstract(IElement element)
    {
      IClassDeclaration classDeclaration = element.GetContainingElement<IClassDeclaration>(true);

      IFunctionDeclaration functionDeclaration = element.GetContainingElement<IFunctionDeclaration>(true);
      if (functionDeclaration != null)
      {
        functionDeclaration.SetAbstract(true);
        functionDeclaration.SetVirtual(false);
        functionDeclaration.SetBody(null);
      }

      IPropertyDeclaration propertyDeclaration = element.GetContainingElement<IPropertyDeclaration>(true);
      if (propertyDeclaration != null)
      {
        propertyDeclaration.SetAbstract(true);
        propertyDeclaration.SetVirtual(false);

        IList<IAccessorDeclaration> accessorDeclarations = propertyDeclaration.AccessorDeclarations;

        foreach (IAccessorDeclaration accessorDeclaration in accessorDeclarations)
        {
          accessorDeclaration.SetBody(null);
        }
      }

      if (classDeclaration == null)
      {
        return;
      }

      classDeclaration.SetAbstract(true);
    }

    #endregion
  }
}