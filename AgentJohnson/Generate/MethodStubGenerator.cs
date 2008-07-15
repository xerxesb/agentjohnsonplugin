using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.TextControl;

namespace AgentJohnson.Generate {
  /// <summary>
  /// This class is <see cref="IGenerator"/> with some page for field selection
  /// </summary>
  public class MethodStubGenerator : StubGeneratorBase {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodStubGenerator"/> class.
    /// </summary>
    /// <param name="textControl">The text control.</param>
    /// <param name="solution">The solution.</param>
    public MethodStubGenerator(ITextControl textControl, ISolution solution): base(textControl, solution) {
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title {
      get {
        return "Generate method stub";
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <returns>The name.</returns>
    protected override string GetTemplateShortcut() {
      return "methodstub";
    }

    /// <summary>
    /// Gets the template XML.
    /// </summary>
    /// <returns>The template XML.</returns>
    protected override string GetTemplateXml() {
      return "<Template text=\"public void $name$() {&#xD;&#xA;  $END$&#xD;&#xA;}&#xD;&#xA;\" shortcut=\"methodstub\" description=\"Generate\" reformat=\"true\" shortenQualifiedReferences=\"true\">" +
             "  <Context>" +
             "    <CSharpContext context=\"TypeMember\" minimumLanguageVersion=\"2.0\" />" +
             "  </Context>" +
             "  <Categories />" +
             "  <Variables>" +
             "    <Variable name=\"name\" expression=\"\" initialRange=\"0\" />" +
             "  </Variables>" +
             "  <CustomProperties />" +
             "</Template>";
    }

    #endregion
  }
}