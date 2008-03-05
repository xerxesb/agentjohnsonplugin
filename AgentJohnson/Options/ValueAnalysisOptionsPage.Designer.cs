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
      this.PrivateMethods = new System.Windows.Forms.CheckBox();
      this.ProtectedMethods = new System.Windows.Forms.CheckBox();
      this.InternalMethods = new System.Windows.Forms.CheckBox();
      this.PublicMethods = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.typeAssertionsGridView = new System.Windows.Forms.DataGridView();
      this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.AssertionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label6 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.AllowNullAttribute = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.typeAssertionsGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // PrivateMethods
      // 
      this.PrivateMethods.AutoSize = true;
      this.PrivateMethods.Checked = true;
      this.PrivateMethods.CheckState = System.Windows.Forms.CheckState.Checked;
      this.PrivateMethods.Location = new System.Drawing.Point(22, 287);
      this.PrivateMethods.Name = "PrivateMethods";
      this.PrivateMethods.Size = new System.Drawing.Size(60, 17);
      this.PrivateMethods.TabIndex = 11;
      this.PrivateMethods.Text = "Private";
      this.PrivateMethods.UseVisualStyleBackColor = true;
      // 
      // ProtectedMethods
      // 
      this.ProtectedMethods.AutoSize = true;
      this.ProtectedMethods.Checked = true;
      this.ProtectedMethods.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ProtectedMethods.Location = new System.Drawing.Point(22, 264);
      this.ProtectedMethods.Name = "ProtectedMethods";
      this.ProtectedMethods.Size = new System.Drawing.Size(75, 17);
      this.ProtectedMethods.TabIndex = 10;
      this.ProtectedMethods.Text = "Protected";
      this.ProtectedMethods.UseVisualStyleBackColor = true;
      // 
      // InternalMethods
      // 
      this.InternalMethods.AutoSize = true;
      this.InternalMethods.Checked = true;
      this.InternalMethods.CheckState = System.Windows.Forms.CheckState.Checked;
      this.InternalMethods.Location = new System.Drawing.Point(22, 241);
      this.InternalMethods.Name = "InternalMethods";
      this.InternalMethods.Size = new System.Drawing.Size(66, 17);
      this.InternalMethods.TabIndex = 9;
      this.InternalMethods.Text = "Internal";
      this.InternalMethods.UseVisualStyleBackColor = true;
      // 
      // PublicMethods
      // 
      this.PublicMethods.AutoSize = true;
      this.PublicMethods.Checked = true;
      this.PublicMethods.CheckState = System.Windows.Forms.CheckState.Checked;
      this.PublicMethods.Location = new System.Drawing.Point(22, 218);
      this.PublicMethods.Name = "PublicMethods";
      this.PublicMethods.Size = new System.Drawing.Size(57, 17);
      this.PublicMethods.TabIndex = 8;
      this.PublicMethods.Text = "Public";
      this.PublicMethods.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(3, 170);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(101, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Member Visibillity";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(3, 193);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(258, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "Check only members with the following visibility:";
      // 
      // typeAssertionsGridView
      // 
      this.typeAssertionsGridView.AllowUserToResizeRows = false;
      this.typeAssertionsGridView.BackgroundColor = System.Drawing.SystemColors.Window;
      this.typeAssertionsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.typeAssertionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.typeAssertionsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                                                                                                   this.TypeColumn,
                                                                                                   this.AssertionColumn});
      this.typeAssertionsGridView.GridColor = System.Drawing.SystemColors.ControlLight;
      this.typeAssertionsGridView.Location = new System.Drawing.Point(6, 27);
      this.typeAssertionsGridView.Margin = new System.Windows.Forms.Padding(8);
      this.typeAssertionsGridView.MultiSelect = false;
      this.typeAssertionsGridView.Name = "typeAssertionsGridView";
      this.typeAssertionsGridView.RowHeadersVisible = false;
      this.typeAssertionsGridView.ShowEditingIcon = false;
      this.typeAssertionsGridView.Size = new System.Drawing.Size(412, 124);
      this.typeAssertionsGridView.TabIndex = 14;
      // 
      // TypeColumn
      // 
      this.TypeColumn.DataPropertyName = "type";
      this.TypeColumn.HeaderText = "Type";
      this.TypeColumn.Name = "TypeColumn";
      // 
      // AssertionColumn
      // 
      this.AssertionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.AssertionColumn.DataPropertyName = "assertion";
      this.AssertionColumn.HeaderText = "Assertion";
      this.AssertionColumn.Name = "AssertionColumn";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(3, 10);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(150, 13);
      this.label6.TabIndex = 15;
      this.label6.Text = "Assertion Methods by Type";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(132, 343);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(111, 13);
      this.label3.TabIndex = 20;
      this.label3.Text = "attribute type name:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(18, 343);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(119, 13);
      this.label2.TabIndex = 19;
      this.label2.Text = "\"Parameter can be null\"";
      // 
      // AllowNullAttribute
      // 
      this.AllowNullAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                             | System.Windows.Forms.AnchorStyles.Right)));
      this.AllowNullAttribute.Location = new System.Drawing.Point(21, 359);
      this.AllowNullAttribute.Name = "AllowNullAttribute";
      this.AllowNullAttribute.Size = new System.Drawing.Size(387, 22);
      this.AllowNullAttribute.TabIndex = 18;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 322);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(102, 13);
      this.label1.TabIndex = 17;
      this.label1.Text = "Code Annotations";
      // 
      // OptionsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.AllowNullAttribute);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.typeAssertionsGridView);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.PrivateMethods);
      this.Controls.Add(this.ProtectedMethods);
      this.Controls.Add(this.InternalMethods);
      this.Controls.Add(this.PublicMethods);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "OptionsPage";
      this.Size = new System.Drawing.Size(475, 572);
      ((System.ComponentModel.ISupportInitialize)(this.typeAssertionsGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox PrivateMethods;
    private System.Windows.Forms.CheckBox ProtectedMethods;
    private System.Windows.Forms.CheckBox InternalMethods;
    private System.Windows.Forms.CheckBox PublicMethods;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.DataGridView typeAssertionsGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn AssertionColumn;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox AllowNullAttribute;
    private System.Windows.Forms.Label label1;





  }
}