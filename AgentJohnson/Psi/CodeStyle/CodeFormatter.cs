namespace AgentJohnson.Psi.CodeStyle
{
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;

  /// <summary>
  /// Defines the code formatter class.
  /// </summary>
  public class CodeFormatter
  {
    #region Public Methods

    /// <summary>
    /// Formats the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="documentRange">The document range.</param>
    public void Format(ISolution solution, DocumentRange documentRange)
    {
      var languageService = CSharpLanguageService.CSHARP.Service;
      if (languageService == null)
      {
        return;
      }

      var formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      formatter.Format(
        solution,
        documentRange,
        CodeStyleSettingsManager.Instance.CodeStyleSettings,
        CodeFormatProfile.DEFAULT,
        true,
        true,
        NullProgressIndicator.Instance);
    }

    #endregion
  }
}