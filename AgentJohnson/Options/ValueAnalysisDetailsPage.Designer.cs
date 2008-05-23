namespace AgentJohnson.Options {
  partial class ValueAnalysisDetailsPage {
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
      this.TypeName = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.CanBeNull = new System.Windows.Forms.CheckBox();
      this.NotNull = new System.Windows.Forms.CheckBox();
      this.ValueAssertions = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.label10 = new System.Windows.Forms.Label();
      this.ReturnAssertion = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.NonPublicParameterAssertion = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.PublicParameterAssertion = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.ValueAssertions)).BeginInit();
      this.SuspendLayout();
      // 
      // TypeName
      // 
      this.TypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TypeName.Location = new System.Drawing.Point(35, 23);
      this.TypeName.Name = "TypeName";
      this.TypeName.Size = new System.Drawing.Size(389, 22);
      this.TypeName.TabIndex = 17;
      this.TypeName.TextChanged += new System.EventHandler(this.TypeName_TextChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(12, 7);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(116, 13);
      this.label5.TabIndex = 16;
      this.label5.Text = "Qualified Type Name";
      // 
      // CanBeNull
      // 
      this.CanBeNull.AutoSize = true;
      this.CanBeNull.Location = new System.Drawing.Point(35, 112);
      this.CanBeNull.Name = "CanBeNull";
      this.CanBeNull.Size = new System.Drawing.Size(86, 17);
      this.CanBeNull.TabIndex = 20;
      this.CanBeNull.Text = "Can Be Null";
      this.CanBeNull.UseVisualStyleBackColor = true;
      this.CanBeNull.CheckedChanged += new System.EventHandler(this.CanBeNull_CheckedChanged);
      // 
      // NotNull
      // 
      this.NotNull.AutoSize = true;
      this.NotNull.Location = new System.Drawing.Point(35, 89);
      this.NotNull.Name = "NotNull";
      this.NotNull.Size = new System.Drawing.Size(69, 17);
      this.NotNull.TabIndex = 19;
      this.NotNull.Text = "Not Null";
      this.NotNull.UseVisualStyleBackColor = true;
      this.NotNull.CheckedChanged += new System.EventHandler(this.NotNull_CheckedChanged);
      // 
      // ValueAssertions
      // 
      this.ValueAssertions.AllowUserToResizeRows = false;
      this.ValueAssertions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ValueAssertions.BackgroundColor = System.Drawing.SystemColors.Window;
      this.ValueAssertions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.ValueAssertions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.ValueAssertions.ColumnHeadersVisible = false;
      this.ValueAssertions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
      this.ValueAssertions.GridColor = System.Drawing.SystemColors.ControlLight;
      this.ValueAssertions.Location = new System.Drawing.Point(35, 313);
      this.ValueAssertions.Margin = new System.Windows.Forms.Padding(8);
      this.ValueAssertions.MultiSelect = false;
      this.ValueAssertions.Name = "ValueAssertions";
      this.ValueAssertions.RowHeadersVisible = false;
      this.ValueAssertions.ShowEditingIcon = false;
      this.ValueAssertions.Size = new System.Drawing.Size(389, 125);
      this.ValueAssertions.TabIndex = 27;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.dataGridViewTextBoxColumn2.DataPropertyName = "assertion";
      this.dataGridViewTextBoxColumn2.HeaderText = "Assertion";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(12, 69);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(173, 13);
      this.label10.TabIndex = 18;
      this.label10.Text = "Default Value Analysis Attribute";
      // 
      // ReturnAssertion
      // 
      this.ReturnAssertion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ReturnAssertion.Location = new System.Drawing.Point(35, 462);
      this.ReturnAssertion.Name = "ReturnAssertion";
      this.ReturnAssertion.Size = new System.Drawing.Size(389, 22);
      this.ReturnAssertion.TabIndex = 29;
      // 
      // label12
      // 
      this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.Location = new System.Drawing.Point(12, 446);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(94, 13);
      this.label12.TabIndex = 28;
      this.label12.Text = "Return Assertion";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(12, 292);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(126, 13);
      this.label11.TabIndex = 26;
      this.label11.Text = "Assignment Assertions";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(32, 251);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(118, 13);
      this.label9.TabIndex = 24;
      this.label9.Text = "Non-public Members:";
      // 
      // NonPublicParameterAssertion
      // 
      this.NonPublicParameterAssertion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.NonPublicParameterAssertion.Location = new System.Drawing.Point(35, 267);
      this.NonPublicParameterAssertion.Name = "NonPublicParameterAssertion";
      this.NonPublicParameterAssertion.Size = new System.Drawing.Size(389, 22);
      this.NonPublicParameterAssertion.TabIndex = 25;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(32, 208);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(91, 13);
      this.label8.TabIndex = 22;
      this.label8.Text = "Public Members:";
      // 
      // PublicParameterAssertion
      // 
      this.PublicParameterAssertion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.PublicParameterAssertion.Location = new System.Drawing.Point(35, 226);
      this.PublicParameterAssertion.Name = "PublicParameterAssertion";
      this.PublicParameterAssertion.Size = new System.Drawing.Size(389, 22);
      this.PublicParameterAssertion.TabIndex = 23;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(12, 187);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(117, 13);
      this.label7.TabIndex = 21;
      this.label7.Text = "Parameter Assertions";
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Location = new System.Drawing.Point(268, 488);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 30;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.OK_Click);
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button2.Location = new System.Drawing.Point(349, 488);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 31;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(32, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(223, 13);
      this.label1.TabIndex = 32;
      this.label1.Text = "E.g. \"System.String\". Use \"*\" to match all types.";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(12, 137);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 13);
      this.label2.TabIndex = 33;
      this.label2.Text = "Assertions";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(32, 150);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(392, 35);
      this.label3.TabIndex = 34;
      this.label3.Text = "Assertions are specified as statements, including the ending semi-colon. Use \"{0}" +
          "\" to specify the name of the member being asserted.";
      // 
      // ValueAnalysisDetailsPage
      // 
      this.AcceptButton = this.button1;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.button2;
      this.ClientSize = new System.Drawing.Size(436, 523);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.TypeName);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.CanBeNull);
      this.Controls.Add(this.NotNull);
      this.Controls.Add(this.ValueAssertions);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.ReturnAssertion);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.NonPublicParameterAssertion);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.PublicParameterAssertion);
      this.Controls.Add(this.label7);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ValueAnalysisDetailsPage";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ValueAnalysisDetailsPage";
      ((System.ComponentModel.ISupportInitialize)(this.ValueAssertions)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox TypeName;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox CanBeNull;
    private System.Windows.Forms.CheckBox NotNull;
    private System.Windows.Forms.DataGridView ValueAssertions;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox ReturnAssertion;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox NonPublicParameterAssertion;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox PublicParameterAssertion;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
  }
}