using System.Xml;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentImplementation]
  public class SmartGenerateSettings : IXmlExternalizableShellComponent {
    #region Fields

    string _disabledActions;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the allow null attribute.
    /// </summary>
    /// <value>The allow null attribute.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = "")]
    public string DisabledActions {
      get {
        return _disabledActions ?? string.Empty;
      }
      set {
        _disabledActions = value;
      }
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SmartGenerateSettings Instance {
      get {
        return (SmartGenerateSettings)Shell.Instance.GetComponent(typeof(SmartGenerateSettings));
      }
    }

    #endregion

    #region IShellComponent implementation

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init() {
    }

    #endregion

    #region IXmlExternalizableShellComponent implementation

    #region Public methods

    /// <summary>
    /// This method must not fail with null or unexpected Xml!!!
    /// </summary>
    /// <param name="element"></param>
    public void ReadFromXml(XmlElement element) {
      if(element == null) {
        return;
      }

      XmlExternalizationUtil.ReadFromXml(element, this);
    }

    /// <summary>
    /// Writes to XML.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public bool WriteToXml(XmlElement element) {
      return XmlExternalizationUtil.WriteToXml(element, this);
    }

    #endregion

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName {
      get {
        return "AgentJohnson.SmartGenerate";
      }
    }

    /// <summary>
    /// Scope that defines which store the data goes into.
    /// Must not be
    /// <c>0</c>.
    /// </summary>
    /// <value></value>
    public XmlExternalizationScope Scope {
      get {
        return XmlExternalizationScope.UserSettings;
      }
    }

    #endregion

  }
}