namespace TemplateProcessor.Generator
{
    partial class FormGenerator
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
            components = new System.ComponentModel.Container();
            cbxActiveObjectType = new ComboBox();
            btnGenerate = new Button();
            tasksBindingSource = new BindingSource(components);
            generatorDataBindingSource = new BindingSource(components);
            cbxActiveTask = new ComboBox();
            textBox_StartRow = new TextBox();
            textBox_EndRow = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)tasksBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)generatorDataBindingSource).BeginInit();
            SuspendLayout();
            // 
            // cbxActiveObjectType
            // 
            cbxActiveObjectType.FormattingEnabled = true;
            cbxActiveObjectType.Location = new Point(12, 74);
            cbxActiveObjectType.Name = "cbxActiveObjectType";
            cbxActiveObjectType.Size = new Size(279, 23);
            cbxActiveObjectType.TabIndex = 3;
            cbxActiveObjectType.SelectedIndexChanged += cbxActiveObjectType_SelectedIndexChanged;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(549, 54);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(154, 43);
            btnGenerate.TabIndex = 4;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // tasksBindingSource
            // 
            tasksBindingSource.DataMember = "Tasks";
            tasksBindingSource.DataSource = generatorDataBindingSource;
            // 
            // generatorDataBindingSource
            // 
            generatorDataBindingSource.DataSource = typeof(GeneratorData);
            // 
            // cbxActiveTask
            // 
            cbxActiveTask.FormattingEnabled = true;
            cbxActiveTask.Location = new Point(78, 27);
            cbxActiveTask.Name = "cbxActiveTask";
            cbxActiveTask.Size = new Size(213, 23);
            cbxActiveTask.TabIndex = 5;
            cbxActiveTask.SelectedIndexChanged += cbxActiveTask_SelectedIndexChanged;
            // 
            // textBox_StartRow
            // 
            textBox_StartRow.Location = new Point(410, 74);
            textBox_StartRow.Name = "textBox_StartRow";
            textBox_StartRow.Size = new Size(53, 23);
            textBox_StartRow.TabIndex = 6;
            textBox_StartRow.Leave += textBox_StartRow_Leave;
            // 
            // textBox_EndRow
            // 
            textBox_EndRow.Location = new Point(469, 74);
            textBox_EndRow.Name = "textBox_EndRow";
            textBox_EndRow.Size = new Size(57, 23);
            textBox_EndRow.TabIndex = 7;
            textBox_EndRow.Leave += textBox_EndRow_Leave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(369, 77);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 8;
            label1.Text = "Rows";
            // 
            // FormGenerator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1672, 978);
            Controls.Add(label1);
            Controls.Add(textBox_EndRow);
            Controls.Add(textBox_StartRow);
            Controls.Add(cbxActiveTask);
            Controls.Add(btnGenerate);
            Controls.Add(cbxActiveObjectType);
            Name = "FormGenerator";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormGenerator";
            ((System.ComponentModel.ISupportInitialize)tasksBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)generatorDataBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbxActiveObjectType;
        private Button btnGenerate;
        private BindingSource generatorDataBindingSource;
        private BindingSource tasksBindingSource;
        private ComboBox cbxActiveTask;
        private TextBox textBox_StartRow;
        private TextBox textBox_EndRow;
        private Label label1;
    }
}