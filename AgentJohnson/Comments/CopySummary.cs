// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopySummary.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the copy summary class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Comments
{
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.CSharp.Util;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// Defines the copy summary class.
  /// </summary>
  [ContextAction(Description = "Replaces the <returns> tag with the text from the <summary> tag.", Name = "Replace <returns> with <summary>", Priority = -1, Group = "C#")]
  public class CopySummary : ContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CopySummary"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public CopySummary(ICSharpContextActionDataProvider provider) : base(provider)
    {
      this.StartTransaction = false;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute([NotNull] IElement element)
    {
      IDocCommentNode summary;
      IDocCommentNode returns;

      GetSummaryAndReturns(element, out summary, out returns);
      if (summary == null || returns == null)
      {
        return;
      }

      var text = GetSummaryText(summary).Trim();

      if (text.StartsWith("Gets or sets the "))
      {
        text = "The " + text.Substring(17);
      }

      if (text.StartsWith("Gets the "))
      {
        text = "The " + text.Substring(9);
      }

      text = "/// <returns>" + text + "</returns>";

      this.ReplaceReturnsTagText(returns, text);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The text in the context menu.
    /// </returns>
    protected override string GetText()
    {
      return "Replace <returns> with <summary> [Agent Johnson]";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable([NotNull] IElement element)
    {
      IDocCommentNode summary;
      IDocCommentNode returns;

      GetSummaryAndReturns(element, out summary, out returns);
      if (summary == null || returns == null)
      {
        return false;
      }

      return returns.GetDocumentRange().Contains(this.Provider.DocumentCaret);
    }

    /// <summary>
    /// Gets the doc comment block.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// Returns the I doc comment block node. 
    /// </returns>
    [CanBeNull]
    private static IDocCommentBlockNode GetDocCommentBlock([NotNull] IElement element)
    {
      var typeMemberDeclaration = element.GetContainingElement<ITypeMemberDeclaration>(true);
      if (typeMemberDeclaration == null)
      {
        return null;
      }

      var docCommentBlockOwnerNode = XmlDocTemplateUtil.FindDocCommentOwner(typeMemberDeclaration);
      if (docCommentBlockOwnerNode == null)
      {
        return null;
      }

      return docCommentBlockOwnerNode.GetDocCommentBlockNode();
    }

    /// <summary>
    /// Gets the summary and returns.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="summary">
    /// The summary.
    /// </param>
    /// <param name="returns">
    /// The returns. 
    /// </param>
    private static void GetSummaryAndReturns([NotNull] IElement element, out IDocCommentNode summary, out IDocCommentNode returns)
    {
      summary = null;
      returns = null;

      var docCommentBlockNode = GetDocCommentBlock(element);
      if (docCommentBlockNode == null)
      {
        return;
      }

      if (!docCommentBlockNode.Contains(element))
      {
        return;
      }

      for (var child = docCommentBlockNode.FirstChild; child != null; child = child.NextSibling)
      {
        var docCommentNode = child as IDocCommentNode;
        if (docCommentNode == null)
        {
          continue;
        }

        var text = docCommentNode.CommentText;

        if (text.IndexOf("<summary>") >= 0)
        {
          summary = docCommentNode;
        }

        if (text.IndexOf("<returns>") >= 0)
        {
          returns = docCommentNode;
        }
      }
    }

    /// <summary>
    /// Gets the summary text.
    /// </summary>
    /// <param name="summary">
    /// The summary.
    /// </param>
    /// <returns>
    /// Returns the string.
    /// </returns>
    private static string GetSummaryText(IDocCommentNode summary)
    {
      var result = string.Empty;

      while (summary != null)
      {
        var text = summary.CommentText;
        var line = string.Empty;

        var start = text.IndexOf("<summary>");
        if (start >= 0)
        {
          line = text.Substring(start + 9).Trim();
        }

        var end = text.IndexOf("</summary>");
        if (end >= 0)
        {
          line = text.Substring(0, end).Trim();
        }

        if (start < 0 && end < 0)
        {
          line = text;
        }

        if (!string.IsNullOrEmpty(line))
        {
          if (!string.IsNullOrEmpty(result))
          {
            result += " ";
          }

          result += line;
        }

        if (end >= 0)
        {
          break;
        }

        summary = summary.FindNextNode(node => node is IDocCommentNode ? TreeNodeActionType.ACCEPT : TreeNodeActionType.CONTINUE) as IDocCommentNode;
      }

      return result;
    }

    /// <summary>
    /// Replaces the returns tag text.
    /// </summary>
    /// <param name="returns">
    /// The returns node <paramref name="text"/>.
    /// </param>
    /// <param name="text">
    /// The text to replace with.
    /// </param>
    private void ReplaceReturnsTagText(IDocCommentNode returns, string text)
    {
      using (CommandCookie.Create(string.Format("Context Action {0}", this.GetText())))
      {
        using (var cookie = this.TextControl.Document.EnsureWritable())
        {
          if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
          {
            return;
          }

          this.Provider.Document.ReplaceText(returns.GetDocumentRange().TextRange, text);
        }
      }
    }

    #endregion
  }
}