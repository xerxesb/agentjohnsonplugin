using System.Drawing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.UI.RichText;

namespace AgentJohnson.Generate {
  /// <summary>
  /// Generation item to be registered by <c>ReSharper</c>
  /// </summary>
  [GenerateItem(Priority = 100)]
  public class GeneratePropertyStub : TextControlGenerateItem {
    #region Public methods

    /// <summary>
    /// Gets the generator.
    /// </summary>
    /// <param name="textControl">The text control.</param>
    /// <param name="solution">The solution.</param>
    /// <returns></returns>
    public override WizardBasedGenerator GetGenerator(ITextControl textControl, ISolution solution) {
      return new PropertyStubGenerator(textControl, solution);
    }

    #endregion

    #region IGenerateItem Members

    /// <summary>
    /// Gets the image.
    /// </summary>
    /// <value>The image.</value>
    public override Image Image {
      get {
        return PsiIconManager.Instance.GetImage(CLRDeclaredElementType.PROPERTY);
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public override RichText Text {
      get {
        return "Property stub";
      }
    }

    #endregion
  }
}