namespace TestProject
{
  using System.IO;

  public class CatchExceptions
  {
    /// <summary>
    /// Tests this instance.
    /// </summary>
    public void Test()
    {
      File.Delete("e:\\1.txt");
    }
  }
}