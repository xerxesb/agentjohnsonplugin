using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI.PopupMenu;
using JetBrains.Util;

namespace AgentJohnson.Strings {
  /// <summary>
  /// Represents a Refactoring.
  /// </summary>
  public class IntroduceStringConstantRefactoring {
    #region Fields

    readonly ISolution _solution;
    readonly ITextControl _textControl;

    static readonly Regex _removeTagsRegex = new Regex("<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantRefactoring"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public IntroduceStringConstantRefactoring(ISolution solution, ITextControl textControl) {
      _solution = solution;
      _textControl = textControl;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution {
      get {
        return _solution;
      }
    }

    /// <summary>
    /// Gets the text control.
    /// </summary>
    /// <value>The text control.</value>
    public ITextControl TextControl {
      get {
        return _textControl;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    public void Execute() {
      List<string> classNames = IntroduceStringConstantSettings.Instance.ClassNames;

      if(classNames.Count == 0) {
        IntroduceLocalStringConstant();
        return;
      }

      JetPopupMenu menu = new JetPopupMenu();

      List<SimpleMenuItem> classes = new List<SimpleMenuItem>(classNames.Count + 1);

      foreach(string className in classNames) {
        SimpleMenuItem item = new SimpleMenuItem();

        item.Text = className;
        item.Style = MenuItemStyle.Enabled;

        item.Clicked += delegate {
          menu_ItemClicked(item.Text);
        };

        classes.Add(item);
      }

      IClassDeclaration classDeclaration = GetClassDeclaration();
      if(classDeclaration != null) {
        SimpleMenuItem item = new SimpleMenuItem();

        item.Text = "<Local>" + GetQualifiedClassDeclarationName(classDeclaration);
        item.Style = MenuItemStyle.Enabled;

        item.Clicked += delegate {
          menu_ItemClicked(item.Text);
        };

        classes.Add(item);
      }

      menu.Caption.Value = WindowlessControl.Create("Introduce String Constant");
      menu.SetItems(classes);
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    /// <summary>
    /// Determines whether the specified solution is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified solution is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAvailable(IElement element) {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      ITokenNode tokenNode = element as ITokenNode;
      if(tokenNode == null) {
        return false;
      }

      TokenNodeType type = tokenNode.GetTokenType();

      if(!type.IsStringLiteral) {
        return false;
      }

      ITreeNode parent = tokenNode.Parent;
      if(parent == null) {
        return true;
      }

      IConstantDeclaration constantDeclaration = parent.Parent as IConstantDeclaration;
      if(constantDeclaration != null) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Gets the type element.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="className">Name of the class.</param>
    /// <returns>The type element.</returns>
    public static IClassDeclaration GetTypeElement(ISolution solution, string className) {
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(DeclarationsCacheScope.SolutionScope(solution, true), true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName(className);
      if(typeElement == null) {
        return null;
      }

      IList<IDeclaration> declarations = typeElement.GetDeclarations();
      if(declarations == null || declarations.Count == 0) {
        return null;
      }

      return declarations[0] as IClassDeclaration;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Clips the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="length">The length.</param>
    /// <returns>The clip.</returns>
    static string Clip(string text, int length) {
      if(text.Length <= length) {
        return text;
      }

      int n = length;

      while(n >= 0 && char.IsLower(text[n])) {
        n--;
      }

      if(n < 0) {
        return text;
      }

      while(n >= 0 && !char.IsLower(text[n])) {
        n--;
      }

      if(n < 0) {
        return text;
      }

      return text.Substring(0, n);
    }

    /// <summary>
    /// Converts to pascal case.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The to pascal case.</returns>
    static string ConvertToPascalCase(string identifier) {
      StringBuilder s = new StringBuilder(identifier);

      int first = -1;

      for(int i = s.Length - 2; i >= 0; i--) {
        if(!char.IsLetterOrDigit(s[i])) {
          s[i + 1] = Char.ToUpper(s[i + 1]);
        } else {
          first = i;
        }
      }

      if(first >= 0) {
        s[first] = Char.ToUpper(s[first]);
      }

      return s.ToString();
    }

    /// <summary>
    /// Generates the function assert statements.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    void ConvertToStringConstant(IClassDeclaration classDeclaration, bool isPublic) {
      IElement element = GetElementAtCaret();
      if(element == null) {
        return;
      }

      ITreeNode treeNode = element as ITreeNode;
      if(treeNode == null) {
        return;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if(expression == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());
      if(factory == null) {
        return;
      }

      string text = treeNode.GetText();

      string identifier = GetExistingIdentifier(classDeclaration, text);

      if(string.IsNullOrEmpty(identifier)) {
        identifier = GetIdentifier(classDeclaration, text);

        string declarationText = string.Format("const string {0} = {1};", identifier, text);

        if(isPublic) {
          declarationText = "public " + declarationText;
        }

        if(IntroduceStringConstantSettings.Instance.GenerateXmlComment) {
          declarationText = "/// <summary>" + text + "</summary>\r\n" + declarationText;
        }

        IClassMemberDeclaration classMemberDeclaration = factory.CreateTypeMemberDeclaration(declarationText) as IClassMemberDeclaration;
        if(classMemberDeclaration == null) {
          return;
        }

        IClassMemberDeclaration anchor = GetClassMemberAnchor(classDeclaration, identifier);

        if(anchor != null) {
          classDeclaration.AddClassMemberDeclarationBefore(classMemberDeclaration, anchor);
        } else {
          classDeclaration.AddClassMemberDeclaration(classMemberDeclaration);
        }
      }

      if(isPublic) {
        string qualifiedName = GetQualifiedClassDeclarationName(classDeclaration);

        if(!string.IsNullOrEmpty(qualifiedName)) {
          identifier = qualifiedName + "." + identifier;
        }
      }

      ICSharpExpression identifierExpression = factory.CreateExpression(identifier);
      if(identifierExpression == null) {
        return;
      }

      ICSharpExpression result = expression.ReplaceBy(identifierExpression);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null){
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if(formatter == null){
        return;
      }

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Gets the qualified name of the class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns>The <see cref="String"/>.</returns>
    static string GetQualifiedClassDeclarationName(ICSharpDeclaration classDeclaration) {
      ICSharpNamespaceDeclaration ns = classDeclaration.GetContainingNamespaceDeclaration();

      if(ns != null) {
        return ns.QualifiedName + "." + classDeclaration.DeclaredName;
      }

      return string.Empty;
    }

    /// <summary>
    /// Executes the transaction event.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    void DoTransaction(IClassDeclaration classDeclaration, bool isPublic) {
      Debug.Assert(classDeclaration != null);

      using(ModificationCookie cookie = TextControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create("Context Action Introduce String Constant")) {
          PsiManager.GetInstance(Solution).DoTransaction(delegate { ConvertToStringConstant(classDeclaration, isPublic); });
        }
      }
    }

    /// <summary>
    /// Gets the class declaration.
    /// </summary>
    /// <returns>The class declaration.</returns>
    IClassDeclaration GetClassDeclaration() {
      IElement element = GetElementAtCaret();
      if(element == null) {
        return null;
      }

      ITreeNode treeNode = element as ITreeNode;
      if(treeNode == null) {
        return null;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if(expression == null) {
        return null;
      }

      return expression.GetContainingTypeDeclaration() as IClassDeclaration;
    }

    /// <summary>
    /// Gets the anchor.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The anchor.</returns>
    static IClassMemberDeclaration GetClassMemberAnchor(IClassDeclaration classDeclaration, string identifier) {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(identifier != null);

      IList<IConstantDeclaration> list = classDeclaration.ConstantDeclarations;

      if(list == null || list.Count == 0) {
        return null;
      }

      foreach(IConstantDeclaration declaration in list) {
        if(string.Compare(identifier, declaration.DeclaredName) < 0) {
          return declaration;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <returns>The element at caret.</returns>
    IElement GetElementAtCaret() {
      IProjectFile projectFile = DocumentManager.GetInstance(Solution).GetProjectFile(TextControl.Document);
      if(projectFile == null) {
        return null;
      }

      ICSharpFile file = PsiManager.GetInstance(Solution).GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return null;
      }

      return file.FindTokenAt(TextControl.CaretModel.Offset);
    }

    /// <summary>
    /// Gets the existing identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="text">The text.</param>
    /// <returns>The existing identifier.</returns>
    static string GetExistingIdentifier(IClassDeclaration classDeclaration, string text) {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(text != null);

      foreach(IConstantDeclaration declaration in classDeclaration.ConstantDeclarations) {
        ICSharpExpression valueExpression = declaration.ValueExpression;

        if(valueExpression == null) {
          continue;
        }

        if(valueExpression.GetText() == text) {
          return declaration.DeclaredName;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="text">The text.</param>
    /// <returns>The identifier.</returns>
    static string GetIdentifier(IClassDeclaration classDeclaration, string text) {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(text != null);

      IntroduceStringConstantSettings settings = IntroduceStringConstantSettings.Instance;

      string identifier = text;

      if(identifier.StartsWith("\"")) {
        identifier = identifier.Substring(1);
      }

      if(identifier.EndsWith("\"")) {
        identifier = identifier.Substring(0, identifier.Length - 1);
      }

      identifier = RemoveTags(identifier);
      identifier = RemoveControlChars(identifier);

      if(settings.TransformIdentifierMode == 0) {
        identifier = identifier.ToUpper();
      } else if(settings.TransformIdentifierMode == 1) {
        identifier = ConvertToPascalCase(identifier);
      }

      if(settings.ReplaceSpacesMode == 0) {
        identifier = identifier.Trim().Replace(' ', '_');
        identifier = Regex.Replace(identifier, "\\W", "_");

        while(identifier.IndexOf("__") >= 0) {
          identifier = identifier.Replace("__", "_");
        }

        while(identifier.StartsWith("_")) {
          identifier = identifier.Substring(1);
        }

        while(identifier.EndsWith("_")) {
          identifier = identifier.Substring(0, identifier.Length - 1);
        }
      } else {
        identifier = identifier.Trim().Replace(" ", string.Empty);
        identifier = Regex.Replace(identifier, "\\W", string.Empty);
      }

      if(string.IsNullOrEmpty(identifier)) {
        identifier = "Text";
      }

      if(!char.IsLetter(identifier[0])) {
        identifier = "_" + identifier;
      }

      identifier = Clip(identifier, 64);

      string result = identifier;
      int n = 1;

      while(HasIdentifier(classDeclaration, result)) {
        result = identifier + n;
        n++;
      }

      return result;
    }

    /// <summary>
    /// Determines whether the specified class declaration has identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="identifier">The identifier.</param>
    /// <returns>
    /// 	<c>true</c> if the specified class declaration has identifier; otherwise, <c>false</c>.
    /// </returns>
    static bool HasIdentifier(IClassDeclaration classDeclaration, string identifier) {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(identifier != null);

      foreach(IConstantDeclaration declaration in classDeclaration.ConstantDeclarations) {
        if(declaration.DeclaredName == identifier) {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Introduces the local string constant.
    /// </summary>
    void IntroduceLocalStringConstant() {
      IClassDeclaration classDeclaration = GetClassDeclaration();
      if(classDeclaration == null) {
        return;
      }

      IntroduceStringConstant(classDeclaration, false);
    }

    /// <summary>
    /// Introduces the local string constant.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    void IntroduceStringConstant(IClassDeclaration classDeclaration, bool isPublic) {
      Debug.Assert(classDeclaration != null);

      ITypeElement element = classDeclaration.DeclaredElement;
      if(element == null) {
        return;
      }

      IList<IProjectFile> files = element.GetProjectFiles();
      if(files == null || files.Count == 0) {
        return;
      }

      IProjectFile projectFile = files[0];
      if(projectFile == null) {
        return;
      }

      ITextControl textControl2 = EditorManager.GetInstance(Solution).OpenProjectFile(projectFile, false);
      if(TextControl == null) {
        return;
      }

      if(textControl2 == TextControl) {
        DoTransaction(classDeclaration, false);
      } else {
        using(ModificationCookie cookie = TextControl.Document.EnsureWritable()) {
          if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
            return;
          }

          DoTransaction(classDeclaration, isPublic);
        }
      }
    }

    /// <summary>
    /// Converts to pascal case.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The to pascal case.</returns>
    static string RemoveControlChars(string identifier) {
      StringBuilder s = new StringBuilder(identifier);

      for(int i = s.Length - 2; i >= 0; i--) {
        if(s[i] == '\\') {
          s.Remove(i, 2);
        }
      }

      return s.ToString();
    }

    /// <summary>
    /// Remove tags from a string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    static string RemoveTags(string text) {
      return _removeTagsRegex.Replace(text, string.Empty);
    }

    /*
    /// <summary>
    /// Handles the Describe Item event of the menu control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    void menu_DescribeItem(object sender, DescribeItemEventArgs e) {
      Debug.Assert(sender != null);
      Debug.Assert(e != null);

      bool isLocal = false;

      string className = e.Key as string;
      string shortName = className;

      if(string.IsNullOrEmpty(className)) {
        shortName = "<Unknown>";
      } else if(className.StartsWith("<Local>")) {
        isLocal = true;

        className = className.Substring(7);
        shortName = string.Empty;

        int n = className.LastIndexOf('.');

        if(n >= 0) {
          shortName = className.Substring(n + 1);
        }
      } else {
        int n = shortName.LastIndexOf('.');

        if(n >= 0) {
          shortName = shortName.Substring(n + 1);
        }
      }

      e.Descriptor.Text = shortName;
      if(!string.IsNullOrEmpty(className)) {
        e.Descriptor.ShortcutText = new RichText("(" + className + ")", TextStyle.FromForeColor(Color.LightGray));
      }

      if(shortName != "<Unknown>") {
        IClassDeclaration classDeclaration = GetTypeElement(Solution, className);

        if (classDeclaration != null) {
          e.Descriptor.Style = MenuItemStyle.Enabled;
        }
        else{
          e.Descriptor.ShortcutText = new RichText("(" + className + " - not found)", TextStyle.FromForeColor(Color.Maroon));
        }
      }

      if(isLocal) {
        e.Descriptor.Text.SetStyle(FontStyle.Italic);
        e.Descriptor.ShortcutText.SetStyle(FontStyle.Italic);
      }

      ArrayList list = e.Menu.Items as ArrayList;
      if(list == null) {
        return;
      }

      int index = list.IndexOf(e.Key) + 1;
      if(index < 1 || index > 9) {
        return;
      }

      e.Descriptor.Mnemonic = index.ToString();
    }
    */

    /// <summary>
    /// Handles the Item Clicked event of the menu control.
    /// </summary>
    /// <param name="className">Name of the class.</param>
    void menu_ItemClicked(string className) {
      using(ReadLockCookie.Create()) {
        Shell.Instance.Locks.AssertReadAccessAllowed();

        if(className.StartsWith("<Local>")) {
          IntroduceLocalStringConstant();
          return;
        }

        IClassDeclaration classDeclaration = GetTypeElement(Solution, className);
        if(classDeclaration == null){
          MessageBox.Show("Class name \"" + className + "\" not found in this solution.");
          return;
        }

        IntroduceStringConstant(classDeclaration, true);
      }
    }

    #endregion
  }
}