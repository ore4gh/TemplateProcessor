using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemplateProcessor.Sequence;

namespace TemplateProcessor
{
    public partial class Form1 : Form
    {
        private Generator generator;
        private GeneratorData generatorData;
        private PathsConfig pathsConfig;

        public Form1()
        {
            InitializeComponent();
            HelpFunc.LogMessage("FormSequence initialized.");

            InitializeConfigRoot("TemplateProcessorSettings.json");
            InitializePathsConfig(Path.Combine(GlobalData.Instance.SharedFolderPath,"PathsConfig.json"));
            InitializeGeneratorData(GlobalData.Instance.GenDataFilePath);
            //new added
            var pathsConfig = PathsConfig.LoadConfig(Path.Combine(GlobalData.Instance.SharedFolderPath, "PathsConfig.json"));
            var generator = new Generator(pathsConfig);

            if (GlobalData.Instance.GenData != null && GlobalData.Instance.GenData.Tasks != null)
            {
                foreach (var task in GlobalData.Instance.GenData.Tasks)
                {
                    cbxActiveObjectType.Items.Add(task.ObjType);
                }

                // Set the ComboBox to the active object type if it exists
                if (!string.IsNullOrEmpty(GlobalData.Instance.GenData.ActiveObjType))
                {
                    int index = cbxActiveObjectType.Items.IndexOf(GlobalData.Instance.GenData.ActiveObjType);
                    if (index >= 0)
                    {
                        cbxActiveObjectType.SelectedIndex = index;
                    }
                    else
                    {
                        HelpFunc.LogMessage("ActiveObjType not found in the list.\n");
                    }
                }

                // Set the ComboBox DataSource
                cbxActiveObjectType.DataSource = GlobalData.Instance.GenData.Tasks.Select(task => task.ObjType).ToList();
                cbxActiveObjectType.SelectedItem = GlobalData.Instance.GenData.ActiveObjType;

                // Subscribe to the SelectedIndexChanged event
                cbxActiveObjectType.SelectedIndexChanged += cbxActiveObjectType_SelectedIndexChanged;
                LoadGridViewDataForActiveObjType();
            }
            else
            {
                // Log or handle the scenario where GenData or Tasks are null
                HelpFunc.LogMessage("GenData or Tasks is not properly initialized.\n");
            }
        }

        private void InitializeConfigRoot(string fileName)
        {
            var jsonData = HelpFunc.LoadConfigRootFromJson(fileName);

            if (jsonData != null)
            {
                var globalData = GlobalData.Instance;
                globalData.SetGlobalConfigRoot(jsonData);

                // Log some values
                HelpFunc.LogMessage($"Shared Folder Path: {globalData.SharedFolderPath}");
                HelpFunc.LogMessage($"SQL Server: {globalData.Server}");
            }
        }
        private void InitializePathsConfig(string fileName)
        {
            try
            {
                var pathsConfig = PathsConfig.LoadConfig(fileName);
                GlobalData.Instance.SetPathsConfig(pathsConfig);

                // Log some values
                HelpFunc.LogMessage($"Import Paths: {string.Join(", ", GlobalData.Instance.ImportPath)}");
                HelpFunc.LogMessage($"Template Paths: {string.Join(", ", GlobalData.Instance.TemplatePath)}");
                HelpFunc.LogMessage($"Rule Paths: {string.Join(", ", GlobalData.Instance.RulePath)}");
                HelpFunc.LogMessage($"Output Paths: {string.Join(", ", GlobalData.Instance.OutputPath)}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error loading paths configuration: {ex.Message}");
            }
        }
        private void InitializeGeneratorData(string filePath)
        {
            var jsonData = HelpFunc.LoadGeneratorDataFromJson(filePath);

            if (jsonData != null)
            {
                var globalData = GlobalData.Instance;
                globalData.SetGlobalGeneratorData(jsonData);

                // Log some values
                HelpFunc.LogMessage($"Generator Data. Total task count: {globalData.GenData.Tasks.Count}");

            }
        }



        private void btnSaveToJson_Click(object sender, EventArgs e)
        {
            // Ensure the selected object type is updated
            GlobalData.Instance.GenData.ActiveObjType = cbxActiveObjectType.SelectedItem.ToString();

            // Refresh generatorData instance
            var selectedObjType = GlobalData.Instance.GenData.ActiveObjType;
            var selectedTask = GlobalData.Instance.GenData.Tasks.FirstOrDefault(task => task.ObjType == selectedObjType);

            if (selectedTask != null)
            {
                // Update the data in the selected task from the grids
                UpdateTaskFromGrids(selectedTask);

                // Save updated data with custom JSON settings
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Include, // Include default values
                    ContractResolver = new DefaultContractResolver()
                };

                string json = JsonConvert.SerializeObject(GlobalData.Instance.GenData, Formatting.Indented, jsonSettings);
                System.IO.File.WriteAllText(GlobalData.Instance.GenDataFilePath, json);

                HelpFunc.LogMessage("Data saved successfully.\n");
            }
            else
            {
                HelpFunc.LogMessage("Selected task not found.\n");
            }
        }

