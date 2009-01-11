// <copyright file="ImplementProxyClassActionHandler.cs" company="Jakob Christensen">
//   Copyright (c) Jakob Christensen. All rights reserved.
// </copyright>

namespace AgentJohnson.Refactorings
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the implement proxy class action handler class.
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementProxyClass")]
  public class ImplementProxyClassActionHandler : ActionHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected override void Execute(ISolution solution, IDataContext context)
    {
      if (!context.CheckAllNotNull(DataConstants.SOLUTION))
      {
        return;
      }

      ITextControl textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      IElement element = GetElementAtCaret(context);
      if (element == null)
      {
        return;
      }

      IClassDeclaration classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      using (ModificationCookie cookie = textControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Action Implement Proxy Class"))
        {
          PsiManager.GetInstance(solution).DoTransaction(delegate { Execute(classDeclaration); });
        }
      }
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Determines if the update succeeded.</returns>
    protected override bool Update(IDataContext context)
    {
      if (!context.CheckAllNotNull(DataConstants.SOLUTION))
      {
        return false;
      }

      IElement element = GetElementAtCaret(context);
      if (element == null)
      {
        return false;
      }

      IClassDeclaration classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return false;
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the get object data method code.
    /// </summary>
    /// <param name="code">The code string.</param>
    /// <param name="cls">The class declaration.</param>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="className">Name of the class.</param>
    private static void AddMembers(StringBuilder code, IClass cls, string variableName, string className)
    {
      IClass superClass = cls.GetSuperClass();

      if (superClass == null)
      {
        return;
      }

      AddMembers(code, superClass, variableName, className);

      AddProperties(cls, code, variableName, className);
      AddMethods(cls, code, variableName, className);
    }

    /// <summary>
    /// Adds the methods.
    /// </summary>
    /// <param name="cls">The class.</param>
    /// <param name="code">The code string.</param>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="className">Name of the class.</param>
    private static void AddMethods(IClass cls, StringBuilder code, string variableName, string className)
    {
      IList<IMethod> methods = cls.Methods;
      if (methods == null)
      {
        return;
      }

      foreach (IMethod method in methods)
      {
        if (method.IsOverride)
        {
          continue;
        }

        AccessRights rights = method.GetAccessRights();
        if (rights != AccessRights.PUBLIC)
        {
          continue;
        }

        string returnType = method.ReturnType.GetPresentableName(cls.Language);

        StringBuilder parameters = new StringBuilder();
        StringBuilder arguments = new StringBuilder();

        bool first = true;

        foreach (IParameter parameter in method.Parameters)
        {
          if (!first)
          {
            parameters.Append(", ");
            arguments.Append(", ");
          }                                          
          if (parameter.Kind == ParameterKind.OUTPUT)
          {
            parameters.Append("out ");
            arguments.Append("out ");
          }

          if (parameter.Kind == ParameterKind.REFERENCE)
          {
            parameters.Append("ref ");
            arguments.Append("ref ");
          }

          parameters.Append(parameter.Type.GetPresentableName(cls.Language));
          parameters.Append(' ');
          parameters.Append(parameter.ShortName);

          arguments.Append(parameter.ShortName);

          first = false;
        }

        string args = arguments.ToString();
        string parms = parameters.ToString();

        if (method.IsStatic)
        {
          if (returnType == "void")
          {
            code.Append(string.Format("\r\npublic static void {0}({2}) {{\r\n {1}.{0}({3});\r\n}}", method.ShortName, className, parms, args));
          }
          else
          {
            code.Append(string.Format("\r\npublic static {0} {1}({3}) {{\r\n return {2}.{1}({4});\r\n}}", returnType, method.ShortName, className, parms, args));
          }
        }
        else
        {
          if (returnType == "void")
          {
            code.Append(string.Format("\r\npublic void {0}({2}) {{\r\n {1}.{0}({3});\r\n}}", method.ShortName, variableName, parms, args));
          }
          else
          {
            code.Append(string.Format("\r\npublic {0} {1}({3}) {{\r\n return {2}.{1}({4});\r\n}}", returnType, method.ShortName, variableName, parms, args));
          }
        }
      }
    }

    /// <summary>
    /// Adds the properties.
    /// </summary>
    /// <param name="cls">The class.</param>
    /// <param name="code">The code string.</param>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="className">Name of the class.</param>
    private static void AddProperties(IClass cls, StringBuilder code, string variableName, string className)
    {
      IList<IProperty> properties = cls.Properties;
      if (properties == null)
      {
        return;
      }

      foreach (IProperty property in properties)
      {
        if (property.IsOverride)
        {
          continue;
        }

        AccessRights rights = property.GetAccessRights();
        if (rights != AccessRights.PUBLIC)
        {
          continue;
        }

        string instance = variableName;

        string staticProperty = string.Empty;
        if (property.IsStatic)
        {
          instance = className;
          staticProperty = " static";
        }

        code.Append(string.Format("\r\npublic{2} {0} {1} {{", property.Type.GetPresentableName(cls.Language), property.ShortName, staticProperty));

        IAccessor getter = property.Getter;
        if (getter != null)
        {
          AccessRights accessRights = getter.GetAccessRights();
          if (accessRights == AccessRights.PUBLIC)
          {
            code.Append(string.Format("\r\nget {{\r\n return {1}.{0};\r\n}}", property.ShortName, instance));
          }
        }

        IAccessor setter = property.Setter;
        if (setter != null)
        {
          AccessRights accessRights = setter.GetAccessRights();
          if (accessRights == AccessRights.PUBLIC)
          {
            code.Append(string.Format("\r\nset {{\r\n {1}.{0} = value;\r\n}}", property.ShortName, instance));
          }
        }

        code.Append("}");
      }
    }

    /// <summary>
    /// Adds the interface.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddProxyClass(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      string className = classDeclaration.DeclaredName;
      string variableName = GetVariableName(classDeclaration);

      string prefix = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings().FieldNameSettings.Prefix;

      string proxyProperty = prefix + variableName;

      bool isStatic = false; // classDeclaration.IsStatic;

      string staticClass = isStatic ? " static" : string.Empty;

      StringBuilder code = new StringBuilder(string.Format("public{1} class {0}Proxy {{", classDeclaration.DeclaredName, staticClass));

      if (!isStatic)
      {
        code.Append(string.Format("\r\nreadonly {0} {1};", classDeclaration.DeclaredName, proxyProperty));
        code.Append(string.Format("public {0}Proxy({0} {1}) {{\r\n{2} = {1};\r\n}}\r\n", classDeclaration.DeclaredName, variableName, proxyProperty));
      }

      IClass cls = classDeclaration.DeclaredElement as IClass;
      if (cls == null)
      {
        return;
      }

      AddMembers(code, cls, proxyProperty, className);

      code.Append("}");

      IClassDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code.ToString()) as IClassDeclaration;

      ICSharpNamespaceDeclaration namespaceDeclaration = classDeclaration.GetContainingNamespaceDeclaration();

      namespaceDeclaration.AddTypeDeclarationAfter(memberDeclaration, classDeclaration);
    }

    /// <summary>
    /// Executes the specified class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    private static void Execute(IClassDeclaration classDeclaration)
    {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(classDeclaration.GetProject());
      if (factory == null)
      {
        return;
      }

      AddProxyClass(classDeclaration, factory);
    }

    /// <summary>
    /// Gets the name of the variable.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns>The variable name.</returns>
    private static string GetVariableName(IClassDeclaration classDeclaration)
    {
      string result = classDeclaration.DeclaredName;

      if (string.IsNullOrEmpty(result))
      {
        return string.Empty;
      }

      char[] charArray = result.ToCharArray();
      charArray[0] = Char.ToLower(charArray[0]);

      return new string(charArray);
    }

    #endregion
  }
}
