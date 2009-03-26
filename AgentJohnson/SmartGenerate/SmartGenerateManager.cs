using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ComponentModel;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  [ShellComponentImplementation(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentInterface(ProgramConfigurations.ALL)]
  public class SmartGenerateManager : ITypeLoadingHandler, IShellComponent, IComparer<SmartGenerateHandlerData> {
    #region Fields

    List<SmartGenerateHandlerData> _handlers = new List<SmartGenerateHandlerData>();
    XmlDocument _templates;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SmartGenerateManager Instance {
      get {
        return Shell.Instance.GetComponent < SmartGenerateManager>();
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      _handlers = new List<SmartGenerateHandlerData>();
    }

    /// <summary>
    /// Gets the handlers.
    /// </summary>
    /// <returns>The handlers.</returns>
    [NotNull]
    public IEnumerable<ISmartGenerateHandler> GetHandlers() {
      List<ISmartGenerateHandler> result = new List<ISmartGenerateHandler>();

      List<string> disabledHandlers = new List<string>(SmartGenerateSettings.Instance.DisabledActions.Split('|'));

      foreach(SmartGenerateHandlerData handler in _handlers) {
        if(disabledHandlers.Contains(handler.Name)) {
          continue;
        }

        result.Add(handler.Handler);
      }

      return result;
    }

    /// <summary>
    /// Gets the template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>The template.</returns>
    [CanBeNull]
    public string GetTemplate([NotNull] string template) {
      if(template.StartsWith("<Template")) {
        return template;
      }

      if(_templates == null) {
        string filename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SmartGenerate.xml";

        if(!File.Exists(filename)) {
          return null;
        }

        _templates = new XmlDocument();

        _templates.Load(filename);
      }

      XmlNode node = _templates.SelectSingleNode(string.Format("/*/Template[@uid='{0}' or shortcut='{0}']", template));
      if(node == null) {
        return template;
      }

      return node.OuterXml;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init() {
      Shell.Instance.RegisterTypeLoadingHandler(this);
    }

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="types">The types.</param>
    public void TypesLoaded(ICollection<Assembly> assemblies, ICollection<Type> types) {
      foreach(Type type in types) {
        object[] attributes = type.GetCustomAttributes(typeof(SmartGenerateAttribute), false);
        if(attributes.Length == 1) {
          SmartGenerateAttribute smartGenerateAttribute = (SmartGenerateAttribute)attributes[0];

          ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

          ISmartGenerateHandler handler = constructor != null ? (ISmartGenerateHandler)constructor.Invoke(new object[] {}) : (ISmartGenerateHandler)Activator.CreateInstance(type);
          if(handler == null) {
            continue;
          }

          SmartGenerateHandlerData entry = new SmartGenerateHandlerData {Priority = smartGenerateAttribute.Priority, Name = smartGenerateAttribute.Name, Description = smartGenerateAttribute.Description, Handler = handler};

          _handlers.Add(entry);
        }
      }

      _handlers.Sort(this);
    }

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="types">The types.</param>
    public void TypesUnloaded(ICollection<Assembly> assemblies, ICollection<Type> types) {
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    int IComparer<SmartGenerateHandlerData>.Compare(SmartGenerateHandlerData x, SmartGenerateHandlerData y) {
      return x.Priority - y.Priority;
    }

    #endregion

    #region ITypeLoadingHandler Members

    /// <summary>
    /// Gets the attribute types.
    /// </summary>
    /// <value>The attribute types.</value>
    public Type[] AttributeTypes {
      get {
        return new[] {typeof(SmartGenerateAttribute)};
      }
    }

    #endregion

    /// <summary>
    /// Gets the handlers.
    /// </summary>
    /// <value>The handlers.</value>
    internal List<SmartGenerateHandlerData> Handlers {
      get {
        return _handlers;
      }
    }
  }
}