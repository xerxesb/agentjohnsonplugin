namespace TestProject
{
  using System;

  public class InvertReturnValue
  {
    /// <summary>
    /// Inverts the return value.
    /// </summary>
    /// <returns>Returns the return value test.</returns>
    public bool InvertReturnValueTest()
    {
      Random random = new Random();

      int next = random.Next(2);
      if (next == 1)
      {
        return false;
      }

      return next == 3;
    }
  }
}