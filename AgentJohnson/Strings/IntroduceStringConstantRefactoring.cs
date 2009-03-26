namespace AgentJohnson.Strings
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Text;
  using System.Text.RegularExpressions;
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
  using JetBrains.ReSharper.Psi.Naming;
  using JetBrains.ReSharper.Psi.Naming.Settings;
  using JetBrains.ReSharper.Psi.Parsing;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.UI.PopupMenu;
  using JetBrains.Util;

  /// <summary>
  /// Represents a Refactoring.
  /// </summary>
  public class IntroduceStringConstantRefactoring
  {
    #region Fields

    private static readonly Regex _removeTagsRegex = new Regex("<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly ISolution _solution;
    private readonly ITextControl _textControl;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantRefactoring"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public IntroduceStringConstantRefactoring(ISolution solution, ITextControl textControl)
    {
      this._solution = solution;
      this._textControl = textControl;
    }

    #endregion

    #region Private properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    private ISolution Solution
    {
      get
      {
        return this._solution;
      }
    }

    /// <summary>
    /// Gets the text control.
    /// </summary>
    /// <value>The text control.</value>
    private ITextControl TextControl
    {
      get
      {
        return this._textControl;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Determines whether the specified solution is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified solution is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAvailable(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      ITokenNode tokenNode = element as ITokenNode;
      if (tokenNode == null)
      {
        return false;
      }

      TokenNodeType type = tokenNode.GetTokenType();

      if (!type.IsStringLiteral)
      {
        return false;
      }

      ITreeNode parent = tokenNode.Parent;
      if (parent == null)
      {
        return true;
      }

      IConstantDeclaration constantDeclaration = parent.Parent as IConstantDeclaration;
      if (constantDeclaration != null)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    public void Execute()
    {
      List<string> classNames = IntroduceStringConstantSettings.Instance.ClassNames;

      if (classNames.Count == 0)
      {
        this.IntroduceLocalStringConstant();
        return;
      }

      JetPopupMenu menu = new JetPopupMenu();

      List<SimpleMenuItem> classes = new List<SimpleMenuItem>(classNames.Count + 1);

      foreach (string className in classNames)
      {
        SimpleMenuItem item = new SimpleMenuItem
        {
          Text = className,
          Style = MenuItemStyle.Enabled
        };

        item.Clicked += delegate { this.menu_ItemClicked(item.Text); };

        classes.Add(item);
      }

      IClassDeclaration classDeclaration = this.GetClassDeclaration();
      if (classDeclaration != null)
      {
        SimpleMenuItem item = new SimpleMenuItem
        {
          Text = ("<Local>" + GetQualifiedClassDeclarationName(classDeclaration)),
          Style = MenuItemStyle.Enabled
        };

        item.Clicked += delegate { this.menu_ItemClicked(item.Text); };

        classes.Add(item);
      }

      menu.Caption.Value = WindowlessControl.Create("Introduce String Constant");
      menu.SetItems(classes.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Clips the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="length">The length.</param>
    /// <returns>The clip.</returns>
    private static string Clip(string text, int length)
    {
      if (text.Length <= length)
      {
        return text;
      }

      int n = length;

      while (n >= 0 && char.IsLower(text[n]))
      {
        n--;
      }

      if (n < 0)
      {
        return text;
      }

      while (n >= 0 && !char.IsLower(text[n]))
      {
        n--;
      }

      if (n < 0)
      {
        return text;
      }

      return text.Substring(0, n);
    }

    /// <summary>
    /// Converts to pascal case.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The to pascal case.</returns>
    private static string ConvertToPascalCase(string identifier)
    {
      StringBuilder s = new StringBuilder(identifier);

      int first = -1;

      for (int i = s.Length - 2; i >= 0; i--)
      {
        if (!char.IsLetterOrDigit(s[i]))
        {
          s[i + 1] = Char.ToUpper(s[i + 1]);
        }
        else
        {
          first = i;
        }
      }

      if (first >= 0)
      {
        s[first] = Char.ToUpper(s[first]);
      }

      return s.ToString();
    }

    /// <summary>
    /// Gets the anchor.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The anchor.</returns>
    private static IClassMemberDeclaration GetClassMemberAnchor(IClassDeclaration classDeclaration, string identifier)
    {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(identifier != null);

      IList<IConstantDeclaration> list = classDeclaration.ConstantDeclarations;

      if (list == null || list.Count == 0)
      {
        return null;
      }

      foreach (IConstantDeclaration declaration in list)
      {
        if (string.Compare(identifier, declaration.DeclaredName) < 0)
        {
          return declaration;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the existing identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="text">The text.</param>
    /// <returns>The existing identifier.</returns>
    private static string GetExistingIdentifier(IClassDeclaration classDeclaration, string text)
    {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(text != null);

      foreach (IConstantDeclaration declaration in classDeclaration.ConstantDeclarations)
      {
        ICSharpExpression valueExpression = declaration.ValueExpression;

        if (valueExpression == null)
        {
          continue;
        }

        if (valueExpression.GetText() == text)
        {
          return declaration.DeclaredName;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the qualified name of the class declaration.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <returns>The <see cref="string"/>.</returns>
    private static string GetQualifiedClassDeclarationName(ICSharpDeclaration classDeclaration)
    {
      ICSharpNamespaceDeclaration ns = classDeclaration.GetContainingNamespaceDeclaration();

      if (ns != null)
      {
        return ns.QualifiedName + "." + classDeclaration.DeclaredName;
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets the type element.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="className">Name of the class.</param>
    /// <returns>The type element.</returns>
    private static IClassDeclaration GetTypeElement(ISolution solution, string className)
    {
      IDeclarationsScope scope = DeclarationsScopeFactory.SolutionScope(solution, false);
      IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

      ITypeElement typeElement = cache.GetTypeElementByCLRName(className);
      if (typeElement == null)
      {
        return null;
      }

      IList<IDeclaration> declarations = typeElement.GetDeclarations();
      if (declarations == null || declarations.Count == 0)
      {
        return null;
      }

      return declarations[0] as IClassDeclaration;
    }

    /// <summary>
    /// Determines whether the specified class declaration has identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="identifier">The identifier.</param>
    /// <returns>
    /// 	<c>true</c> if the specified class declaration has identifier; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasIdentifier(IClassDeclaration classDeclaration, string identifier)
    {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(identifier != null);

      foreach (IConstantDeclaration declaration in classDeclaration.ConstantDeclarations)
      {
        if (declaration.DeclaredName == identifier)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Converts to pascal case.
    /// </summary>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The to pascal case.</returns>
    private static string RemoveControlChars(string identifier)
    {
      StringBuilder s = new StringBuilder(identifier);

      for (int i = s.Length - 2; i >= 0; i--)
      {
        if (s[i] == '\\')
        {
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
    private static string RemoveTags(string text)
    {
      return _removeTagsRegex.Replace(text, string.Empty);
    }

    /// <summary>
    /// Generates the function assert statements.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    private void ConvertToStringConstant(IClassDeclaration classDeclaration, bool isPublic)
    {
      IElement element = this.GetElementAtCaret();
      if (element == null)
      {
        return;
      }

      ITreeNode treeNode = element as ITreeNode;
      if (treeNode == null)
      {
        return;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      string text = treeNode.GetText();

      string identifier = GetExistingIdentifier(classDeclaration, text);

      if (string.IsNullOrEmpty(identifier))
      {
        identifier = this.GetIdentifier(classDeclaration, text);

        string declarationText = string.Format("const string {0} = {1};", identifier, text);

        if (isPublic)
        {
          declarationText = "public " + declarationText;
        }

        if (IntroduceStringConstantSettings.Instance.GenerateXmlComment)
        {
          declarationText = "/// <summary>" + text + "</summary>\r\n" + declarationText;
        }

        IClassMemberDeclaration classMemberDeclaration = factory.CreateTypeMemberDeclaration(declarationText) as IClassMemberDeclaration;
        if (classMemberDeclaration == null)
        {
          return;
        }

        IClassMemberDeclaration anchor = GetClassMemberAnchor(classDeclaration, identifier);

        if (anchor != null)
        {
          classDeclaration.AddClassMemberDeclarationBefore(classMemberDeclaration, anchor);
        }
        else
        {
          classDeclaration.AddClassMemberDeclaration(classMemberDeclaration);
        }
      }

      if (isPublic)
      {
        string qualifiedName = GetQualifiedClassDeclarationName(classDeclaration);

        if (!string.IsNullOrEmpty(qualifiedName))
        {
          identifier = qualifiedName + "." + identifier;
        }
      }

      ICSharpExpression identifierExpression = factory.CreateExpression(identifier);
      if (identifierExpression == null)
      {
        return;
      }

      ICSharpExpression result = expression.ReplaceBy(identifierExpression);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Executes the transaction event.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    private void DoTransaction(IClassDeclaration classDeclaration, bool isPublic)
    {
      Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue("Introduce String Constant", delegate
      {
        using (ReadLockCookie.Create())
        {
          using (ModificationCookie cookie = this.TextControl.Document.EnsureWritable())
          {
            if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
            {
              return;
            }

            using (CommandCookie.Create("Context Action Introduce String Constant"))
            {
              PsiManager.GetInstance(this.Solution).DoTransaction(delegate { this.ConvertToStringConstant(classDeclaration, isPublic); });
            }
          }
        }
      }
        );
    }

    /// <summary>
    /// Gets the class declaration.
    /// </summary>
    /// <returns>The class declaration.</returns>
    private IClassDeclaration GetClassDeclaration()
    {
      IElement element = this.GetElementAtCaret();
      if (element == null)
      {
        return null;
      }

      ITreeNode treeNode = element as ITreeNode;
      if (treeNode == null)
      {
        return null;
      }

      ICSharpExpression expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return null;
      }

      return expression.GetContainingTypeDeclaration() as IClassDeclaration;
    }

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <returns>The element at caret.</returns>
    private IElement GetElementAtCaret()
    {
      IProjectFile projectFile = DocumentManager.GetInstance(this.Solution).GetProjectFile(this.TextControl.Document);
      if (projectFile == null)
      {
        return null;
      }

      ICSharpFile file = PsiManager.GetInstance(this.Solution).GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return null;
      }

      return file.FindTokenAt(this.TextControl.CaretModel.Offset);
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="text">The text.</param>
    /// <returns>The identifier.</returns>
    private string GetIdentifier(IClassDeclaration classDeclaration, string text)
    {
      Debug.Assert(classDeclaration != null);
      Debug.Assert(text != null);

      IntroduceStringConstantSettings settings = IntroduceStringConstantSettings.Instance;

      string identifier = text;

      if (identifier.StartsWith("\""))
      {
        identifier = identifier.Substring(1);
      }

      if (identifier.EndsWith("\""))
      {
        identifier = identifier.Substring(0, identifier.Length - 1);
      }

      identifier = RemoveTags(identifier);
      identifier = RemoveControlChars(identifier);

      if (settings.ReplaceSpacesMode == 0)
      {
        identifier = identifier.Trim().Replace(' ', '_');
        identifier = Regex.Replace(identifier, "\\W", "_");

        while (identifier.IndexOf("__") >= 0)
        {
          identifier = identifier.Replace("__", "_");
        }

        while (identifier.StartsWith("_"))
        {
          identifier = identifier.Substring(1);
        }

        while (identifier.EndsWith("_"))
        {
          identifier = identifier.Substring(0, identifier.Length - 1);
        }
      }
      else
      {
        identifier = identifier.Trim().Replace(" ", string.Empty);
        identifier = Regex.Replace(identifier, "\\W", string.Empty);
      }

      if (string.IsNullOrEmpty(identifier))
      {
        identifier = "Text";
      }

      if (!char.IsLetter(identifier[0]))
      {
        identifier = "_" + identifier;
      }

      identifier = Clip(identifier, 64);

      NamingRule rule = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings2().PredefinedNamingRules[NamedElementKinds.Constants].NamingRule;
      INamingRulesProvider rulesManager = NamingManager.GetRulesProvider(classDeclaration.Language);
      identifier = NameParser.Parse(identifier, rule, rulesManager, this.Solution).GetCanonicalName();

      string result = identifier;
      int n = 1;

      while (HasIdentifier(classDeclaration, result))
      {
        result = identifier + n;
        n++;
      }

      return result;
    }

    /// <summary>
    /// Introduces the local string constant.
    /// </summary>
    private void IntroduceLocalStringConstant()
    {
      IClassDeclaration classDeclaration = this.GetClassDeclaration();
      if (classDeclaration == null)
      {
        return;
      }

      this.IntroduceStringConstant(classDeclaration, false);
    }

    /// <summary>
    /// Introduces the local string constant.
    /// </summary>
    /// <param name="classDeclaration">The class declaration.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    private void IntroduceStringConstant(IClassDeclaration classDeclaration, bool isPublic)
    {
      Debug.Assert(classDeclaration != null);

      ITypeElement element = classDeclaration.DeclaredElement;
      if (element == null)
      {
        return;
      }

      IList<IProjectFile> files = element.GetProjectFiles();
      if (files == null || files.Count == 0)
      {
        return;
      }

      IProjectFile projectFile = files[0];
      if (projectFile == null)
      {
        return;
      }

      EditorManager editorManager = EditorManager.GetInstance(this.Solution);

      ITextControl textControl2 = editorManager.GetTextControl(projectFile);
      if (this.TextControl == null)
      {
        return;
      }

      if (textControl2 == this.TextControl)
      {
        this.DoTransaction(classDeclaration, false);
      }
      else
      {
        using (ModificationCookie cookie = this.TextControl.Document.EnsureWritable())
        {
          if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
          {
            return;
          }

          this.DoTransaction(classDeclaration, isPublic);
        }
      }
    }

    /// <summary>
    /// Handles the Item Clicked event of the menu control.
    /// </summary>
    /// <param name="className">Name of the class.</param>
    private void menu_ItemClicked(string className)
    {
      if (className.StartsWith("<Local>"))
      {
        this.IntroduceLocalStringConstant();
        return;
      }

      IClassDeclaration classDeclaration = GetTypeElement(this.Solution, className);
      if (classDeclaration == null)
      {
        System.Windows.Forms.MessageBox.Show("Class name \"" + className + "\" not found in this solution.");
        return;
      }

      this.IntroduceStringConstant(classDeclaration, true);
    }

    #endregion
  }
}