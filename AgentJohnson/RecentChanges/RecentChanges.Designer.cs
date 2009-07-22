namespace AgentJohnson.RecentChanges
{
  partial class RecentChanges
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.Listbox = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.Preview = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // Listbox
      // 
      this.Listbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Listbox.FormattingEnabled = true;
      this.Listbox.IntegralHeight = false;
      this.Listbox.Location = new System.Drawing.Point(16, 29);
      this.Listbox.Name = "Listbox";
      this.Listbox.Size = new System.Drawing.Size(617, 234);
      this.Listbox.TabIndex = 0;
      this.Listbox.SelectedIndexChanged += new System.EventHandler(this.Listbox_SelectedIndexChanged);
      this.Listbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeOnKeyDown);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Recent Edits:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 277);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Preview:";
      // 
      // Preview
      // 
      this.Preview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Preview.BackColor = System.Drawing.SystemColors.Window;
      this.Preview.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Preview.HideSelection = false;
      this.Preview.Location = new System.Drawing.Point(16, 294);
      this.Preview.Multiline = true;
      this.Preview.Name = "Preview";
      this.Preview.ReadOnly = true;
      this.Preview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.Preview.Size = new System.Drawing.Size(617, 167);
      this.Preview.TabIndex = 3;
      this.Preview.WordWrap = false;
      this.Preview.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CodeOnKeyDown);
      // 
      // RecentChanges
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.ClientSize = new System.Drawing.Size(645, 473);
      this.Controls.Add(this.Preview);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.Listbox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "RecentChanges";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "RecentChanges";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox Listbox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox Preview;

  }
}