using System.Collections.Specialized;
using System.Text;
using System.Xml;
using JetBrains.ComponentModel;
using JetBrains.Shell;
using JetBrains.Util;

namespace AgentJohnson.ValueAnalysis {
  [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentImplementation]
  public class ValueAnalysisSettings : IXmlExternalizableShellComponent {
    #region Fields

    bool _internalMethods = true;
    bool _privateMethods = true;
    bool _protectedMethods = true;
    bool _publicMethods = true;
    NameValueCollection _typeAssertions;
    string _allowNullAttribute;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValueAnalysisSettings Instance {
      get {
        return (ValueAnalysisSettings)Shell.Instance.GetComponent(typeof(ValueAnalysisSettings));
      }
    }

    /// <summary>
    /// Gets or sets the allow null attribute.
    /// </summary>
    /// <value>The allow null attribute.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue="")]
    public string AllowNullAttribute {
      get {
        return _allowNullAttribute;
      }
      set {
        _allowNullAttribute = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ValueAnalysisSettings"/> internals the methods.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the <see cref="ValueAnalysisSettings"/> internals the methods; otherwise, <c>false</c>.
    /// </value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public bool InternalMethods {
      get {
        return _internalMethods;
      }
      set {
        _internalMethods = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ValueAnalysisSettings"/> privates the methods.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the <see cref="ValueAnalysisSettings"/> privates the methods; otherwise, <c>false</c>.
    /// </value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public bool PrivateMethods {
      get {
        return _privateMethods;
      }
      set {
        _privateMethods = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ValueAnalysisSettings"/> protecteds the methods.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the <see cref="ValueAnalysisSettings"/> protecteds the methods; otherwise, <c>false</c>.
    /// </value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public bool ProtectedMethods {
      get {
        return _protectedMethods;
      }
      set {
        _protectedMethods = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="ValueAnalysisSettings"/> publics the methods.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the <see cref="ValueAnalysisSettings"/> publics the methods; otherwise, <c>false</c>.
    /// </value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public bool PublicMethods {
      get {
        return _publicMethods;
      }
      set {
        _publicMethods = value;
      }
    }

    /// <summary>
    /// Gets or sets the type assertions.
    /// </summary>
    /// <remarks>This is for serialization only.</remarks>
    /// <value>The type assertions.</value>
    [XmlExternalizationUtil.ExternalizableAttribute(DefaultValue = true)]
    public string SerializableTypeAssertions {
      get {
        StringBuilder result = new StringBuilder();

        bool first = true;

        foreach(string key in TypeAssertions.Keys){
          if(!first){
            result.Append("|");
          }

          first = false;

          result.Append(key);
          result.Append('^');
          result.Append(TypeAssertions[key]);
        }

        return result.ToString();
      }
      set {
        _typeAssertions = new NameValueCollection();

        if(!string.IsNullOrEmpty(value)){
          foreach(string pair in value.Split('|')){
            string[] parts = pair.Split('^');

            _typeAssertions.Add(parts[0], parts[1]);
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the type assertions.
    /// </summary>
    /// <value>The type assertions.</value>
    public NameValueCollection TypeAssertions {
      get {
        if(_typeAssertions == null){
          InitTypeAssertions();
        }

        return _typeAssertions;
      }
      set {
        _typeAssertions = value;
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

    #region Public methods

    /// <summary>
    /// This method must not fail with null or unexpected Xml!!!
    /// </summary>
    /// <param name="element"></param>
    public void ReadFromXml(XmlElement element) {
      if(element == null){
        InitTypeAssertions();
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
        return "ValueAnalysisAnnotations";
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

    #region Private methods

    /// <summary>
    /// Inits the type assertions.
    /// </summary>
    void InitTypeAssertions() {
      _typeAssertions = new NameValueCollection();

      _typeAssertions.Add("*", string.Empty);

      // _typeAssertions.Add("*", "Debug.Assert({0} != null, \"Parameter '{0}' cannot be null.\");");
      /*
      _typeAssertions.Add("*", "Sitecore.Diagnostics.Assert.ArgumentNotNull({0}, \"{0}\");");
      _typeAssertions.Add("string", "Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty({0}, \"{0}\");");

      _allowNullAttribute = "Sitecore.AllowNullAttribute";
      */
    }

    #endregion
  }
}