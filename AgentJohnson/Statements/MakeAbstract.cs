// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MakeAbstract.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the make abstract class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Statements
{
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
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeAbstract"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public MakeAbstract(ICSharpContextActionDataProvider provider) : base(provider)
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
      var classDeclaration = element.GetContainingElement<IClassDeclaration>(true);

      var functionDeclaration = element.GetContainingElement<IMethodDeclaration>(true);
      if (functionDeclaration != null)
      {
        functionDeclaration.SetAbstract(true);
        functionDeclaration.SetVirtual(false);
        functionDeclaration.SetBody(null);
      }

      var propertyDeclaration = element.GetContainingElement<IPropertyDeclaration>(true);
      if (propertyDeclaration != null)
      {
        propertyDeclaration.SetAbstract(true);
        propertyDeclaration.SetVirtual(false);

        var accessorDeclarations = propertyDeclaration.AccessorDeclarations;

        foreach (var accessorDeclaration in accessorDeclarations)
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
    /// <returns>
    /// The text in the context menu.
    /// </returns>
    protected override string GetText()
    {
      return "Make abstract [Agent Johnson]";
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

      if (text != "virtual")
      {
        return false;
      }

      var functionDeclaration = element.GetContainingElement<IFunctionDeclaration>(true);
      if (functionDeclaration != null)
      {
        return true;
      }

      var propertyDeclaration = element.GetContainingElement<IPropertyDeclaration>(true);
      if (propertyDeclaration != null)
      {
        return true;
      }

      return false;
    }

    #endregion
  }
}