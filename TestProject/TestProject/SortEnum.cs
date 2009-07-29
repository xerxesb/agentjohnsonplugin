// <copyright file="SortEnum.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  /// <summary>
  /// The test enumeration.
  /// </summary>
  public enum Modifiers
  {
    Public,
    Private,
    Protected
  }

  public class Generate
  {
    public void GenerateSwith()
    {
      Modifiers modifiers = Modifiers.Private;
    }
  }
}