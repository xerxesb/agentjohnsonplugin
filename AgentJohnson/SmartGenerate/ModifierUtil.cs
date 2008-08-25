using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the modifier util class.
  /// </summary>
  public static class ModifierUtil {
    #region Public methods

    /// <summary>
    /// Gets the modifier.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns></returns>
    public static string GetModifier(IElement element, IClassLikeDeclaration classDeclaration) {
      ITypeMemberDeclaration classMember = null;

      int caret = element.GetTreeStartOffset();

      foreach(ICSharpTypeMemberDeclaration typeMemberDeclaration in classDeclaration.MemberDeclarations) {
        if(typeMemberDeclaration.GetTreeStartOffset() > caret) {
          break;
        }

        classMember = typeMemberDeclaration;
      }

      string modifier = "public";

      IAccessRightsOwner accessRightsOwner = classMember as IAccessRightsOwner;
      if(accessRightsOwner != null) {
        AccessRights rights = accessRightsOwner.GetAccessRights();
        switch(rights) {
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
            modifier = string.Empty;
            break;
          case AccessRights.NONE:
            modifier = string.Empty;
            break;
        }
      }

      if(!string.IsNullOrEmpty(modifier)) {
        modifier += ' ';
      }

      return modifier;
    }

    #endregion
  }
}