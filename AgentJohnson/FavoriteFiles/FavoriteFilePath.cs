// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FavoriteFilePath.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents a FavoriteFilePath.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.FavoriteFiles
{
  /// <summary>
  /// Represents a FavoriteFilePath.
  /// </summary>
  public class FavoriteFilePath
  {
    #region Constants and Fields

    /// <summary>
    /// The _path.
    /// </summary>
    private string _path;

    /// <summary>
    /// The _project name.
    /// </summary>
    private string _projectName;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoriteFilePath"/> class.
    /// </summary>
    public FavoriteFilePath()
    {
      this._path = string.Empty;
      this._projectName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoriteFilePath"/> class.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    public FavoriteFilePath(string path)
    {
      this._path = path;
      this._projectName = string.Empty;

      if (!path.StartsWith("<"))
      {
        return;
      }

      var n = path.IndexOf('>');
      if (n < 0)
      {
        return;
      }

      this._projectName = this._path.Substring(1, n - 1);
      this._path = this._path.Substring(n + 1);

      if (this._path.StartsWith("\\"))
      {
        this._path = this._path.Substring(1);
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>The path.</value>
    public string Path
    {
      get
      {
        return this._path;
      }

      set
      {
        this._path = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    /// <value>The name of the project.</value>
    public string ProjectName
    {
      get
      {
        return this._projectName;
      }

      set
      {
        this._projectName = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.ProjectName))
      {
        return this.Path;
      }

      return "<" + this.ProjectName + ">\\" + this.Path;
    }

    #endregion
  }
}