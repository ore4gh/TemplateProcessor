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
        private TextBox txtFileContent;
        private Label lblFileName;

        public FormTextFileViewer(TextBox log, CommonConfig config)
        {
            _config = config;
            txtLog = log;
            InitializeComponent();
            LoadFirstTextFile();
        }

        private void InitializeComponent()
        {
            this.panelTextDisplay = new Panel();
            this.lblFileName = new Label();
            this.txtFileContent = new TextBox();
            this.SuspendLayout();

            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFileName.Location = new System.Drawing.Point(10, 10);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(200, 23);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "No file loaded";

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
            // txtFileContent
            // 
            this.txtFileContent.Dock = DockStyle.Fill;
            this.txtFileContent.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtFileContent.Location = new System.Drawing.Point(0, 0);
            this.txtFileContent.Multiline = true;
            this.txtFileContent.Name = "txtFileContent";
            this.txtFileContent.ScrollBars = ScrollBars.Both;
            this.txtFileContent.Size = new System.Drawing.Size(778, 543);
            this.txtFileContent.TabIndex = 0;
            this.txtFileContent.WordWrap = false;
            this.txtFileContent.ReadOnly = true;

            // Add TextBox to Panel
            this.panelTextDisplay.Controls.Add(this.txtFileContent);

            // 
            // FormTextFileViewer
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panelTextDisplay);
            this.Controls.Add(this.lblFileName);
            this.Name = "FormTextFileViewer";
            this.Text = "Text File Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadFirstTextFile()
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
                    lblFileName.Text = "GenDataConfig.json not found";
                    txtFileContent.Text = "ERROR: GenDataConfig.json not found in project folder.\n\nPlease ensure the configuration file exists.";
                    HelpFunc.LogMessage($"ERROR: Config file not found: {configFilePath}");
                    return;
                }

                // Parse the JSON config to get ActiveObjType
                string configJson = File.ReadAllText(configFilePath);
                JObject config = JObject.Parse(configJson);
                string activeObjType = config["ActiveObjType"]?.ToString();

                if (string.IsNullOrEmpty(activeObjType))
                {
                    lblFileName.Text = "ActiveObjType not found in config";
                    txtFileContent.Text = "ERROR: ActiveObjType not specified in GenDataConfig.json";
                    HelpFunc.LogMessage($"ERROR: ActiveObjType not found in config");
                    return;
                }

                HelpFunc.LogMessage($"Active Object Type: {activeObjType}");

                // Build the output folder path using ActiveObjType
                string objectOutputFolder = Path.Combine(projectFolder, activeObjType, "Output");

                if (!Directory.Exists(objectOutputFolder))
                {
                    lblFileName.Text = $"No Output folder for {activeObjType}";
                    txtFileContent.Text = $"Output folder not found:\n{objectOutputFolder}\n\nPlease generate output files for {activeObjType} first.";
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
                    lblFileName.Text = $"No output files for {activeObjType}";
                    txtFileContent.Text = $"No .txt output files found in:\n{searchFolder}\n\nPlease run the Generator to create output files.";
                    HelpFunc.LogMessage($"No .txt files found in: {searchFolder}");
                    return;
                }

                // Get the first .txt file (alphabetically)
                string firstTxtFile = txtFiles[0];
                
                // Read the file content
                string content = File.ReadAllText(firstTxtFile);

                // Display file name with folder info
                string relativePath = Path.GetRelativePath(projectFolder, firstTxtFile);
                lblFileName.Text = $"Object: {activeObjType} | File: {Path.GetFileName(firstTxtFile)} (1 of {txtFiles.Length})";

                // Display file content
                txtFileContent.Text = content;

                HelpFunc.LogMessage($"Loaded generated output file: {Path.GetFileName(firstTxtFile)}");
                HelpFunc.LogMessage($"Full path: {relativePath}");
                HelpFunc.LogMessage($"File size: {new FileInfo(firstTxtFile).Length} bytes");
                HelpFunc.LogMessage($"Total output files in folder: {txtFiles.Length}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR loading text file: {ex.Message}");
                lblFileName.Text = "Error loading file";
                txtFileContent.Text = $"Error loading text file:\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}";
                MessageBox.Show($"Error loading text file:\n{ex.Message}", 
                    "Load Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}
