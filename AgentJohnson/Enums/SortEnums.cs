// <copyright file="SortEnums.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Enums
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;
  using JetBrains.Application;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Formats the current 'enum', sorts it by value.", Name = "Format 'enum'", Priority = -1, Group = "C#")]
  public class SortEnumContextAction : OneItemContextActionBase
  {
    #region Fields

    /// <summary>
    /// The solution.
    /// </summary>
    private readonly ISolution solution;

    /// <summary>
    /// The text control.
    /// </summary>
    private readonly ITextControl textControl;

    /// <summary>
    /// Regular expression.
    /// </summary>
    private static readonly Regex regex = new Regex("([ ]*)([A-Za-z0-9_]+)[ ]*(=[ ]*([0-9xulXULABCDEFabcdef]+)[ ]*|),([ //A-Za-z0-9,\\:\"\'\\?\\.\r\t]*)\n", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SortEnumContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public SortEnumContextAction(ISolution solution, ITextControl textControl) : base(solution, textControl)
    {
      this.solution = solution;
      this.textControl = textControl;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The context action text.</value>
    public override string Text
    {
      get
      {
        return string.Format("Format 'enum'");
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void ExecuteInternal()
    {
      IEnumDeclaration enumerate = this.GetSelectedElement<IEnumDeclaration>(true);
      if (enumerate == null || enumerate.DeclaredElement == null)
      {
        return;
      }

      using (ModificationCookie cookie = this.textControl.Document.EnsureWritable())
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
      PsiManager.GetInstance(this.solution).CommitTransaction();
      PsiManager.GetInstance(this.solution).CommitAllDocuments();
      PsiManager.GetInstance(this.solution).StartTransaction();
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <returns><c>true</c>, if the context action is available.</returns>
    protected override bool IsAvailableInternal()
    {
      IEnumDeclaration enumerate = this.GetSelectedElement<IEnumDeclaration>(true);
      if (enumerate == null)
      {
        return false;
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Sorts the enum.
    /// </summary>
    /// <param name="enumerate">The enumerate.</param>
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

      int maxFieldChars = 0;
      bool flags = enumerate.DeclaredElement.HasAttributeInstance(new CLRTypeName("System.FlagsAttribute"), true);
      SortedDictionary<uint, KeyValuePair<string, string>> values = new SortedDictionary<uint, KeyValuePair<string, string>>();

      foreach (IEnumMemberDeclaration field in enumerate.EnumMemberDeclarations)
      {
        if (field.EnumMember.ShortName.Length > maxFieldChars)
        {
          maxFieldChars = field.EnumMember.ShortName.Length;
        }
      }

      int startIndexFirst = -1;
      uint lastEnumValue = 0;
      string current = enumerate.GetText();

      StringBuilder code = new StringBuilder(current);
      StringBuilder newLines = new StringBuilder();

      MatchCollection macthes = regex.Matches(current);

      foreach (Match match in macthes)
      {
        uint enumValue;
        string newline;

        if (startIndexFirst == -1)
        {
          startIndexFirst = current.IndexOf(match.Value);
        }

        if (match.Groups[3].Value.Length == 0)
        {
          enumValue = lastEnumValue++;
        }
        else if (uint.TryParse(match.Groups[4].Value.Replace("0x", string.Empty), NumberStyles.AllowHexSpecifier | NumberStyles.HexNumber, CultureInfo.InvariantCulture, out enumValue) || uint.TryParse(match.Groups[4].Value, out enumValue))
        {
          lastEnumValue = enumValue + 1;
        }
        else
        {
          MessageBox.Show("Unidentifed number style (" + match.Groups[4].Value + ").");
          continue;
        }

        if (flags)
        {
          newline = String.Format("{2}{0,-" + maxFieldChars + "} = 0x{1:X8},{3}\n", match.Groups[2].Value, enumValue, match.Groups[1].Value, match.Groups[5].Value);
        }
        else
        {
          newline = String.Format("{2}{0,-" + maxFieldChars + "} = {1},{3}\n", match.Groups[2].Value, enumValue, match.Groups[1].Value, match.Groups[5].Value);
        }

        values.Add(enumValue, new KeyValuePair<string, string>(match.Value, newline));
      }

      foreach (KeyValuePair<string, string> pair in values.Values)
      {
        code.Replace(pair.Key, string.Empty);
        newLines.Append(pair.Value);
      }

      if (startIndexFirst >= 0)
      {
        code.Insert(startIndexFirst, newLines.ToString());
      }

      TextRange textrange = enumerate.GetDocumentRange().TextRange;
      IDocument document = this.textControl.Document;

      document.ReplaceText(textrange, code.ToString());
    }

    #endregion
  }
}