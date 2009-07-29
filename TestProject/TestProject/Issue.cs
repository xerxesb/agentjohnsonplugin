// <copyright file="Issue.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  using System;
  using System.Collections;

  /// <summary>
  /// Defines the issue class.
  /// </summary>
  public class Issue
  {
    #region Public methods

    /// <summary>
    /// Adds a value list to a grid column for value-name lookups.
    /// </summary>
    /// <param name="grid">The grid that owns the column.</param>
    /// <param name="valueMember">The value member for the dropdown.</param>
    /// <param name="data">The collection to which the combo will be bound.</param>
    /// <param name="displayMember">The display member for the dropdown.</param>
    /// <param name="gridColumn">The grid column to which the combo will be bound.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="grid"/>can not be <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="data"/>can not be <see langword="null"/>.</exception
    /// <exception cref="System.ArgumentNullException"><paramref name="displayMember"/> can not be <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="valueMember"/> can not be <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="gridColumn"/> can not be <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="displayMember"/> can not be empty.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="valueMember"/> can not be empty.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="gridColumn"/> can not be empty.</exception>
    public static void AddValueList(object grid,
                                    IEnumerable data,
                                    string valueMember,
                                    string displayMember,
                                    string gridColumn)
    {
      if (grid == null)
      {
        throw new ArgumentNullException("grid", "\"grid\" can not be null.");
      }

      if (data == null)
      {
        throw new ArgumentNullException("data", "\"collectiondata can not be null.");
      }

      if (displayMember == null)
      {
        throw new ArgumentNullException("displayMember", "\"displayMember\" can not be null.");
      }

      if (displayMember.Length <= 0)
      {
        throw new ArgumentException("\"displayMember\" can not be empty.", "displayMember");
      }

      if (valueMember == null)
      {
        throw new ArgumentNullException("valueMember", "\"valueMember\" can not be null.");
      }

      if (valueMember.Length <= 0)
      {
        throw new ArgumentException("\"valueMember\" can not be empty.", "valueMember");
      }

      if (gridColumn == null)
      {
        throw new ArgumentNullException("gridColumn", "\"gridColumn\" can not be null.");
      }

      if (gridColumn.Length <= 0)
      {
        throw new ArgumentException("\"gridColumn\" can not be empty.", "gridColumn");
      }
    }

    #endregion
  }
}