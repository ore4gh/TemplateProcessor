using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TemplateProcessor
{
    public partial class FormTextFileViewer : Form
    {
        private CommonConfig _config;
        private TextBox txtLog;
        private Panel panelTextDisplay;
        private TabControl tabControl;
        private Label lblFileInfo;

        public FormTextFileViewer(TextBox log, CommonConfig config)
        {
            _config = config;
            txtLog = log;
            InitializeComponent();
            LoadAllTextFiles();
        }

        private void InitializeComponent()
        {
            this.panelTextDisplay = new Panel();
            this.lblFileInfo = new Label();
            this.tabControl = new TabControl();
            this.SuspendLayout();

            // 
            // lblFileInfo
            // 
            this.lblFileInfo.AutoSize = true;
            this.lblFileInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFileInfo.Location = new System.Drawing.Point(10, 10);
            this.lblFileInfo.Name = "lblFileInfo";
            this.lblFileInfo.Size = new System.Drawing.Size(200, 23);
            this.lblFileInfo.TabIndex = 0;
            this.lblFileInfo.Text = "Loading files...";

            // 
            // panelTextDisplay
            // 
            this.panelTextDisplay.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.panelTextDisplay.BorderStyle = BorderStyle.FixedSingle;
            this.panelTextDisplay.Location = new System.Drawing.Point(10, 45);
            this.panelTextDisplay.Name = "panelTextDisplay";
            this.panelTextDisplay.Size = new System.Drawing.Size(780, 545);
            this.panelTextDisplay.TabIndex = 1;

            // 
            // tabControl
            // 
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Size = new System.Drawing.Size(778, 543);
            this.tabControl.TabIndex = 0;

            // Add TabControl to Panel
            this.panelTextDisplay.Controls.Add(this.tabControl);

            // 
            // FormTextFileViewer
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panelTextDisplay);
            this.Controls.Add(this.lblFileInfo);
            this.Name = "FormTextFileViewer";
            this.Text = "Text File Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadAllTextFiles()
        {
            try
            {
                // Get the project folder path
                string projectFolder = Path.Combine(
                    _config.CommonData.SharedFolderPath,
                    _config.CommonData.ProjectFolderName
                );

                // Check if directory exists
                if (!Directory.Exists(projectFolder))
                {
                    HelpFunc.LogMessage($"ERROR: Project folder not found: {projectFolder}");
                    lblFileInfo.Text = "ERROR: Project folder not found";
                    MessageBox.Show($"Project folder not found:\n{projectFolder}", 
                        "Folder Not Found", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    return;
                }

                // Read the GenDataConfig.json to get ActiveObjType
                string configFilePath = Path.Combine(projectFolder, "GenDataConfig.json");
                
                if (!File.Exists(configFilePath))
                {
                    lblFileInfo.Text = "GenDataConfig.json not found";
                    AddErrorTab("ERROR: GenDataConfig.json not found in project folder.\n\nPlease ensure the configuration file exists.");
                    HelpFunc.LogMessage($"ERROR: Config file not found: {configFilePath}");
                    return;
                }

                // Parse the JSON config to get ActiveObjType
                string configJson = File.ReadAllText(configFilePath);
                JObject config = JObject.Parse(configJson);
                string activeObjType = config["ActiveObjType"]?.ToString();

                if (string.IsNullOrEmpty(activeObjType))
                {
                    lblFileInfo.Text = "ActiveObjType not found in config";
                    AddErrorTab("ERROR: ActiveObjType not specified in GenDataConfig.json");
                    HelpFunc.LogMessage($"ERROR: ActiveObjType not found in config");
                    return;
                }

                HelpFunc.LogMessage($"Active Object Type: {activeObjType}");

                // Build the output folder path using ActiveObjType
                string objectOutputFolder = Path.Combine(projectFolder, activeObjType, "Output");

                if (!Directory.Exists(objectOutputFolder))
                {
                    lblFileInfo.Text = $"No Output folder for {activeObjType}";
                    AddErrorTab($"Output folder not found:\n{objectOutputFolder}\n\nPlease generate output files for {activeObjType} first.");
                    HelpFunc.LogMessage($"ERROR: Output folder not found: {objectOutputFolder}");
                    return;
                }

                // Look for version subfolders (v01, v02, v03, etc.) and get the latest
                string[] versionFolders = Directory.GetDirectories(objectOutputFolder, "v*")
                    .OrderByDescending(f => f)
                    .ToArray();

                string searchFolder = objectOutputFolder;
                if (versionFolders.Length > 0)
                {
                    searchFolder = versionFolders[0];  // Get the latest version folder
                    HelpFunc.LogMessage($"Using latest version folder: {Path.GetFileName(searchFolder)}");
                }

                // Find all .txt files in the search folder
                string[] txtFiles = Directory.GetFiles(searchFolder, "*.txt")
                    .OrderBy(f => Path.GetFileName(f))
                    .ToArray();

                if (txtFiles.Length == 0)
                {
                    lblFileInfo.Text = $"No output files for {activeObjType}";
                    AddErrorTab($"No .txt output files found in:\n{searchFolder}\n\nPlease run the Generator to create output files.");
                    HelpFunc.LogMessage($"No .txt files found in: {searchFolder}");
                    return;
                }

                // Clear existing tabs
                tabControl.TabPages.Clear();

                // Create a tab for each file
                foreach (string filePath in txtFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    
                    // Read the file content
                    string content = File.ReadAllText(filePath);

                    // Create a new tab page
                    TabPage tabPage = new TabPage(fileName);

                    // Create a TextBox for this file
                    TextBox txtContent = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        Font = new System.Drawing.Font("Consolas", 9F),
                        Multiline = true,
                        ScrollBars = ScrollBars.Both,
                        WordWrap = false,
                        ReadOnly = true,
                        Text = content
                    };

                    // Add TextBox to TabPage
                    tabPage.Controls.Add(txtContent);

                    // Add TabPage to TabControl
                    tabControl.TabPages.Add(tabPage);

                    HelpFunc.LogMessage($"Loaded file into tab: {fileName} ({new FileInfo(filePath).Length} bytes)");
                }

                // Update label with info
                string relativePath = Path.GetRelativePath(projectFolder, searchFolder);
                lblFileInfo.Text = $"Object: {activeObjType} | Folder: {relativePath} | Files: {txtFiles.Length}";

                HelpFunc.LogMessage($"Successfully loaded {txtFiles.Length} output files into tabs");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR loading text files: {ex.Message}");
                lblFileInfo.Text = "Error loading files";
                AddErrorTab($"Error loading text files:\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}");
                MessageBox.Show($"Error loading text files:\n{ex.Message}", 
                    "Load Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void AddErrorTab(string errorMessage)
        {
            tabControl.TabPages.Clear();
            TabPage errorTab = new TabPage("Error");
            TextBox txtError = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9F),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = true,
                ReadOnly = true,
                Text = errorMessage,
                ForeColor = System.Drawing.Color.Red
            };
            errorTab.Controls.Add(txtError);
            tabControl.TabPages.Add(errorTab);
        }
    }
}
