using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.TextControl;

namespace AgentJohnson.Generate {
  /// <summary>
  /// This class is <see cref="IGenerator"/> with some page for field selection
  /// </summary>
  public class PropertyStubGenerator : StubGeneratorBase {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStubGenerator"/> class.
    /// </summary>
    /// <param name="textControl">The text control.</param>
    /// <param name="solution">The solution.</param>
    public PropertyStubGenerator(ITextControl textControl, ISolution solution): base(textControl, solution) {
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title {
      get {
        return "Generate property stub";
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <returns>The name.</returns>
    protected override string GetTemplateShortcut() {
      return "propertystub";
    }

    /// <summary>
    /// Gets the template XML.
    /// </summary>
    /// <returns>The template XML.</returns>
    protected override string GetTemplateXml() {
      return "<Template uid=\"a684b217-f179-431b-a485-e3d76dbe57fd\" text=\"public $type$ $name$ {&#xD;&#xA;  get {&#xD;&#xA;    $END$  }&#xD;&#xA;  set {&#xD;&#xA;  }&#xD;&#xA;}&#xD;&#xA;\" shortcut=\"propertystub\" description=\"propertystub\" reformat=\"true\" shortenQualifiedReferences=\"true\">" +
             "  <Context>" +
             "    <CSharpContext context=\"TypeMember\" minimumLanguageVersion=\"2.0\" />" +
             "  </Context>" +
             "  <Categories />" +
             "  <Variables>" +
             "    <Variable name=\"type\" expression=\"suggestVariableType()\" initialRange=\"0\" />" +
             "    <Variable name=\"name\" expression=\"\" initialRange=\"0\" />" +
             "  </Variables>" +
             "  <CustomProperties />" +
             "</Template>";
    }

    #endregion
  }
}