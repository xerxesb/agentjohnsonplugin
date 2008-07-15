using EnvDTE;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.VSIntegration.Shell;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates a function with Value Analysis attributes and assert statements.", Name = "Value Analysis Annotations", Priority = 0, Group = "C#")]
  public class ValueAnalysisContextAction : OneItemContextActionBase {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public ValueAnalysisContextAction(ISolution solution, ITextControl textControl) : base(solution, textControl) {
    }

    #endregion



    #region Public methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public override string Text {
      get {
        return "Annotate with Value Analysis attributes";
      }
    }

    /// <summary>
    /// Executes the internal post PSI transaction.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The internal post PSI transaction.</returns>
    protected override object[] ExecuteInternalPostPSITransaction(params object[] data) {
      if (!ValueAnalysisSettings.Instance.ExecuteGhostDoc) {
        return data;
      }

      _DTE dte = VSShell.Instance.ApplicationObject;
      Command command;

      try {
        command = dte.Commands.Item("Weigelt.GhostDoc.AddIn.DocumentThis", -1);
      }
      catch {
        command = null;
      }

      if(command != null) {
        dte.ExecuteCommand("Weigelt.GhostDoc.AddIn.DocumentThis", string.Empty);
      }

      return data;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void ExecuteInternal() {
      ITypeMemberDeclaration typeMemberDeclaration = GetTypeMemberDeclaration();
      if(typeMemberDeclaration == null) {
        return;
      }

      ValueAnalysisRefactoring valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, Provider);

      valueAnalysisRefactoring.Execute();
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if PsiManager, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailableInternal() {
      ITypeMemberDeclaration typeMemberDeclaration = GetTypeMemberDeclaration();
      if(typeMemberDeclaration == null){
        return false;
      }

      ValueAnalysisRefactoring valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, Provider);

      return valueAnalysisRefactoring.IsAvailable();
    }

    /// <summary>
    /// Gets the type member declaration.
    /// </summary>
    /// <returns>The type member declaration.</returns>
    [CanBeNull]
    ITypeMemberDeclaration GetTypeMemberDeclaration() {
      ITypeMemberDeclaration typeMemberDeclaration;

      IElement element = Provider.SelectedElement;
      if(element == null){
        return null;
      }

      typeMemberDeclaration = null;

      ITreeNode treeNode  = element as ITreeNode;
      if(treeNode != null){
        typeMemberDeclaration = treeNode.Parent as ITypeMemberDeclaration;
      }

      if(typeMemberDeclaration == null){
        IIdentifierNode identifierNode = element as IIdentifierNode;

        if(identifierNode != null){
          typeMemberDeclaration = identifierNode.Parent as ITypeMemberDeclaration;
        }
      }

      return typeMemberDeclaration;
    }

    #endregion  
  }
}
  