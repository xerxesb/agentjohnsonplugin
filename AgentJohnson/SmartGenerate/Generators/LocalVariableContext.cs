// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalVariableContext.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the local variable context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.Util;

  /// <summary>
  /// Defines the local variable context class.
  /// </summary>
  [SmartGenerate("Local Variable Context", "Generates code based on the current local variable context.", Priority = 500)]
  public class LocalVariableContext : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;
      var scope = smartGenerateParameters.Scope;
      if (scope.Count == 0)
      {
        return;
      }

      var name = scope[smartGenerateParameters.ScopeIndex].Name;
      var type = scope[smartGenerateParameters.ScopeIndex].Type;
      var isAssigned = scope[smartGenerateParameters.ScopeIndex].IsAssigned;

      var range = StatementUtil.GetNewStatementPosition(element);

      if (!isAssigned)
      {
        this.AddAction("Assign value to '{0}'", "9BD23D35-AC2A-46E2-AADA-C81D9B53795A", range, name);

        if (HasAccessableConstructor(type))
        {
          this.AddAction("Assign new instance to '{0}'", "208A11F8-DEE1-4B8B-838F-17DA883DA7A5", range, name, type.GetPresentableName(element.Language));
        }

        return;
      }

      var declaredType = type as IDeclaredType;
      if (declaredType != null)
      {
        var enumerate = declaredType.GetTypeElement() as IEnum;

        if (enumerate != null)
        {
          this.AddAction("Generate 'switch' from '{0}'", "EBAF3559-41C5-471D-8457-A20C9566D397", range, name);
        }

        var typeName = type.GetPresentableName(element.Language);

        var module = declaredType.Module;
        if (module != null && typeName != "string")
        {
          var enumerable = TypeFactory.CreateTypeByCLRName("System.Collections.IEnumerable", module);

          if (declaredType.IsSubtypeOf(enumerable))
          {
            this.AddAction("Iterate '{0}' via 'foreach'", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", range, name);
          }
        }
      }
      else
      {
        if (type is IArrayType)
        {
          this.AddAction("Iterate '{0}' via 'foreach'", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", range, name);
        }
      }

      // AddMenuItem(items, "if ({0}.<Method>)...", "1438A7F2-B12C-4784-BFDE-A803FA8F1279", name);
      // AddMenuItem(items, "var Var = {0}.Method();", "11BACA25-C561-4FE8-934B-41246B7CFAC9", name);
      // AddMenuItem(items, "if ({0}.Method() == Value)...", "43E2C069-A3E6-4649-A374-104A16C59305", name);
      // AddMenuItem("Invoke method on '{0}'", "FE9C6A6B-A068-4182-B301-8002FE05A458", range, name);

      // if(HasWritableProperty(type)) {
      // AddMenuItem("Assign property on '{0}'", "DA0860C6-535C-489E-940C-841AA6C54C96", range, name);
      // }
    }

    /// <summary>
    /// Determines whether the specified type has constructor.
    /// </summary>
    /// <param name="type">
    /// The type parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified type has constructor; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasAccessableConstructor(IType type)
    {
      var declaredType = type as IDeclaredType;
      if (declaredType == null)
      {
        return false;
      }

      var resolve = declaredType.Resolve();

      var classDeclaration = resolve.DeclaredElement as IClass;
      if (classDeclaration == null)
      {
        return false;
      }

      var constructors = classDeclaration.Constructors;

      foreach (var constructor in constructors)
      {
        if (constructor.IsAbstract || constructor.IsStatic)
        {
          continue;
        }

        var rights = constructor.GetAccessRights();
        if (rights == AccessRights.PRIVATE)
        {
          continue;
        }

        return true;
      }

      return false;
    }

    /// <summary>
    /// Determines whether [has writable property] [the specified type].
    /// </summary>
    /// <param name="type">
    /// The type parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if [has writable property] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasWritableProperty(IType type)
    {
      var declaredType = type as IDeclaredType;
      if (declaredType == null)
      {
        return false;
      }

      var typeElement = declaredType.GetTypeElement();
      if (typeElement == null)
      {
        return false;
      }

      var properties = typeElement.Properties;
      if (properties == null || properties.Count == 0)
      {
        return false;
      }

      foreach (var property in properties)
      {
        if (property.IsWritable)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Performs the value analysis.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="type">
    /// The type parameter.
    /// </param>
    /// <param name="range">
    /// The range.
    /// </param>
    /// <param name="name">
    /// The variable name.
    /// </param>
    private void PerformValueAnalysis(IElement element, IType type, TextRange range, string name)
    {
      if (!type.IsReferenceType())
      {
        return;
      }

      var functionDeclaration = element.GetContainingElement(typeof(IFunctionDeclaration), true) as ICSharpFunctionDeclaration;
      if (functionDeclaration == null)
      {
        return;
      }

      var expression = element.GetContainingElement(typeof(IReferenceExpression), true) as IReferenceExpression;
      if (expression == null)
      {
        return;
      }

      var graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      var inspect = graf.Inspect(true);

      var state = inspect.GetExpressionNullReferenceState(expression);

      switch (state)
      {
        case CSharpControlFlowNullReferenceState.UNKNOWN:
          this.AddAction("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
          break;
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          break;
        case CSharpControlFlowNullReferenceState.NULL:
          this.AddAction("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
          break;
        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          break;
      }
    }

    #endregion
  }
}