

namespace TemplateProcessor.Sequence
{
    partial class FormSequence
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
            dataGridViewSequence = new DataGridView();
            dataGridViewSequenceDesc = new DataGridView();
            dataGridViewStep = new DataGridView();
            dataGridViewStepDesc = new DataGridView();
            btnAddSequence = new Button();
            btnDeleteSequence = new Button();
            btnAddStep = new Button();
            btnDeleteStep = new Button();
            tbxStepNumber = new TextBox();
            dataGridViewTransition = new DataGridView();
            dataGridViewTransitionDesc = new DataGridView();
            btnAddTransition = new Button();
            btnDeleteTransition = new Button();
            tbxTransNumber = new TextBox();
            dataGridViewCondition = new DataGridView();
            dataGridViewConditionDesc = new DataGridView();
            btnAddCondition = new Button();
            btnDeleteCondition = new Button();
            tbxCondNumber = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSequence).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSequenceDesc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStepDesc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTransition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTransitionDesc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCondition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewConditionDesc).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewSequence
            // 
            dataGridViewSequence.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewSequence.Location = new Point(35, 93);
            dataGridViewSequence.Name = "dataGridViewSequence";
            dataGridViewSequence.Size = new Size(510, 207);
            dataGridViewSequence.TabIndex = 0;
            dataGridViewSequence.SelectionChanged += dataGridViewSequence_SelectionChanged;
            // 
            // dataGridViewSequenceDesc
            // 
            dataGridViewSequenceDesc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewSequenceDesc.Location = new Point(574, 93);
            dataGridViewSequenceDesc.Name = "dataGridViewSequenceDesc";
            dataGridViewSequenceDesc.Size = new Size(680, 207);
            dataGridViewSequenceDesc.TabIndex = 1;
            // 
            // dataGridViewStep
            // 
            dataGridViewStep.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStep.Location = new Point(35, 359);
            dataGridViewStep.Name = "dataGridViewStep";
            dataGridViewStep.Size = new Size(510, 204);
            dataGridViewStep.TabIndex = 2;
            dataGridViewStep.SelectionChanged += dataGridViewStep_SelectionChanged;
            // 
            // dataGridViewStepDesc
            // 
            dataGridViewStepDesc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStepDesc.Location = new Point(574, 359);
            dataGridViewStepDesc.Name = "dataGridViewStepDesc";
            dataGridViewStepDesc.Size = new Size(680, 204);
            dataGridViewStepDesc.TabIndex = 3;
            // 
            // btnAddSequence
            // 
            btnAddSequence.Location = new Point(291, 44);
            btnAddSequence.Name = "btnAddSequence";
            btnAddSequence.Size = new Size(106, 43);
            btnAddSequence.TabIndex = 4;
            btnAddSequence.Text = "Add sequence";
            btnAddSequence.UseVisualStyleBackColor = true;
            btnAddSequence.Click += btnAddSequence_Click;
            // 
            // btnDeleteSequence
            // 
            btnDeleteSequence.Location = new Point(415, 44);
            btnDeleteSequence.Name = "btnDeleteSequence";
            btnDeleteSequence.Size = new Size(109, 43);
            btnDeleteSequence.TabIndex = 5;
            btnDeleteSequence.Text = "Delete sequence";
            btnDeleteSequence.UseVisualStyleBackColor = true;
            btnDeleteSequence.Click += btnDeleteSequence_Click;
            // 
            // btnAddStep
            // 
            btnAddStep.Location = new Point(175, 312);
            btnAddStep.Name = "btnAddStep";
            btnAddStep.Size = new Size(112, 41);
            btnAddStep.TabIndex = 6;
            btnAddStep.Text = "Add step";
            btnAddStep.UseVisualStyleBackColor = true;
            btnAddStep.Click += btnAddStep_Click;
            // 
            // btnDeleteStep
            // 
            btnDeleteStep.Location = new Point(412, 312);
            btnDeleteStep.Name = "btnDeleteStep";
            btnDeleteStep.Size = new Size(112, 41);
            btnDeleteStep.TabIndex = 7;
            btnDeleteStep.Text = "Delete step";
            btnDeleteStep.UseVisualStyleBackColor = true;
            btnDeleteStep.Click += btnDeleteStep_Click;
            // 
            // tbxStepNumber
            // 
            tbxStepNumber.Location = new Point(293, 315);
            tbxStepNumber.Name = "tbxStepNumber";
            tbxStepNumber.Size = new Size(86, 23);
            tbxStepNumber.TabIndex = 8;
            // 
            // dataGridViewTransition
            // 
            dataGridViewTransition.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTransition.Location = new Point(35, 617);
            dataGridViewTransition.Name = "dataGridViewTransition";
            dataGridViewTransition.Size = new Size(504, 110);
            dataGridViewTransition.TabIndex = 9;
            dataGridViewTransition.SelectionChanged += dataGridViewTransition_SelectionChanged;
            // 
            // dataGridViewTransitionDesc
            // 
            dataGridViewTransitionDesc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTransitionDesc.Location = new Point(574, 617);
            dataGridViewTransitionDesc.Name = "dataGridViewTransitionDesc";
            dataGridViewTransitionDesc.Size = new Size(681, 110);
            dataGridViewTransitionDesc.TabIndex = 10;
            // 
            // btnAddTransition
            // 
            btnAddTransition.Location = new Point(175, 569);
            btnAddTransition.Name = "btnAddTransition";
            btnAddTransition.Size = new Size(98, 36);
            btnAddTransition.TabIndex = 11;
            btnAddTransition.Text = "Add transition";
            btnAddTransition.UseVisualStyleBackColor = true;
            btnAddTransition.Click += btnAddTransition_Click;
            // 
            // btnDeleteTransition
            // 
            btnDeleteTransition.Location = new Point(415, 575);
            btnDeleteTransition.Name = "btnDeleteTransition";
            btnDeleteTransition.Size = new Size(113, 36);
            btnDeleteTransition.TabIndex = 12;
            btnDeleteTransition.Text = "DeleteTransition";
            btnDeleteTransition.UseVisualStyleBackColor = true;
            btnDeleteTransition.Click += btnDeleteTransition_Click;
            // 
            // tbxTransNumber
            // 
            tbxTransNumber.Location = new Point(293, 575);
            tbxTransNumber.Name = "tbxTransNumber";
            tbxTransNumber.Size = new Size(86, 23);
            tbxTransNumber.TabIndex = 13;
            // 
            // dataGridViewCondition
            // 
            dataGridViewCondition.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCondition.Location = new Point(35, 769);
            dataGridViewCondition.Name = "dataGridViewCondition";
            dataGridViewCondition.Size = new Size(504, 136);
            dataGridViewCondition.TabIndex = 14;
            dataGridViewCondition.SelectionChanged += dataGridViewCondition_SelectionChanged;
            // 
            // dataGridViewConditionDesc
            // 
            dataGridViewConditionDesc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewConditionDesc.Location = new Point(575, 769);
            dataGridViewConditionDesc.Name = "dataGridViewConditionDesc";
            dataGridViewConditionDesc.Size = new Size(680, 75);
            dataGridViewConditionDesc.TabIndex = 15;
            // 
            // btnAddCondition
            // 
            btnAddCondition.Location = new Point(168, 732);
            btnAddCondition.Name = "btnAddCondition";
            btnAddCondition.Size = new Size(117, 30);
            btnAddCondition.TabIndex = 16;
            btnAddCondition.Text = "Add condition";
            btnAddCondition.UseVisualStyleBackColor = true;
            btnAddCondition.Click += btnAddCondition_Click;
            // 
            // btnDeleteCondition
            // 
            btnDeleteCondition.Location = new Point(392, 733);
            btnDeleteCondition.Name = "btnDeleteCondition";
            btnDeleteCondition.Size = new Size(121, 33);
            btnDeleteCondition.TabIndex = 17;
            btnDeleteCondition.Text = "Delete condition";
            btnDeleteCondition.UseVisualStyleBackColor = true;
            btnDeleteCondition.Click += btnDeleteCondition_Click;
            // 
            // tbxCondNumber
            // 
            tbxCondNumber.Location = new Point(295, 736);
            tbxCondNumber.Name = "tbxCondNumber";
            tbxCondNumber.Size = new Size(86, 23);
            tbxCondNumber.TabIndex = 18;
            // 
            // FormSequence
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1650, 917);
            Controls.Add(tbxCondNumber);
            Controls.Add(btnDeleteCondition);
            Controls.Add(btnAddCondition);
            Controls.Add(dataGridViewConditionDesc);
            Controls.Add(dataGridViewCondition);
            Controls.Add(tbxTransNumber);
            Controls.Add(btnDeleteTransition);
            Controls.Add(btnAddTransition);
            Controls.Add(dataGridViewTransitionDesc);
            Controls.Add(dataGridViewTransition);
            Controls.Add(tbxStepNumber);
            Controls.Add(btnDeleteStep);
            Controls.Add(btnAddStep);
            Controls.Add(btnDeleteSequence);
            Controls.Add(btnAddSequence);
            Controls.Add(dataGridViewStepDesc);
            Controls.Add(dataGridViewStep);
            Controls.Add(dataGridViewSequenceDesc);
            Controls.Add(dataGridViewSequence);
            Name = "FormSequence";
            Text = "FormSequence";
            ((System.ComponentModel.ISupportInitialize)dataGridViewSequence).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSequenceDesc).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStep).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStepDesc).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTransition).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTransitionDesc).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCondition).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewConditionDesc).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridViewSequence;
        private DataGridView dataGridViewSequenceDesc;
        private DataGridView dataGridViewStep;
        private DataGridView dataGridViewStepDesc;
        private Button btnAddSequence;
        private Button btnDeleteSequence;
        private Button btnAddStep;
        private Button btnDeleteStep;
        private TextBox tbxStepNumber;
        private DataGridView dataGridViewTransition;
        private DataGridView dataGridViewTransitionDesc;
        private Button btnAddTransition;
        private Button btnDeleteTransition;
        private TextBox tbxTransNumber;
        private DataGridView dataGridViewCondition;
        private DataGridView dataGridViewConditionDesc;
        private Button btnAddCondition;
        private Button btnDeleteCondition;
        private TextBox tbxCondNumber;
    }
}