        private void UpdateTaskFromGrids(Task selectedTask)
        {
            if (dataGridViewTemplates.DataSource != null)
            {
                var updatedTemplates = (List<Template>)dataGridViewTemplates.DataSource;

                foreach (var updatedTemplate in updatedTemplates)
                {
                    var existingTemplate = selectedTask.Templates.FirstOrDefault(t => t.Name == updatedTemplate.Name);

                    if (existingTemplate != null)
                    {
                        // Update only modified properties
                        existingTemplate.Version = updatedTemplate.Version;
                        existingTemplate.Enabled = updatedTemplate.Enabled;

                        // Update commands if the data source is not null
                        if (dataGridViewCommands.DataSource != null)
                        {
                            var allCommands = (List<Command>)dataGridViewCommands.DataSource;

                            // Only update commands that belong to this template
                            var commandsForTemplate = allCommands.Where(cmd => cmd.Name == updatedTemplate.Name).ToList();

                            // Avoid updating to empty list if no commands exist for this template in the grid
                            if (commandsForTemplate.Any())
                            {
                                existingTemplate.Commands = commandsForTemplate;
                            }
                        }
                    }
                }
            }
            else
            {
                HelpFunc.LogMessage("Templates DataSource is null.\n");
            }
        }
        private void LoadGridViewDataForActiveObjType()
        {
            if (cbxActiveObjectType.SelectedItem == null)
            {
                HelpFunc.LogMessage("No active object type selected.\n");
                return;
            }

            string selectedObjType = cbxActiveObjectType.SelectedItem.ToString();
            var selectedTask = GlobalData.Instance.GenData.Tasks.FirstOrDefault(task => task.ObjType == selectedObjType);

            if (selectedTask != null)
            {
                // Clear and set new data for the first grid (Task)
                dataGridViewObjTypes.DataSource = null;
                dataGridViewObjTypes.Rows.Clear();
                dataGridViewObjTypes.DataSource = new List<Task> { selectedTask };

                // Automatically select the first row in the first grid
                InitializeGrids();

                // Log success message
                HelpFunc.LogMessage("Data successfully loaded and grids updated.\n");
            }
            else
            {
                HelpFunc.LogMessage($"No task found for object type: {selectedObjType}\n");
            }
        }
        private void dataGridViewObjTypes_SelectionChanged(object sender, EventArgs e)
        {
            ShowTemplatesForSelectedRow();
        }
        private void dataGridViewTemplates_SelectionChanged(object sender, EventArgs e)
        {
            ShowCommandsForSelectedRow();
        }
        private void InitializeGrids()
        {
            // Set the first grid's active row to the first row
            if (dataGridViewObjTypes.Rows.Count > 0)
            {
                dataGridViewObjTypes.Rows[0].Selected = true;
                ShowTemplatesForSelectedRow();

                // Ensure the command grid is empty during startup
                dataGridViewCommands.DataSource = null;
                dataGridViewCommands.Rows.Clear();
            }
            else
            {
                // Clear the command grid if there are no rows in the first grid
                dataGridViewCommands.DataSource = null;
                dataGridViewCommands.Rows.Clear();
            }
        }
        private void ShowTemplatesForSelectedRow()
        {
            if (dataGridViewObjTypes.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewObjTypes.SelectedRows[0];
                var selectedTask = (Task)selectedRow.DataBoundItem;

                // Update the second grid with templates
                dataGridViewTemplates.DataSource = null;
                dataGridViewTemplates.Rows.Clear();
                dataGridViewTemplates.DataSource = selectedTask.Templates;

                // Ensure the first template is selected
                if (dataGridViewTemplates.Rows.Count > 0)
                {
                    dataGridViewTemplates.Rows[0].Selected = true;
                    ShowCommandsForSelectedRow();
                }
                else
                {
                    // Clear the command grid if there are no templates
                    dataGridViewCommands.DataSource = null;
                    dataGridViewCommands.Rows.Clear();
                }
            }
            else
            {
                // Clear the templates grid if no rows are selected in the first grid
                dataGridViewTemplates.DataSource = null;
                dataGridViewTemplates.Rows.Clear();
            }
        }

