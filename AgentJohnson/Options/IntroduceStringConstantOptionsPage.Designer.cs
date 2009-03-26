namespace AgentJohnson.Options {
  partial class IntroduceStringConstantOptionsPage {
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.methodsGridView = new System.Windows.Forms.DataGridView();
      this.ClassNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.rbReplaceWithNothing = new System.Windows.Forms.RadioButton();
      this.rbReplaceWithUnderscore = new System.Windows.Forms.RadioButton();
      this.cbGenerateXmlComment = new System.Windows.Forms.CheckBox();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.methodsGridView)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.methodsGridView);
      this.groupBox2.Location = new System.Drawing.Point(6, 6);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
      this.groupBox2.Size = new System.Drawing.Size(370, 154);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Fully Qualified Class Names for Storing Strings";
      // 
      // methodsGridView
      // 
      this.methodsGridView.AllowUserToResizeRows = false;
      this.methodsGridView.BackgroundColor = System.Drawing.SystemColors.Window;
      this.methodsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.methodsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.methodsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClassNameColumn});
      this.methodsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.methodsGridView.GridColor = System.Drawing.SystemColors.ControlLight;
      this.methodsGridView.Location = new System.Drawing.Point(6, 19);
      this.methodsGridView.Margin = new System.Windows.Forms.Padding(8);
      this.methodsGridView.MultiSelect = false;
      this.methodsGridView.Name = "methodsGridView";
      this.methodsGridView.RowHeadersVisible = false;
      this.methodsGridView.ShowEditingIcon = false;
      this.methodsGridView.Size = new System.Drawing.Size(358, 129);
      this.methodsGridView.TabIndex = 0;
      // 
      // ClassNameColumn
      // 
      this.ClassNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.ClassNameColumn.DataPropertyName = "className";
      this.ClassNameColumn.HeaderText = "Class Name";
      this.ClassNameColumn.Name = "ClassNameColumn";
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.groupBox3);
      this.groupBox1.Location = new System.Drawing.Point(6, 168);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
      this.groupBox1.Size = new System.Drawing.Size(370, 95);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Identifier Naming";
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Controls.Add(this.rbReplaceWithNothing);
      this.groupBox3.Controls.Add(this.rbReplaceWithUnderscore);
      this.groupBox3.Location = new System.Drawing.Point(9, 22);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(352, 65);
      this.groupBox3.TabIndex = 0;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Spaces and Invalid Characters:";
      // 
      // rbReplaceWithNothing
      // 
      this.rbReplaceWithNothing.AutoSize = true;
      this.rbReplaceWithNothing.Location = new System.Drawing.Point(7, 42);
      this.rbReplaceWithNothing.Name = "rbReplaceWithNothing";
      this.rbReplaceWithNothing.Size = new System.Drawing.Size(65, 17);
      this.rbReplaceWithNothing.TabIndex = 1;
      this.rbReplaceWithNothing.Text = "Remove";
      this.rbReplaceWithNothing.UseVisualStyleBackColor = true;
      // 
      // rbReplaceWithUnderscore
      // 
      this.rbReplaceWithUnderscore.AutoSize = true;
      this.rbReplaceWithUnderscore.Location = new System.Drawing.Point(7, 19);
      this.rbReplaceWithUnderscore.Name = "rbReplaceWithUnderscore";
      this.rbReplaceWithUnderscore.Size = new System.Drawing.Size(150, 17);
      this.rbReplaceWithUnderscore.TabIndex = 0;
      this.rbReplaceWithUnderscore.Text = "Replace with Underscores";
      this.rbReplaceWithUnderscore.UseVisualStyleBackColor = true;
      // 
      // cbGenerateXmlComment
      // 
      this.cbGenerateXmlComment.AutoSize = true;
      this.cbGenerateXmlComment.Location = new System.Drawing.Point(22, 269);
      this.cbGenerateXmlComment.Name = "cbGenerateXmlComment";
      this.cbGenerateXmlComment.Size = new System.Drawing.Size(142, 17);
      this.cbGenerateXmlComment.TabIndex = 2;
      this.cbGenerateXmlComment.Text = "Generate XML Comment";
      this.cbGenerateXmlComment.UseVisualStyleBackColor = true;
      // 
      // IntroduceStringConstantOptionsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.cbGenerateXmlComment);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBox2);
      this.Name = "IntroduceStringConstantOptionsPage";
      this.Size = new System.Drawing.Size(407, 399);
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.methodsGridView)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.DataGridView methodsGridView;
    private System.Windows.Forms.DataGridViewTextBoxColumn ClassNameColumn;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.RadioButton rbReplaceWithNothing;
    private System.Windows.Forms.RadioButton rbReplaceWithUnderscore;
    private System.Windows.Forms.CheckBox cbGenerateXmlComment;





  }
}