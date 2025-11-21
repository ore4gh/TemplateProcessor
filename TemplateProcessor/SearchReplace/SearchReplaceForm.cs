using System;
using System.IO;
using System.Windows.Forms;
using TemplateProcessor.Generator;
using static TemplateProcessor.SearchReplace.SearchReplaceEngine;

namespace TemplateProcessor.SearchReplace
{
    public partial class SearchReplaceForm : Form
    {
        private SearchReplaceEngine _searchReplaceEngine;
        private TextBox _txtLog;
        private Dictionary<string, AlarmNumber> _alarmNumbers;

        public SearchReplaceForm(TextBox txtLog, Dictionary<string, AlarmNumber> alarmNumbers)
        {
            InitializeComponent();
            _txtLog = txtLog;
            _alarmNumbers = alarmNumbers;
            HelpFunc.LogMessage("SearchReplaceForm initialized.");

            // Use the global configuration loaded in the Program class
            var commonConfig = Program.Config;

            // Initialize the SearchReplaceEngine with the loaded paths configuration and alarmNumbers
            _searchReplaceEngine = new SearchReplaceEngine("TextReplacementConfig.json", commonConfig, _alarmNumbers);

            PopulateActiveTaskComboBox();
            PopulateComboBox();
            PrintAlarmNumbers();
        }
        private void PopulateActiveTaskComboBox()
        {
            var searchReplaceData = _searchReplaceEngine.GetSearchReplaceData();
            if (searchReplaceData == null)
                return;

            cbxActiveTask.Items.Clear();

            // Iterate through all tasks and add their names
            foreach (var task in searchReplaceData.Tasks)
            {
                cbxActiveTask.Items.Add(task.Name);
            }

            // Set the currently active task if it exists
            if (!string.IsNullOrEmpty(searchReplaceData.ActiveTask) && cbxActiveTask.Items.Contains(searchReplaceData.ActiveTask))
            {
                cbxActiveTask.SelectedItem = searchReplaceData.ActiveTask;
            }
        }
        private void PopulateComboBox()
        {
            var searchReplaceData = _searchReplaceEngine.GetSearchReplaceData();

            if (searchReplaceData != null)
            {
                // Get the enabled object types from ObjTypes
                var enabledObjTypes = searchReplaceData.ObjTypes
                    .Where(objType => objType.Value == 1)
                    .Select(objType => objType.Key)
                    .ToList();

                foreach (var objType in enabledObjTypes)
                {
                    cbxActiveObjectType.Items.Add(objType);
                }

                // Set the ComboBox to the active object type if it exists
                if (!string.IsNullOrEmpty(searchReplaceData.ActiveObjType))
                {
                    int index = cbxActiveObjectType.Items.IndexOf(searchReplaceData.ActiveObjType);
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
        private void PrintAlarmNumbers()
        {
            foreach (var kvp in _alarmNumbers)
            {
                var alarmNumber = kvp.Value;
                HelpFunc.LogMessage($"{kvp.Key}: {alarmNumber.TagStartNrAct}, {alarmNumber.DescStartNrAct}, {alarmNumber.AlmStartNrAct}, " +
                                    $"{alarmNumber.StaStartNrAct}, {alarmNumber.StaCountAct}, {alarmNumber.TagStartNrNew}, {alarmNumber.DescStartNrNew}, " +
                                    $"{alarmNumber.AlmStartNrNew}, {alarmNumber.StaStartNrNew}, {alarmNumber.StaCountNew}");
            }
        }

        private void btnPerformSearchReplace_Click(object sender, EventArgs e)
        {
            if (cbxActiveObjectType.SelectedItem != null)
            {
                string selectedObjType = cbxActiveObjectType.SelectedItem.ToString();
                _searchReplaceEngine.ProcessSearchReplace(selectedObjType);
                HelpFunc.LogMessage("Search replace complited!");
            }
            else
            {
                HelpFunc.LogMessage("No ObjType selected for search-replace process.");
            }
        }

        private void cbxActiveObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxActiveObjectType.SelectedItem != null)
            {
                string selectedObjType = cbxActiveObjectType.SelectedItem.ToString();
                _searchReplaceEngine.UpdateActiveObjType(selectedObjType);
                HelpFunc.LogMessage($"ActiveObjType updated to {selectedObjType}");
            }

        }

        private void cbxActiveTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxActiveTask.SelectedItem != null)
            {
                var searchReplaceData = _searchReplaceEngine.GetSearchReplaceData();
                searchReplaceData.ActiveTask = cbxActiveTask.SelectedItem.ToString();

            }
        }

        // Other methods that utilize _searchReplaceEngine can be added here
    }
}