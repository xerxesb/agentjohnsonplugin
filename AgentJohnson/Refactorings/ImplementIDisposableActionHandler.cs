// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementIDisposableActionHandler.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the implement I disposable action handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Refactorings
{
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Services;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the implement I disposable action handler class.
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementIDisposable")]
  public class ImplementIDisposableActionHandler : ActionHandlerBase
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
      if (!context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION))
      {
        return;
      }

      var textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      var element = TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);
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

        using (CommandCookie.Create("Context Action Implement IDisposable"))
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
    /// <c>true</c>, if the update succeeds.
    /// </returns>
    protected override bool Update(IDataContext context)
    {
      if (!context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION))
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

        if (typeName == "System.IDisposable")
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Adds the destructor.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="factory">
    /// The factory.
    /// </param>
    private static void AddDestructor(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string Code = @"~Disposable() {
          DisposeObject(false);
        }";

      var memberDeclaration = factory.CreateTypeMemberDeclaration(Code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Adds the dispose method.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="factory">
    /// The factory.
    /// </param>
    private static void AddDisposeMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string code = @"
        public void Dispose() {
          DisposeObject(true);
          GC.SuppressFinalize(this);
        }";

      var memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
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
    private static void AddDisposeObjectMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string code = @"
        void DisposeObject(bool disposing) {
          if(_disposed) {
            return;
          }
          if (disposing) {
            // Dispose managed resources.
          }
          // Dispose unmanaged resources.
          _disposed = true;         
        }";

      var memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
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
    private static void AddField(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string code = @"
        bool _disposed;
        ";

      var memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
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

      var typeElement = cache.GetTypeElementByCLRName("System.IDisposable");
      if (typeElement == null)
      {
        return;
      }

      var declaredType = TypeFactory.CreateType(typeElement);

      classDeclaration.AddSuperInterface(declaredType, false);
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
      AddField(classDeclaration, factory);
      AddDestructor(classDeclaration, factory);
      AddDisposeMethod(classDeclaration, factory);
      AddDisposeObjectMethod(classDeclaration, factory);
    }

    #endregion
  }
}