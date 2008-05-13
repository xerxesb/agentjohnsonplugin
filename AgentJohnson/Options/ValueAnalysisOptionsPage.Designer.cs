namespace AgentJohnson.Options {
  partial class ValueAnalysisOptionsPage
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.AllowNullAttribute = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.Types = new System.Windows.Forms.ListBox();
      this.label4 = new System.Windows.Forms.Label();
      this.Add = new System.Windows.Forms.Button();
      this.Remove = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.ExecuteGhostDoc = new System.Windows.Forms.CheckBox();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(138, 354);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(111, 13);
      this.label3.TabIndex = 20;
      this.label3.Text = "attribute type name:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(24, 354);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(119, 13);
      this.label2.TabIndex = 19;
      this.label2.Text = "\"Parameter can be null\"";
      // 
      // AllowNullAttribute
      // 
      this.AllowNullAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.AllowNullAttribute.Location = new System.Drawing.Point(27, 370);
      this.AllowNullAttribute.Name = "AllowNullAttribute";
      this.AllowNullAttribute.Size = new System.Drawing.Size(344, 22);
      this.AllowNullAttribute.TabIndex = 21;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(9, 333);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(102, 13);
      this.label1.TabIndex = 18;
      this.label1.Text = "Code Annotations";
      // 
      // Types
      // 
      this.Types.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Types.FormattingEnabled = true;
      this.Types.Location = new System.Drawing.Point(12, 27);
      this.Types.Name = "Types";
      this.Types.Size = new System.Drawing.Size(278, 290);
      this.Types.TabIndex = 1;
      this.Types.DoubleClick += new System.EventHandler(this.Edit_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(9, 11);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(217, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Assertions and Value Analysis Attributes";
      // 
      // Add
      // 
      this.Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.Add.Location = new System.Drawing.Point(296, 27);
      this.Add.Name = "Add";
      this.Add.Size = new System.Drawing.Size(75, 23);
      this.Add.TabIndex = 16;
      this.Add.Text = "Add";
      this.Add.UseVisualStyleBackColor = true;
      this.Add.Click += new System.EventHandler(this.Add_Click);
      // 
      // Remove
      // 
      this.Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.Remove.Location = new System.Drawing.Point(296, 85);
      this.Remove.Name = "Remove";
      this.Remove.Size = new System.Drawing.Size(75, 23);
      this.Remove.TabIndex = 17;
      this.Remove.Text = "Remove";
      this.Remove.UseVisualStyleBackColor = true;
      this.Remove.Click += new System.EventHandler(this.Remove_Click);
      // 
      // label6
      // 
      this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(9, 404);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(58, 13);
      this.label6.TabIndex = 22;
      this.label6.Text = "GhostDoc";
      // 
      // ExecuteGhostDoc
      // 
      this.ExecuteGhostDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.ExecuteGhostDoc.AutoSize = true;
      this.ExecuteGhostDoc.Location = new System.Drawing.Point(27, 420);
      this.ExecuteGhostDoc.Name = "ExecuteGhostDoc";
      this.ExecuteGhostDoc.Size = new System.Drawing.Size(326, 17);
      this.ExecuteGhostDoc.TabIndex = 23;
      this.ExecuteGhostDoc.Text = "Execute GhostDoc After Applying Value Analysis Attributes";
      this.ExecuteGhostDoc.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(296, 56);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 24;
      this.button1.Text = "Edit";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.Edit_Click);
      // 
      // ValueAnalysisOptionsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.button1);
      this.Controls.Add(this.ExecuteGhostDoc);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.Remove);
      this.Controls.Add(this.Add);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.Types);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.AllowNullAttribute);
      this.Controls.Add(this.label1);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "ValueAnalysisOptionsPage";
      this.Size = new System.Drawing.Size(385, 456);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox AllowNullAttribute;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox Types;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button Add;
    private System.Windows.Forms.Button Remove;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox ExecuteGhostDoc;
    private System.Windows.Forms.Button button1;





  }
}