using System;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace AgentJohnson {
  /// <summary>
  /// Agent Johnson stage.
  /// </summary>
  [DaemonStage(StagesBefore = new Type[]{typeof(GlobalErrorStage)},
    StagesAfter = new Type[]{typeof(LanguageSpecificDaemonStage)}, RunForInvisibleDocument = true)]
  public class DaemonStage : CSharpDaemonStageBase {
    #region Public methods

    /// <summary>
    /// Creates the process.
    /// </summary>
    /// <param name="process">The process.</param>
    /// <returns></returns>
    public override IDaemonStageProcess CreateProcess(IDaemonProcess process) {
      if(!IsSupported(process.ProjectFile)){
        return null;
      }
      return new DaemonStageProcess(process);
    }

    /// <summary>
    /// Needs the error stripe.
    /// </summary>
    /// <param name="projectFile">The project file.</param>
    /// <returns></returns>
    public override ErrorStripeRequest NeedsErrorStripe(IProjectFile projectFile) {
      return ErrorStripeRequest.STRIPE_AND_ERRORS;
    }

    #endregion
  }
}