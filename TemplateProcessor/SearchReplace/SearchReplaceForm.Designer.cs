namespace TemplateProcessor.SearchReplace
{
    partial class SearchReplaceForm
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
            cbxActiveObjectType = new ComboBox();
            btnPerformSearchReplace = new Button();
            cbxActiveTask = new ComboBox();
            SuspendLayout();
            // 
            // cbxActiveObjectType
            // 
            cbxActiveObjectType.FormattingEnabled = true;
            cbxActiveObjectType.Location = new Point(19, 64);
            cbxActiveObjectType.Name = "cbxActiveObjectType";
            cbxActiveObjectType.Size = new Size(279, 23);
            cbxActiveObjectType.TabIndex = 3;
            cbxActiveObjectType.SelectedIndexChanged += cbxActiveObjectType_SelectedIndexChanged;
            // 
            // btnPerformSearchReplace
            // 
            btnPerformSearchReplace.Location = new Point(328, 61);
            btnPerformSearchReplace.Name = "btnPerformSearchReplace";
            btnPerformSearchReplace.Size = new Size(164, 36);
            btnPerformSearchReplace.TabIndex = 4;
            btnPerformSearchReplace.Text = "Perform Replace";
            btnPerformSearchReplace.UseVisualStyleBackColor = true;
            btnPerformSearchReplace.Click += btnPerformSearchReplace_Click;
            // 
            // cbxActiveTask
            // 
            cbxActiveTask.FormattingEnabled = true;
            cbxActiveTask.Location = new Point(85, 12);
            cbxActiveTask.Name = "cbxActiveTask";
            cbxActiveTask.Size = new Size(213, 23);
            cbxActiveTask.TabIndex = 6;
            cbxActiveTask.SelectedIndexChanged += cbxActiveTask_SelectedIndexChanged;
            // 
            // SearchReplaceForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cbxActiveTask);
            Controls.Add(btnPerformSearchReplace);
            Controls.Add(cbxActiveObjectType);
            Name = "SearchReplaceForm";
            Text = "SearchReplaceForm";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cbxActiveObjectType;
        private Button btnPerformSearchReplace;
        private ComboBox cbxActiveTask;
    }
}