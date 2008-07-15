using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.LiveTemplates.Execution;
using JetBrains.ReSharper.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.PopupWindowManager;
using JetBrains.Util;

namespace AgentJohnson.Refactorings {
  /// <summary>
  /// Handles Smart Generation, see <c>Actions.xml</c>
  /// </summary>
  [ActionHandler("SmartGenerate")]
  public class SmartGenerate : IActionHandler {
    #region Fields

    ISolution _solution;
    XmlDocument _templates;
    ITextControl _textControl;

    #endregion

    #region Public methods

    ///<summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>.Enabled to true.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="nextExecute">delegate to call</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute) {
      ISolution solution = context.GetData(DataConstants.SOLUTION);
      if(solution == null) {
        return;
      }

      Execute(solution, context);
    }

    ///<summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) {
      return context.CheckAllNotNull(DataConstants.SOLUTION);
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected void Execute(ISolution solution, IDataContext context) {
      _solution = solution;
      _textControl = context.GetData(DataConstants.TEXT_CONTROL);

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      IElement element;
      
      if (!GetElementAtCaret(_solution, _textControl, out element)) {
        return;
      }

      if(element == null) {
        Insert("63CBED21-2B8A-4722-B585-6F90C35BC0E5");
        return;
      }

      GenerateInsideFile(items, element);
      GenerateInsideNamespace(items, element);
      GenerateInsideClass(items, element);
      GenerateInsideEnum(items, element);
      GenerateInsideInterface(items, element);
      GenerateInsideStruct(items, element);
      GenerateInsideProperty(items, element);
      GenerateInsideLoop(items, element);
      GenerateInsideSwitch(items, element);
      GenerateAfterIf(items, element);
      GenerateInsideMethod(items, element);
      GenerateReturn(items, element);

      if(items.Length() == 0) {
        return;
      }

      JetPopupMenu menu = new JetPopupMenu();

      IPopupWindowContext popupWindowContext = context.GetData(JetBrains.UI.DataConstants.POPUP_WINDOW_CONTEXT);
      if(popupWindowContext != null) {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Smart Generate");
      menu.SetItems(items);
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <returns>The element.</returns>
    protected static bool GetElementAtCaret(ISolution solution, ITextControl textControl, out IElement element) {
      element = null;

      IProjectFile projectFile = DocumentManager.GetInstance(solution).GetProjectFile(textControl.Document);
      if(projectFile == null) {
        return false;
      }

      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null) {
        return false;
      }

      if (!psiManager.IsCommitted(projectFile)) {
        return false;
      }

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return false;
      }

      element = file.FindTokenAt(textControl.CaretModel.Offset);

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="text">The text.</param>
    /// <param name="template">The template.</param>
    /// <param name="parameters">The parameters.</param>
    void AddMenuItem(List<SimpleMenuItem> items, string text, string template, params string[] parameters) {
      if(!template.StartsWith("<Template")) {
        template = GetTemplate(template);
      }

      if(string.IsNullOrEmpty(template)) {
        return;
      }

      if(parameters.Length > 0) {
        text = string.Format(text, parameters);
        template = string.Format(template, parameters);
      }

      SimpleMenuItem item = new SimpleMenuItem { Text = text, Style = MenuItemStyle.Enabled, Tag = template };

      item.Clicked += Complete;

      items.Add(item);
    }

    /// <summary>
    /// Handles the Clicked event of the item control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Complete(object sender, EventArgs e) {
      SimpleMenuItem item = sender as SimpleMenuItem;
      if(item == null) {
        return;
      }

      string xml = (item.Tag as string) ?? string.Empty;

      InsertTemplate(xml);
    }

