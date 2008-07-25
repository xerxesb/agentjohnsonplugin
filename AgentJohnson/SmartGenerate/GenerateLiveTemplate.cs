using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate("Execute Live Template", "Executes a Live Template.", Priority = -10)]
  public class GenerateLiveTemplate : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IStatement previousStatement = StatementUtil.GetPreviousStatement(element);

      TextRange range = GetNewStatementPosition(element);

      foreach(LiveTemplateInfo liveTemplateInfo in LiveTemplateManager.Instance.LiveTemplateInfos) {
        ConstructorInfo constructor = liveTemplateInfo.Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

        ILiveTemplate handler = constructor != null ? (ILiveTemplate)constructor.Invoke(new object[] {}) : (ILiveTemplate)Activator.CreateInstance(liveTemplateInfo.Type);
        if(handler == null) {
          continue;
        }

        IEnumerable<LiveTemplateItem> liveTemplateItems = handler.GetItems(solution, context, previousStatement, element);
        if(liveTemplateItems == null) {
          continue;
        }

        foreach(LiveTemplateItem liveTemplateMenuItem in liveTemplateItems) {
          string shortcut = "Do not change: " + liveTemplateMenuItem.Shortcut;

          foreach(Template template in LiveTemplatesManager.Instance.TemplateFamily.UserStorage.Templates) {
            if(template.Shortcut != shortcut) {
              continue;
            }

            XmlDocument doc = new XmlDocument();

            XmlElement templateElement = template.WriteToXml(doc);

            string xml = templateElement.OuterXml;

            foreach(string key in liveTemplateMenuItem.Variables.Keys) {
              xml = xml.Replace("$" + key + "$", liveTemplateMenuItem.Variables[key]);
            }

            AddMenuItem(template.Description, xml, range);
          }
        }
      }
    }

    #endregion
  }
}