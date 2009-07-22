// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementICloneableActionHandler.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the implement &lt;c&gt;ICloneable&lt;/c&gt; action handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Refactorings
{
  using System.Text;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the implement <c>ICloneable</c> action handler class.
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementICloneable")]
  public class ImplementICloneableActionHandler : ActionHandlerBase
  {
    #region Methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation.Enabled</c> to true.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(ISolution solution, IDataContext context)
    {
      if (!context.CheckAllNotNull(DataConstants.SOLUTION))
      {
        return;
      }

      var textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      var element = GetElementAtCaret(context);
      if (element == null)
      {
        return;
      }

      var classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return;
      }

      using (var cookie = textControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Action Implement ICloneable"))
        {
          PsiManager.GetInstance(solution).DoTransaction(delegate { Execute(solution, classDeclaration); });
        }
      }
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// <c>true</c>, if updateable.
    /// </returns>
    protected override bool Update(IDataContext context)
    {
      if (!context.CheckAllNotNull(DataConstants.SOLUTION))
      {
        return false;
      }

      var element = GetElementAtCaret(context);
      if (element == null)
      {
        return false;
      }

      var classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if (classDeclaration == null)
      {
        return false;
      }

      var types = classDeclaration.DeclaredElement.GetSuperTypes();

      foreach (var type in types)
      {
        var typeName = type.GetLongPresentableName(element.Language);

        if (typeName == "System.ICloneable")
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="factory">
    /// The factory.
    /// </param>
    private static void AddCloneMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      var cls = classDeclaration.DeclaredElement as IClass;
      if (cls == null)
      {
        return;
      }

      var code = "var result = new " + cls.ShortName + "();";

      code += AddCloneMethodCode(cls, false);

      foreach (var declaredType in cls.GetSuperTypes())
      {
        var resolve = declaredType.Resolve();

        var superClass = resolve.DeclaredElement as IClass;
        if (superClass == null)
        {
          continue;
        }

        code += AddCloneMethodCode(superClass, true);
      }

      code += "\r\nreturn result;";

      AddMember(classDeclaration, factory, @"
        public object Clone() {" +
                                           code + @"
        }
      ");
    }

    /// <summary>
    /// Adds the get object data method code.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="isSuperClass">
    /// if set to <c>true</c> [is super class].
    /// </param>
    /// <returns>
    /// Returns the clone method code.
    /// </returns>
    private static string AddCloneMethodCode(IClass classDeclaration, bool isSuperClass)
    {
      var code = new StringBuilder();

      foreach (var field in classDeclaration.Fields)
      {
        if (isSuperClass)
        {
          var rights = field.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        code.Append(string.Format("\r\nresult.{0} = {0};", field.ShortName));
      }

      foreach (var property in classDeclaration.Properties)
      {
        if (isSuperClass)
        {
          var rights = property.GetAccessRights();
          if (rights == AccessRights.PRIVATE)
          {
            continue;
          }
        }

        var declarations = property.GetDeclarations();
        if (declarations.Count != 1)
        {
          continue;
        }

        var propertyDeclaration = declarations[0] as IPropertyDeclaration;
        if (propertyDeclaration == null || !propertyDeclaration.IsAuto)
        {
          continue;
        }

        code.Append(string.Format("\r\nresult.{0} = {0};", property.ShortName));
      }

      return code.ToString();
    }

    /// <summary>
    /// Adds the interface.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    private static void AddInterface(ISolution solution, IClassDeclaration classDeclaration)
    {
      var scope = DeclarationsScopeFactory.SolutionScope(solution, true);
      var cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

      var typeElement = cache.GetTypeElementByCLRName("System.ICloneable");
      if (typeElement == null)
      {
        return;
      }

      var declaredType = TypeFactory.CreateType(typeElement);

      classDeclaration.AddSuperInterface(declaredType, false);
    }

    /// <summary>
    /// Adds the member.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="factory">
    /// The factory.
    /// </param>
    /// <param name="code">
    /// The member code.
    /// </param>
    private static void AddMember(IClassDeclaration classDeclaration, CSharpElementFactory factory, string code)
    {
      var memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Executes the specified class declaration.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    private static void Execute(ISolution solution, IClassDeclaration classDeclaration)
    {
      var factory = CSharpElementFactory.GetInstance(classDeclaration.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      AddInterface(solution, classDeclaration);
      AddCloneMethod(classDeclaration, factory);
    }

    #endregion
  }
}