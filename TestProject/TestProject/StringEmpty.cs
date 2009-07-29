namespace TestProject
{
  public class StringEmpty
  {
    #region Public methods

    /// <summary>
    /// Methods this instance.
    /// </summary>
    public void Method()
    {
      string s = string.Empty;
      switch (s)
      {
        case "":
          return;
      }
    }

    #endregion
  }
}