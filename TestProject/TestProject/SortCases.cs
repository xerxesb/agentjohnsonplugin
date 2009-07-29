// <copyright file="SortCases.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  using Sitecore.Annotations;

  /// <summary>
  /// The Numbers
  /// </summary>
  public enum Numbers
  {
    /// <summary>
    /// The One constant.
    /// </summary>
    One,

    /// <summary>
    /// The One constant.
    /// </summary>
    Two,

    /// <summary>
    /// The One constant.
    /// </summary>
    Three
  }

  /// <summary>
  /// Defines the sort cases class.
  /// </summary>
  public class SortCases
  {
    #region Public methods

    /// <summary>
    /// Methods this instance.
    /// </summary>
    /// <returns>Returns the string.</returns>
    [NotNull]
    public string Method()
    {
      string s = "1";

      string result = "1";

      switch (s)
      {
        case "3":
          result = "1111111";
          break;
        case "2":
          result = "222222222222222222";
          break;
        default:
          result = "000";
          break;
        case "1":
          result = "333333333333333333333333333333333333333333";
          break;
      }

      int i = 1;

      switch (i)
      {
        case 3:
          result += "1111111";
          break;
        default:
          result += "000";
          break;
        case 2:
          result += "222222222222222222";
          break;
        case 1:
          result += "333333333333333333333333333333333333333333";
          break;
      }

      Numbers n = Numbers.One;

      switch (n)
      {
        case Numbers.Three:
          result += "1111111";
          break;
        default:
          result += "000";
          break;
        case Numbers.Two:
          result += "222222222222222222";
          break;
        case Numbers.One:
          result += "333333333333333333333333333333333333333333";
          break;
      }

      return result;
    }

    #endregion
  }
}