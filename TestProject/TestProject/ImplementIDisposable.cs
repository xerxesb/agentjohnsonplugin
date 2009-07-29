// <copyright file="ImplementIDisposable.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  using System;

  /// <summary>
  /// Defines the implement I disposable class.
  /// </summary>
  public class ImplementIDisposable : IDisposable
  {
    private bool _disposed;

    ~ImplementIDisposable()
    {
      DisposeObject(false);
    }

    public void Dispose()
    {
      DisposeObject(true);
      GC.SuppressFinalize(this);
    }

    private void DisposeObject(bool disposing)
    {
      if (_disposed)
      {
        return;
      }
      if (disposing)
      {
        // Dispose managed resources.
      }
      // Dispose unmanaged resources.
      _disposed = true;
    }
  }

  /// <summary>
  /// Defines the derived class class.
  /// </summary>
  internal class DerivedClass : ImplementIDisposable
  {
  }
}