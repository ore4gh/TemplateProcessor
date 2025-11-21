namespace TemplateProcessor.Sequence
{
    partial class FormTextListGenerator
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
            labelTitle = new Label();
            labelSelectSheet = new Label();
            comboBoxSheets = new ComboBox();
            btnGenerateText = new Button();
            btnReloadSheets = new Button();
            groupBoxSheet = new GroupBox();
            groupBoxActions = new GroupBox();
            labelInstructions = new Label();
            groupBoxSheet.SuspendLayout();
            groupBoxActions.SuspendLayout();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            labelTitle.Location = new Point(20, 20);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(191, 25);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "TextList Generator";
            // 
            // labelSelectSheet
            // 
            labelSelectSheet.AutoSize = true;
            labelSelectSheet.Location = new Point(15, 30);
            labelSelectSheet.Name = "labelSelectSheet";
            labelSelectSheet.Size = new Size(169, 15);
            labelSelectSheet.TabIndex = 1;
            labelSelectSheet.Text = "Select Sheet (TextEq_Number):";
            // 
            // comboBoxSheets
            // 
            comboBoxSheets.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSheets.FormattingEnabled = true;
            comboBoxSheets.Location = new Point(15, 55);
            comboBoxSheets.Name = "comboBoxSheets";
            comboBoxSheets.Size = new Size(400, 23);
            comboBoxSheets.TabIndex = 2;
            // 
            // btnGenerateText
            // 
            btnGenerateText.BackColor = Color.FromArgb(0, 120, 215);
            btnGenerateText.FlatStyle = FlatStyle.Flat;
            btnGenerateText.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGenerateText.ForeColor = Color.White;
            btnGenerateText.Location = new Point(15, 30);
            btnGenerateText.Name = "btnGenerateText";
            btnGenerateText.Size = new Size(200, 50);
            btnGenerateText.TabIndex = 3;
            btnGenerateText.Text = "Generate Text";
            btnGenerateText.UseVisualStyleBackColor = false;
            btnGenerateText.Click += btnGenerateText_Click;
            // 
            // btnReloadSheets
            // 
            btnReloadSheets.Location = new Point(430, 55);
            btnReloadSheets.Name = "btnReloadSheets";
            btnReloadSheets.Size = new Size(100, 23);
            btnReloadSheets.TabIndex = 4;
            btnReloadSheets.Text = "Reload Sheets";
            btnReloadSheets.UseVisualStyleBackColor = true;
            btnReloadSheets.Click += btnReloadSheets_Click;
            // 
            // groupBoxSheet
            // 
            groupBoxSheet.Controls.Add(labelSelectSheet);
            groupBoxSheet.Controls.Add(btnReloadSheets);
            groupBoxSheet.Controls.Add(comboBoxSheets);
            groupBoxSheet.Location = new Point(20, 60);
            groupBoxSheet.Name = "groupBoxSheet";
            groupBoxSheet.Size = new Size(550, 100);
            groupBoxSheet.TabIndex = 5;
            groupBoxSheet.TabStop = false;
            groupBoxSheet.Text = "Sheet Selection";
            // 
            // groupBoxActions
            // 
            groupBoxActions.Controls.Add(btnGenerateText);
            groupBoxActions.Location = new Point(20, 170);
            groupBoxActions.Name = "groupBoxActions";
            groupBoxActions.Size = new Size(550, 100);
            groupBoxActions.TabIndex = 6;
            groupBoxActions.TabStop = false;
            groupBoxActions.Text = "Actions";
            // 
            // labelInstructions
            // 
            labelInstructions.AutoSize = true;
            labelInstructions.ForeColor = Color.DarkGray;
            labelInstructions.Location = new Point(20, 285);
            labelInstructions.Name = "labelInstructions";
            labelInstructions.Size = new Size(520, 75);
            labelInstructions.TabIndex = 7;
            labelInstructions.Text = "Instructions:\n" +
                "1. Select a sheet from the dropdown (sheets starting with TextEq_)\n" +
                "2. Click 'Generate Text' to process the template and update database\n" +
                "3. Output file will be created in TextEq\\Output\\v02\\ folder\n" +
                "4. Database TextLists table will be updated";
            // 
            // FormTextListGenerator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 380);
            Controls.Add(labelInstructions);
            Controls.Add(groupBoxActions);
            Controls.Add(groupBoxSheet);
            Controls.Add(labelTitle);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormTextListGenerator";
            Text = "TextList Generator";
            groupBoxSheet.ResumeLayout(false);
            groupBoxSheet.PerformLayout();
            groupBoxActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelTitle;
        private Label labelSelectSheet;
        private ComboBox comboBoxSheets;
        private Button btnGenerateText;
        private Button btnReloadSheets;
        private GroupBox groupBoxSheet;
        private GroupBox groupBoxActions;
        private Label labelInstructions;
    }
}
