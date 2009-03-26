namespace AgentJohnson.ValueAnalysis
{
  using EnvDTE;
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.VSIntegration.Application;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Annotates a function with Value Analysis attributes and assert statements.", Name = "Value Analysis Annotations", Priority = 0, Group = "C#")]
  public class ValueAnalysisContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public ValueAnalysisContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void Execute(IElement element)
    {
      ITypeMemberDeclaration typeMemberDeclaration = this.GetTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return;
      }

      ValueAnalysisRefactoring valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, this.Provider);

      valueAnalysisRefactoring.Execute();
    }

    /// <summary>
    /// Executes the internal post PSI transaction.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The internal post PSI transaction.</returns>
    protected override object[] ExecuteInternalPostPSITransaction(params object[] data)
    {
      if (!ValueAnalysisSettings.Instance.ExecuteGhostDoc)
      {
        return data;
      }

      _DTE dte = VSShell.Instance.ApplicationObject;
      Command command;

      try
      {
        command = dte.Commands.Item("Weigelt.GhostDoc.AddIn.DocumentThis", -1);
      }
      catch
      {
        command = null;
      }

      if (command != null)
      {
        dte.ExecuteCommand("Weigelt.GhostDoc.AddIn.DocumentThis", string.Empty);
      }

      return data;
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    protected override string GetText()
    {
      return "Annotate with Value Analysis attributes";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      ITypeMemberDeclaration typeMemberDeclaration = this.GetTypeMemberDeclaration();
      if (typeMemberDeclaration == null)
      {
        return false;
      }

      ValueAnalysisRefactoring valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, this.Provider);

      return valueAnalysisRefactoring.IsAvailable();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the type member declaration.
    /// </summary>
    /// <returns>The type member declaration.</returns>
    [CanBeNull]
    private ITypeMemberDeclaration GetTypeMemberDeclaration()
    {
      IElement element = this.Provider.SelectedElement;
      if (element == null)
      {
        return null;
      }

      ITypeMemberDeclaration typeMemberDeclaration = null;

      ITreeNode treeNode = element as ITreeNode;
      if (treeNode != null)
      {
        typeMemberDeclaration = treeNode.Parent as ITypeMemberDeclaration;
      }

      if (typeMemberDeclaration == null)
      {
        IIdentifierNode identifierNode = element as IIdentifierNode;

        if (identifierNode != null)
        {
          typeMemberDeclaration = identifierNode.Parent as ITypeMemberDeclaration;
        }
      }

      return typeMemberDeclaration;
    }

    #endregion
  }
}