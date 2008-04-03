using System.Collections.Generic;
using AgentJohnson.Exceptions;
using AgentJohnson.Strings;
using AgentJohnson.ValueAnalysis;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Shell.Progress;

namespace AgentJohnson {
  /// <summary>
  /// </summary>
  public class DaemonStageProcess : ElementVisitor, IDaemonStageProcess, IRecursiveElementProcessor {
    #region Fields

    readonly IDeclarationAnalyzer[] _declarationAnalyzers;
    readonly IFunctionDeclarationAnalyzer[] _functionDeclarationAnalyzers;
    readonly List<HighlightingInfo> _highlightings = new List<HighlightingInfo>();
    readonly IInvocationExpressionAnalyzer[] _invocationExpressionAnalyzers;
    readonly IDaemonProcess _process;
    readonly IStatementAnalyzer[] _statementAnalyzers;
    readonly ITokenTypeAnalyzer[] _tokenTypeAnalyzers;
    readonly ITypeMemberDeclarationAnalyzer[] _typeMemberDeclarationAnalyzers;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonStageProcess"/> class.
    /// </summary>
    /// <param name="daemonProcess">The daemon process.</param>
    public DaemonStageProcess(IDaemonProcess daemonProcess) {
      _process = daemonProcess;

      _declarationAnalyzers = new IDeclarationAnalyzer[] {};

      _statementAnalyzers = new IStatementAnalyzer[]
                              {
                                new DocumentThrownExceptionAnalyzer(_process.Solution)
                              };

      _functionDeclarationAnalyzers = new IFunctionDeclarationAnalyzer[]
                                        {
                                        };

      _tokenTypeAnalyzers = new ITokenTypeAnalyzer[]
                              {
                                new StringEmptyAnalyzer(_process.Solution)
                              };

      _typeMemberDeclarationAnalyzers = new ITypeMemberDeclarationAnalyzer[]
                                          {
                                            new ValueAnalysisAnalyzer(_process.Solution)
                                          };

      _invocationExpressionAnalyzers = new IInvocationExpressionAnalyzer[]
                                         {
                                         };
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the process and returns resulting highlightings and embedded objects to be inserted into the editor.
    /// The process should check for <see cref="P:JetBrains.ReSharper.Daemon.IDaemonProcess.InterruptFlag"/> periodically (with intervals less than 100 ms)
    /// and throw <see cref="T:JetBrains.Shell.Progress.ProcessCancelledException"/> if it is true.
    /// Failing to do so may cause the program to prevent user from typing while analyzing the code.
    /// </summary>
    /// <returns>
    /// New highlightings and embedded objects. Return <c>null</c> if this stage doesn't produce
    /// any of them.
    /// </returns>
    public DaemonStageProcessResult Execute() {
      var file = (ICSharpFile)PsiManager.GetInstance(_process.Solution).GetPsiFile(_process.ProjectFile);
      ProcessFile(file);

      var result = new DaemonStageProcessResult();
      result.FullyRehighlighted = true;
      result.Highlightings = _highlightings.ToArray();

      return result;
    }

    /// <summary>
    /// Interiors the should be processed.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public bool InteriorShouldBeProcessed(IElement element) {
      return true;
    }

    /// <summary>
    /// Processes the after interior.
    /// </summary>
    /// <param name="element">The element.</param>
    public void ProcessAfterInterior(IElement element) {
    }

    /// <summary>
    /// Processes the before interior.
    /// </summary>
    /// <param name="element">The element.</param>
    public void ProcessBeforeInterior(IElement element) {
      ProcessDeclarations(element);

      ProcessStatements(element);

      ProcessFunctionDeclarations(element);

      ProcessTypeMemberDeclarations(element);

      ProcessTokenTypes(element);

      ProcessInvocationExpressions(element);
    }

    /// <summary>
    /// Processes the file.
    /// </summary>
    /// <param name="file">The file.</param>
    public void ProcessFile(ICSharpFile file) {
      file.ProcessDescendants(this);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the highlighting.
    /// </summary>
    /// <param name="highlighting">The highlighting.</param>
    void AddHighlighting(SuggestionBase highlighting) {
      _highlightings.Add(new HighlightingInfo(highlighting.Range, highlighting));
    }

    /// <summary>
    /// Processes the declarations.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessDeclarations(IElement element) {
      var declaration = element as IDeclaration;
      if(declaration == null) {
        return;
      }

      foreach(IDeclarationAnalyzer analyzer in _declarationAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(declaration);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the function declarations.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessFunctionDeclarations(IElement element) {
      var functionDeclaration = element as IFunctionDeclaration;
      if(functionDeclaration == null) {
        return;
      }

      foreach(IFunctionDeclarationAnalyzer analyzer in _functionDeclarationAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(functionDeclaration);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the invocation expressions.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessInvocationExpressions(IElement element) {
      var invocationExpression = element as IInvocationExpression;
      if(invocationExpression == null) {
        return;
      }

      foreach(IInvocationExpressionAnalyzer analyzer in _invocationExpressionAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(invocationExpression);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the statements.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessStatements(IElement element) {
      var statement = element as IStatement;
      if(statement == null) {
        return;
      }

      foreach(IStatementAnalyzer analyzer in _statementAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(statement);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the token types.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessTokenTypes(IElement element) {
      var tokenType = element as ITokenNode;
      if(tokenType == null) {
        return;
      }

      foreach(ITokenTypeAnalyzer analyzer in _tokenTypeAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(tokenType);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
        }
      }
    }

    /// <summary>
    /// Processes the function declarations.
    /// </summary>
    /// <param name="element">The element.</param>
    void ProcessTypeMemberDeclarations(IElement element) {
      var typeMemberDeclaration = element as ITypeMemberDeclaration;
      if(typeMemberDeclaration == null) {
        return;
      }

      foreach(ITypeMemberDeclarationAnalyzer analyzer in _typeMemberDeclarationAnalyzers) {
        SuggestionBase[] result = analyzer.Analyze(typeMemberDeclaration);
        if(result == null) {
          continue;
        }

        foreach(SuggestionBase highlighting in result) {
          AddHighlighting(highlighting);
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
    public bool ProcessingIsFinished {
      get {
        if(_process.InterruptFlag) {
          throw new ProcessCancelledException();
        }
        return false;
      }
    }

    #endregion
  }
}