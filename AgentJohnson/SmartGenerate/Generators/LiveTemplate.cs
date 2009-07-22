// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveTemplate.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The live template.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using System;
  using System.Reflection;
  using System.Xml;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
  using JetBrains.Util;

  /// <summary>
  /// The live template.
  /// </summary>
  [SmartGenerate("Execute Live Template", "Executes a Live Template.", Priority = -10)]
  public class LiveTemplate : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      smartGenerateParameters.PreviousStatement = StatementUtil.GetPreviousStatement(element);

      var defaultRange = StatementUtil.GetNewStatementPosition(element);

      foreach (var liveTemplateInfo in LiveTemplateManager.Instance.LiveTemplateInfos)
      {
        var constructor = liveTemplateInfo.Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

        var handler = constructor != null ? (ILiveTemplate)constructor.Invoke(new object[]
        {
        }) : (ILiveTemplate)Activator.CreateInstance(liveTemplateInfo.Type);
        if (handler == null)
        {
          continue;
        }

        var liveTemplateItems = handler.GetItems(smartGenerateParameters);
        if (liveTemplateItems == null)
        {
          continue;
        }

        foreach (var liveTemplateMenuItem in liveTemplateItems)
        {
          var shortcut = "@Do not change: " + liveTemplateMenuItem.Shortcut;

          foreach (var template in LiveTemplatesManager.Instance.TemplateFamily.UserStorage.Templates)
          {
            if (template.Shortcut != shortcut)
            {
              continue;
            }

            var doc = new XmlDocument();

            var templateElement = template.WriteToXml(doc);

            var xml = templateElement.OuterXml;

            foreach (var key in liveTemplateMenuItem.Variables.Keys)
            {
              xml = xml.Replace("$" + key + "$", liveTemplateMenuItem.Variables[key]);
              template.Description = template.Description.Replace("$" + key + "$", liveTemplateMenuItem.Variables[key]);
            }

            var range = liveTemplateMenuItem.Range;
            if (range == TextRange.InvalidRange)
            {
              range = defaultRange;
            }

            this.AddAction(template.Description, xml, range);
          }
        }
      }
    }

    #endregion
  }
}