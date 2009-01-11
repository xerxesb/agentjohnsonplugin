// <copyright file="ImplementISerializableActionHandler.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Refactorings
{
  using System.Collections.Generic;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Resolve;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the implement <c>ISerializable</c> action handler class.
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementISerializable")]
  public class ImplementISerializableActionHandler : ActionHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation.Enabled</c> to true.
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

        using (CommandCookie.Create("Context Action Implement ISerializable"))
        {
          PsiManager.GetInstance(solution).DoTransaction(delegate { Execute(solution, classDeclaration); });
        }
      }
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns><c>true</c>, if update is available.</returns>
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

      IDeclaredType[] types = MiscUtil.GetAllSuperTypes(classDeclaration.DeclaredElement);

      foreach (IDeclaredType type in types)
      {
        string typeName = type.GetLongPresentableName(element.Language);

        if (typeName == "System.Runtime.Serialization.ISerializable")
        {
          return false;
        }
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the attribute.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddAttribute(ISolution solution, IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      CLRTypeName typeName = new CLRTypeName("System.SerializableAttribute");

      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(DeclarationsCacheScope.SolutionScope(solution, true), true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName(typeName);
      if (typeElement == null)
      {
        return;
      }

      IAttribute attribute = factory.CreateAttribute(typeElement);

      classDeclaration.AddAttributeAfter(attribute, null);
    }

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddConstructor(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      IClass cls = classDeclaration.DeclaredElement as IClass;
      if (cls == null)
      {
        return;
      }

      string code = AddConstructorCode(cls, false);

      foreach (IDeclaredType declaredType in cls.GetSuperTypes())
      {
        ResolveResult resolve = declaredType.Resolve();

        IClass superClass = resolve.DeclaredElement as IClass;
        if (superClass == null)
        {
          continue;
        }

        code += AddConstructorCode(superClass, true);
      }

      AddMember(
        classDeclaration, 
        factory, 
        @"protected Constructor(SerializationInfo info, StreamingContext context) {" + code + @"
        }
      ");
    }

    /// <summary>
    /// Adds the constructor code.
    /// </summary>
    /// <param name="cls">The class declaration.</param>
    /// <param name="isSuperClass">if set to <c>true</c> [is super class].</param>
    /// <returns>Returns the constructor code.</returns>
    private static string AddConstructorCode(IClass cls, bool isSuperClass)
    {
      string code = string.Empty;

      foreach (IField field in cls.Fields)
      {
        if (isSuperClass)
        {
          AccessRights rights = field.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        string name = field.ShortName;
        string type = field.Type.GetPresentableName(cls.Language);

        if (field.Type.IsReferenceType())
        {
          code += string.Format("\r\n{0} = info.GetValue(\"{0}\", typeof({1})) as {1};", name, type);
        }
        else
        {
          code += string.Format("\r\n{0} = ({1})info.GetValue(\"{0}\", typeof({1}));", name, type);
        }
      }

      foreach (IProperty property in cls.Properties)
      {
        if (isSuperClass)
        {
          AccessRights rights = property.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        IList<IDeclaration> declarations = property.GetDeclarations();
        if (declarations.Count != 1)
        {
          continue;
        }

        IPropertyDeclaration propertyDeclaration = declarations[0] as IPropertyDeclaration;
        if (propertyDeclaration == null || !propertyDeclaration.IsAuto)
        {
          continue;
        }

        string name = property.ShortName;
        string type = property.Type.GetPresentableName(cls.Language);

        if (property.Type.IsReferenceType())
        {
          code += string.Format("\r\n{0} = info.GetValue(\"{0}\", typeof({1})) as {1};", name, type);
        }
        else
        {
          code += string.Format("\r\n{0} = ({1})info.GetValue(\"{0}\", typeof({1}));", name, type);
        }
      }

      return code;
    }

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddGetObjectDataMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      IClass cls = classDeclaration.DeclaredElement as IClass;
      if (cls == null)
      {
        return;
      }

      string code = AddGetObjectDataMethodCode(cls, false);

      foreach (IDeclaredType declaredType in cls.GetSuperTypes())
      {
        ResolveResult resolve = declaredType.Resolve();

        IClass superClass = resolve.DeclaredElement as IClass;
        if (superClass == null)
        {
          continue;
        }

        code += AddGetObjectDataMethodCode(superClass, true);
      }

      AddMember(
        classDeclaration, 
        factory, 
        @"
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {" + code + @"
        }
      ");
    }

    /// <summary>
    /// Adds the get object data method code.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isSuperClass">if set to <c>true</c> [is super class].</param>
    /// <returns>Returns the get object data method code.</returns>
    private static string AddGetObjectDataMethodCode(IClass classDeclaration, bool isSuperClass)
    {
      string code = string.Empty;

      foreach (IField field in classDeclaration.Fields)
      {
        if (isSuperClass)
        {
          AccessRights rights = field.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        code += string.Format("\r\ninfo.AddValue(\"{0}\", {0}, {0}.GetType());", field.ShortName);
      }

      foreach (IProperty property in classDeclaration.Properties)
      {
        if (isSuperClass)
        {
          AccessRights rights = property.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        IList<IDeclaration> declarations = property.GetDeclarations();
        if (declarations.Count != 1)
        {
          continue;
        }

        IPropertyDeclaration propertyDeclaration = declarations[0] as IPropertyDeclaration;
        if (propertyDeclaration == null || !propertyDeclaration.IsAuto)
        {
          continue;
        }

        code += string.Format("\r\ninfo.AddValue(\"{0}\", {0}, {0}.GetType());", property.ShortName);
      }

      return code;
    }

    /// <summary>
    /// Adds the interface.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    private static void AddInterface(ISolution solution, IClassDeclaration classDeclaration)
    {
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(DeclarationsCacheScope.SolutionScope(solution, true), true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName("System.Runtime.Serialization.ISerializable");
      if (typeElement == null)
      {
        return;
      }

      IDeclaredType declaredType = TypeFactory.CreateType(typeElement);

      classDeclaration.AddSuperInterface(declaredType, false);
    }

    /// <summary>
    /// Adds the member.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    /// <param name="code">The member code.</param>
    private static void AddMember(IClassDeclaration classDeclaration, CSharpElementFactory factory, string code)
    {
      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Executes the specified class declaration.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    private static void Execute(ISolution solution, IClassDeclaration classDeclaration)
    {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(classDeclaration.GetProject());
      if (factory == null)
      {
        return;
      }

      AddInterface(solution, classDeclaration);
      AddAttribute(solution, classDeclaration, factory);
      AddConstructor(classDeclaration, factory);
      AddGetObjectDataMethod(classDeclaration, factory);
    }

    #endregion
  }
}