
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemplateProcessor.SearchReplace;

namespace TemplateProcessor.Generator
{
    public partial class FormGenerator : Form
    {
        private GeneratorEngine _generatorEngine;
        private TextBox _txtLog;


        public FormGenerator(TextBox txtLog)
        {
            InitializeComponent();
            _txtLog = txtLog;
            HelpFunc.LogMessage("FormGenerator initialized.");

            // Use the global configuration loaded in the Program class
            var commonConfig = Program.Config;

            // Initialize the GeneratorEngine with the loaded paths configuration
            _generatorEngine = new GeneratorEngine("GenDataConfig.json", commonConfig, _txtLog);

            PopulateActiveTaskComboBox();
            PopulateComboBox();
            // Use UpdateStartEndRows instead of AssignStartEndRows
            if (cbxActiveObjectType.SelectedItem is string selectedObjType)
            {
                UpdateStartEndRows(selectedObjType);
            }
        }
        private void PopulateActiveTaskComboBox()
        {
            var generatorData = _generatorEngine.GetGeneratorData();
            if (generatorData == null || generatorData.Tasks == null)
                return;

            cbxActiveTask.Items.Clear();

            // Iterate through all tasks and add their names
            foreach (var task in generatorData.Tasks)
            {
                cbxActiveTask.Items.Add(task.Name);
            }

            // Set the currently active task if it exists
            if (!string.IsNullOrEmpty(generatorData.ActiveTask) && cbxActiveTask.Items.Contains(generatorData.ActiveTask))
            {
                cbxActiveTask.SelectedItem = generatorData.ActiveTask;
            }
        }
        private void PopulateComboBox()
        {
            var generatorData = _generatorEngine.GetGeneratorData();

            if (generatorData != null)
            {
                // Get the enabled object types from ObjTypes
                var enabledObjTypes = generatorData.ObjTypes
                    .Where(objType => objType.Value[0] == 1)
                    .Select(objType => objType.Key)
                    .ToList();

                foreach (var objType in enabledObjTypes)
                {
                    cbxActiveObjectType.Items.Add(objType);
                }

                // Set the ComboBox to the active object type if it exists
                if (!string.IsNullOrEmpty(generatorData.ActiveObjType))
                {
                    int index = cbxActiveObjectType.Items.IndexOf(generatorData.ActiveObjType);
                    if (index >= 0)
                    {
                        cbxActiveObjectType.SelectedIndex = index;
                    }
                    else
                    {
                        HelpFunc.LogMessage("ActiveObjType not found in the list.\n");
                    }
                }
            }
            else
            {
                HelpFunc.LogMessage("No tasks found in the search replace data.");
            }
        }

        // Method to update StartRow and EndRow
        private void UpdateStartEndRows(string objType)
        {
            var generatorData = _generatorEngine.GetGeneratorData();

            if (generatorData != null)
            {
                var objDetails = generatorData.GetObjTypeDetails(objType);

                if (objDetails != null)
                {
                    textBox_StartRow.Text = objDetails.StartRow.ToString();
                    textBox_EndRow.Text = objDetails.EndRow.ToString();
                }
                else
                {
                    HelpFunc.LogMessage($"No details found for ObjType '{objType}'.\n");
                }
            }
            else
            {
                HelpFunc.LogMessage("No tasks found in the search replace data.");
            }
        }
        private void UpdateGeneratorEngine()
        {
            if (cbxActiveObjectType.SelectedItem is string selectedObjType)
            {
                var generatorData = _generatorEngine.GetGeneratorData();
                var objDetails = generatorData.GetObjTypeDetails(selectedObjType);

                if (objDetails != null)
                {
                    if (int.TryParse(textBox_StartRow.Text, out int startRow) &&
                        int.TryParse(textBox_EndRow.Text, out int endRow))
                    {
                        objDetails.StartRow = startRow;
                        objDetails.EndRow = endRow;
                        HelpFunc.LogMessage($"Updated StartRow: {startRow}, EndRow: {endRow} for {selectedObjType}");
                    }
                    else
                    {
                        HelpFunc.LogMessage("Invalid StartRow or EndRow input. Please enter valid numbers.");
                    }
                }
            }
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (cbxActiveObjectType.SelectedItem is string selectedObjType)
            {
                var generatorData = _generatorEngine.GetGeneratorData();
                var objDetails = generatorData.GetObjTypeDetails(selectedObjType);

                if (objDetails != null)
                {
                    // Update StartRow and EndRow values if they are valid
                    if (int.TryParse(textBox_StartRow.Text, out int startRow) &&
                        int.TryParse(textBox_EndRow.Text, out int endRow))
                    {
                        objDetails.StartRow = startRow;
                        objDetails.EndRow = endRow;
                        HelpFunc.LogMessage($"Updated StartRow: {startRow}, EndRow: {endRow} for {selectedObjType}");

                        // Call Generate method with updated values
                        _generatorEngine.Generate(selectedObjType, startRow, endRow);
                    }
                    else
                    {
                        HelpFunc.LogMessage("Invalid StartRow or EndRow input. Please enter valid numbers.");
                    }
                }
                else
                {
                    HelpFunc.LogMessage($"No details found for ObjType '{selectedObjType}'.");
                }
            }
            else
            {
                HelpFunc.LogMessage("No ObjType selected for generation process.");
            }
        }

