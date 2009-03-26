// <copyright file="ImplementIDisposableActionHandler.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Refactorings
{
  using System.Collections.Generic;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.Caches;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Services;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the implement I disposable action handler class.
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementIDisposable")]
  public class ImplementIDisposableActionHandler : ActionHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation.Enabled</c> to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected override void Execute(ISolution solution, IDataContext context)
    {
      if (!context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION))
      {
        return;
      }

      ITextControl textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      IElement element = TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);
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

        using (CommandCookie.Create("Context Action Implement IDisposable"))
        {
          PsiManager.GetInstance(solution).DoTransaction(delegate { Execute(solution, classDeclaration); });
        }
      }
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns><c>true</c>, if the update succeeds.</returns>
    protected override bool Update(IDataContext context)
    {
      if (!context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION))
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

      IList<IDeclaredType> types = classDeclaration.DeclaredElement.GetSuperTypes();

      foreach (IDeclaredType type in types)
      {
        string typeName = type.GetLongPresentableName(element.Language);

        if (typeName == "System.IDisposable")
        {
          return false;
        }
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the destructor.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddDestructor(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string Code = @"~Disposable() {
          DisposeObject(false);
        }";

      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(Code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Adds the dispose method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddDisposeMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string code = @"
        public void Dispose() {
          DisposeObject(true);
          GC.SuppressFinalize(this);
        }";

      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
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

      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    private static void AddField(IClassDeclaration classDeclaration, CSharpElementFactory factory)
    {
      const string code = @"
        bool _disposed;
        ";

      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Adds the interface.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    private static void AddInterface(ISolution solution, IClassDeclaration classDeclaration)
    {
      IDeclarationsScope scope = DeclarationsScopeFactory.SolutionScope(solution, true);
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName("System.IDisposable");
      if (typeElement == null)
      {
        return;
      }

      IDeclaredType declaredType = TypeFactory.CreateType(typeElement);

      classDeclaration.AddSuperInterface(declaredType, false);
    }

    /// <summary>
    /// Executes the specified class declaration.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    private static void Execute(ISolution solution, IClassDeclaration classDeclaration)
    {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(classDeclaration.GetPsiModule());
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