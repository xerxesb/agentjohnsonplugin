namespace TestProject
{
  using System.Text;

  /// <summary>
  /// Defines the use string builder class.
  /// </summary>
  public class UseStringBuilder
  {
    /// <summary>
    /// Methods this instance.
    /// </summary>
    public void Method()
    {
      var text = "123" + "1234" + "123";

      var text2 = text + "123" + string.Format("{0}", 0);
    }

  }
}