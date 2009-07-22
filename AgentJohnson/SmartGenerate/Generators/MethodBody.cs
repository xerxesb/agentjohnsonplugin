// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBody.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The method body.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>
  /// The method body.
  /// </summary>
  [SmartGenerate("Generate method body", "Generates the body of a method with a return value.", Priority = 0)]
  public class MethodBody : SmartGenerateHandlerBase
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

      var methodDeclaration = element.GetContainingElement(typeof(IMethodDeclaration), true) as IMethodDeclaration;
      if (methodDeclaration == null)
      {
        return;
      }

      var body = methodDeclaration.Body;
      if (body == null || body.Statements.Count > 0)
      {
        return;
      }

      var name = methodDeclaration.DeclaredName;
      if (string.IsNullOrEmpty(name))
      {
        return;
      }

      var method = methodDeclaration.DeclaredElement;
      if (method == null)
      {
        return;
      }

      var returnType = method.ReturnType;
      var returnTypeName = returnType.GetPresentableName(element.Language);
      if (returnTypeName == "void" || string.IsNullOrEmpty(returnTypeName))
      {
        return;
      }

      if (returnType.IsReferenceType())
      {
        this.AddAction("Declare and return variable", "D00FD7E4-EE48-40F0-A126-D1E8AE8C031E", returnTypeName);
      }
      else
      {
        this.AddAction("Declare and return variable", "1E9A90C2-2FB9-451F-8566-BA59AAB707DF", returnTypeName);
      }
    }

    #endregion
  }
}