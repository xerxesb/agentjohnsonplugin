namespace AgentJohnson.SmartGenerate
{
  using System.Xml;
  using JetBrains.Application;
  using JetBrains.ComponentModel;
  using JetBrains.Util;

  /// <summary>
  /// Defines the smart generate settings class.
  /// </summary>
  [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentImplementation]
  public class SmartGenerateSettings : IXmlExternalizableShellComponent
  {
    #region Fields

    private string _disabledActions;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SmartGenerateSettings Instance
    {
      get
      {
        return Shell.Instance.GetComponent<SmartGenerateSettings>();
      }
    }

    /// <summary>
    /// Gets or sets the allow null attribute.
    /// </summary>
    /// <value>The allow null attribute.</value>
    [XmlExternalizable("")]
    public string DisabledActions
    {
      get
      {
        return this._disabledActions ?? string.Empty;
      }
      set
      {
        this._disabledActions = value;
      }
    }

    #endregion

    #region IShellComponent implementation

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init()
    {
    }

    #endregion

    #region IXmlExternalizableShellComponent implementation

    #region Public methods

    /// <summary>
    /// This method must not fail with null or unexpected Xml!!!
    /// </summary>
    /// <param name="element"></param>
    public void ReadFromXml(XmlElement element)
    {
      if (element == null)
      {
        return;
      }

      XmlExternalizationUtil.ReadFromXml(element, this);
    }

    /// <summary>
    /// Writes to XML.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public void WriteToXml(XmlElement element)
    {
      XmlExternalizationUtil.WriteToXml(element, this);
    }

    #endregion

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName
    {
      get
      {
        return "AgentJohnson.SmartGenerate";
      }
    }

    /// <summary>
    /// Scope that defines which store the data goes into.
    /// Must not be
    /// <c>0</c>.
    /// </summary>
    /// <value></value>
    public XmlExternalizationScope Scope
    {
      get
      {
        return XmlExternalizationScope.UserSettings;
      }
    }

    #endregion
  }
}