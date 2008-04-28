namespace AgentJohnson.Options {
  partial class ImportExportPage {
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
      this.Import = new System.Windows.Forms.Button();
      this.Export = new System.Windows.Forms.Button();
      this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.SuspendLayout();
      // 
      // Import
      // 
      this.Import.Location = new System.Drawing.Point(3, 3);
      this.Import.Name = "Import";
      this.Import.Size = new System.Drawing.Size(75, 23);
      this.Import.TabIndex = 0;
      this.Import.Text = "Import...";
      this.Import.UseVisualStyleBackColor = true;
      this.Import.Click += new System.EventHandler(this.Import_Click);
      // 
      // Export
      // 
      this.Export.Location = new System.Drawing.Point(84, 3);
      this.Export.Name = "Export";
      this.Export.Size = new System.Drawing.Size(75, 23);
      this.Export.TabIndex = 1;
      this.Export.Text = "Export...";
      this.Export.UseVisualStyleBackColor = true;
      this.Export.Click += new System.EventHandler(this.Export_Click);
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
      // ImportExportPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.Export);
      this.Controls.Add(this.Import);
      this.Name = "ImportExportPage";
      this.Size = new System.Drawing.Size(502, 460);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button Import;
    private System.Windows.Forms.Button Export;
    private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    private System.Windows.Forms.SaveFileDialog SaveFileDialog;
  }
}
