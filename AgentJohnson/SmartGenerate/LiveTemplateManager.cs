namespace AgentJohnson.SmartGenerate
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.ComponentModel;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
  using JetBrains.ReSharper.LiveTemplates.Templates;
  using JetBrains.ReSharper.LiveTemplates.UI.TemplateEditor;
  using JetBrains.UI.PopupMenu;

  /// <summary>
  /// 
  /// </summary>
  [ShellComponentImplementation(ProgramConfigurations.VS_ADDIN)]
  [ShellComponentInterface(ProgramConfigurations.ALL)]
  public class LiveTemplateManager : ITypeLoadingHandler, IShellComponent, IComparer<LiveTemplateInfo>
  {
    #region Fields

    private List<LiveTemplateInfo> _liveTemplateInfos = new List<LiveTemplateInfo>();

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static LiveTemplateManager Instance
    {
      get
      {
        return Shell.Instance.GetComponent<LiveTemplateManager>();
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Handles the Clicked event of the menuItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public static void AddLiveTemplate(object sender, EventArgs e)
    {
      SimpleMenuItem simpleMenuItem = sender as SimpleMenuItem;
      if (simpleMenuItem == null)
      {
        return;
      }

      LiveTemplateItem liveTemplateItem = simpleMenuItem.Tag as LiveTemplateItem;
      if (liveTemplateItem == null)
      {
        return;
      }

      Template template = new Template("@Do not change: " + liveTemplateItem.Shortcut, liveTemplateItem.Description, string.Empty, true, true, true);

      Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue("Create Live Template", delegate
      {
        using (ReadLockCookie.Create())
        {
          using (CommandCookie.Create("Context Action Create Live Template"))
          {
            TemplateEditorManager.Instance.CreateTemplate(template, LiveTemplatesManager.Instance.TemplateFamily.UserStorage);
          }
        }
      });
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this._liveTemplateInfos = new List<LiveTemplateInfo>();
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The live templates.</returns>
    public List<LiveTemplateItem> GetLiveTemplates(SmartGenerateParameters parameters)
    {
      List<LiveTemplateItem> result = new List<LiveTemplateItem>();

      parameters.PreviousStatement = StatementUtil.GetPreviousStatement(parameters.Element);

      foreach (LiveTemplateInfo liveTemplateInfo in this.LiveTemplateInfos)
      {
        ConstructorInfo constructor = liveTemplateInfo.Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

        ILiveTemplate liveTemplate = constructor != null ? (ILiveTemplate)constructor.Invoke(new object[]
        {
        }) : (ILiveTemplate)Activator.CreateInstance(liveTemplateInfo.Type);
        if (liveTemplate == null)
        {
          continue;
        }

        IEnumerable<LiveTemplateItem> liveTemplateItems = liveTemplate.GetItems(parameters);
        if (liveTemplateItems == null)
        {
          continue;
        }

        foreach (LiveTemplateItem liveTemplateItem in liveTemplateItems)
        {
          result.Add(liveTemplateItem);
        }
      }

      return result;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public void Init()
    {
      Shell.Instance.RegisterTypeLoadingHandler(this);

      IUpdatableAction action = ActionManager.Instance.GetAction("CompleteStatement");
      if (action != null)
      {
        action.AddHandler(new SmartGenerateContextAction());
      }
    }

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="types">The types.</param>
    public void TypesLoaded(ICollection<Assembly> assemblies, ICollection<Type> types)
    {
      foreach (Type type in types)
      {
        object[] attributes = type.GetCustomAttributes(typeof(LiveTemplateAttribute), false);

        if (attributes.Length != 1)
        {
          continue;
        }

        LiveTemplateAttribute liveLiveTemplateAttribute = (LiveTemplateAttribute)attributes[0];

        LiveTemplateInfo entry = new LiveTemplateInfo
        {
          Priority = liveLiveTemplateAttribute.Priority,
          Name = liveLiveTemplateAttribute.Name,
          Description = liveLiveTemplateAttribute.Description,
          Type = type
        };

        this._liveTemplateInfos.Add(entry);
      }

      this._liveTemplateInfos.Sort(this);
    }

    /// <summary>
    /// Called when types have been loaded.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="types">The types.</param>
    public void TypesUnloaded(ICollection<Assembly> assemblies, ICollection<Type> types)
    {
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <returns>
    /// Value Condition Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
    /// </returns>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    int IComparer<LiveTemplateInfo>.Compare(LiveTemplateInfo x, LiveTemplateInfo y)
    {
      return x.Priority - y.Priority;
    }

    #endregion

    #region ITypeLoadingHandler Members

    /// <summary>
    /// Gets the attribute types.
    /// </summary>
    /// <value>The attribute types.</value>
    public Type[] AttributeTypes
    {
      get
      {
        return new[]
        {
          typeof(LiveTemplateAttribute)
        };
      }
    }

    #endregion

    /// <summary>
    /// Gets the live templates.
    /// </summary>
    /// <value>The live templates.</value>
    internal List<LiveTemplateInfo> LiveTemplateInfos
    {
      get
      {
        return this._liveTemplateInfos;
      }
    }
  }
}