using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TemplateProcessor
{
    partial class Form1
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
            btnGenerate = new Button();
            cbxActiveObjectType = new ComboBox();
            dataGridViewObjTypes = new DataGridView();
            dataGridViewTemplates = new DataGridView();
            dataGridViewCommands = new DataGridView();
            btnSaveToJson = new Button();
            btnSequenceTexts = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            btnCreateDirectories = new Button();
            btnRenameDirectory = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewObjTypes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTemplates).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCommands).BeginInit();
            SuspendLayout();
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(417, 602);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(159, 47);
            btnGenerate.TabIndex = 1;
            btnGenerate.Text = "Generate";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // cbxActiveObjectType
            // 
            cbxActiveObjectType.FormattingEnabled = true;
            cbxActiveObjectType.Location = new Point(12, 12);
            cbxActiveObjectType.Name = "cbxActiveObjectType";
            cbxActiveObjectType.Size = new Size(279, 23);
            cbxActiveObjectType.TabIndex = 2;
            cbxActiveObjectType.SelectedIndexChanged += cbxActiveObjectType_SelectedIndexChanged;
            // 
            // dataGridViewObjTypes
            // 
            dataGridViewObjTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewObjTypes.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridViewObjTypes.Location = new Point(12, 68);
            dataGridViewObjTypes.Name = "dataGridViewObjTypes";
            dataGridViewObjTypes.Size = new Size(564, 101);
            dataGridViewObjTypes.TabIndex = 3;
            dataGridViewObjTypes.SelectionChanged += dataGridViewTemplates_SelectionChanged;
            // 
            // dataGridViewTemplates
            // 
            dataGridViewTemplates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTemplates.Location = new Point(12, 185);
            dataGridViewTemplates.Name = "dataGridViewTemplates";
            dataGridViewTemplates.Size = new Size(564, 161);
            dataGridViewTemplates.TabIndex = 4;
            dataGridViewTemplates.SelectionChanged += dataGridViewTemplates_SelectionChanged;
            // 
            // dataGridViewCommands
            // 
            dataGridViewCommands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCommands.Location = new Point(12, 361);
            dataGridViewCommands.Name = "dataGridViewCommands";
            dataGridViewCommands.Size = new Size(564, 161);
            dataGridViewCommands.TabIndex = 5;
            // 
            // btnSaveToJson
            // 
            btnSaveToJson.Location = new Point(417, 12);
            btnSaveToJson.Name = "btnSaveToJson";
            btnSaveToJson.Size = new Size(161, 47);
            btnSaveToJson.TabIndex = 6;
            btnSaveToJson.Text = "Save JSON";
            btnSaveToJson.UseVisualStyleBackColor = true;
            btnSaveToJson.Click += btnSaveToJson_Click;
            // 
            // btnSequenceTexts
            // 
            btnSequenceTexts.Location = new Point(584, 12);
            btnSequenceTexts.Name = "btnSequenceTexts";
            btnSequenceTexts.Size = new Size(70, 47);
            btnSequenceTexts.TabIndex = 7;
            btnSequenceTexts.Text = "Sequence texts";
            btnSequenceTexts.UseVisualStyleBackColor = true;
            btnSequenceTexts.Click += btnSequenceTexts_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // btnCreateDirectories
            // 
            btnCreateDirectories.Enabled = false;
            btnCreateDirectories.Location = new Point(1536, 22);
            btnCreateDirectories.Name = "btnCreateDirectories";
            btnCreateDirectories.Size = new Size(123, 44);
            btnCreateDirectories.TabIndex = 8;
            btnCreateDirectories.Text = "btnCreateDirectories";
            btnCreateDirectories.UseVisualStyleBackColor = true;
            btnCreateDirectories.Click += btnCreateDirectories_Click;
            // 
            // btnRenameDirectory
            // 
            btnRenameDirectory.Enabled = false;
            btnRenameDirectory.Location = new Point(1536, 80);
            btnRenameDirectory.Name = "btnRenameDirectory";
            btnRenameDirectory.Size = new Size(124, 42);
            btnRenameDirectory.TabIndex = 9;
            btnRenameDirectory.Text = "Rename toImport";
            btnRenameDirectory.UseVisualStyleBackColor = true;
            btnRenameDirectory.Click += btnRenameDirectory_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1672, 978);
            Controls.Add(btnRenameDirectory);
            Controls.Add(btnCreateDirectories);
            Controls.Add(btnSequenceTexts);
            Controls.Add(btnSaveToJson);
            Controls.Add(dataGridViewCommands);
            Controls.Add(dataGridViewTemplates);
            Controls.Add(dataGridViewObjTypes);
            Controls.Add(cbxActiveObjectType);
            Controls.Add(btnGenerate);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)dataGridViewObjTypes).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTemplates).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCommands).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button btnGenerate;
        private ComboBox cbxActiveObjectType;
        private DataGridView dataGridViewObjTypes;
        private DataGridView dataGridViewTemplates;
        private DataGridView dataGridViewCommands;
        private Button btnSaveToJson;
        private Button btnSequenceTexts;
        private ContextMenuStrip contextMenuStrip1;
        private Button btnCreateDirectories;
        private Button btnRenameDirectory;
    }
}