// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveTemplateAttribute.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The live template attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;

  /// <summary>
  /// The live template attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class LiveTemplateAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The _description.
    /// </summary>
    private readonly string _description;

    /// <summary>
    /// The _name.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// The _priority.
    /// </summary>
    private int _priority;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTemplateAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    public LiveTemplateAttribute(string name, string description)
    {
      this._description = description;
      this._name = name;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
      get
      {
        return this._description ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get
      {
        return this._name ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public int Priority
    {
      get
      {
        return this._priority;
      }

      set
      {
        this._priority = value;
      }
    }

    #endregion
  }
}