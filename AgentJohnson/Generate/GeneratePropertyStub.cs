using System.Drawing;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;

namespace AgentJohnson.Generate {
  /// <summary>
  /// Generation item to be registered by <c>ReSharper</c>
  /// </summary>
  [GenerateItem(Priority = 100)]
  public class GeneratePropertyStub : IGenerateItem {
    #region Public methods

    /// <summary>
    /// Gets the generator.
    /// </summary>
    /// <param name="dataContext">The data context.</param>
    /// <returns></returns>
    public IGenerator GetGenerator(IDataContext dataContext) {
      return new PropertyStubGenerator(dataContext);
    }

    #endregion

    #region IGenerateItem Members

    /// <summary>
    /// Gets the image.
    /// </summary>
    /// <value>The image.</value>
    public Image Image {
      get {
        return PsiIconManager.Instance.GetImage(CLRDeclaredElementType.PROPERTY);
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public RichText Text {
      get {
        return "Property stub";
      }
    }

    #endregion
  }
}