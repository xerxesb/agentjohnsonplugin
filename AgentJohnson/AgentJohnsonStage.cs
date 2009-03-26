namespace AgentJohnson
{
  using System;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Daemon.CSharp.Stages;
  using JetBrains.ReSharper.Daemon.CSharp.Stages.UsageChecking;

  /// <summary>
  /// Agent Johnson stage.
  /// </summary>
  [DaemonStage(StagesBefore = new[] { typeof(UsageCheckingStage) }, StagesAfter = new[] { typeof(LanguageSpecificDaemonStage) })]
  public class AgentJohnsonStage : CSharpDaemonStageBase
  {
    #region Public methods

    /// <summary>
    /// Creates the process.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <param name="processKind">Kind of the process.</param>
    /// <returns>Returns the IDaemon stage process.</returns>
    public override IDaemonStageProcess CreateProcess(IDaemonProcess process, DaemonProcessKind processKind)
    {
      if (!IsSupported(process.ProjectFile))
      {
        return null;
      }

      return new DaemonStageProcess(process);
    }

    /// <summary>
    /// Needs the error stripe.
    /// </summary>
    /// <param name="projectFile">The project file.</param>
    /// <returns></returns>
    public override ErrorStripeRequest NeedsErrorStripe(IProjectFile projectFile)
    {
      if (!IsSupported(projectFile))
      {
        return ErrorStripeRequest.NONE;
      }

      return ErrorStripeRequest.STRIPE_AND_ERRORS;
    }

    #endregion
  }
}