    /// <summary>
    /// Completes if else statement.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateAfterIf(List<SimpleMenuItem> items, IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return;
      }
      IElement statement = element.GetContainingElement(typeof(IStatement), true);
      if(statement != null && !block.Contains(statement)) {
        return;
      }

      IIfStatement ifStatement = GetPreviousStatement(block, element) as IIfStatement;
      if(ifStatement == null) {
        return;
      }

      IStatement elseStatement = ifStatement.Else;
      while(elseStatement != null && elseStatement is IIfStatement) {
        elseStatement = (elseStatement as IIfStatement).Else;
      }
      if(elseStatement != null) {
        return;
      }

      AddMenuItem(items, "else", "9F134F1B-3F0D-4C9E-B549-A469828D1A7F");
      AddMenuItem(items, "else if...", "94F834F9-110D-4608-A780-9BD05FE826A1");
    }

    /// <summary>
    /// Completes the inside class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideClass(List<SimpleMenuItem> items, IElement element) {
      IClassDeclaration classDeclaration = element.GetContainingElement(typeof(IClassDeclaration), true) as IClassDeclaration;
      if(classDeclaration == null) {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IClassDeclaration)) {
        return;
      }

      string modifier = GetModifier(element, classDeclaration);

      AddMenuItem(items, "{0}<Type> <Property>...", "a684b217-f179-431b-a485-e3d76dbe57fd", modifier);
      AddMenuItem(items, "{0}void <Method>...", "85BBC654-4EE4-4932-BB0C-E0670FA1BB82", modifier);
    }

    /// <summary>
    /// Completes the enum statement.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideEnum(List<SimpleMenuItem> items, IElement element) {
      IEnumDeclaration enumDeclaration = element.GetContainingElement(typeof(IEnumDeclaration), false) as IEnumDeclaration;
      if(enumDeclaration == null) {
        return;
      }

      AddMenuItem(items, "<Constant>", "587F88E2-6876-41F2-885C-58AD93BBC8B4");
    }

    /// <summary>
    /// Generates the inside switch.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideSwitch(List<SimpleMenuItem> items, IElement element) {
      ISwitchStatement switchStatement = element.GetContainingElement(typeof(ISwitchStatement), false) as ISwitchStatement;
      if(switchStatement == null) {
        return;
      }

      IBlock block = switchStatement.Block;
      if (block == null) {
        return;
      }

      if (element.ToTreeNode().Parent != block) {
        return;
      }

      AddMenuItem(items, "case <Value>:...", "16E39695-5810-4C3E-A3CD-AB0CC0127C60");
    }

    /// <summary>
    /// Completes the namespace.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideFile(List<SimpleMenuItem> items, IElement element) {
      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration != null) {
        return;
      }

      AddMenuItem(items, "namespace <Namespace>...", "63CBED21-2B8A-4722-B585-6F90C35BC0E5");
    }

    /// <summary>
    /// Completes the inside interface.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideInterface(List<SimpleMenuItem> items, IElement element) {
      IElement interfaceDeclaration = element.GetContainingElement(typeof(IInterfaceDeclaration), true);
      if(interfaceDeclaration == null) {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(ITypeMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IInterfaceDeclaration)) {
        return;
      }

      AddMenuItem(items, "<Property>...", "D6EB42DA-2858-46B3-8CB3-E3DEFB245D11");
      AddMenuItem(items, "<Method>...", "B3DB6158-D43E-42EE-8E67-F10CF7344106");
    }

    /// <summary>
    /// Completes the class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideNamespace(List<SimpleMenuItem> items, IElement element) {
      IElement classLikeDeclaration = element.GetContainingElement(typeof(IClassLikeDeclaration), true);
      if(classLikeDeclaration != null) {
        return;
      }

      IEnumDeclaration enumDecl = element.GetContainingElement(typeof(IEnumDeclaration), true) as IEnumDeclaration;
      if(enumDecl != null) {
        return;
      }

      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration == null) {
        return;
      }

      AddMenuItem(items, "public class <Class>...", "0BC4B773-20A9-4F12-B486-5C5DB7D39C73");
      AddMenuItem(items, "public enum <Enum>...", "3B6DA53E-E57F-4A22-ACF6-55F65645AF92");
      AddMenuItem(items, "public interface <Interface>...", "7D2E8D45-8562-45DD-8415-3E98F0EC24BD");
      AddMenuItem(items, "public struct <Struct>...", "51BFA78B-7FDA-42CC-85E4-8B29BB1103E5");
    }

    /// <summary>
    /// Completes the inside struct.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideProperty(List<SimpleMenuItem> items, IElement element) {
      IProperty propertyDeclaration = element.GetContainingElement(typeof(IPropertyDeclaration), true) as IProperty;
      if(propertyDeclaration == null) {
        return;
      }

      IAccessorDeclaration accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;
      if(accessorDeclaration == null) {
        return;
      }

      IBlock body = accessorDeclaration.Body;
      if(body == null || body.Statements.Count > 0) {
        return;
      }

      string name = propertyDeclaration.ShortName;
      if(string.IsNullOrEmpty(name)) {
        return;
      }

      char[] charArray = name.ToCharArray();
      charArray[0] = char.ToLower(charArray[0]);
      name = new string(charArray);

      string prefix = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings().FieldNameSettings.Prefix;

      string typeName = propertyDeclaration.Type.GetPresentableName(element.Language);

      if(accessorDeclaration.Kind == AccessorKind.GETTER) {
        AddMenuItem(items, "return {0}{1};", "0E03C1D4-A5DF-4011-86AE-4E561E419CD0", prefix, name);

        if(typeName == "string") {
          AddMenuItem(items, "return {0}{1} ?? string.Empty;", "BDC2EEB0-F626-4D6A-AB06-DD5C7C80BB30", prefix, name);
        }
      }

      if(accessorDeclaration.Kind == AccessorKind.SETTER) {
        AddMenuItem(items, "{0}{1} = value;", "A97E734B-C24B-44A3-A914-C67BBB3FAC65", prefix, name);
      }
    }

    /// <summary>
    /// Completes the inside struct.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideStruct(List<SimpleMenuItem> items, IElement element) {
      IStructDeclaration structDeclaration = element.GetContainingElement(typeof(IStructDeclaration), true) as IStructDeclaration;
      if(structDeclaration == null) {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IStructDeclaration)) {
        return;
      }

      string modifier = GetModifier(element, structDeclaration);

      AddMenuItem(items, "{0}<Type> <Property>...", "a684b217-f179-431b-a485-e3d76dbe57fd", modifier);
      AddMenuItem(items, "{0}void <Method>...", "85BBC654-4EE4-4932-BB0C-E0670FA1BB82", modifier);
    }

    /// <summary>
    /// Completes if statement.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideMethod(List<SimpleMenuItem> items, IElement element) {
      string name;
      IType type;

      GetNearestVariable(element, out name, out type);
      if(type == null) {
        return;
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType != null) {
        IEnum enumerate = declaredType.GetTypeElement() as IEnum;

        if(enumerate != null) {
          AddMenuItem(items, "switch({0})...", "EBAF3559-41C5-471D-8457-A20C9566D397", name);
        }


        IModule module = declaredType.Module;
        if(module != null) {
          IDeclaredType enumerable = TypeFactory.CreateTypeByCLRName("System.Collections.IEnumerable", module);

          if(declaredType.IsSubtypeOf(enumerable)) {
            AddMenuItem(items, "foreach(<Variable> in {0}))...", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", name);
          }
        }
      }
      else {
        string presentableName = type.GetPresentableName(element.Language);
        if (presentableName.IndexOf("[]") >= 0) {
          AddMenuItem(items, "foreach(<Variable> in {0}))...", "9CA009C7-468A-4D3E-ACEC-A12F2FAF4B67", name);
        }
      }

      if(type.GetPresentableName(element.Language) == "string") {
        AddMenuItem(items, "if (string.IsNullOrEmpty({0}))...", "514313A0-91F4-4AE5-B4EB-2BB53736A023", name);
      }
      else {
        if(type.IsReferenceType()) {
          AddMenuItem(items, "if ({0} == null)...", "F802DB32-A0B1-4227-BE5C-E7D20670284B", name);
        }
      }

      // AddMenuItem(items, "if ({0}.<Method>)...", "1438A7F2-B12C-4784-BFDE-A803FA8F1279", name);
      AddMenuItem(items, "var <Variable> = {0}.<Method>;", "11BACA25-C561-4FE8-934B-41246B7CFAC9", name);
      AddMenuItem(items, "if ({0}.<Method> == <Value>)...", "43E2C069-A3E6-4649-A374-104A16C59305", name);
      AddMenuItem(items, "{0}.<Method>;", "FE9C6A6B-A068-4182-B301-8002FE05A458", name);
    }

    /// <summary>
    /// Completes the loop statement.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateInsideLoop(List<SimpleMenuItem> items, IElement element) {
      bool hasLoop = element.GetContainingElement(typeof(IForeachStatement), false) as IForeachStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IForStatement), false) as IForStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IWhileStatement), false) as IWhileStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IDoStatement), false) as IDoStatement != null;

      if(!hasLoop) {
        return;
      }

      if(!IsAfterLastStatement(element)) {
        return;
      }

      AddMenuItem(items, "continue;", "F849A86C-A93E-4805-B8E1-4B02CA8807CC");
      AddMenuItem(items, "break;", "42DA21AD-1F9F-4ECE-B5F6-E7AFC5EAAE14");
    }

    /// <summary>
    /// Completes the return statement.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="element">The element.</param>
    void GenerateReturn(List<SimpleMenuItem> items, IElement element) {
      if(!IsAfterLastStatement(element)) {
        return;
      }

      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return;
      }

      string typeName = string.Empty;
      IType returnType = null;

      IFunction function = block.GetContainingTypeMemberDeclaration() as IFunction;
      if(function != null) {
        returnType = function.ReturnType;
        typeName = returnType.GetPresentableName(element.Language);
      }
      else {
        IProperty property = block.GetContainingTypeMemberDeclaration() as IProperty;
        if(property != null) {
          IAccessorDeclaration accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;

          if(accessorDeclaration != null && accessorDeclaration.Kind == AccessorKind.GETTER) {
            returnType = property.Type;
            typeName = returnType.GetPresentableName(element.Language);
          }
        }
      }

      // return;
      if(string.IsNullOrEmpty(typeName) || typeName == "void") {
        IFunctionDeclaration functionDeclaration = function as IFunctionDeclaration;

        if(functionDeclaration != null && functionDeclaration.Body != block) {
          AddMenuItem(items, "return;", "19B0E24A-C3C3-489A-BF20-122C5114D7FF");
        }
      }
      else if(typeName == "bool") {
        AddMenuItem(items, "return true;", "459C8B38-0048-43DF-9279-3E946A3A65F2");
        AddMenuItem(items, "return false;", "9F342BE4-4A55-48FF-BECF-A67C7D79BF76");
      }
      else {
        AddMenuItem(items, "return <Value>;", "39530254-7198-4A3C-B528-6160324E9792");

        if(returnType != null && returnType.IsReferenceType()) {
          AddMenuItem(items, "return null;", "D34007F3-C131-46F4-96B7-8D2654727D0B");
        }
      }
    }

    /// <summary>
    /// Gets the modifier.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns></returns>
    static string GetModifier(IElement element, IClassLikeDeclaration classDeclaration) {
      ITypeMemberDeclaration classMember = null;

      int caret = element.GetTreeStartOffset();

      foreach(ICSharpTypeMemberDeclaration typeMemberDeclaration in classDeclaration.MemberDeclarations) {
        if(typeMemberDeclaration.GetTreeStartOffset() > caret) {
          break;
        }

        classMember = typeMemberDeclaration;
      }

      string modifier = "public";

      IAccessRightsOwner accessRightsOwner = classMember as IAccessRightsOwner;
      if(accessRightsOwner != null) {
        AccessRights rights = accessRightsOwner.GetAccessRights();
        switch(rights) {
          case AccessRights.PUBLIC:
            modifier = "public";
            break;
          case AccessRights.INTERNAL:
            modifier = "internal";
            break;
          case AccessRights.PROTECTED:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_OR_INTERNAL:
            modifier = "protected";
            break;
          case AccessRights.PROTECTED_AND_INTERNAL:
            modifier = "protected internal";
            break;
          case AccessRights.PRIVATE:
            modifier = "";
            break;
          case AccessRights.NONE:
            modifier = "";
            break;
        }
      }

      if(!string.IsNullOrEmpty(modifier)) {
        modifier += ' ';
      }

      return modifier;
    }

    /// <summary>
    /// Gets the previous statement.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="block">The block.</param>
    static IStatement GetPreviousStatement(IBlock block, IElement element) {
      IStatement result = null;

      int caret = element.GetTreeStartOffset();

      foreach(IStatement stm in block.Statements) {
        if(stm.GetTreeStartOffset() > caret) {
          break;
        }

        result = stm;
      }

      return result;
    }

    /// <summary>
    /// Gets the template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>The template.</returns>
    [CanBeNull]
    string GetTemplate([NotNull] string template) {
      if(_templates == null) {
        string filename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SmartGenerate.xml";

        if(!File.Exists(filename)) {
          return null;
        }

        _templates = new XmlDocument();

        _templates.Load(filename);
      }

      XmlNode node = _templates.SelectSingleNode(string.Format("/*/Template[@uid='{0}' or shortcut='{0}']", template));
      if(node == null) {
        return null;
      }

      return node.OuterXml;
    }

    /// <summary>
    /// Inserts the specified template.
    /// </summary>
    /// <param name="template">The template.</param>
    void Insert([NotNull] string template) {
      string xml = GetTemplate(template);
      if(string.IsNullOrEmpty(xml)) {
        return;
      }

      InsertTemplate(xml);
    }

    /// <summary>
    /// Inserts the template.
    /// </summary>
    /// <param name="xml">The XML.</param>
    void InsertTemplate([NotNull] string xml) {
      XmlDocument doc = new XmlDocument();

      doc.LoadXml(xml);

      Template template = Template.CreateFromXml(doc.DocumentElement);

      LiveTemplatesController.Instance.ExecuteTemplate(_solution, template, _textControl);
    }

    /// <summary>
    /// Determines whether [is after last statement] [the specified element].
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if [is after last statement] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    static bool IsAfterLastStatement(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return false;
      }

      if(block.Statements.Count <= 0) {
        return true;
      }

      IStatement statement = block.Statements[block.Statements.Count - 1];
      DocumentRange range = statement.GetDocumentRange();

      int end = range.TextRange.StartOffset + range.TextRange.Length;
      if(end > element.GetTreeTextRange().StartOffset) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Gets the nearest variable.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    static void GetNearestVariable(IElement element, out string name, out IType type) {
      name = null;
      type = null;

      ITreeNode node = element.ToTreeNode();

      while(node != null) {
        // local variable
        IDeclarationStatement declarationStatement = node as IDeclarationStatement;
        if(declarationStatement != null) {
          IList<ILocalVariableDeclaration> localVariableDeclarations = declarationStatement.VariableDeclarations;
       
          if(localVariableDeclarations.Count == 1) {
            ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];

            if(localVariableDeclaration != null) {
              ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
 
              if(localVariable != null) {
                name = localVariable.ShortName;
                type = localVariable.Type;
                return;
              }
            }
          }
        }

        ITreeNode prevSibling = node.PrevSibling;

        if (prevSibling == null) {
          node = node.Parent;

          // foreach
          IForeachStatement foreachStatement = node as IForeachStatement;
          if(foreachStatement != null) {
          }

          // for
          IForStatement forStatement = node as IForStatement;
          if(forStatement != null) {
            IList<ILocalVariableDeclaration> initializerDeclarations = forStatement.InitializerDeclarations;
            if(initializerDeclarations.Count == 1) {
              ILocalVariableDeclaration localVariableDeclaration = initializerDeclarations[0];
              if(localVariableDeclaration != null) {
                ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;

                if(localVariable != null) {
                  name = localVariable.ShortName;
                  type = localVariable.Type;
                  return;
                }
              }
            }
          }

          // parameter
          IParametersOwner parametersOwner = node as IParametersOwner;
          if (parametersOwner != null) {
            if (parametersOwner.Parameters.Count > 0) {
              IParameter parameter = parametersOwner.Parameters[0];
              name = parameter.ShortName;
              type = parameter.Type;
              return;
            }
          }
        }
        else {
          node = prevSibling;
        }
      }
    }

    #endregion
  }
}