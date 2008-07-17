using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate(Priority = 500)]
  public class GenerateNearestVariable : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      string name;
      IType type;
      bool isAssigned;

      GetNearestVariable(element, out name, out type, out isAssigned);
      if(type == null) {
        return;
      }

      TextRange range = GetNewStatementPosition(element);

      if(!isAssigned) {
        AddMenuItem("Assign value to '{0}'", "9BD23D35-AC2A-46E2-AADA-C81D9B53795A", range, name);

        if(HasAccessableConstructor(type)) {
          AddMenuItem("Assign new instance to '{0}'", "208A11F8-DEE1-4B8B-838F-17DA883DA7A5", range, name);
        }

        return;
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType != null) {
        IEnum enumerate = declaredType.GetTypeElement() as IEnum;

        if(enumerate != null) {
          AddMenuItem("Generate 'switch' from '{0}'", "EBAF3559-41C5-471D-8457-A20C9566D397", range, name);
        }

        IModule module = declaredType.Module;
        if(module != null) {
          IDeclaredType enumerable = TypeFactory.CreateTypeByCLRName("System.Collections.IEnumerable", module);

          if(declaredType.IsSubtypeOf(enumerable)) {
            AddMenuItem("Iterate '{0}' via 'foreach'", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", range, name);
          }
        }
      }
      else {
        if(type is IArrayType) {
          AddMenuItem("Iterate '{0}' via 'foreach'", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", range, name); 
        }
      }

      if(type.GetPresentableName(element.Language) == "string") {
        AddMenuItem("Check if '{0}' is null or empty", "514313A0-91F4-4AE5-B4EB-2BB53736A023", range, name);
      }
      else {
        if(type.IsReferenceType()) {
          AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
        }
      }

      // AddMenuItem(items, "if ({0}.<Method>)...", "1438A7F2-B12C-4784-BFDE-A803FA8F1279", name);
      // AddMenuItem(items, "var Var = {0}.Method();", "11BACA25-C561-4FE8-934B-41246B7CFAC9", name);
      // AddMenuItem(items, "if ({0}.Method() == Value)...", "43E2C069-A3E6-4649-A374-104A16C59305", name);
      AddMenuItem("Invoke method on '{0}'", "FE9C6A6B-A068-4182-B301-8002FE05A458", range, name);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Determines whether the specified type has constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// 	<c>true</c> if the specified type has constructor; otherwise, <c>false</c>.
    /// </returns>
    static bool HasAccessableConstructor(IType type) {
      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType == null) {
        return false;
      }

      ResolveResult resolve = declaredType.Resolve();

      IClass classDeclaration = resolve.DeclaredElement as IClass;
      if(classDeclaration == null) {
        return false;
      }

      IList<IConstructor> constructors = classDeclaration.Constructors;

      foreach(IConstructor constructor in constructors) {
        if(constructor.IsAbstract || constructor.IsStatic) {
          continue;
        }

        AccessRights rights = constructor.GetAccessRights();
        if(rights == AccessRights.PRIVATE) {
          continue;
        }

        return true;
      }

      return false;
    }

    #endregion
  }
}