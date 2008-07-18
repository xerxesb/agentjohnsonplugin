namespace AgentJohnson.Options {
  partial class SmartGenerateOptionsPage {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.Handlers = new System.Windows.Forms.CheckedListBox();
      this.ActionName = new System.Windows.Forms.Label();
      this.ActionDescription = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // OpenFileDialog
      // 
      this.OpenFileDialog.DefaultExt = "xml";
      this.OpenFileDialog.Filter = "Xml Files|*.xml|All files|*.*";
      this.OpenFileDialog.Title = "Import Agent Johnson settings";
      // 
      // SaveFileDialog
      // 
      this.SaveFileDialog.DefaultExt = "xml";
      this.SaveFileDialog.Filter = "Xml Files|*.xml|All files|*.*";
      this.SaveFileDialog.Title = "Export Agent Johnson Settings";
      // 
      // Handlers
      // 
      this.Handlers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Handlers.CheckOnClick = true;
      this.Handlers.FormattingEnabled = true;
      this.Handlers.Location = new System.Drawing.Point(6, 18);
      this.Handlers.Name = "Handlers";
      this.Handlers.Size = new System.Drawing.Size(407, 274);
      this.Handlers.TabIndex = 0;
      this.Handlers.SelectedIndexChanged += new System.EventHandler(this.Handlers_SelectedIndexChanged);
      // 
      // ActionName
      // 
      this.ActionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ActionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ActionName.Location = new System.Drawing.Point(3, 305);
      this.ActionName.Name = "ActionName";
      this.ActionName.Size = new System.Drawing.Size(443, 22);
      this.ActionName.TabIndex = 1;
      this.ActionName.Text = "Action";
      // 
      // ActionDescription
      // 
      this.ActionDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ActionDescription.Location = new System.Drawing.Point(3, 327);
      this.ActionDescription.Name = "ActionDescription";
      this.ActionDescription.Size = new System.Drawing.Size(446, 121);
      this.ActionDescription.TabIndex = 2;
      this.ActionDescription.Text = "Description";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(446, 15);
      this.label1.TabIndex = 3;
      this.label1.Text = "The selected actions will be available for use in code files:";
      // 
      // SmartGenerateOptionsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ActionDescription);
      this.Controls.Add(this.ActionName);
      this.Controls.Add(this.Handlers);
      this.Name = "SmartGenerateOptionsPage";
      this.Size = new System.Drawing.Size(502, 460);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    private System.Windows.Forms.SaveFileDialog SaveFileDialog;
    private System.Windows.Forms.CheckedListBox Handlers;
    private System.Windows.Forms.Label ActionName;
    private System.Windows.Forms.Label ActionDescription;
    private System.Windows.Forms.Label label1;
  }
}
