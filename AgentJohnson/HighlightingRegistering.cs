using AgentJohnson.Exceptions;
using AgentJohnson.Strings;
using AgentJohnson.ValueAnalysis;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.ReSharper.Daemon;

namespace AgentJohnson {
  /// <summary>
  /// Registers Agent Smith highlighters.
  /// </summary>
  [ShellComponentImplementation(ProgramConfigurations.ALL)]
  public class HighlightingRegistering : IShellComponent {
    #region Public methods

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
    }

    /// <summary>
    /// Inits this instance.
    /// </summary>
    public void Init() {
      const string group = "Agent Johnson";

      HighlightingSettingsManager manager = HighlightingSettingsManager.Instance;

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
    }

    #endregion
  }
}