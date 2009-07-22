// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortEnums.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Enums
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Text;
  using System.Text.RegularExpressions;
  using JetBrains.Application;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Formats the current 'enum', sorts it by value.", Name = "Format 'enum'", Priority = -1, Group = "C#")]
  public class SortEnumContextAction : ContextActionBase
  {
    #region Constants and Fields

    /// <summary>
    /// Regular expression.
    /// </summary>
    private static readonly Regex regex = new Regex("([ //]*)([A-Z0-9_]+)[ ]*(=[ ]*([0-9XULABCDEF]+)[ ]*|),(.*)\n", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SortEnumContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public SortEnumContextAction(ICSharpContextActionDataProvider provider)
      : base(provider)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute(IElement element)
    {
      var enumerate = this.GetSelectedElement<IEnumDeclaration>(true);
      if (enumerate == null || enumerate.DeclaredElement == null)
      {
        return;
      }

      using (var cookie = this.Provider.TextControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Sort Enum"))
        {
          this.SortEnum(enumerate);
        }
      }

      // This is really hacky
      PsiManager.GetInstance(this.Provider.Solution).CommitTransaction();
      PsiManager.GetInstance(this.Provider.Solution).CommitAllDocuments();
      PsiManager.GetInstance(this.Provider.Solution).StartTransaction();
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>
    /// The context action text.
    /// </value>
    /// <returns>
    /// The get text.
    /// </returns>
    protected override string GetText()
    {
      return string.Format("Format 'enum' [Agent Johnson]");
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == <c>null</c>
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c>, if the context action is available.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      var enumerate = this.GetSelectedElement<IEnumDeclaration>(true);
      return enumerate != null;
    }

    /// <summary>
    /// Sorts the enum.
    /// </summary>
    /// <param name="enumerate">
    /// The enumerate.
    /// </param>
    private void SortEnum(IEnumDeclaration enumerate)
    {
      if (enumerate == null)
      {
        return;
      }

      if (enumerate.DeclaredElement == null)
      {
        return;
      }

      if (enumerate.GetContainingFile() == null)
      {
        return;
      }

      var maxFieldChars = 0;
      var flags = enumerate.DeclaredElement.HasAttributeInstance(new CLRTypeName("System.FlagsAttribute"), true);
      var enumType = enumerate.GetUnderlyingType().GetPresentableName(enumerate.Language);

      foreach (var field in enumerate.EnumMemberDeclarations)
      {
        if (field.EnumMember.ShortName.Length > maxFieldChars)
        {
          maxFieldChars = field.EnumMember.ShortName.Length;
        }
      }

      var startIndexFirst = -1;
      var current = enumerate.GetText();

      var code = new StringBuilder(current);
      var newLines = new StringBuilder();
      var lastEnumValue = new DualEnumValue(enumType);
      var enumValue = new DualEnumValue(enumType);
      var values = new SortedDictionary<DualEnumValue, List<KeyValuePair<string, string>>>();

      var macthes = regex.Matches(current);

      foreach (Match match in macthes)
      {
        string newline;

        if (startIndexFirst == -1)
        {
          startIndexFirst = current.IndexOf(match.Value);
        }

        if (match.Groups[3].Value.Length == 0)
        {
          enumValue = lastEnumValue++;
        }
        else
        {
          enumValue.Set(match.Groups[4].Value);
          lastEnumValue = enumValue + 1;
        }

        if (flags)
        {
          newline = String.Format("{2}{0,-" + maxFieldChars + "} = 0x{1:X8},{3}\n", match.Groups[2].Value, enumValue.UnderlyingValue, match.Groups[1].Value, match.Groups[5].Value);
        }
        else
        {
          newline = String.Format("{2}{0,-" + maxFieldChars + "} = {1},{3}\n", match.Groups[2].Value, enumValue.UnderlyingValue, match.Groups[1].Value, match.Groups[5].Value);
        }

        List<KeyValuePair<string, string>> list;

        if (!values.TryGetValue(enumValue, out list))
        {
          values.Add(enumValue, list = new List<KeyValuePair<string, string>>());
        }

        list.Add(new KeyValuePair<string, string>(match.Value, newline));
      }

      foreach (var list in values.Values)
      {
        foreach (var pair in list)
        {
          code.Replace(pair.Key, string.Empty);
          newLines.Append(pair.Value);
        }
      }

      if (startIndexFirst >= 0)
      {
        code.Insert(startIndexFirst, newLines.ToString());
      }

      var textrange = enumerate.GetDocumentRange().TextRange;
      var document = this.Provider.TextControl.Document;

      document.ReplaceText(textrange, code.ToString());
    }

    #endregion

    /// <summary>
    /// The dual enum value.
    /// </summary>
    private class DualEnumValue : IComparable<DualEnumValue>, IComparable
    {
      #region Constants and Fields

      /// <summary>
      /// The base type.
      /// </summary>
      private readonly string baseType;

      /// <summary>
      /// The signed.
      /// </summary>
      private long signed;

      /// <summary>
      /// The signed style.
      /// </summary>
      private bool signedStyle;

      /// <summary>
      /// The unsigned.
      /// </summary>
      private ulong unsigned;

      #endregion

      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="DualEnumValue"/> class.
      /// </summary>
      /// <param name="baseType">
      /// The base type.
      /// </param>
      public DualEnumValue(string baseType)
      {
        this.baseType = baseType;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="DualEnumValue"/> class.
      /// </summary>
      /// <param name="baseType">
      /// The base type.
      /// </param>
      /// <param name="value">
      /// The value.
      /// </param>
      public DualEnumValue(string baseType, string value)
        : this(baseType)
      {
        this.Set(value);
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets the underlying value.
      /// </summary>
      /// <value>The underlying value.</value>
      public object UnderlyingValue
      {
        get
        {
          return this.signedStyle ? this.signed : (object)this.unsigned;
        }
      }

      #endregion

      #region Operators

      /// <summary>
      /// Implements the operator +.
      /// </summary>
      /// <param name="a">Argument A</param>
      /// <param name="b">Argument b.</param>
      /// <returns>The result of the operator.</returns>
      public static DualEnumValue operator +(DualEnumValue a, long b)
      {
        var result = new DualEnumValue(a.baseType)
        {
          signedStyle = a.signedStyle
        };

        if (a.signedStyle)
        {
          result.signed = a.signed + b;
        }
        else
        {
          if (b > 0)
          {
            result.unsigned = a.unsigned + (ulong)b;
          }
          else
          {
            result.unsigned = a.unsigned - (ulong)(-b);
          }
        }

        return result;
      }

      /// <summary>
      /// Implements the operator --.
      /// </summary>
      /// <param name="a">The argument.</param>
      /// <returns>The result of the operator.</returns>
      public static DualEnumValue operator --(DualEnumValue a)
      {
        var result = new DualEnumValue(a.baseType)
        {
          signedStyle = a.signedStyle
        };

        if (a.signedStyle)
        {
          result.signed = a.signed - 1;
        }
        else
        {
          result.unsigned = a.unsigned - 1;
        }

        return result;
      }

      /// <summary>
      /// Implements the operator ++.
      /// </summary>
      /// <param name="a">Argument A.</param>
      /// <returns>The result of the operator.</returns>
      public static DualEnumValue operator ++(DualEnumValue a)
      {
        var result = new DualEnumValue(a.baseType)
        {
          signedStyle = a.signedStyle, signed = a.signed, unsigned = a.unsigned
        };

        if (a.signedStyle)
        {
          result.signed = a.signed + 1;
        }
        else
        {
          result.unsigned = a.unsigned + 1;
        }

        return result;
      }

      /// <summary>
      /// Implements the operator -.
      /// </summary>
      /// <param name="a">Argument A.</param>
      /// <param name="b">Argument B.</param>
      /// <returns>The result of the operator.</returns>
      public static DualEnumValue operator -(DualEnumValue a, long b)
      {
        var result = new DualEnumValue(a.baseType)
        {
          signedStyle = a.signedStyle
        };

        if (a.signedStyle)
        {
          result.signed = a.signed - b;
        }
        else
        {
          if (b > 0)
          {
            result.unsigned = a.unsigned - (ulong)b;
          }
          else
          {
            result.unsigned = a.unsigned + (ulong)(-b);
          }
        }

        return result;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Sets the specified value.
      /// </summary>
      /// <param name="value">The value.</param>
      public void Set(string value)
      {
        switch (this.baseType)
        {
          case "sbyte":
          case "short":
          case "int":
          case "long":
          {
            this.signedStyle = true;

            // hex number
            if (!value.StartsWith("0x") ||
                !long.TryParse(value.Replace("0x", string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.signed))
            {
              // normal number
              if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.signed))
              {
                System.Windows.Forms.MessageBox.Show("Unidentifed number style (" + value + ").");
              }
            }
          }

            break;
          case "byte":
          case "ushort":
          case "uint":
          case "ulong":
          {
            // hex number
            if (!value.StartsWith("0x") ||
                !ulong.TryParse(value.Replace("0x", string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out this.unsigned))
            {
              // normal number
              if (!ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.unsigned))
              {
                System.Windows.Forms.MessageBox.Show("Unidentifed number style (" + value + ").");
              }
            }
          }

            break;
        }
      }

      /// <summary>
      /// Returns a <see cref="System.String"/> that represents this instance.
      /// </summary>
      /// <returns>
      /// A <see cref="System.String"/> that represents this instance.
      /// </returns>
      public override string ToString()
      {
        return this.signedStyle ? this.signed.ToString() : this.unsigned.ToString();
      }

      #endregion

      #region Implemented Interfaces

      #region IComparable

      /// <summary>
      /// Compares to.
      /// </summary>
      /// <param name="other">The other.</param>
      /// <returns>Returns the to.</returns>
      public int CompareTo(object other)
      {
        if (other is DualEnumValue)
        {
          return this.CompareTo((DualEnumValue)other);
        }

        return this.signedStyle ? this.signed.CompareTo(other) : this.unsigned.CompareTo(other);
      }

      #endregion

      #region IComparable<DualEnumValue>

      /// <summary>
      /// Compares the current object with another object of the same type.
      /// </summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>
      /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
      /// Value
      /// Meaning
      /// Less than zero
      /// This object is less than the <paramref name="other"/> parameter.
      /// Zero
      /// This object is equal to <paramref name="other"/>.
      /// Greater than zero
      /// This object is greater than <paramref name="other"/>.
      /// </returns>
      public int CompareTo(DualEnumValue other)
      {
        if (this.signedStyle)
        {
          return other.signedStyle ? this.signed.CompareTo(other.signed) : this.signed.CompareTo((long)other.unsigned);
        }

        return other.signedStyle ? this.unsigned.CompareTo((ulong)other.signed) : this.unsigned.CompareTo(other.unsigned);
      }

      #endregion

      #endregion
    }
  }
}