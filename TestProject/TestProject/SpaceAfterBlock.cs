// <copyright file="SpaceAfterBlock.cs" company="Sitecore">
//   Copyright (c) Sitecore. All rights reserved.
// </copyright>

namespace TestProject
{
  using Sitecore.Annotations;

  /// <summary>
  /// Defines the space after block class.
  /// </summary>
  internal class SpaceAfterBlock
  {
    /// <summary>
    /// The property.
    /// </summary>
    private string property;

    /// <summary>
    /// Gets or sets the property.
    /// </summary>
    /// <value>The property.</value>
    public string Property
    {
      get
      {
        return this.property;
      }

      set
      {
        this.property = value;
      }
    }

    /// <summary>
    /// Methods the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The object.</returns>
    public object Method(int index)
    {
      if (index < 0)
      {
        return null;
      }

      return new object();
    }

    /// <summary>
    /// Method2s the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The string.</returns>
    public string Method2(int index)
    {
      object o = Method(i);

      return o.ToString();
    }
  }
}