        private void cbxActiveTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxActiveTask.SelectedItem != null)
            {
                var generatorData = _generatorEngine.GetGeneratorData();
                generatorData.ActiveTask = cbxActiveTask.SelectedItem.ToString();

            }
        }

        private void cbxActiveObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxActiveObjectType.SelectedItem is string selectedObjType)
            {
                UpdateStartEndRows(selectedObjType);
            }
        }

        private void textBox_StartRow_Leave(object sender, EventArgs e)
        {
            UpdateGeneratorEngine();
        }

        private void textBox_EndRow_Leave(object sender, EventArgs e)
        {
            UpdateGeneratorEngine();
        }

        /// <summary>
        /// Public method to reload configuration - can be called from MainForm
        /// </summary>
        public void ReloadConfiguration()
        {
            reloadToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Public method to save configuration - can be called from MainForm
        /// </summary>
        public void SaveConfiguration()
        {
            saveConfigToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Reload the configuration from the JSON file
                _generatorEngine.ReloadGenData();

                // Clear and repopulate combo boxes with the new data
                cbxActiveTask.Items.Clear();
                cbxActiveObjectType.Items.Clear();

                PopulateActiveTaskComboBox();
                PopulateComboBox();

                // Update the StartRow and EndRow textboxes if an object type is selected
                if (cbxActiveObjectType.SelectedItem is string selectedObjType)
                {
                    UpdateStartEndRows(selectedObjType);
                }

                HelpFunc.LogMessage("Configuration reloaded successfully from JSON file.");
                MessageBox.Show(
                    "Configuration reloaded successfully!",
                    "Reload Config",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error reloading configuration: {ex.Message}");
                MessageBox.Show(
                    $"Error reloading configuration:\n{ex.Message}",
                    "Reload Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var generatorData = _generatorEngine.GetGeneratorData();
                
                // Update ActiveTask from combo box
                if (cbxActiveTask.SelectedItem != null)
                {
                    generatorData.ActiveTask = cbxActiveTask.SelectedItem.ToString();
                    HelpFunc.LogMessage($"ActiveTask set to: {generatorData.ActiveTask}");
                }

                // Update ActiveObjType from combo box
                if (cbxActiveObjectType.SelectedItem is string selectedObjType)
                {
                    generatorData.ActiveObjType = selectedObjType;
                    HelpFunc.LogMessage($"ActiveObjType set to: {selectedObjType}");
                    
                    // Update the StartRow and EndRow values
                    if (int.TryParse(textBox_StartRow.Text, out int startRow) &&
                        int.TryParse(textBox_EndRow.Text, out int endRow))
                    {
                        // Update the underlying dictionary with new values
                        _generatorEngine.UpdateObjTypeRows(selectedObjType, startRow, endRow);

                        // Save the configuration to JSON file
                        _generatorEngine.SaveGenDataConfig();

                        HelpFunc.LogMessage($"Configuration saved successfully! ActiveTask={generatorData.ActiveTask}, ActiveObjType={selectedObjType}, StartRow={startRow}, EndRow={endRow}");
                        MessageBox.Show(
                            $"Configuration saved successfully!\n\n" +
                            $"Active Task: {generatorData.ActiveTask}\n" +
                            $"Active Object Type: {selectedObjType}\n" +
                            $"StartRow: {startRow}\n" +
                            $"EndRow: {endRow}",
                            "Save Config",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        HelpFunc.LogMessage("Invalid StartRow or EndRow input. Please enter valid numbers.");
                        MessageBox.Show(
                            "Invalid StartRow or EndRow input.\nPlease enter valid numbers.",
                            "Save Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                else
                {
                    HelpFunc.LogMessage("No object type selected. Please select an object type first.");
                    MessageBox.Show(
                        "No object type selected.\nPlease select an object type first.",
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error saving configuration: {ex.Message}");
                MessageBox.Show(
                    $"Error saving configuration:\n{ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}