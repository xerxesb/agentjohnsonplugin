// <copyright file="DaemonStageProcess.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson
{
  using System;
  using System.Collections.Generic;
  using Exceptions;
  using JetBrains.Annotations;
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

    private readonly DocumentThrownExceptionAnalyzer documentThrownExceptionAnalyzer;
    private readonly ReturnAnalyzer returnAnalyzer;
    private readonly StringEmptyAnalyzer stringEmptyAnalyzer;
    private readonly ValueAnalysisAnalyzer valueAnalysisAnalyzer;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonStageProcess"/> class.
    /// </summary>
    /// <param name="process">The process.</param>
    public DaemonStageProcess(IDaemonProcess process) : base(process)
    {
      this.stringEmptyAnalyzer = new StringEmptyAnalyzer(this.DaemonProcess.Solution);
      this.returnAnalyzer = new ReturnAnalyzer(this.DaemonProcess.Solution);
      this.documentThrownExceptionAnalyzer = new DocumentThrownExceptionAnalyzer(this.DaemonProcess.Solution);
      this.valueAnalysisAnalyzer = new ValueAnalysisAnalyzer(this.DaemonProcess.Solution);
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
      this.HighlightInFile((file, consumer) => file.ProcessDescendants(this, consumer), commiter);
    }

    /// <summary>
    /// Visits the constructor declaration.
    /// </summary>
    /// <param name="constructorDeclaration">The constructor declaration.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitConstructorDeclaration(IConstructorDeclaration constructorDeclaration, IHighlightingConsumer consumer)
    {
      this.VisitTypeMember(constructorDeclaration, consumer);
      return base.VisitConstructorDeclaration(constructorDeclaration, consumer);
    }

    /// <summary>
    /// Visits the element.
    /// </summary>
    /// <param name="element">The param.</param>
    /// <param name="consumer">The context.</param>
    /// <returns></returns>
    public override object VisitElement(IElement element, IHighlightingConsumer consumer)
    {
      var tokenType = element as ITokenNode;
      if (tokenType != null)
      {
        AddHighlighting(consumer, this.stringEmptyAnalyzer.Analyze(tokenType));
      }

      return base.VisitElement(element, consumer);
    }

    /// <summary>
    /// Visits the indexer declaration.
    /// </summary>
    /// <param name="indexerDeclarationParam">The indexer declaration param.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitIndexerDeclaration(IIndexerDeclaration indexerDeclarationParam, IHighlightingConsumer consumer)
    {
      this.VisitTypeMember(indexerDeclarationParam, consumer);
      return base.VisitIndexerDeclaration(indexerDeclarationParam, consumer);
    }

    /// <summary>
    /// Visits the method declaration.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitMethodDeclaration(IMethodDeclaration methodDeclaration, IHighlightingConsumer consumer)
    {
      this.VisitTypeMember(methodDeclaration, consumer);
      return base.VisitMethodDeclaration(methodDeclaration, consumer);
    }

    /// <summary>
    /// Visits the property declaration.
    /// </summary>
    /// <param name="propertyDeclaration">The property declaration.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitPropertyDeclaration(IPropertyDeclaration propertyDeclaration, IHighlightingConsumer consumer)
    {
      this.VisitTypeMember(propertyDeclaration, consumer);
      return base.VisitPropertyDeclaration(propertyDeclaration, consumer);
    }

    /// <summary>
    /// Visits the return statement.
    /// </summary>
    /// <param name="returnStatement">The return statement.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitReturnStatement(IReturnStatement returnStatement, IHighlightingConsumer consumer)
    {
      AddHighlighting(consumer, this.returnAnalyzer.AnalyzeReturnStatement(returnStatement));

      return base.VisitReturnStatement(returnStatement, consumer);
    }

    /// <summary>
    /// Visits the throw statement.
    /// </summary>
    /// <param name="throwStatement">The throw statement.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    public override object VisitThrowStatement(IThrowStatement throwStatement, IHighlightingConsumer consumer)
    {
      AddHighlighting(consumer, this.documentThrownExceptionAnalyzer.AnalyzeThrowStatement(throwStatement));

      return base.VisitThrowStatement(throwStatement, consumer);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the highlighting.
    /// </summary>
    /// <param name="consumer">The consumer.</param>
    /// <param name="suggestions">The suggestions.</param>
    private static void AddHighlighting(IHighlightingConsumer consumer, IEnumerable<SuggestionBase> suggestions)
    {
      if (suggestions == null)
      {
        return;
      }

      foreach (var highlighting in suggestions)
      {
        AddHighlighting(consumer, highlighting);
      }
    }

    /// <summary>
    /// Adds the highlighting.
    /// </summary>
    ///<param name="consumer">The consumer.</param>
    ///<param name="highlighting">The highlighting.</param>
    private static void AddHighlighting(IHighlightingConsumer consumer, SuggestionBase highlighting)
    {
      var range = highlighting.Range;
      if (!range.IsValid())
      {
        return;
      }

      consumer.AddHighlighting(range, highlighting);
    }

    /// <summary>
    /// Visits the type member.
    /// </summary>
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    /// <param name="consumer">The consumer.</param>
    /// <returns></returns>
    private void VisitTypeMember(ITypeMemberDeclaration typeMemberDeclaration, IHighlightingConsumer consumer)
    {
      AddHighlighting(consumer, this.valueAnalysisAnalyzer.Analyze(typeMemberDeclaration));
    }

    #endregion
  }
}