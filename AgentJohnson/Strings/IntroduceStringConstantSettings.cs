using System.Collections.Generic;
using System.Text;
using System.Xml;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.Util;

namespace AgentJohnson.Strings {
  /// <summary>
  /// Represents the Favorite Files Settings.
  /// </summary>
  [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentImplementation]
  public class IntroduceStringConstantSettings : IXmlExternalizableShellComponent {
    #region Fields

    List<string> _classNames;
    bool _generateXmlComment;
    int _replaceSpacesMode;
    int _transformIdentifierMode;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    /// <value>The class names.</value>
    public List<string> ClassNames {
      get {
        if(_classNames == null) {
          InitClasses();
        }

        return _classNames;
      }
      set {
        _classNames = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [generate XML comment].
    /// </summary>
    /// <value><c>true</c> if [generate XML comment]; otherwise, <c>false</c>.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public bool GenerateXmlComment {
      get {
        return _generateXmlComment;
      }
      set {
        _generateXmlComment = value;
      }
    }
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static IntroduceStringConstantSettings Instance {
      get {
        return Shell.Instance.GetComponent<IntroduceStringConstantSettings>();
      }
    }

    /// <summary>
    /// Gets or sets the replace spaces mode.
    /// </summary>
    /// <value>The replace spaces mode.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = 0)]
    public int ReplaceSpacesMode {
      get {
        return _replaceSpacesMode;
      }
      set {
        _replaceSpacesMode = value;
      }
    }

    /// <summary>
    /// Gets or sets the class names in a serializable format.
    /// </summary>                             
    /// <remarks>This is for serialization only.</remarks>
    /// <value>The class names.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = "")]
    public string SerializableClassNames {
      get {
        StringBuilder result = new StringBuilder();

        bool first = true;

        foreach(string className in ClassNames) {
          if(!first) {
            result.Append("|");
          }

          first = false;

          result.Append(className);
        }

        return result.ToString();
      }
      set {
        _classNames = new List<string>();

        string[] classes = value.Split('|');

        foreach(string className in classes) {
          if(!string.IsNullOrEmpty(className)) {
            _classNames.Add(className);
          }
        }
      }
    }
    /// <summary>
    /// Gets or sets the transform identifier mode.
    /// </summary>
    /// <value>The transform identifier mode.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = 0)]
    public int TransformIdentifierMode {
      get {
        return _transformIdentifierMode;
      }
      set {
        _transformIdentifierMode = value;
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
    /// Inits this instance.
    /// </summary>
    public void Init() {
    }

    #endregion

    #region IXmlExternalizableShellComponent implementation

    #region Private methods

    /// <summary>
    /// This method must not fail with null or unexpected Xml!!!
    /// </summary>
    /// <param name="element"></param>
    void IXmlExternalizable.ReadFromXml(XmlElement element) {
      if(element == null) {
        InitClasses();
        return;
      }

      XmlExternalizationUtil.ReadFromXml(element, this);
    }

    /// <summary>
    /// Writes to XML.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    bool IXmlExternalizable.WriteToXml(XmlElement element) {
      return XmlExternalizationUtil.WriteToXml(element, this);
    }

    #endregion

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    string IXmlExternalizableComponent.TagName {
      get {
        return "AgentJohnson.IntroduceStringConstant";
      }
    }

    ///<summary>
    ///
    ///            Scope that defines which store the data goes into.
    ///            Must not be 
    ///<c>0</c>.
    ///            
    ///</summary>
    ///
    public XmlExternalizationScope Scope {
      get {
        return XmlExternalizationScope.WorkspaceSettings;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Reads the settings.
    /// </summary>
    /// <param name="doc">The doc.</param>
    public void ReadSettings(XmlDocument doc) {
      XmlNode node = doc.SelectSingleNode("/settings/introducestringconstant");
      if(node == null) {
        return;
      }

      GenerateXmlComment = GetAttributeString(node, "generatexmlcomment") == "true";
      ReplaceSpacesMode = int.Parse(GetAttributeString(node, "replacespacesmode"));
      TransformIdentifierMode = int.Parse(GetAttributeString(node, "transformidentifiermode"));

      _classNames.Clear();

      XmlNodeList classes = node.SelectNodes("class");
      if(classes == null) {
        return;
      }

      foreach(XmlNode xmlNode in classes) {
        _classNames.Add(xmlNode.InnerText);
      }
    }

    /// <summary>
    /// Writes the settings.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void WriteSettings(XmlTextWriter writer) {
      writer.WriteStartElement("introducestringconstant");

      writer.WriteAttributeString("generatexmlcomment", GenerateXmlComment ? "true" : "false");

      writer.WriteAttributeString("replacespacesmode", ReplaceSpacesMode.ToString());

      writer.WriteAttributeString("transformidentifiermode", TransformIdentifierMode.ToString());

      foreach(string className in ClassNames) {
        writer.WriteElementString("class", className);
      }

      writer.WriteEndElement();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="name">The name.</param>
    /// <returns>The attribute.</returns>
    static string GetAttributeString(XmlNode node, string name) {
      XmlAttribute attribute = node.Attributes[name];

      return attribute == null ? string.Empty : (attribute.Value ?? string.Empty);
    }

    /// <summary>
    /// Inits the files.
    /// </summary>
    void InitClasses() {
      _classNames = new List<string>();
    }

    #endregion
  }
}