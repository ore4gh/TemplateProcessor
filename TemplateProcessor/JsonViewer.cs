using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateProcessor
{
    public class JsonViewer
    {
        private GeneratorData generatorData;
        private ComboBox cbxActiveObjType;
        private DataGridView dataViewGridObjType;
        private DataGridView dataViewGridTemplates;
        private DataGridView dataViewGridCommands;
        private TextBox txtLog;

        public JsonViewer(ComboBox comboBox, DataGridView gridObjType, DataGridView gridTemplates, DataGridView gridCommands, TextBox logTextBox)
        {
            cbxActiveObjType = comboBox;
            dataViewGridObjType = gridObjType;
            dataViewGridTemplates = gridTemplates;
            dataViewGridCommands = gridCommands;
            txtLog = logTextBox;
            HelpFunc.Initialize(txtLog);

            InitializeData();
        }

        private void InitializeData()
        {
            generatorData = HelpFunc.LoadGeneratorDataFromJson(GlobalData.Instance.GenDataFilePath);

            if (generatorData != null)
            {
                cbxActiveObjType.DataSource = generatorData.Tasks.Select(task => task.ObjType).ToList();
                cbxActiveObjType.SelectedItem = generatorData.ActiveObjType;

                cbxActiveObjType.SelectedIndexChanged += (s, e) => LoadDataForActiveObjType();
                LoadDataForActiveObjType();
            }
            else
            {
                HelpFunc.LogMessage("Failed to initialize data. Configuration root is null.");
            }
        }

        private void LoadDataForActiveObjType()
        {
            string selectedObjType = cbxActiveObjType.SelectedItem.ToString();
            var selectedTask = generatorData.Tasks.FirstOrDefault(task => task.ObjType == selectedObjType);

            if (selectedTask != null)
            {
                dataViewGridObjType.DataSource = new List<Task> { selectedTask };
                dataViewGridTemplates.DataSource = selectedTask.Templates;

                var commands = selectedTask.Templates.Where(template => template.Commands != null)
                                                     .SelectMany(template => template.Commands)
                                                     .ToList();
                dataViewGridCommands.DataSource = commands;
            }
        }

        public void SaveConfiguration()
        {
            generatorData.ActiveObjType = cbxActiveObjType.SelectedItem.ToString();
            HelpFunc.SaveGeneratorData(GlobalData.Instance.GenDataFilePath, generatorData);
        }
    }
}
