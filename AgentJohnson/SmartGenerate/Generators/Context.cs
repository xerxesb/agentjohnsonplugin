using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Context", "Generates code based on the current context.", Priority = 500)]
  public class Context : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IElement targetElement;
      string name;
      IType type;
      bool isAssigned;

      GetNearestVariable(element, out targetElement, out name, out type, out isAssigned);
      if(type == null) {
        return;
      }

      TextRange range = GetNewStatementPosition(element);

      if(!isAssigned) {
        AddMenuItem("Assign value to '{0}'", "9BD23D35-AC2A-46E2-AADA-C81D9B53795A", range, name);

        if(HasAccessableConstructor(type)) {
          AddMenuItem("Assign new instance to '{0}'", "208A11F8-DEE1-4B8B-838F-17DA883DA7A5", range, name, type.GetPresentableName(element.Language));
        }

        return;
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType != null) {
        IEnum enumerate = declaredType.GetTypeElement() as IEnum;

        if(enumerate != null) {
          AddMenuItem("Generate 'switch' from '{0}'", "EBAF3559-41C5-471D-8457-A20C9566D397", range, name);
        }

        string typeName = type.GetPresentableName(element.Language);

        IModule module = declaredType.Module;
        if(module != null && typeName != "string") {
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

      // AddMenuItem(items, "if ({0}.<Method>)...", "1438A7F2-B12C-4784-BFDE-A803FA8F1279", name);
      // AddMenuItem(items, "var Var = {0}.Method();", "11BACA25-C561-4FE8-934B-41246B7CFAC9", name);
      // AddMenuItem(items, "if ({0}.Method() == Value)...", "43E2C069-A3E6-4649-A374-104A16C59305", name);
      AddMenuItem("Invoke method on '{0}'", "FE9C6A6B-A068-4182-B301-8002FE05A458", range, name);

      if(HasWritableProperty(type)) {
        AddMenuItem("Assign property on '{0}'", "DA0860C6-535C-489E-940C-841AA6C54C96", range, name);
      }
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

    /// <summary>
    /// Determines whether [has writable property] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// 	<c>true</c> if [has writable property] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    static bool HasWritableProperty(IType type) {
      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType == null) {
        return false;
      }

      ITypeElement typeElement = declaredType.GetTypeElement();
      if(typeElement == null) {
        return false;
      }

      IList<IProperty> properties = typeElement.Properties;
      if(properties == null || properties.Count == 0) {
        return false;
      }

      foreach(IProperty property in properties) {
        if(property.IsWritable) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Performs the value analysis.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="type">The type.</param>
    /// <param name="range">The range.</param>
    /// <param name="name">The name.</param>
    void PerformValueAnalysis(IElement element, IType type, TextRange range, string name) {
      if(!type.IsReferenceType()) {
        return;
      }

      IFunctionDeclaration functionDeclaration = element.GetContainingElement(typeof(IFunctionDeclaration), true) as IFunctionDeclaration;
      if(functionDeclaration == null) {
        return;
      }

      IReferenceExpression expression = element.GetContainingElement(typeof(IReferenceExpression), true) as IReferenceExpression;
      if(expression == null) {
        return;
      }

      ICSharpControlFlowGraf graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      ICSharpControlFlowAnalysisResult inspect = graf.Inspect(true);

      CSharpControlFlowNullReferenceState state = inspect.GetExpressionNullReferenceState(expression);

      switch(state) {
        case CSharpControlFlowNullReferenceState.UNKNOWN:
          AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
          break;
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          break;
        case CSharpControlFlowNullReferenceState.NULL:
          AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
          break;
        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          break;
      }
    }

    #endregion
  }
}