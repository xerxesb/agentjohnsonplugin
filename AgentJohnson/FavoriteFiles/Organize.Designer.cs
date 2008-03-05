namespace AgentJohnson.FavoriteFiles {
  partial class Organize {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.Listbox = new System.Windows.Forms.ListBox();
      this.OKButton = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.UpButton = new System.Windows.Forms.Button();
      this.DownButton = new System.Windows.Forms.Button();
      this.DeleteButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // Listbox
      // 
      this.Listbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                   | System.Windows.Forms.AnchorStyles.Left)
                                                                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Listbox.FormattingEnabled = true;
      this.Listbox.IntegralHeight = false;
      this.Listbox.Location = new System.Drawing.Point(12, 12);
      this.Listbox.Name = "Listbox";
      this.Listbox.Size = new System.Drawing.Size(320, 186);
      this.Listbox.TabIndex = 0;
      this.Listbox.SelectedIndexChanged += new System.EventHandler(this.Listbox_SelectedIndexChanged);
      // 
      // OKButton
      // 
      this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OKButton.Location = new System.Drawing.Point(257, 204);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(75, 23);
      this.OKButton.TabIndex = 1;
      this.OKButton.Text = "OK";
      this.OKButton.UseVisualStyleBackColor = true;
      this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button2.Location = new System.Drawing.Point(338, 204);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 2;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // UpButton
      // 
      this.UpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.UpButton.Location = new System.Drawing.Point(338, 12);
      this.UpButton.Name = "UpButton";
      this.UpButton.Size = new System.Drawing.Size(75, 23);
      this.UpButton.TabIndex = 3;
      this.UpButton.Text = "Up";
      this.UpButton.UseVisualStyleBackColor = true;
      this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
      // 
      // DownButton
      // 
      this.DownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DownButton.Location = new System.Drawing.Point(338, 41);
      this.DownButton.Name = "DownButton";
      this.DownButton.Size = new System.Drawing.Size(75, 23);
      this.DownButton.TabIndex = 4;
      this.DownButton.Text = "Down";
      this.DownButton.UseVisualStyleBackColor = true;
      this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
      // 
      // DeleteButton
      // 
      this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.DeleteButton.Location = new System.Drawing.Point(338, 80);
      this.DeleteButton.Name = "DeleteButton";
      this.DeleteButton.Size = new System.Drawing.Size(75, 23);
      this.DeleteButton.TabIndex = 5;
      this.DeleteButton.Text = "Delete";
      this.DeleteButton.UseVisualStyleBackColor = true;
      this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
      // 
      // Organize
      // 
      this.AcceptButton = this.OKButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.button2;
      this.ClientSize = new System.Drawing.Size(425, 237);
      this.Controls.Add(this.DeleteButton);
      this.Controls.Add(this.DownButton);
      this.Controls.Add(this.UpButton);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.OKButton);
      this.Controls.Add(this.Listbox);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Organize";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Organize Favorite Files";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox Listbox;
    private System.Windows.Forms.Button OKButton;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button UpButton;
    private System.Windows.Forms.Button DownButton;
    private System.Windows.Forms.Button DeleteButton;
  }
}