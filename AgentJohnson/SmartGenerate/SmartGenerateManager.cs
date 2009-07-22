// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateManager.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The smart generate manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Xml;
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.ComponentModel;

  /// <summary>
  /// The smart generate manager.
  /// </summary>
  [ShellComponentImplementation(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentInterface(ProgramConfigurations.ALL)]
  public class SmartGenerateManager : ITypeLoadingHandler, IShellComponent, IComparer<SmartGenerateHandlerData>
  {
    #region Constants and Fields

    /// <summary>
    /// The _handlers.
    /// </summary>
    private List<SmartGenerateHandlerData> _handlers = new List<SmartGenerateHandlerData>();

    /// <summary>
    /// The _templates.
    /// </summary>
    private XmlDocument _templates;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SmartGenerateManager Instance
    {
      get
      {
        return Shell.Instance.GetComponent<SmartGenerateManager>();
      }
    }

    /// <summary>
    /// Gets the attribute types.
    /// </summary>
    /// <value>The attribute types.</value>
    public Type[] AttributeTypes
    {
      get
      {
        return new[]
        {
          typeof(SmartGenerateAttribute)
        };
      }
    }

    /// <summary>
    /// Gets the handlers.
    /// </summary>
    /// <value>The handlers.</value>
    internal List<SmartGenerateHandlerData> Handlers
    {
      get
      {
        return this._handlers;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the handlers.
    /// </summary>
    /// <returns>
    /// The handlers.
    /// </returns>
    [NotNull]
    public IEnumerable<ISmartGenerateHandler> GetHandlers()
    {
      var result = new List<ISmartGenerateHandler>();

      var disabledHandlers = new List<string>(SmartGenerateSettings.Instance.DisabledActions.Split('|'));

      foreach (var handler in this._handlers)
      {
        if (disabledHandlers.Contains(handler.Name))
        {
          continue;
        }

        result.Add(handler.Handler);
      }

      return result;
    }

    /// <summary>
    /// Gets the template.
    /// </summary>
    /// <param name="template">
    /// The template.
    /// </param>
    /// <returns>
    /// The template.
    /// </returns>
    [CanBeNull]
    public string GetTemplate([NotNull] string template)
    {
      if (template.StartsWith("<Template"))
      {
        return template;
      }

      if (this._templates == null)
      {
        var filename = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SmartGenerate.xml";

        if (!File.Exists(filename))
        {
          return null;
        }

        this._templates = new XmlDocument();

        this._templates.Load(filename);
      }

      var node = this._templates.SelectSingleNode(string.Format("/*/Template[@uid='{0}' or shortcut='{0}']", template));
      if (node == null)
      {
        return template;
      }

      return node.OuterXml;
    }

    #endregion

    #region Implemented Interfaces

    #region IComparer<SmartGenerateHandlerData>

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">
    /// The first object to compare.
    /// </param>
    /// <param name="y">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    int IComparer<SmartGenerateHandlerData>.Compare(SmartGenerateHandlerData x, SmartGenerateHandlerData y)
    {
      return x.Priority - y.Priority;
    }

    #endregion

    #region IComponent

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init()
    {
      Shell.Instance.RegisterTypeLoadingHandler(this);
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this._handlers = new List<SmartGenerateHandlerData>();
    }

    #endregion

    #region ITypeLoadingHandler

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="types">
    /// The types.
    /// </param>
    public void TypesLoaded(ICollection<Assembly> assemblies, ICollection<Type> types)
    {
      foreach (var type in types)
      {
        var attributes = type.GetCustomAttributes(typeof(SmartGenerateAttribute), false);
        if (attributes.Length == 1)
        {
          var smartGenerateAttribute = (SmartGenerateAttribute)attributes[0];

          var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

          var handler = constructor != null ? (ISmartGenerateHandler)constructor.Invoke(new object[]
          {
          }) : (ISmartGenerateHandler)Activator.CreateInstance(type);
          if (handler == null)
          {
            continue;
          }

          var entry = new SmartGenerateHandlerData
          {
            Priority = smartGenerateAttribute.Priority, Name = smartGenerateAttribute.Name, Description = smartGenerateAttribute.Description, Handler = handler
          };

          this._handlers.Add(entry);
        }
      }

      this._handlers.Sort(this);
    }

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies.
    /// </param>
    /// <param name="types">
    /// The types.
    /// </param>
    public void TypesUnloaded(ICollection<Assembly> assemblies, ICollection<Type> types)
    {
    }

    #endregion

    #endregion
  }
}