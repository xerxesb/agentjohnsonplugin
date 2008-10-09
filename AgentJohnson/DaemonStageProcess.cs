// <copyright file="DaemonStageProcess.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson
{
  using System.Collections.Generic;
  using Exceptions;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using Strings;
  using ValueAnalysis;

  /// <summary>
  /// Defines the daemon stage process class.
  /// </summary>
  public class DaemonStageProcess : ElementVisitor, IDaemonStageProcess, IRecursiveElementProcessor
  {
    #region Fields

    private readonly List<HighlightingInfo> highlightings = new List<HighlightingInfo>();
    private readonly IDaemonProcess process;

    private readonly IStatementAnalyzer[] _statementAnalyzers;
    private readonly ITokenTypeAnalyzer[] _tokenTypeAnalyzers;
    private readonly ITypeMemberDeclarationAnalyzer[] _typeMemberDeclarationAnalyzers;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonStageProcess"/> class.
    /// </summary>
    /// <param name="daemonProcess">The daemon process.</param>
    public DaemonStageProcess(IDaemonProcess daemonProcess)
    {
      this.process = daemonProcess;

      this._statementAnalyzers = new IStatementAnalyzer[]
      {
        new DocumentThrownExceptionAnalyzer(this.process.Solution),
        new ReturnAnalyzer(this.process.Solution)
      };

      this._tokenTypeAnalyzers = new ITokenTypeAnalyzer[]
      {
        new StringEmptyAnalyzer(this.process.Solution)
      };

      this._typeMemberDeclarationAnalyzers = new ITypeMemberDeclarationAnalyzer[]
      {
        new ValueAnalysisAnalyzer(this.process.Solution)
      };
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the process and returns resulting highlightings and embedded objects to be inserted into the editor.
    /// The process should check for <see cref="P:JetBrains.ReSharper.Daemon.IDaemonProcess.InterruptFlag"/> periodically (with intervals less than 100 ms)
    /// and throw <see cref="T:JetBrains.Application.Progress.ProcessCancelledException"/> if it is true.
    /// Failing to do so may cause the program to prevent user from typing while analysing the code.
    /// </summary>
    /// <returns>
    /// New highlightings and embedded objects. Return <c>null</c> if this stage doesn't produce
    /// any of them.
    /// </returns>
    public DaemonStageProcessResult Execute()
    {
      ICSharpFile file = (ICSharpFile)PsiManager.GetInstance(this.process.Solution).GetPsiFile(this.process.ProjectFile);
      if (file == null)
      {
        return null;
      }

      if (file.Language.Name != "CSHARP")
      {
        return null;
      }

      this.ProcessFile(file);

      var result = new DaemonStageProcessResult();
      result.FullyRehighlighted = true;
      result.Highlightings = this.highlightings.ToArray();

      return result;
    }

    /// <summary>
    /// Interiors the should be processed.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>Returns the boolean.</returns>
    public bool InteriorShouldBeProcessed(IElement element)
    {
      return true;
    }

    /// <summary>
    /// Processes the after interior.
    /// </summary>
    /// <param name="element">The element.</param>
    public void ProcessAfterInterior(IElement element)
    {
    }

    /// <summary>
    /// Processes the before interior.
    /// </summary>
    /// <param name="element">The element.</param>
    public void ProcessBeforeInterior(IElement element)
    {
      this.ProcessStatements(element);

      this.ProcessTypeMemberDeclarations(element);

      this.ProcessTokenTypes(element);
    }

    /// <summary>
    /// Processes the file.
    /// </summary>
    /// <param name="file">The file process.</param>
    public void ProcessFile(ICSharpFile file)
    {
      file.ProcessDescendants(this);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the highlighting.
    /// </summary>
    /// <param name="highlighting">The highlighting.</param>
    private void AddHighlighting(SuggestionBase highlighting)
    {
      DocumentRange range = highlighting.Range;
      if (!range.IsValid)
      {
        return;
      }

      this.highlightings.Add(new HighlightingInfo(range, highlighting));
    }

    /// <summary>
    /// Processes the statements.
    /// </summary>
    /// <param name="element">The element.</param>
    private void ProcessStatements(IElement element)
    {
      var statement = element as IStatement;
      if (statement == null)
      {
        return;
      }

      foreach (IStatementAnalyzer analyzer in this._statementAnalyzers)
      {
        SuggestionBase[] result = analyzer.Analyze(statement);
        if (result == null)
        {
          continue;
        }

        foreach (SuggestionBase highlighting in result)
        {
          this.AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the token types.
    /// </summary>
    /// <param name="element">The element.</param>
    private void ProcessTokenTypes(IElement element)
    {
      var tokenType = element as ITokenNode;
      if (tokenType == null)
      {
        return;
      }

      foreach (ITokenTypeAnalyzer analyzer in this._tokenTypeAnalyzers)
      {
        SuggestionBase[] result = analyzer.Analyze(tokenType);
        if (result == null)
        {
          continue;
        }

        foreach (SuggestionBase highlighting in result)
        {
          this.AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the function declarations.
    /// </summary>
    /// <param name="element">The element.</param>
    private void ProcessTypeMemberDeclarations(IElement element)
    {
      var typeMemberDeclaration = element as ITypeMemberDeclaration;
      if (typeMemberDeclaration == null)
      {
        return;
      }

      foreach (ITypeMemberDeclarationAnalyzer analyzer in this._typeMemberDeclarationAnalyzers)
      {
        SuggestionBase[] result = analyzer.Analyze(typeMemberDeclaration);
        if (result == null)
        {
          continue;
        }

        foreach (SuggestionBase highlighting in result)
        {
          this.AddHighlighting(highlighting);
        }
      }
    }

    #endregion

    #region IRecursiveElementProcessor Members

    /// <summary>
    /// Gets a value indicating whether [processing is finished].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [processing is finished]; otherwise, <c>false</c>.
    /// </value>
    public bool ProcessingIsFinished
    {
      get
      {
        if (this.process.InterruptFlag)
        {
          throw new ProcessCancelledException();
        }

        return false;
      }
    }

    #endregion
  }
}