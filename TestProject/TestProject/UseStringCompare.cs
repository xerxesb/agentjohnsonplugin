namespace TestProject
{
  /// <summary>
  /// Defines the use string builder class.
  /// </summary>
  public class UseStringCompare
  {
    /// <summary>
    /// Methods this instance.
    /// </summary>
    public void Method()
    {
      var text = "123" + "1234" + "123";

      var text2 = text + "123" + string.Format("{0}", 0);

      if (text != text2)
      {
        text = "123";
      }

      int i = 0;
      int j = 2;

      if (i != j)
      {
        text = 123;
      }
    }

  }
}