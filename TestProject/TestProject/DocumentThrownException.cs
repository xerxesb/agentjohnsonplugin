// <copyright file="DocumentThrownException.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  using System;
  using System.IO;

  /// <summary>
  /// Defines the document thrown exception class.
  /// </summary>
  internal class DocumentThrownException
  {
    #region Public methods

    /// <summary>
    /// Throws the exception.
    /// </summary>
    public void ThrowException()
    {
      File.Delete("c:\autoexec.bat");

      ThrowNotImplemented();

      throw new InvalidOperationException("Fail!");
    }

    /// <summary>
    /// Throws the not implemented.
    /// </summary>
    /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
    public void ThrowNotImplemented()
    {
      File.Delete("c:\autoexec.bat");

      throw new NotImplementedException();
    }

    #endregion
  }
}