        private void ShowCommandsForSelectedRow()
        {
            if (dataGridViewTemplates.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewTemplates.SelectedRows[0];
                var selectedTemplate = (Template)selectedRow.DataBoundItem;

                // Update the third grid with commands if they are present
                var commands = selectedTemplate.Commands?.ToList() ?? new List<Command>();

                dataGridViewCommands.DataSource = null;
                dataGridViewCommands.Rows.Clear();

                if (commands.Count > 0)
                {
                    dataGridViewCommands.DataSource = commands;
                }
            }
            else
            {
                // Clear the command grid if no rows are selected in the templates grid
                dataGridViewCommands.DataSource = null;
                dataGridViewCommands.Rows.Clear();
            }
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            generator.Generate();
        }


        private void cbxActiveObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGridViewDataForActiveObjType();
            //Enable active objType and save to Json
            if (GlobalData.Instance.GenData != null && GlobalData.Instance.GenData.Tasks != null)
            {
                string selectedObjType = cbxActiveObjectType.SelectedItem.ToString();

                foreach (var task in GlobalData.Instance.GenData.Tasks)
                {
                    task.Enabled = task.ObjType == selectedObjType ? 1 : 0;
                }

                // Save the updated generator data to the JSON file
                HelpFunc.SaveGeneratorData(GlobalData.Instance.GenDataFilePath, GlobalData.Instance.GenData);
            }

        }

        private void btnSequenceTexts_Click(object sender, EventArgs e)
        {
            // Open a new FormSequence
            var formSequence = new Sequence.FormSequence();
            formSequence.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FormSequence formSequence = new Sequence.FormSequence();
            formSequence.Show();

        }

        private void btnCreateDirectories_Click(object sender, EventArgs e)
        {
            string genDataPath = GlobalData.Instance.GenDataFolderPath;

            try
            {


                CreateSubdirectories(genDataPath);

                MessageBox.Show("Directories created successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        //
        private void CreateSubdirectories(string parentDir)
        {
            // Get the first level of subdirectories within GenData
            var subDirectories = Directory.GetDirectories(parentDir, "*", SearchOption.TopDirectoryOnly);

            foreach (var subDirectory in subDirectories)
            {
                // Ensure Rule directory exists in each first-level subdirectory
                string ruleDir = Path.Combine(subDirectory, "Rule");
                if (!Directory.Exists(ruleDir))
                {
                    Directory.CreateDirectory(ruleDir);
                    Console.WriteLine($"Created directory: {ruleDir}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {ruleDir}");
                }

                // Create Template, Output, and Rule directories with v01 subdirectory within each first-level subdirectory
                CreateSubdirectoriesWithV01(subDirectory);
            }

            // Also create Template, Output, and Rule directories with v01 subdirectory in the root of GenData
            CreateSubdirectoriesWithV01(parentDir);
        }

        private void CreateSubdirectoriesWithV01(string parentDir)
        {
            string[] subDirs = { "Template", "Output", "Rule" };

            foreach (var subDir in subDirs)
            {
                string fullSubDirPath = Path.Combine(parentDir, subDir);
                if (!Directory.Exists(fullSubDirPath))
                {
                    Directory.CreateDirectory(fullSubDirPath);
                    Console.WriteLine($"Created directory: {fullSubDirPath}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {fullSubDirPath}");
                }

                string v01Dir = Path.Combine(fullSubDirPath, "v01");
                if (!Directory.Exists(v01Dir))
                {
                    Directory.CreateDirectory(v01Dir);
                    Console.WriteLine($"Created directory: {v01Dir}");
                }
                else
                {
                    Console.WriteLine($"Directory already exists: {v01Dir}");
                }
            }
        }
        private void RenameSubdirectories(string parentDir)
        {
            // Get the first level of subdirectories within GenData
            var subDirectories = Directory.GetDirectories(parentDir, "*", SearchOption.TopDirectoryOnly);


            foreach (var subDirectory in subDirectories)
            {
                // Ensure Export directory exists in each first-level subdirectory

                string exportDir = Path.Combine(subDirectory, "Export");
                string importDir = Path.Combine(subDirectory, "Import");
                if (Directory.Exists(exportDir))
                {
                    Directory.Move(exportDir, importDir);
                    Console.WriteLine($"Renamed directory from 'Export' to 'Import': {importDir}");
                }
                else
                {
                    Console.WriteLine($"Directory 'Export' does not exist in: {parentDir}");
                }
            }

        }

        private void btnRenameDirectory_Click(object sender, EventArgs e)
        {
            string genDataPath = GlobalData.Instance.GenDataFolderPath;

            try
            {
                RenameSubdirectories(genDataPath);

                MessageBox.Show("Directories renamed successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }
    }
}