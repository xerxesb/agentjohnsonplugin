using System.Collections.Generic;
using System.Xml;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CSharp.Generate.Util;
using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.Generate {
  /// <summary>
  /// 
  /// </summary>
  public abstract class StubGeneratorBase : WizardBasedGenerator {
    #region Fields

    readonly ISolution _solution;
    readonly StubGeneratorPage _stubGeneratorPage;
    readonly ITextControl _textControl;
    readonly IClassLikeDeclaration _typeDeclaration;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodStubGenerator"/> class.
    /// </summary>
    /// <param name="textControl">The text control.</param>
    /// <param name="solution">The solution.</param>
    protected StubGeneratorBase(ITextControl textControl, ISolution solution) {
      _solution = solution;
      _textControl = textControl;

      if(_solution == null || _textControl == null) {
        return;
      }

      _typeDeclaration = SharedUtil.GetClassOrStructDeclaration(_textControl, _solution);
      if(_typeDeclaration == null) {
        return;
      }

      _stubGeneratorPage = new StubGeneratorPage();
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets a value indicating whether this instance is applicable.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is applicable; otherwise, <c>false</c>.
    /// </value>
    public override bool IsApplicable {
      get {
        if(_typeDeclaration == null || !_typeDeclaration.IsValid()) {
          return false;
        }

        return _typeDeclaration.DeclaredElement != null;
      }
    }
    /// <summary>
    /// Gets the pages.
    /// </summary>
    /// <value>The pages.</value>
    public override IEnumerable<IGeneratePage> Pages {
      get {
        yield return _stubGeneratorPage;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Afters the psi change.
    /// </summary>
    public override void AfterPsiChange() {
      Template template = GetTemplate();
      if(template == null) {
        return;
      }

      ITreeNode node = GetElementAtCaret() as ITreeNode;
      while(node != null && !(node is IDeclaredElement)) {
        node = node.Parent;
      }

      IDeclaredElement declaredElement = node as IDeclaredElement;
      if(declaredElement != null) {
        ITypeMemberDeclaration member = declaredElement as ITypeMemberDeclaration;
        if(member == null) {
          member = declaredElement.GetContainingTypeMember() as ITypeMemberDeclaration;
        }
          
        if(member != null) {
          _textControl.CaretModel.MoveTo(member.ToTreeNode().GetTreeStartOffset());
        }
      }

      TemplateUtil.ExecuteTemplate(_solution, _textControl, template);
    }

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <returns>The element.</returns>
    protected IElement GetElementAtCaret() {
      IProjectFile projectFile = DocumentManager.GetInstance(_solution).GetProjectFile(_textControl.Document);
      if(projectFile == null) {
        return null;
      }

      PsiManager psiManager = PsiManager.GetInstance(_solution);
      if(psiManager == null) {
        return null;
      }

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return null;
      }

      return file.FindTokenAt(_textControl.CaretModel.Offset);
    }

    /// <summary>
    /// Called before the PSI change.
    /// </summary>
    public override void BeforePsiChange() {
    }

    /// <summary>
    /// Ensures the writable.
    /// </summary>
    /// <returns></returns>
    public override ModificationCookie EnsureWritable() {
      return _textControl.Document.EnsureWritable();
    }

    /// <summary>
    /// Called while under PSI change transaction.
    /// </summary>
    public override void UnderPsiChange() {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Gets the template shortcut.
    /// </summary>
    /// <returns>The template shortcut.</returns>
    protected abstract string GetTemplateShortcut();

    /// <summary>
    /// Gets the template XML.
    /// </summary>
    /// <returns>The template XML.</returns>
    protected abstract string GetTemplateXml();

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the template.
    /// </summary>
    /// <returns></returns>
    Template GetTemplate() {
      foreach(Template template in LiveTemplatesManager.Instance.TemplateFamily.UserStorage.Templates) {
        string shortcut = GetTemplateShortcut();
        if(template.Shortcut == shortcut) {
          return template;
        }
      }

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(GetTemplateXml());

      Template result = Template.CreateFromXml(doc.DocumentElement);

      LiveTemplatesManager.Instance.TemplateFamily.UserStorage.Templates.Add(result);

      return result;
    }

    #endregion
  }
}