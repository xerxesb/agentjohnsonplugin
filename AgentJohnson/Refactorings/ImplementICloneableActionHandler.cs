using System.Collections.Generic;
using System.Text;
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
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.Refactorings {
  /// <summary>
  /// </summary>
  [ActionHandler("AgentJohnson.ImplementICloneable")]
  public class ImplementICloneableActionHandler : ActionHandlerBase {
    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected override void Execute(ISolution solution, IDataContext context) {
      if(!context.CheckAllNotNull(DataConstants.SOLUTION)) {
        return;
      }

      ITextControl textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if(textControl == null) {
        return;
      }

      IElement element = GetElementAtCaret(context);
      if(element == null) {
        return;
      }

      IClassDeclaration classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if(classDeclaration == null) {
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create("Context Action Implement ICloneable")) {
          PsiManager.GetInstance(solution).DoTransaction(delegate {
            Execute(solution, classDeclaration);
          });
        }
      }
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    protected override bool Update(IDataContext context) {
      if(!context.CheckAllNotNull(DataConstants.SOLUTION)) {
        return false;
      }

      IElement element = GetElementAtCaret(context);
      if(element == null) {
        return false;
      }

      IClassDeclaration classDeclaration = element.ToTreeNode().Parent as IClassDeclaration;
      if(classDeclaration == null) {
        return false;
      }

      foreach(IDeclaredType type in classDeclaration.SuperTypes) {
        string typeName = type.GetLongPresentableName(element.Language);

        if(typeName == "System.ICloneable") {
          return false;
        }
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the dispose object method.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="factory">The factory.</param>
    static void AddCloneMethod(IClassDeclaration classDeclaration, CSharpElementFactory factory) {
      IClass cls = classDeclaration.DeclaredElement as IClass;
      if(cls == null) {
        return;
      }

      string code = "var result = new " + cls.ShortName + "();";

      code += AddCloneMethodCode(cls, false);

      foreach(IDeclaredType declaredType in cls.GetSuperTypes()) {
        ResolveResult resolve = declaredType.Resolve();

        IClass superClass = resolve.DeclaredElement as IClass;
        if(superClass == null) {
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
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isSuperClass">if set to <c>true</c> [is super class].</param>
    /// <returns></returns>
    static string AddCloneMethodCode(IClass classDeclaration, bool isSuperClass) {
      StringBuilder code = new StringBuilder();

      foreach(IField field in classDeclaration.Fields) {
        if(isSuperClass) {
          AccessRights rights = field.GetAccessRights();
          if(rights == AccessRights.PRIVATE) {
            continue;
          }
        }

        code.Append(string.Format("\r\nresult.{0} = {0};", field.ShortName));
      }

      foreach(IProperty property in classDeclaration.Properties) {
        if(isSuperClass) {
          AccessRights rights = property.GetAccessRights();
          if(rights == AccessRights.PRIVATE) {
            continue;
          }
        }

        IList<IDeclaration> declarations = property.GetDeclarations();
        if(declarations.Count != 1) {
          continue;
        }

        IPropertyDeclaration propertyDeclaration = declarations[0] as IPropertyDeclaration;
        if(propertyDeclaration == null || !propertyDeclaration.IsAuto) {
          continue;
        }

        code.Append(string.Format("\r\nresult.{0} = {0};", property.ShortName));
      }

      return code.ToString();
    }

    /// <summary>
    /// Adds the interface.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    static void AddInterface(ISolution solution, IClassDeclaration classDeclaration) {
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(DeclarationsCacheScope.SolutionScope(solution, true), true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName("System.ICloneable");
      if(typeElement == null) {
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
    /// <param name="code">The code.</param>
    static void AddMember(IClassDeclaration classDeclaration, CSharpElementFactory factory, string code) {
      IClassMemberDeclaration memberDeclaration = factory.CreateTypeMemberDeclaration(code) as IClassMemberDeclaration;

      classDeclaration.AddClassMemberDeclaration(memberDeclaration);
    }

    /// <summary>
    /// Executes the specified class declaration.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    static void Execute(ISolution solution, IClassDeclaration classDeclaration) {
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(classDeclaration.GetProject());
      if(factory == null) {
        return;
      }

      AddInterface(solution, classDeclaration);
      AddCloneMethod(classDeclaration, factory);
    }

    #endregion
  }
}