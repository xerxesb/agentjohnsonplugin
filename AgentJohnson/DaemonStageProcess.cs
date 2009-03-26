// <copyright file="DaemonStageProcess.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson
{
  using System;
  using Exceptions;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Daemon.CSharp.Stages;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using Strings;
  using ValueAnalysis;

  /// <summary>
  /// Defines the daemon stage process class.
  /// </summary>
  public class DaemonStageProcess : CSharpDaemonStageProcessBase 
  {
    #region Fields

    private readonly IStatementAnalyzer[] statementAnalyzers;
    private readonly ITokenTypeAnalyzer[] tokenTypeAnalyzers;
    private readonly ITypeMemberDeclarationAnalyzer[] typeMemberDeclarationAnalyzers;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonStageProcess"/> class.
    /// </summary>
    /// <param name="process">The process.</param>
    public DaemonStageProcess(IDaemonProcess process) : base(process)
    {
      this.statementAnalyzers = new IStatementAnalyzer[]
      {
        new DocumentThrownExceptionAnalyzer(this.DaemonProcess.Solution),
        new ReturnAnalyzer(this.DaemonProcess.Solution)
      };

      this.tokenTypeAnalyzers = new ITokenTypeAnalyzer[]
      {
        new StringEmptyAnalyzer(this.DaemonProcess.Solution)
      };

      this.typeMemberDeclarationAnalyzers = new ITypeMemberDeclarationAnalyzer[]
      {
        new ValueAnalysisAnalyzer(this.DaemonProcess.Solution)
      };
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the process.
    /// The process should check for <see cref="P:JetBrains.ReSharper.Daemon.IDaemonProcess.InterruptFlag"/> periodically (with intervals less than 100 ms)
    /// and throw <see cref="T:JetBrains.Application.Progress.ProcessCancelledException"/> if it is true.
    /// Failing to do so may cause the program to prevent user from typing while analysing the code.
    /// Stage results should be passed to <param name="commiter"/>. If DaemonStageResult is <c>null</c>, it means that no highlightings available
    /// </summary>
    /// <param name="commiter"></param>
    public override void Execute(Action<DaemonStageResult> commiter)
    {
      HighlightInFile(file => file.ProcessDescendants(this), commiter);
    }

    /// <summary>
    /// Processes the before interior.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void ProcessBeforeInterior(IElement element)
    {
      this.ProcessStatements(element);

      this.ProcessTypeMemberDeclarations(element);

      this.ProcessTokenTypes(element);
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
      if (!range.IsValid())
      {
        return;
      }

      this.AddHighlighting(range, highlighting);
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

      foreach (IStatementAnalyzer analyzer in this.statementAnalyzers)
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

      foreach (ITokenTypeAnalyzer analyzer in this.tokenTypeAnalyzers)
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

      foreach (ITypeMemberDeclarationAnalyzer analyzer in this.typeMemberDeclarationAnalyzers)
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
  }
}