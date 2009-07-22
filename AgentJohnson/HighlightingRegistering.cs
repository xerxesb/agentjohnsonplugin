// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightingRegistering.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Registers Agent Smith highlighters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using Exceptions;
  using Strings;
  using ValueAnalysis;
  using JetBrains.Application;
  using JetBrains.ComponentModel;
  using JetBrains.ReSharper.Daemon;

  /// <summary>
  /// Registers Agent Smith highlighters.
  /// </summary>
  [ShellComponentImplementation(ProgramConfigurations.ALL)]
  public class HighlightingRegistering : IShellComponent
  {
    #region Implemented Interfaces

    #region IComponent

    /// <summary>
    /// Inits this instance.
    /// </summary>
    public void Init()
    {
      const string group = "Agent Johnson";

      var manager = HighlightingSettingsManager.Instance;

      manager.RegisterConfigurableSeverity(DocumentThrownExceptionWarning.NAME, group,
        "Undocumented thrown exception.",
        "Thrown exceptions should be documented in XML comments.",
        Severity.WARNING);
      manager.RegisterConfigurableSeverity(StringEmptySuggestion.NAME, group,
        "Replace \"\" with string.Empty.",
        "Empty string literals (\"\") should be replaced with string.Empty.",
        Severity.SUGGESTION);
      manager.RegisterConfigurableSeverity(ValueAnalysisSuggestion.NAME, group,
        "Annotate type members with Value Analysis attributes and assert statements.",
        "Type members should be annotated with Value Analysis attributes and have assert statements.",
        Severity.WARNING);
      manager.RegisterConfigurableSeverity(ReturnWarning.NAME, group,
        "Return values should be asserted.",
        "The return value must not null and should be asserted.",
        Severity.WARNING);
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    #endregion
  }
}