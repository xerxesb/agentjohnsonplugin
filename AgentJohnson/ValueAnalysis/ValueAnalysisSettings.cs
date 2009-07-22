// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisSettings.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The value analysis settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;
  using JetBrains.Application;
  using JetBrains.ComponentModel;
  using JetBrains.Util;

  /// <summary>
  /// The value analysis settings.
  /// </summary>
  [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentImplementation]
  public class ValueAnalysisSettings : IXmlExternalizableShellComponent
  {
    #region Constants and Fields

    /// <summary>
    /// The _allow null attribute.
    /// </summary>
    private string _allowNullAttribute;

    /// <summary>
    /// The _rules.
    /// </summary>
    private List<Rule> _rules = new List<Rule>();

    /// <summary>
    /// The execute ghost doc.
    /// </summary>
    private bool executeGhostDoc;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValueAnalysisSettings Instance
    {
      get
      {
        return Shell.Instance.GetComponent<ValueAnalysisSettings>();
      }
    }

    /// <summary>
    /// Gets or sets the allow null attribute.
    /// </summary>
    /// <value>The allow null attribute.</value>
    [XmlExternalizable("")]
    public string AllowNullAttribute
    {
      get
      {
        return this._allowNullAttribute ?? string.Empty;
      }

      set
      {
        this._allowNullAttribute = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether GhostDoc should be executed.
    /// </summary>
    /// <value><c>true</c> if GhostDoc should be executed; otherwise, <c>false</c>.</value>
    [XmlExternalizable(false)]
    public bool ExecuteGhostDoc
    {
      get
      {
        return this.executeGhostDoc;
      }

      set
      {
        this.executeGhostDoc = value;
      }
    }

    /// <summary>
    /// Gets or sets the rules.
    /// </summary>
    /// <value>The rules.</value>
    public List<Rule> Rules
    {
      get
      {
        return this._rules;
      }

      set
      {
        this._rules = value;
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

    /// <summary>
    /// Gets or sets the type assertions.
    /// </summary>
    /// <remarks>This is for serialization only.</remarks>
    /// <value>The type assertions.</value>
    [XmlExternalizable("")]
    public string SerializableTypeConfigurations
    {
      get
      {
        var stringWriter = new StringWriter();

        var writer = new XmlTextWriter(stringWriter);

        this.Write(writer);

        return stringWriter.ToString();
      }

      set
      {
        this._rules = new List<Rule>();

        if (string.IsNullOrEmpty(value))
        {
          return;
        }

        var doc = new XmlDocument();

        doc.LoadXml(value);

        var nodes = doc.SelectNodes("/types/type");
        if (nodes == null)
        {
          return;
        }

        this.Read(nodes);
      }
    }

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName
    {
      get
      {
        return "AgentJohnson";
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Reads the settings.
    /// </summary>
    /// <param name="doc">
    /// The document.
    /// </param>
    public void ReadSettings(XmlDocument doc)
    {
      this._rules.Clear();
      this.AllowNullAttribute = string.Empty;

      var nodes = doc.SelectNodes("/settings/valueanalysis/types/type");

      this.Read(nodes);

      var allowNullNode = doc.SelectSingleNode("/settings/valueanalysis/allownull");
      if (allowNullNode != null)
      {
        this.AllowNullAttribute = allowNullNode.InnerText;
      }

      var executeGhostDocNode = doc.SelectSingleNode("/settings/valueanalysis/executeghostdoc");
      if (executeGhostDocNode != null)
      {
        this.ExecuteGhostDoc = executeGhostDocNode.InnerText == "true";
      }
    }

    /// <summary>
    /// Writes the specified writer.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public void WriteSettings(XmlTextWriter writer)
    {
      writer.WriteStartElement("valueanalysis");

      this.Write(writer);

      writer.WriteElementString("allownull", this.AllowNullAttribute);
      writer.WriteElementString("executeghostdoc", this.ExecuteGhostDoc ? "true" : "false");

      writer.WriteEndElement();
    }

    #endregion

    #region Implemented Interfaces

    #region IComponent

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init()
    {
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    #region IXmlExternalizable

    /// <summary>
    /// This method must not fail with null or unexpected Xml!!!
    /// </summary>
    /// <param name="element">
    /// </param>
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
    /// <param name="element">
    /// The element.
    /// </param>
    public void WriteToXml(XmlElement element)
    {
      XmlExternalizationUtil.WriteToXml(element, this);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The attribute.
    /// </returns>
    private static string GetAttributeString(XmlNode node, string name)
    {
      var attribute = node.Attributes[name];

      return attribute == null ? string.Empty : (attribute.Value ?? string.Empty);
    }

    /// <summary>
    /// Gets the element.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The element.
    /// </returns>
    private static string GetElementString(XmlNode node, string name)
    {
      var element = node.SelectSingleNode(name);

      return element == null ? string.Empty : element.InnerText;
    }

    /// <summary>
    /// Reads the specified nodes.
    /// </summary>
    /// <param name="nodes">
    /// The nodes.
    /// </param>
    private void Read(XmlNodeList nodes)
    {
      foreach (XmlNode type in nodes)
      {
        var rule = new Rule
        {
          TypeName = GetAttributeString(type, "type"), NotNull = GetAttributeString(type, "notnull") == "true", CanBeNull = GetAttributeString(type, "canbenull") == "true", PublicParameterAssertion = GetElementString(type, "publicparameterassertion"), NonPublicParameterAssertion = GetElementString(type, "nonpublicparameterassertion"), ReturnAssertion = GetElementString(type, "returnassertion")
        };

        var valueAssertions = type.SelectNodes("valueassertions/valueassertion");
        if (valueAssertions != null)
        {
          foreach (XmlNode valueAssertion in valueAssertions)
          {
            rule.ValueAssertions.Add(valueAssertion.InnerText);
          }
        }

        this._rules.Add(rule);
      }
    }

    /// <summary>
    /// Writes the settings.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void Write(XmlTextWriter writer)
    {
      writer.WriteStartElement("types");

      foreach (var configuration in this.Rules)
      {
        writer.WriteStartElement("type");

        writer.WriteAttributeString("type", configuration.TypeName);
        writer.WriteAttributeString("notnull", configuration.NotNull ? "true" : "false");
        writer.WriteAttributeString("canbenull", configuration.CanBeNull ? "true" : "false");
        writer.WriteElementString("publicparameterassertion", configuration.PublicParameterAssertion);
        writer.WriteElementString("nonpublicparameterassertion", configuration.NonPublicParameterAssertion);
        writer.WriteElementString("returnassertion", configuration.ReturnAssertion);

        writer.WriteStartElement("valueassertions");

        foreach (var assertion in configuration.ValueAssertions)
        {
          writer.WriteElementString("valueassertion", assertion);
        }

        writer.WriteEndElement();

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }

    #endregion
  }
}