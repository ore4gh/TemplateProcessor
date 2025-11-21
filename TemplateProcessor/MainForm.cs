using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemplateProcessor.Generator;
using TemplateProcessor.SearchReplace;
//using TemplateProcessor.Sequence;

namespace TemplateProcessor
{
    public partial class MainForm : Form
    {


        private CommonConfig _config;
        private Dictionary<string, AlarmNumber> alarmNumbers;
        public MainForm(CommonConfig config)
        {
            InitializeComponent();
            _config = config;

            HelpFunc.Initialize(txtLog);
            
            // Discover and save Excel sheet names (after HelpFunc is initialized)
            DiscoveredSheetsConfig.DiscoverAndSaveSheets(_config);
            
            InitializeAlarmNumbers();
        }
        private void InitializeAlarmNumbers()
        {
            string filePath =  Path.Combine(
                _config.CommonData.SharedFolderPath,
                _config.CommonData.ProjectFolderName,
                "AlmNum_Output_Plc_v01.txt"
            );

            try
            {
                var dataLoader = new DataLoader(filePath);
                alarmNumbers = dataLoader.DataDict;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the data: {ex.Message}");
            }
        }

        // Method to reload GenDataConfig.json file
        private void ReloadGenDataConfig()
        {
            try
            {
                HelpFunc.LogMessage("=== Starting GenDataConfig.json reload ===");

                string configFilePath = Path.Combine(
                    _config.CommonData.SharedFolderPath,
                    _config.CommonData.ProjectFolderName,
                    "GenDataConfig.json"
                );

                // Check if config file exists
                if (!File.Exists(configFilePath))
                {
                    HelpFunc.LogMessage($"ERROR: Config file not found: {configFilePath}");
                    MessageBox.Show($"Config file not found:\n{configFilePath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Test if file can be read (basic validation)
                try
                {
                    string testRead = File.ReadAllText(configFilePath);
                    HelpFunc.LogMessage($"GenDataConfig.json reloaded successfully from: {configFilePath}");
                    HelpFunc.LogMessage($"Config file size: {new FileInfo(configFilePath).Length} bytes");
                }
                catch (Exception readEx)
                {
                    HelpFunc.LogMessage($"ERROR: Could not read config file: {readEx.Message}");
                    MessageBox.Show($"Error reading config file:\n{readEx.Message}", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                HelpFunc.LogMessage("=== GenDataConfig.json reload completed ===");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR during config reload: {ex.Message}");
                MessageBox.Show($"An error occurred while reloading config:\n{ex.Message}", "Config Reload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to reload Excel file - re-copy from source to Tmp_ file
        private void ReloadExcelData()
        {
            try
            {
                HelpFunc.LogMessage("=== Starting Excel file reload ===");
                
                string workDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string excelFileName = _config.CommonData.ExcelDataFileName;
                string excelSourcePath = Path.Combine(workDirectory, excelFileName);
                string excelDestinationPath = Path.Combine(workDirectory, "Tmp_" + excelFileName);

                // Check if source file exists
                if (!File.Exists(excelSourcePath))
                {
                    HelpFunc.LogMessage($"ERROR: Source Excel file not found: {excelSourcePath}");
                    MessageBox.Show($"Source Excel file not found:\n{excelSourcePath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Delete old Tmp_ file if it exists
                if (File.Exists(excelDestinationPath))
                {
                    try
                    {
                        File.Delete(excelDestinationPath);
                        HelpFunc.LogMessage($"Deleted old temporary file: Tmp_{excelFileName}");
                    }
                    catch (IOException)
                    {
                        HelpFunc.LogMessage("WARNING: Could not delete old Tmp_ file - it may be open in Excel");
                        MessageBox.Show("Cannot delete old Tmp_ file - it may be open in Excel.\nPlease close Excel and try again.", "File Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Copy fresh Excel file
                HelpFunc.CopyExcelFile(excelSourcePath, excelDestinationPath);
                HelpFunc.LogMessage($"Excel file reloaded successfully: {excelFileName} -> Tmp_{excelFileName}");
                HelpFunc.LogMessage("=== Excel reload completed ===");
                
                MessageBox.Show($"Excel file reloaded successfully!\n\nSource: {excelFileName}\nDestination: Tmp_{excelFileName}", 
                    "Data Reloaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR during Excel reload: {ex.Message}");
                MessageBox.Show($"An error occurred while reloading Excel:\n{ex.Message}", "Reload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load FormGenerator into panelPlaceholder at startup
            ShowFormInPanel(new FormGenerator(txtLog));
        }
        private void MenuItemGenerator_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new FormGenerator(txtLog));
        }

        private void MenuItemSequenceEditor_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new Sequence.FormTextListGenerator(txtLog, _config));
        }

        private void MenuItemTextViewer_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new FormTextFileViewer(txtLog, _config));
        }

        private void MenuItemReloadExcel_Click(object sender, EventArgs e)
        {
            // Step 1: Reload config file FIRST
            ReloadGenDataConfig();

            // Step 2: Reload Excel file AFTER config
            ReloadExcelData();

            // Step 3: Reload the currently displayed form to show updated data
            RefreshCurrentForm();
        }

        private void MenuItemReload_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the current form is FormGenerator
                if (panelPlaceholder.Controls.Count > 0 && panelPlaceholder.Controls[0] is FormGenerator formGenerator)
                {
                    // Call the reload method on the FormGenerator
                    formGenerator.ReloadConfiguration();
                    HelpFunc.LogMessage("Configuration reloaded from menu.");
                }
                else
                {
                    MessageBox.Show(
                        "Reload is only available when the Generator form is active.",
                        "Reload",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error during reload: {ex.Message}");
                MessageBox.Show($"Error during reload:\n{ex.Message}", "Reload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MenuItemSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the current form is FormGenerator
                if (panelPlaceholder.Controls.Count > 0 && panelPlaceholder.Controls[0] is FormGenerator formGenerator)
                {
                    // Call the save config method on the FormGenerator
                    formGenerator.SaveConfiguration();
                    HelpFunc.LogMessage("Configuration saved from menu.");
                }
                else
                {
                    MessageBox.Show(
                        "Save Config is only available when the Generator form is active.",
                        "Save Config",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error saving configuration: {ex.Message}");
                MessageBox.Show($"Error saving configuration:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to refresh/reload the currently displayed form
        private void RefreshCurrentForm()
        {
            // Check which form is currently displayed and reload it
            if (panelPlaceholder.Controls.Count > 0 && panelPlaceholder.Controls[0] is Form currentForm)
            {
                if (currentForm is FormGenerator)
                {
                    // Reload the Generator form with fresh data
                    ShowFormInPanel(new FormGenerator(txtLog));
                    HelpFunc.LogMessage("Generator form refreshed with new data.");
                }
                else if (currentForm is SearchReplaceForm)
                {
                    // Reload the SearchReplace form with fresh data
                    ShowFormInPanel(new SearchReplaceForm(txtLog, alarmNumbers));
                    HelpFunc.LogMessage("SearchReplace form refreshed with new data.");
                }
            }
        }

        private void ShowFormInPanel(Form form)
        {
            // Clear the existing form in the panel
            panelPlaceholder.Controls.Clear();

            // Set the form's TopLevel property to false and add it to the panel
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelPlaceholder.Controls.Add(form);
            form.Show();
        }
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            panelLog = new Panel();
            txtLog = new TextBox();
            panelPlaceholder = new Panel();
            toolStripMenuItem20 = new ToolStripMenuItem();
            toolStripMenuItemTextViewer = new ToolStripMenuItem();
            toolStripMenuItemReloadExcel = new ToolStripMenuItem();
            toolStripMenuItemReload = new ToolStripMenuItem();
            toolStripMenuItemSaveConfig = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            panelLog.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem20, toolStripMenuItem2, toolStripMenuItemTextViewer, toolStripMenuItemReloadExcel, toolStripMenuItemReload, toolStripMenuItemSaveConfig });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1813, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(71, 20);
            toolStripMenuItem1.Text = "Generator";
            toolStripMenuItem1.Click += MenuItemGenerator_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(104, 20);
            toolStripMenuItem2.Text = "Sequence editor";
            toolStripMenuItem2.Click += MenuItemSequenceEditor_Click;
            // 
            // panelLog
            // 
            panelLog.Controls.Add(txtLog);
            panelLog.Dock = DockStyle.Bottom;
            panelLog.Location = new Point(0, 885);
            panelLog.Name = "panelLog";
            panelLog.Size = new Size(1813, 141);
            panelLog.TabIndex = 13;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(0, 0);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(866, 141);
            txtLog.TabIndex = 1;
            // 
            // panelPlaceholder
            // 
            panelPlaceholder.Dock = DockStyle.Fill;
            panelPlaceholder.Location = new Point(0, 24);
            panelPlaceholder.Name = "panelPlaceholder";
            panelPlaceholder.Size = new Size(1813, 1002);
            panelPlaceholder.TabIndex = 12;
            // 
            // toolStripMenuItem20
            // 
            toolStripMenuItem20.Name = "toolStripMenuItem20";
            toolStripMenuItem20.Size = new Size(72, 20);
            toolStripMenuItem20.Text = "Templates";
            toolStripMenuItem20.Click += toolStripMenuItem20_Click;
            // 
            // toolStripMenuItemTextViewer
            // 
            toolStripMenuItemTextViewer.Name = "toolStripMenuItemTextViewer";
            toolStripMenuItemTextViewer.Size = new Size(79, 20);
            toolStripMenuItemTextViewer.Text = "Text Viewer";
            toolStripMenuItemTextViewer.Click += MenuItemTextViewer_Click;
            // 
            // toolStripMenuItemReloadExcel
            // 
            toolStripMenuItemReloadExcel.Name = "toolStripMenuItemReloadExcel";
            toolStripMenuItemReloadExcel.Size = new Size(85, 20);
            toolStripMenuItemReloadExcel.Text = "Reload Excel";
            toolStripMenuItemReloadExcel.Click += MenuItemReloadExcel_Click;
            // 
            // toolStripMenuItemReload
            // 
            toolStripMenuItemReload.Name = "toolStripMenuItemReload";
            toolStripMenuItemReload.Size = new Size(96, 20);
            toolStripMenuItemReload.Text = "Reload Config";
            toolStripMenuItemReload.Click += MenuItemReload_Click;
            // 
            // toolStripMenuItemSaveConfig
            // 
            toolStripMenuItemSaveConfig.Name = "toolStripMenuItemSaveConfig";
            toolStripMenuItemSaveConfig.Size = new Size(83, 20);
            toolStripMenuItemSaveConfig.Text = "Save Config";
            toolStripMenuItemSaveConfig.Click += MenuItemSaveConfig_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(1813, 1026);
            Controls.Add(panelLog);
            Controls.Add(panelPlaceholder);
            Controls.Add(menuStrip1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panelLog.ResumeLayout(false);
            panelLog.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private Panel panelLog;
        private Panel panelPlaceholder;
        private TextBox txtLog;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItemTextViewer;
        private ToolStripMenuItem toolStripMenuItemReloadExcel;
        private ToolStripMenuItem toolStripMenuItemReload;
        private ToolStripMenuItem toolStripMenuItemSaveConfig;

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            ShowFormInPanel(new SearchReplace.SearchReplaceForm(txtLog, alarmNumbers));
        }
    }
}
