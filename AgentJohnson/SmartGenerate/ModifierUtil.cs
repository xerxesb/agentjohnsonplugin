// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifierUtil.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the modifier util class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the modifier utility class.
  /// </summary>
  public static class ModifierUtil
  {
    #region Public Methods

    /// <summary>
    /// Gets the modifier.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <returns>
    /// The get modifier.
    /// </returns>
    public static string GetModifier(IElement element, IClassLikeDeclaration classDeclaration)
    {
      ITypeMemberDeclaration classMember = null;

      var caret = element.GetTreeStartOffset();

      foreach (var typeMemberDeclaration in classDeclaration.MemberDeclarations)
      {
        if (typeMemberDeclaration.GetTreeStartOffset() > caret)
        {
          break;
        }

        classMember = typeMemberDeclaration;
      }

      var modifier = "public";

      var accessRightsOwner = classMember as IAccessRightsOwner;
      if (accessRightsOwner != null)
      {
        var rights = accessRightsOwner.GetAccessRights();
        switch (rights)
        {
          case AccessRights.PUBLIC:
            modifier = "public";
            break;
          case AccessRights.INTERNAL:
            modifier = "internal";
            break;
          case AccessRights.PROTECTED:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_OR_INTERNAL:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_AND_INTERNAL:
            modifier = "protected internal";
            break;
          case AccessRights.PRIVATE:
            modifier = "private";
            break;
          case AccessRights.NONE:
            modifier = string.Empty;
            break;
        }
      }

      var modifiersOwner = classMember as IModifiersOwner;
      if (modifiersOwner != null)
      {
        if (modifiersOwner.IsStatic)
        {
          if (!string.IsNullOrEmpty(modifier))
          {
            modifier += ' ';
          }

          modifier += "static";
        }
      }

      if (!string.IsNullOrEmpty(modifier))
      {
        modifier += ' ';
      }

      return modifier;
    }

    #endregion
  }
}