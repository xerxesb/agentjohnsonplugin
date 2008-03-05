namespace AgentJohnson.FavoriteFiles {
  /// <summary>
  /// Represents a FavoriteFilePath.
  /// </summary>
  public class FavoriteFilePath {
    #region Fields

    string _path;
    string _projectName;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoriteFilePath"/> class.
    /// </summary>
    public FavoriteFilePath() {
      _path = string.Empty;
      _projectName = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoriteFilePath"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    public FavoriteFilePath(string path) {
      _path = path;
      _projectName = string.Empty;

      if(!path.StartsWith("<")){
        return;
      }

      int n = path.IndexOf('>');
      if(n < 0){
        return;
      }
      _projectName = _path.Substring(1, n - 1);
      _path = _path.Substring(n + 1);

      if (_path.StartsWith("\\")){
        _path = _path.Substring(1);
      }
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>The path.</value>
    public string Path {
      get {
        return _path;
      }
      set {
        _path = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    /// <value>The name of the project.</value>
    public string ProjectName {
      get {
        return _projectName;
      }
      set {
        _projectName = value;
      }
    }

    #endregion

    #region Public methods

    ///<summary>
    ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public override string ToString() {
      if (string.IsNullOrEmpty(ProjectName)){
        return Path;
      }

      return "<" + ProjectName + ">\\" + Path;
    }

    #endregion
  }
}