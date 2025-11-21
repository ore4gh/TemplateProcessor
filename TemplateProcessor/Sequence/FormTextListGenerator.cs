using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using ClosedXML.Excel;
using Newtonsoft.Json;

namespace TemplateProcessor.Sequence
{
    public partial class FormTextListGenerator : Form
    {
        private TextBox txtLog;
        private CommonConfig _config;
        private Dictionary<string, List<string>> templateDict;
        private string excelFilePath;

        public FormTextListGenerator(TextBox log, CommonConfig config)
        {
            InitializeComponent();
            txtLog = log;
            _config = config;
            
            LoadTemplateXml();
            LoadExcelSheets();
        }

        private void LoadTemplateXml()
        {
            try
            {
                // Load template from TextEq\Template\v02 folder
                string baseDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string templatePath = Path.Combine(baseDirectory, "TextEq", "Template", "v02", "TextEq_Template_SqlEnEn_v02.xml");
                
                if (!File.Exists(templatePath))
                {
                    HelpFunc.LogMessage($"ERROR: Template file not found at: {templatePath}");
                    MessageBox.Show($"Template file not found!\n\nExpected location:\nTextEq\\Template\\v02\\TextEq_Template_SqlEnEn_v02.xml", 
                        "Template Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                templateDict = ReadTemplateXml(templatePath);
                HelpFunc.LogMessage($"Template loaded successfully from: TextEq\\Template\\v02\\TextEq_Template_SqlEnEn_v02.xml");
                HelpFunc.LogMessage($"Found {templateDict.Count} templates: {string.Join(", ", templateDict.Keys)}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR loading template: {ex.Message}");
                MessageBox.Show($"Error loading template:\n\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExcelSheets()
        {
            try
            {
                // Read discoveredSheets.json
                string workDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string jsonFilePath = Path.Combine(workDirectory, "discoveredSheets.json");

                // Also set excelFilePath for other methods that might need it
                string excelFileName = _config.CommonData.ExcelDataFileName;
                excelFilePath = Path.Combine(workDirectory, "Tmp_" + excelFileName);

                if (!File.Exists(jsonFilePath))
                {
                    HelpFunc.LogMessage($"WARNING: discoveredSheets.json not found at: {jsonFilePath}");
                    MessageBox.Show($"discoveredSheets.json not found!\n\nExpected location:\n{jsonFilePath}", 
                        "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Read and deserialize JSON
                string jsonContent = File.ReadAllText(jsonFilePath);
                var discoveredConfig = JsonConvert.DeserializeObject<DiscoveredSheetsConfig>(jsonContent);

                if (discoveredConfig == null || discoveredConfig.Sheets == null)
                {
                    HelpFunc.LogMessage("ERROR: Failed to load discoveredSheets.json");
                    return;
                }

                // Filter sheets: starts with "TextEq_" AND IsEnabled = true
                var filteredSheets = discoveredConfig.Sheets
                    .Where(s => s.SheetName.StartsWith("TextEq_") && s.IsEnabled)
                    .Select(s => s.SheetName)
                    .ToList();

                HelpFunc.LogMessage($"=== Sheet Loading Complete ===");
                HelpFunc.LogMessage($"Total sheets in JSON: {discoveredConfig.Sheets.Count}");
                HelpFunc.LogMessage($"Filtered sheets (TextEq_* AND Enabled=true): {filteredSheets.Count}");
                
                if (filteredSheets.Count == 0)
                {
                    HelpFunc.LogMessage("No enabled sheets starting with 'TextEq_' found.");
                    MessageBox.Show("No enabled sheets starting with 'TextEq_' found.\n\nPlease enable sheets in discoveredSheets.json", 
                        "No Enabled Sheets", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Populate ComboBox
                comboBoxSheets.Items.Clear();
                comboBoxSheets.Items.AddRange(filteredSheets.ToArray());
                
                if (comboBoxSheets.Items.Count > 0)
                {
                    comboBoxSheets.SelectedIndex = 0;
                }

                HelpFunc.LogMessage($"Loaded {filteredSheets.Count} enabled TextEq sheets into combobox");
                foreach (var sheet in filteredSheets)
                {
                    HelpFunc.LogMessage($"  - {sheet}");
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR loading Excel sheets: {ex.Message}");
                MessageBox.Show($"Error loading Excel sheets:\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetSheetsStartingWith(string filePath, string prefix)
        {
            List<string> filteredSheets = new List<string>();
            
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    HelpFunc.LogMessage($"Total worksheets in Excel file: {workbook.Worksheets.Count}");
                    
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        HelpFunc.LogMessage($"Checking sheet: '{worksheet.Name}' - Starts with '{prefix}': {worksheet.Name.StartsWith(prefix)}");
                        
                        if (worksheet.Name.StartsWith(prefix))
                        {
                            filteredSheets.Add(worksheet.Name);
                        }
                    }
                    
                    HelpFunc.LogMessage($"Filtered sheets starting with '{prefix}': {filteredSheets.Count}");
                    foreach (var sheet in filteredSheets)
                    {
                        HelpFunc.LogMessage($"  - {sheet}");
                    }
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR reading Excel sheets: {ex.Message}");
                throw;
            }

            return filteredSheets;
        }

        private Dictionary<string, List<string>> ReadTemplateXml(string filePath)
        {
            Dictionary<string, List<string>> templateDict = new Dictionary<string, List<string>>();

            XDocument doc = XDocument.Load(filePath);
            string currentTemplate = null;
            List<string> currentTemplateData = new List<string>();

            foreach (XElement elem in doc.Descendants())
            {
                if (elem.Name.LocalName == "templateName")
                {
                    if (currentTemplate != null)
                    {
                        templateDict[currentTemplate] = currentTemplateData;
                        currentTemplateData = new List<string>();
                    }
                    currentTemplate = elem.Value;
                }
                else if (elem.Name.LocalName == "tBody")
                {
                    currentTemplateData.Add(elem.Value.Trim());
                }
            }

            if (currentTemplate != null)
            {
                templateDict[currentTemplate] = currentTemplateData;
            }

            return templateDict;
        }

        private void btnGenerateText_Click(object sender, EventArgs e)
        {
            if (comboBoxSheets.SelectedItem == null)
            {
                MessageBox.Show("Please select a sheet first.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string selectedSheetName = comboBoxSheets.SelectedItem.ToString();
                HelpFunc.LogMessage($"=== Starting TextList generation for sheet: {selectedSheetName} ===");

                // Copy Excel file from T: to Z: (same as Generator does at start of Generate)
                string inputDrive = _config.CommonData.SharedInputDrive;
                string workDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string excelFileName = _config.CommonData.ExcelDataFileName;
                string excelSourcePath = Path.Combine(inputDrive, excelFileName);
                string excelDestinationPath = Path.Combine(workDirectory, "Tmp_" + excelFileName);
                
                HelpFunc.LogMessage($"Copying Excel file from T: to Z:");
                HelpFunc.LogMessage($"  Source: {excelSourcePath}");
                HelpFunc.LogMessage($"  Destination: {excelDestinationPath}");
                HelpFunc.CopyExcelFile(excelSourcePath, excelDestinationPath);
                
                // Update excelFilePath to point to the fresh copy
                excelFilePath = excelDestinationPath;

                // Read Excel data
                DataTable excelData = ReadExcelSheet(excelFilePath, selectedSheetName, skipRows: 3, maxRows: 1854);
                HelpFunc.LogMessage($"Excel sheet '{selectedSheetName}' read successfully. Rows: {excelData.Rows.Count}");

                // Get tagname from column 2 (index 1)
                string tagName = excelData.Rows.Count > 0 && excelData.Columns.Count > 1
                    ? excelData.Rows[0][1]?.ToString()
                    : null;
                HelpFunc.LogMessage($"TagName from column 2: {tagName}");

                if (string.IsNullOrEmpty(tagName))
                {
                    MessageBox.Show("TagName not found in column 2!", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create result strings list
                List<string> resultStrings = new List<string>();

                // Process Step names (column 7, index 7)
                if (templateDict.ContainsKey("tEqStep"))
                {
                    DataTable filteredDf = FilterDataTableByColumn(excelData, 7);
                    string templateString = string.Join("\n", templateDict["tEqStep"]);
                    List<string> stepResults = ReplaceTemplateStr(filteredDf, templateString);
                    resultStrings.AddRange(stepResults);
                    HelpFunc.LogMessage($"Processed {stepResults.Count} step entries");
                }

                // Process Transition names (column 8, index 8)
                if (templateDict.ContainsKey("tEqTrans"))
                {
                    DataTable filteredDf = FilterDataTableByColumn(excelData, 8);
                    string templateString = string.Join("\n", templateDict["tEqTrans"]);
                    List<string> transResults = ReplaceTemplateStr(filteredDf, templateString);
                    resultStrings.AddRange(transResults);
                    HelpFunc.LogMessage($"Processed {transResults.Count} transition entries");
                }

                // Process Condition names (column 9, index 9)
                if (templateDict.ContainsKey("tEqCond"))
                {
                    DataTable filteredDf = FilterDataTableByColumn(excelData, 9);
                    string templateString = string.Join("\n", templateDict["tEqCond"]);
                    List<string> condResults = ReplaceTemplateStr(filteredDf, templateString);
                    resultStrings.AddRange(condResults);
                    HelpFunc.LogMessage($"Processed {condResults.Count} condition entries");
                }

                // Process result strings into array
                List<string[]> arrayOfStrings = new List<string[]>();

                foreach (string str in resultStrings)
                {
                    string[] splitStrings = str.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    foreach (string value in splitStrings)
                    {
                        string[] splitValues = value.Split('\t');
                        arrayOfStrings.Add(splitValues);
                    }
                }

                // Sort array by second column (index 1) as integer
                var sortedArray = arrayOfStrings
                    .Where(x => x.Length > 1 && int.TryParse(x[1], out _))
                    .OrderBy(x => int.Parse(x[1]))
                    .ToList();

                HelpFunc.LogMessage($"Total entries after sorting: {sortedArray.Count}");

                // Write to text file
                string baseDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string outputDirectory = Path.Combine(baseDirectory, "TextEq", "Output", "v02");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    HelpFunc.LogMessage($"Created output directory: {outputDirectory}");
                }
                
                string outfile = Path.Combine(outputDirectory, SanitizeFileName(selectedSheetName) + ".txt");
                // Write without BOM (UTF-8 without byte order mark)
                File.WriteAllText(outfile, string.Join("\n", resultStrings), new UTF8Encoding(false));
                HelpFunc.LogMessage($"Output written to: {outfile}");

                // SQL operations
                string textlistSuffix = "_Eq";
                string textlistName = tagName + textlistSuffix;

                // Get SQL connection details from config
                using (SqlTools sTools = new SqlTools(
                    _config.SqlCommonData.Server, 
                    _config.SqlCommonData.Database, 
                    _config.SqlCommonData.Username, 
                    _config.SqlCommonData.Password))
                {
                    // Delete existing rows
                    HelpFunc.LogMessage($"Deleting existing TextList entries for: {textlistName}");
                    sTools.DeleteTextlistFromTable("dbo.TextLists", "TextList", textlistName);

                    // Insert/update sorted array
                    int processedCount = 0;
                    foreach (var lineValues in sortedArray)
                    {
                        if (lineValues.Length >= 9)
                        {
                            string listName = lineValues[0];
                            int textlistId = int.Parse(lineValues[1]);
                            string l1Text = lineValues[2];
                            string l2Text = lineValues[3];
                            string l3Text = lineValues[4];
                            string l4Text = lineValues[5];
                            string l5Text = lineValues[6];
                            string tagnameValue = lineValues[7];
                            bool inUse = lineValues[8] == "1";

                            sTools.InsertUpdateTextLists(listName, textlistId, l1Text, l2Text, 
                                l3Text, l4Text, l5Text, tagnameValue, inUse);
                            processedCount++;
                        }
                    }
                    
                    HelpFunc.LogMessage($"Inserted/updated {processedCount} TextList entries");
                }

                HelpFunc.LogMessage("=== TextList generation completed successfully ===");
                MessageBox.Show($"TextList generation completed!\n\nProcessed: {sortedArray.Count} entries\nOutput file: TextEq\\Output\\v02\\{Path.GetFileName(outfile)}\nDatabase updated: {textlistName}", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR during TextList generation: {ex.Message}");
                HelpFunc.LogMessage($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error generating TextList:\n\n{ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ReadExcelSheet(string filePath, string sheetName, int skipRows, int maxRows)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(sheetName);
                    if (worksheet == null)
                    {
                        throw new Exception($"Worksheet '{sheetName}' not found.");
                    }

                    // Python skiprows=range(1, 4) skips rows 1,2,3 (Excel rows 2,3,4) and row 0 is header (Excel row 1)
                    // So data starts from Excel row 5
                    // skipRows=3 means skip 3 rows after header, so startRow = 3 + 1 (header) + 1 (Excel 1-indexed) = 5
                    int startRow = skipRows + 2; // +1 for header row, +1 for Excel 1-indexed
                    int lastUsedRow = worksheet.LastRowUsed()?.RowNumber() ?? startRow;
                    int endRow = Math.Min(startRow + maxRows - 1, lastUsedRow);
                    int lastUsedCol = worksheet.LastColumnUsed()?.ColumnNumber() ?? 1;

                    // Add columns
                    for (int col = 1; col <= lastUsedCol; col++)
                    {
                        dt.Columns.Add($"Column{col}", typeof(string));
                    }

                    // Read data
                    for (int row = startRow; row <= endRow; row++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int col = 1; col <= lastUsedCol; col++)
                        {
                            var cell = worksheet.Cell(row, col);
                            dr[col - 1] = cell.IsEmpty() ? string.Empty : cell.Value.ToString();
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR reading Excel sheet: {ex.Message}");
                throw;
            }

            return dt;
        }

        private DataTable FilterDataTableByColumn(DataTable dt, int columnIndex)
        {
            DataTable filteredDt = dt.Clone();

            foreach (DataRow row in dt.Rows)
            {
                if (columnIndex < row.ItemArray.Length)
                {
                    string value = row[columnIndex]?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        filteredDt.ImportRow(row);
                    }
                }
            }

            return filteredDt;
        }

        private List<string> ReplaceTemplateStr(DataTable filteredDf, string templateString)
        {
            List<string> resultStr = new List<string>();

            // Iterate through each row in the DataTable
            foreach (DataRow row in filteredDf.Rows)
            {
                string updatedTemplateString = templateString;

                // Iterate through placeholders from $1 to max column index
                // Replace in reverse order to avoid issues with $1 being replaced before $10, etc.
                for (int j = filteredDf.Columns.Count; j >= 2; j--)
                {
                    string rowValue = row[j - 1]?.ToString() ?? string.Empty;
                    updatedTemplateString = updatedTemplateString.Replace($"${j}", rowValue);
                }

                resultStr.Add(updatedTemplateString);
            }

            return resultStr;
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove invalid file name characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder sb = new StringBuilder(fileName.Length);

            foreach (char c in fileName)
            {
                if (invalidChars.Contains(c) || c < 32)
                {
                    sb.Append('_');
                }
                else
                {
                    sb.Append(c);
                }
            }

            // Remove trailing spaces and dots
            return sb.ToString().TrimEnd(' ', '.');
        }

        private void btnReloadSheets_Click(object sender, EventArgs e)
        {
            try
            {
                HelpFunc.LogMessage("=== Reloading Excel sheets ===");
                
                // Copy Excel file from T: to Z: to get latest version
                string inputDrive = _config.CommonData.SharedInputDrive;
                string workDirectory = Path.Combine(_config.CommonData.SharedFolderPath, _config.CommonData.ProjectFolderName);
                string excelFileName = _config.CommonData.ExcelDataFileName;
                string excelSourcePath = Path.Combine(inputDrive, excelFileName);
                string excelDestinationPath = Path.Combine(workDirectory, "Tmp_" + excelFileName);
                
                HelpFunc.LogMessage($"Copying Excel file:");
                HelpFunc.LogMessage($"  From: {excelSourcePath}");
                HelpFunc.LogMessage($"  To: {excelDestinationPath}");
                HelpFunc.CopyExcelFile(excelSourcePath, excelDestinationPath);
                
                // Update the path
                excelFilePath = excelDestinationPath;
                
                // Now load the sheets
                LoadExcelSheets();
                
                HelpFunc.LogMessage("Excel sheets reloaded successfully");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"ERROR reloading sheets: {ex.Message}");
                MessageBox.Show($"Error reloading sheets:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
