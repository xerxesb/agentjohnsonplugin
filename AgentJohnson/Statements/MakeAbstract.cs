// <copyright file="MakeAbstract.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

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
    /// <param name="provider">The provider.</param>
    public MakeAbstract(ICSharpContextActionDataProvider provider) : base(provider)
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
      IClassDeclaration classDeclaration = element.GetContainingElement<IClassDeclaration>(true);

      IMethodDeclaration functionDeclaration = element.GetContainingElement<IMethodDeclaration>(true);
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
  }
}