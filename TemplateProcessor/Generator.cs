using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TemplateProcessor
{
    public partial class Generator
    {
        private readonly TextBox txtLog;
        private readonly PathsConfig _pathsConfig;

        public Generator(PathsConfig pathsConfig)
        {
            _pathsConfig = pathsConfig;
        }

        public void Generate()
        {
            var globalData = GlobalData.Instance;

            HelpFunc fileHelper = new HelpFunc();
            fileHelper.CopyExcelFile(globalData.SourceFilePath, globalData.DestinationFilePath);
            // Path to the copy of ExcelData
            string excelPath = globalData.DestinationFilePath;
            // Create a dictionary to store enabled tasks by ObjType
            Dictionary<string, List<Task>> enabledTasksDict = new Dictionary<string, List<Task>>();

            // Get GenData settings
            string GenDataFilePath = Path.Combine(globalData.SharedFolderPath, globalData.GenerateSettingFileName);

            // Process tasks to populate the dictionary
            if (globalData.GenData?.Tasks != null)
            {
                foreach (var task in globalData.GenData.Tasks)
                {
                    if (task.Enabled == 1 && task.Templates != null)
                    {
                        if (!enabledTasksDict.ContainsKey(task.ObjType))
                        {
                            enabledTasksDict[task.ObjType] = new List<Task>();
                        }
                        var enabledTemplates = task.Templates.Where(t => t.Enabled == 1).ToList();
                        if (enabledTemplates.Count > 0)
                        {
                            enabledTasksDict[task.ObjType].Add(new Task
                            {
                                ObjType = task.ObjType,
                                Enabled = task.Enabled,
                                Templates = enabledTemplates,
                                StartRow = task.StartRow,
                                EndRow = task.EndRow
                            });
                        }
                    }
                }
            }

            // Next step: loop all enabled templates
            foreach (var keyValuePair in enabledTasksDict)
            {
                string objType = keyValuePair.Key;
                List<Task> valueTask = keyValuePair.Value;

                HelpFunc.LogMessage($"\nComponents for {objType}:");
                foreach (var task in valueTask)
                {
                    foreach (var template in task.Templates)
                    {
                        HelpFunc.LogMessage($"ObjType: {task.ObjType}, Templates: {template.Name}, StartRow: {task.StartRow}, EndRow: {task.EndRow}");
                        HelpFunc.LogMessage($"The path to the Excel file is: {excelPath}");

                        // Read Excel file
                        List<string[]> valuesArray = fileHelper.ReadExcelFile(excelPath, task.ObjType, task.StartRow, task.EndRow);

                        if (valuesArray != null && valuesArray.Count > 0)
                        {
                            for (int i = 0; i < valuesArray.Count; i++)
                            {
                                HelpFunc.LogMessage($"Row {i + 1}: {string.Join(", ", valuesArray[i])}");
                            }
                        }
                        else
                        {
                            HelpFunc.LogMessage("No data read from Excel file.");
                        }

                        // Read template
                        try
                        {
                            var paths = _pathsConfig.ProcessPath(task.ObjType, template.Name, template.Version, template.Comment);

                            // Get input and output path for template
                            string templatePath = paths.TemplatePaths.Count > 0 ? paths.TemplatePaths[template.TemplatePathIndex] : string.Empty;
                            string outputPath = paths.OutputPaths.Count > 0 ? paths.OutputPaths[template.OutputPathIndex] : string.Empty;

                            string templateContent = string.Empty; // Initialize templateContent
                            string fullTemplatePath = Path.Combine(GlobalData.Instance.GenDataFolderPath, templatePath);

                            if (File.Exists(fullTemplatePath))
                            {
                                // Read the template file
                                using (StreamReader reader = new StreamReader(fullTemplatePath))
                                {
                                    templateContent = reader.ReadToEnd();
                                }

                                // If the version is "v00", apply specific replacements
                                if (template.Version == "v00")
                                {
                                    templateContent = templateContent.Replace("[HOVED]", "<ROOT>")
                                                                     .Replace("[DETALJER]", "<DETAILS>")
                                                                     .Replace("[FOD]", "</DETAILS>\n</ROOT>");
                                }
                            }
                            else
                            {
                                // If the file does not exist, log the message
                                HelpFunc.LogMessage($"Error: File not found: {fullTemplatePath}\n");
                            }

                            // Continue with the rest of the template processing
                            var rootRegex = new Regex(@"<ROOT>(.*?)</ROOT>", RegexOptions.Singleline);
                            var rootMatch = rootRegex.Match(templateContent);
                            if (!rootMatch.Success)
                            {
                                HelpFunc.LogMessage("Error: <ROOT> section is mandatory and missing.");
                                return;
                            }

                            string wholeTemplate = rootMatch.Groups[1].Value;
                            string headerSection = GetSection(wholeTemplate, "HEADER");
                            string detailSection = GetSection(wholeTemplate, "DETAILS");
                            string footSection = GetSection(wholeTemplate, "FOOT");

                            string processedHeader = !string.IsNullOrEmpty(headerSection)
                                ? ReplaceSection(headerSection, "HEADER", headerSection, valuesArray[0])
                                : string.Empty;

                            System.Text.StringBuilder processedDetail = new System.Text.StringBuilder();
                            if (!string.IsNullOrEmpty(detailSection))
                            {
                                foreach (var rowData in valuesArray)
                                {
                                    HelpFunc.LogMessage($"Processing detail row: {string.Join(", ", rowData)}"); // Debugging line
                                    string processedLine = ProcessLine(detailSection, rowData);
                                    processedDetail.AppendLine(processedLine); // Append processed line directly
                                }
                            }

                            string processedFoot = !string.IsNullOrEmpty(footSection)
                                ? ReplaceSection(footSection, "FOOT", footSection, valuesArray[0])
                                : string.Empty;

                            string outputContent = ReplaceSection(wholeTemplate, "HEADER", processedHeader, valuesArray[0]);
                            // Use the keyword "DETAIL" or "DETAILS" based on what was found earlier
                            outputContent = ReplaceSection(outputContent, "DETAILS", processedDetail.ToString(), valuesArray[0]);
                            outputContent = ReplaceSection(outputContent, "FOOT", processedFoot, valuesArray[0]);
                            outputContent = RemoveControlWordLines(outputContent);

                            string fullOutputPath = Path.Combine(globalData.GenDataFolderPath, outputPath);
                            File.WriteAllText(fullOutputPath, outputContent, new System.Text.UnicodeEncoding(false, true));

                            // SQL function
                            SqlTools sqlTools = new SqlTools(globalData.Server, globalData.Database, globalData.Username, globalData.Password);

                            if (template.Commands != null)
                            {
                                foreach (var command in template.Commands)
                                {
                                    if (command.Enabled == 1)
                                    {
                                        var lines = outputContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                                        foreach (var line in lines)
                                        {
                                            var columns = line.Split('\t');
                                            if (columns.Length == 9)
                                            {
                                                string sqlTextListName = columns[0];
                                                int id = int.Parse(columns[1]);
                                                bool inUseValue = columns[8] == "1";

                                                // Inserting/Updating text lists
                                                sqlTools.InsertUpdateTextLists(
                                                    textlistName: sqlTextListName,
                                                    textlistId: id,
                                                    l1Text: columns[2],
                                                    l2Text: columns[3],
                                                    l3Text: columns[4],
                                                    l4Text: columns[5],
                                                    l5Text: columns[6],
                                                    tagName: columns[7],
                                                    inUse: inUseValue,
                                                    txtLog
                                                );
                                            }
                                        }
                                    }
                                }
                            }

                            // For StartRow to EndRow, delete by calculated ID from SQL if InUse=0 in Excel
                            if (valuesArray != null && valuesArray.Count > 0)
                            {
                                foreach (var taskItem in enabledTasksDict[objType])
                                {
                                    foreach (var templateItem in taskItem.Templates)
                                    {
                                        if (templateItem?.Commands != null)
                                        {
                                            foreach (var commandItem in templateItem.Commands)
                                            {
                                                if (commandItem.Enabled == 1)
                                                {
                                                    for (int i = 0; i < valuesArray.Count; i++)
                                                    {
                                                        string[] row = valuesArray[i];
                                                        if (row.Length > 59)
                                                        {
                                                            if (row[55].Equals("0"))
                                                            {
                                                                sqlTools.DeleteFromTableById("TextLists", "TextList", commandItem.Parameter, "ID", int.Parse(row[2]) * 10 + 1, txtLog);
                                                            }
                                                            if (row[56].Equals("0"))
                                                            {
                                                                sqlTools.DeleteFromTableById("TextLists", "TextList", commandItem.Parameter, "ID", int.Parse(row[2]) * 10 + 2, txtLog);
                                                            }
                                                            if (row[57].Equals("0"))
                                                            {
                                                                sqlTools.DeleteFromTableById("TextLists", "TextList", commandItem.Parameter, "ID", int.Parse(row[2]) * 10 + 3, txtLog);
                                                            }
                                                            if (row[58].Equals("0"))
                                                            {
                                                                sqlTools.DeleteFromTableById("TextLists", "TextList", commandItem.Parameter, "ID", int.Parse(row[2]) * 10 + 4, txtLog);
                                                            }
                                                            if (row[59].Equals("0"))
                                                            {
                                                                sqlTools.DeleteFromTableById("TextLists", "TextList", commandItem.Parameter, "ID", int.Parse(row[2]) * 10 + 5, txtLog);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            HelpFunc.LogMessage($"Row {i + 1} does not have 59 elements.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                HelpFunc.LogMessage("No data read from Excel file.");
                            }
                            HelpFunc.LogMessage($"Template processing completed. Check '{fullOutputPath}' for the results.");
                        }
                        catch (FileNotFoundException fnfe)
                        {
                            // Log the exception message to txtLog
                            HelpFunc.LogMessage($"File not found: {fnfe.Message}\n");
                        }
                        catch (Exception ex)
                        {
                            // Handle other exceptions
                            HelpFunc.LogMessage($"An error occurred: {ex.Message}\n");
                            HelpFunc.LogMessage($"{ex.StackTrace}\n");
                        }
                    }
                }
            }
        }

        // Other helper methods remain unchanged
        private string GetSection(string content, string sectionName)
        {
            var sectionRegex = new Regex($@"<{sectionName}>(.*?)</{sectionName}>", RegexOptions.Singleline);
            var match = sectionRegex.Match(content);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private string ReplaceSection(string content, string sectionName, string replacementContent, string[] valuesArray)
        {
            var sectionRegex = new Regex($@"<{sectionName}>(.*?)</{sectionName}>", RegexOptions.Singleline);
            if (sectionRegex.IsMatch(content))
            {
                replacementContent = HandleIfConditions(replacementContent, valuesArray);
                replacementContent = ProcessLine(replacementContent, valuesArray);
                return sectionRegex.Replace(content, replacementContent);
            }
            return content;
        }

        private string RemoveControlWordLines(string content)
        {
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var filteredLines = new List<string>();

            // DETAIL for vbs version, DETAILS for v01
            var controlWordsRegex = new Regex(@"<(ROOT|HEADER|DETAIL|DETAILS|FOOT)>|</(ROOT|HEADER|DETAIL|DETAILS|FOOT)>|<IF\s+.*?>|</IF>");

            foreach (var line in lines)
            {
                // Remove lines that match control words and skip empty lines
                if (!controlWordsRegex.IsMatch(line) && !string.IsNullOrWhiteSpace(line))
                {
                    filteredLines.Add(line);
                }
            }
            return string.Join(Environment.NewLine, filteredLines);
        }

        private string ProcessLine(string input, string[] valuesArray)
        {
            // First pass: Replace $index with values
            string replacedInput = Regex.Replace(input, @"\$\d+", match =>
            {
                int index = int.Parse(match.Value.Substring(1)) - 1;
                return valuesArray.Length > index ? valuesArray[index] : match.Value;
            });

            // Second pass: Evaluate expressions
            string finalOutput = MyRegex().Replace(replacedInput, match =>
            {
                var result = new DataTable().Compute(match.Value, null);
                return result.ToString();
            });

            return finalOutput;
        }


    }

    [GeneratedRegex(@"\d+([*/+-]\d+)+")]
        private static partial Regex MyRegex();
    }
}