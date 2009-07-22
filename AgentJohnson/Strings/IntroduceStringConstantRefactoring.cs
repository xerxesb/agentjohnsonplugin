// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntroduceStringConstantRefactoring.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents a Refactoring.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using System.Collections.Generic;
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
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.UI.PopupMenu;
  using JetBrains.Util;

  /// <summary>
  /// Represents a Refactoring.
  /// </summary>
  public class IntroduceStringConstantRefactoring
  {
    #region Constants and Fields

    /// <summary>
    /// Remove tags regular expression.
    /// </summary>
    private static readonly Regex removeTagsRegex = new Regex("<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// The current solution.
    /// </summary>
    private readonly ISolution solution;

    /// <summary>
    /// The text control.
    /// </summary>
    private readonly ITextControl textControl;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantRefactoring"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="textControl">
    /// The text control.
    /// </param>
    public IntroduceStringConstantRefactoring(ISolution solution, ITextControl textControl)
    {
      this.solution = solution;
      this.textControl = textControl;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    private ISolution Solution
    {
      get
      {
        return this.solution;
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
        return this.textControl;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether the specified solution is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified solution is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAvailable(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      var tokenNode = element as ITokenNode;
      if (tokenNode == null)
      {
        return false;
      }

      var type = tokenNode.GetTokenType();
      if (!type.IsStringLiteral)
      {
        return false;
      }

      var parent = tokenNode.Parent;
      if (parent == null)
      {
        return true;
      }

      var constantDeclaration = parent.Parent as IConstantDeclaration;
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
      var classNames = IntroduceStringConstantSettings.Instance.ClassNames;
      if (classNames.Count == 0)
      {
        this.IntroduceLocalStringConstant();
        return;
      }

      var menu = new JetPopupMenu();

      var classes = new List<SimpleMenuItem>(classNames.Count + 1);
      foreach (var className in classNames)
      {
        var item = new SimpleMenuItem
        {
          Text = className,
          Style = MenuItemStyle.Enabled
        };

        item.Clicked += delegate { this.MenuItemClicked(item.Text); };

        classes.Add(item);
      }

      var classDeclaration = this.GetClassDeclaration();
      if (classDeclaration != null)
      {
        var item = new SimpleMenuItem
        {
          Text = "<Local>" + GetQualifiedClassDeclarationName(classDeclaration),
          Style = MenuItemStyle.Enabled
        };

        item.Clicked += delegate { this.MenuItemClicked(item.Text); };

        classes.Add(item);
      }

      menu.Caption.Value = WindowlessControl.Create("Introduce String Constant");
      menu.SetItems(classes.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clips the specified text.
    /// </summary>
    /// <param name="text">
    /// The text to clip.
    /// </param>
    /// <param name="length">
    /// The length.
    /// </param>
    /// <returns>
    /// The clipped text.
    /// </returns>
    private static string Clip(string text, int length)
    {
      if (text.Length <= length)
      {
        return text;
      }

      var n = length;
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
    /// Gets the anchor.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="identifier">
    /// The identifier.
    /// </param>
    /// <returns>
    /// The anchor.
    /// </returns>
    private static IClassMemberDeclaration GetClassMemberAnchor(IClassDeclaration classDeclaration, string identifier)
    {
      var list = classDeclaration.ConstantDeclarations;

      if (list == null || list.Count == 0)
      {
        return null;
      }

      foreach (var declaration in list)
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
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="text">
    /// The identifier text.
    /// </param>
    /// <returns>
    /// The existing identifier.
    /// </returns>
    private static string GetExistingIdentifier(IClassDeclaration classDeclaration, string text)
    {
      foreach (var declaration in classDeclaration.ConstantDeclarations)
      {
        var valueExpression = declaration.ValueExpression;

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
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static string GetQualifiedClassDeclarationName(ICSharpDeclaration classDeclaration)
    {
      var ns = classDeclaration.GetContainingNamespaceDeclaration();

      if (ns != null)
      {
        return ns.QualifiedName + "." + classDeclaration.DeclaredName;
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets the type element.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="className">
    /// Name of the class.
    /// </param>
    /// <returns>
    /// The type element.
    /// </returns>
    private static IClassDeclaration GetTypeElement(ISolution solution, string className)
    {
      var scope = DeclarationsScopeFactory.SolutionScope(solution, false);
      var cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

      var typeElement = cache.GetTypeElementByCLRName(className);
      if (typeElement == null)
      {
        return null;
      }

      var declarations = typeElement.GetDeclarations();
      if (declarations == null || declarations.Count == 0)
      {
        return null;
      }

      return declarations[0] as IClassDeclaration;
    }

    /// <summary>
    /// Determines whether the specified class declaration has identifier.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="identifier">
    /// The identifier.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified class declaration has identifier; otherwise, <c>false</c>.
    /// </returns>
    private static bool HasIdentifier(IClassDeclaration classDeclaration, string identifier)
    {
      foreach (var declaration in classDeclaration.ConstantDeclarations)
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
    /// <param name="identifier">
    /// The identifier.
    /// </param>
    /// <returns>
    /// The to pascal case.
    /// </returns>
    private static string RemoveControlChars(string identifier)
    {
      var s = new StringBuilder(identifier);

      for (var i = s.Length - 2; i >= 0; i--)
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
    /// <param name="text">
    /// The text to remove tags from.
    /// </param>
    /// <returns>
    /// The text without tags.
    /// </returns>
    private static string RemoveTags(string text)
    {
      return removeTagsRegex.Replace(text, string.Empty);
    }

    /// <summary>
    /// Generates the function assert statements.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="isPublic">
    /// if set to <c>true</c> [is public].
    /// </param>
    private void ConvertToStringConstant(IClassDeclaration classDeclaration, bool isPublic)
    {
      var element = this.GetElementAtCaret();
      if (element == null)
      {
        return;
      }

      var treeNode = element as ITreeNode;
      if (treeNode == null)
      {
        return;
      }

      var expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var text = treeNode.GetText();

      var identifier = GetExistingIdentifier(classDeclaration, text);

      if (string.IsNullOrEmpty(identifier))
      {
        identifier = this.GetIdentifier(classDeclaration, text);

        var declarationText = string.Format("const string {0} = {1};", identifier, text);

        if (isPublic)
        {
          declarationText = "public " + declarationText;
        }

        if (IntroduceStringConstantSettings.Instance.GenerateXmlComment)
        {
          declarationText = "/// <summary>" + text + "</summary>\r\n" + declarationText;
        }

        var classMemberDeclaration =
          factory.CreateTypeMemberDeclaration(declarationText) as IClassMemberDeclaration;
        if (classMemberDeclaration == null)
        {
          return;
        }

        var anchor = GetClassMemberAnchor(classDeclaration, identifier);

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
        var qualifiedName = GetQualifiedClassDeclarationName(classDeclaration);

        if (!string.IsNullOrEmpty(qualifiedName))
        {
          identifier = qualifiedName + "." + identifier;
        }
      }

      var identifierExpression = factory.CreateExpression(identifier);
      if (identifierExpression == null)
      {
        return;
      }

      var result = expression.ReplaceBy(identifierExpression);

      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      var formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      var range = result.GetDocumentRange();
      var marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Executes the transaction event.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="isPublic">
    /// if set to <c>true</c> [is public].
    /// </param>
    private void DoTransaction(IClassDeclaration classDeclaration, bool isPublic)
    {
      Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
        "Introduce String Constant",
        delegate
        {
          using (ReadLockCookie.Create())
          {
            using (var cookie = this.TextControl.Document.EnsureWritable())
            {
              if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
              {
                return;
              }

              using (CommandCookie.Create("Context Action Introduce String Constant"))
              {
                PsiManager.GetInstance(this.Solution).DoTransaction(
                  delegate { this.ConvertToStringConstant(classDeclaration, isPublic); });
              }
            }
          }
        });
    }

    /// <summary>
    /// Gets the class declaration.
    /// </summary>
    /// <returns>
    /// The class declaration.
    /// </returns>
    private IClassDeclaration GetClassDeclaration()
    {
      var element = this.GetElementAtCaret();
      if (element == null)
      {
        return null;
      }

      var treeNode = element as ITreeNode;
      if (treeNode == null)
      {
        return null;
      }

      var expression = treeNode.Parent as ICSharpExpression;
      if (expression == null)
      {
        return null;
      }

      return expression.GetContainingTypeDeclaration() as IClassDeclaration;
    }

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <returns>
    /// The element at caret.
    /// </returns>
    private IElement GetElementAtCaret()
    {
      var projectFile = DocumentManager.GetInstance(this.Solution).GetProjectFile(this.TextControl.Document);
      if (projectFile == null)
      {
        return null;
      }

      var file = PsiManager.GetInstance(this.Solution).GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return null;
      }

      return file.FindTokenAt(this.TextControl.CaretModel.Offset);
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="text">
    /// The identifier text.
    /// </param>
    /// <returns>
    /// The identifier.
    /// </returns>
    private string GetIdentifier(IClassDeclaration classDeclaration, string text)
    {
      var settings = IntroduceStringConstantSettings.Instance;

      var identifier = text;

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
        identifier = "Text_" + identifier;
      }

      var rule = CodeStyleSettingsManager.Instance.CodeStyleSettings.GetNamingSettings2().PredefinedNamingRules[NamedElementKinds.Constants].NamingRule;
      var rulesManager = NamingManager.GetRulesProvider(classDeclaration.Language);
      identifier = NameParser.Parse(identifier, rule, rulesManager, this.Solution).GetCanonicalName();

      identifier = Clip(identifier, 64);

      var result = identifier;
      var n = 1;

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
      var classDeclaration = this.GetClassDeclaration();
      if (classDeclaration == null)
      {
        return;
      }

      this.IntroduceStringConstant(classDeclaration, false);
    }

    /// <summary>
    /// Introduces the local string constant.
    /// </summary>
    /// <param name="classDeclaration">
    /// The class declaration.
    /// </param>
    /// <param name="isPublic">
    /// if set to <c>true</c> [is public].
    /// </param>
    private void IntroduceStringConstant(IClassDeclaration classDeclaration, bool isPublic)
    {
      var element = classDeclaration.DeclaredElement;
      if (element == null)
      {
        return;
      }

      var files = element.GetProjectFiles();
      if (files == null || files.Count == 0)
      {
        return;
      }

      var projectFile = files[0];
      if (projectFile == null)
      {
        return;
      }

      var editorManager = EditorManager.GetInstance(this.Solution);

      var textControl2 = editorManager.GetTextControl(projectFile);
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
        this.DoTransaction(classDeclaration, isPublic);
      }
    }

    /// <summary>
    /// Handles the Item Clicked event of the menu control.
    /// </summary>
    /// <param name="className">
    /// Name of the class.
    /// </param>
    private void MenuItemClicked(string className)
    {
      IClassDeclaration classDeclaration = null;
      bool isPublic;

      if (className.StartsWith("<Local>"))
      {
        isPublic = false;

        Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
          "Introduce String Constant",
          delegate
          {
            using (ReadLockCookie.Create())
            {
              classDeclaration = this.GetClassDeclaration();
            }
          });

        if (classDeclaration == null)
        {
          return;
        }
      }
      else
      {
        isPublic = true;

        Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
          "Introduce String Constant",
          delegate
          {
            using (ReadLockCookie.Create())
            {
              classDeclaration = GetTypeElement(this.Solution, className);
            }
          });

        if (classDeclaration == null)
        {
          System.Windows.Forms.MessageBox.Show("Class name \"" + className + "\" not found in this solution.");
          return;
        }
      }

      this.IntroduceStringConstant(classDeclaration, isPublic);
    }

    #endregion
  }
}