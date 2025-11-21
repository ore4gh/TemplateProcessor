using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Globalization;

namespace TemplateProcessor.Generator
{

    public partial class GeneratorEngine
    {
        private GenData _genData;
        private readonly CommonConfig _commonConfig;
        private readonly string _fileNameConfig;

        public GeneratorEngine(string fileNameConfig, CommonConfig commonConfig, TextBox txtLog)
        {
            _fileNameConfig = fileNameConfig;
            _commonConfig = commonConfig;

            // Initialize GenData or other properties using _commonConfig if needed
            InitializeGenData();
        }

        private void InitializeGenData()
        {
            string genDataFilePath = Path.Combine(
                _commonConfig.CommonData.SharedFolderPath,
                _commonConfig.CommonData.ProjectFolderName,
                _fileNameConfig
            );

            if (File.Exists(genDataFilePath))
            {
                string json = File.ReadAllText(genDataFilePath);
                _genData = JsonSerializer.Deserialize<GenData>(json);
            }
            else
            {
                // Handle file not found scenario
                throw new FileNotFoundException("GenData file not found.", genDataFilePath);
            }
        }

        public GenData GetGeneratorData()
        {
            return _genData;
        }
        
        /// <summary>
        /// Reloads the GenData configuration from the JSON file
        /// </summary>
        public void ReloadGenData()
        {
            string genDataFilePath = Path.Combine(
                _commonConfig.CommonData.SharedFolderPath,
                _commonConfig.CommonData.ProjectFolderName,
                _fileNameConfig
            );

            if (File.Exists(genDataFilePath))
            {
                string json = File.ReadAllText(genDataFilePath);
                _genData = JsonSerializer.Deserialize<GenData>(json);
                HelpFunc.LogMessage("GenData configuration reloaded successfully.");
            }
            else
            {
                // Handle file not found scenario
                throw new FileNotFoundException("GenData file not found.", genDataFilePath);
            }
        }
        private (string[] versions, string[] extNames, string[] separators) ExtractAndSplitPathVariables(PathVariables pathVariables)
        {
            var versions = pathVariables.Version.Split(';');
            var extNames = pathVariables.ExtName.Split(';');
            var separators = pathVariables.Separator.Split(';');
            return (versions, extNames, separators);
        }
        private string ReplaceDataInTemplate(string templateContent, List<string[]> valuesArray, string outputPath)
        {
            // Continue with the rest of the template processing
            var rootRegex = new Regex(@"<ROOT>(.*?)</ROOT>", RegexOptions.Singleline);
            var rootMatch = rootRegex.Match(templateContent);
            if (!rootMatch.Success)
            {
                HelpFunc.LogMessage("Error: <ROOT> section is mandatory and missing.");
                return null;
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
                    //HelpFunc.LogMessage($"Processing detail row: {string.Join(", ", rowData)}"); // Debugging line
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

            string fullOutputPath = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName, outputPath);
            // Ensure the directory exists
            string? directoryPath = Path.GetDirectoryName(fullOutputPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            // Ensure the file exists (create an empty one if it doesn't)
            if (!File.Exists(fullOutputPath))
            {
                File.WriteAllText(fullOutputPath, string.Empty); // Creates an empty file
            }
            File.WriteAllText(fullOutputPath, outputContent, new System.Text.UnicodeEncoding(false, true));
            return outputContent;
        }
        // Method to perform the generation process
        public void Generate(string objType, int sRow, int eRow)
        {
            // Reload the JSON data to ensure it's up-to-date
            ReloadGenData();


            HelpFunc.LogMessage($"Generation process started for selected {objType}.");
            string inputDrive = _commonConfig.CommonData.SharedInputDrive;
            string workDirectory = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName);
            string excelFileName = _commonConfig.CommonData.ExcelDataFileName;
            string excelDestinationPath = Path.Combine(workDirectory, "Tmp_" + excelFileName);
            // Copy excel file
            //string excelSourcePath = Path.Combine(workDirectory, _commonConfig.CommonData.ExcelDataFileName);
            string excelSourcePath = Path.Combine(inputDrive, _commonConfig.CommonData.ExcelDataFileName);
            HelpFunc.CopyExcelFile(excelSourcePath, excelDestinationPath);

            // Get ObjTypeDetails from _genData
            var details = _genData.GetObjTypeDetails(objType);

            if (details == null || details.Enabled == 0)
            {
                HelpFunc.LogMessage($"No enabled task found for ObjType: {objType}");
                return;
            }
            //replace start/end rows
            details.StartRow = sRow;
            details.EndRow = eRow;


            List<string[]> valuesArray = HelpFunc.ReadExcelFile(excelDestinationPath, objType, details.StartRow, details.EndRow);

            if (valuesArray != null && valuesArray.Count > 0)
            {


                HelpFunc.LogMessage($"Total readed rows from Excel {valuesArray.Count}");

            }
            else
            {
                HelpFunc.LogMessage("No data read from Excel file.");
            }
            //Get TEXTref sheet where textlib references are located
            List<string[]> valuesTextRefArray = HelpFunc.ReadExcelFile(excelDestinationPath, "TEXTref", 5,null);

            // Create the dictionary to store TEXTref values
            Dictionary<string, string[]> DictionaryTextRef = new Dictionary<string, string[]>();

            // Loop through each row in the valuesTextRefArray
            foreach (var row in valuesTextRefArray)
            {
                // Make sure the row has enough columns to access the necessary indices
                if (row.Length > 6)
                {
                    // The key is from column 0
                    string key = row[0];

                    // The descriptions are from columns 4, 5, and 6 (descEn, descDa, descNo)
                    string[] descriptions = new string[] { row[4], row[5], row[6] };

                    // Add the key and its associated descriptions to the dictionary
                    DictionaryTextRef[key] = descriptions;
                }
            }


            // Get enabled included categories for the ActiveTask and selected objType
            var includedCategories = _genData.GetEnabledIncludedCategoriesForActiveTask(objType);

            // Get paths for the active task and selected objType
            var pathCategories = _genData.GetObjTypePathsForActiveTask(_genData.ActiveTask, objType);

            //loop for each enabled category
            foreach (var category in pathCategories)
            {
                string categoryName = category.Key;
                PathTemplate pathTemplate = category.Value;

                // Check if at least one rule path exists
                string rulePath = null;
                foreach (var rulePathCandidate in pathTemplate.RulePath.Split(';'))
                {
                    string fullRulePath = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName, rulePathCandidate);

                    if (File.Exists(fullRulePath))
                    {
                        rulePath = fullRulePath;
                        break;
                    }
                }

                if (rulePath != null)
                {
                    // Check if there are any records in valuesArray
                    if (valuesArray == null || valuesArray.Count == 0)
                    {
                        HelpFunc.LogMessage("Input data from Excel has 0 records. Check Start and End row count");
                        return;
                    }

                    // Read template content
                    string templateContent;
                    using (StreamReader reader = new StreamReader(rulePath))
                    {
                        templateContent = reader.ReadToEnd();
                    }
                    HelpFunc.LogMessage($"Template content read from: {rulePath}");

                    string outputContent = ReplaceDataInTemplate(templateContent, valuesArray, pathTemplate.OutputPath);

                    // Command processing
                    List<string> commands = _genData.GetCommandListForCategory(categoryName);
                    List<string> cmdParameters = null;

                    foreach (var cmd in commands)
                    {
                        if (cmd == "GetTextRef")
                        {
                            GetTextRef(ref outputContent, DictionaryTextRef);
                            string fullOutputPath = Path.Combine(
                                _commonConfig.CommonData.SharedFolderPath,
                                _commonConfig.CommonData.ProjectFolderName,
                                pathTemplate.OutputPath
                            );
                            File.WriteAllText(fullOutputPath, outputContent, new System.Text.UnicodeEncoding(false, true));
                        }
                        if (cmd == "InsertUpdateIlockTextList")
                        {
                            InsertUpdateIlockTextList(outputContent, valuesArray, categoryName);
                        }
                        if (cmd == "DeleteIlockTextList")
                        {
                            cmdParameters = _genData.GetCommandParameters(cmd, objType);
                            DeleteIlockTextList(cmdParameters[0], valuesArray);
                        }
                    }
                }
                else
                {
                    HelpFunc.LogMessage($"Template not found");
                }



            }
        }
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
            // Join the filtered lines and ensure the result ends with a newline
            var result = string.Join(Environment.NewLine, filteredLines);

            // Ensure the result ends with a newline
            if (!result.EndsWith(Environment.NewLine))
            {
                result += Environment.NewLine;
            }

            return result;
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
        private string HandleIfConditions(string replacementContent, string[] valuesArray)
        {
            // Supports:
            // <IF $2 IN(11,12)>...</IF>        // spaces allowed between LHS and operator
            // <IF 22 IN(11,21)>...</IF>
            // <IF $3=5>...</IF>, <IF Sub!=X>...</IF>, <IF 7>5>...</IF>
            var ifRegex = new Regex(
                @"<IF\s+" +
                @"(?:(?<direct>\d+)|\$(?<col>\d+)|(?<ident>[A-Za-z_]\w*))" +  // LHS
                @"\s*" +                                                     // <-- OPTIONAL space before operator
                @"(?:" +
                    @"(?<op>=|!=|>|<)\s*(?<rhs>[^\s>]+)" +                   // comparison branch
                    @"|" +
                    @"(?i:IN)\((?<inlist>\d+(?:,\d+){0,9})\)" +              // IN(...) branch (no spaces inside)
                @")\s*>" +
                @"(?<inner>.*?)</IF>",
                RegexOptions.Singleline | RegexOptions.CultureInvariant
            );

            return ifRegex.Replace(replacementContent, match =>
            {
                // LHS resolution
                string lhs = null;

                if (match.Groups["direct"].Success)
                {
                    lhs = match.Groups["direct"].Value;
                }
                else if (match.Groups["col"].Success)
                {
                    if (int.TryParse(match.Groups["col"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int col) && col > 0)
                    {
                        int idx = col - 1;
                        if (idx >= 0 && idx < valuesArray.Length)
                            lhs = valuesArray[idx];
                    }
                }
                else if (match.Groups["ident"].Success)
                {
                    lhs = match.Groups["ident"].Value;
                }

                if (lhs is null) return string.Empty;
                lhs = lhs.Trim();

                string inner = match.Groups["inner"].Value;

                // IN(...) handling
                if (match.Groups["inlist"].Success)
                {
                    var tokens = match.Groups["inlist"].Value.Split(','); // digits only, 1..10
                    if (double.TryParse(lhs, NumberStyles.Float, CultureInfo.InvariantCulture, out var ln))
                    {
                        var set = tokens.Select(t => double.Parse(t, CultureInfo.InvariantCulture)).ToHashSet();
                        return set.Contains(ln) ? inner : string.Empty;
                    }
                    else
                    {
                        // fallback string membership (exact)
                        var setStr = tokens.ToHashSet(StringComparer.Ordinal);
                        return setStr.Contains(lhs) ? inner : string.Empty;
                    }
                }

                // =, !=, <, > handling (same semantics you had)
                if (match.Groups["op"].Success)
                {
                    string op = match.Groups["op"].Value;
                    string rhs = match.Groups["rhs"].Value;

                    bool conditionMet = false;

                    if (double.TryParse(lhs, NumberStyles.Float, CultureInfo.InvariantCulture, out var ln) &&
                        double.TryParse(rhs, NumberStyles.Float, CultureInfo.InvariantCulture, out var rn))
                    {
                        conditionMet = op switch
                        {
                            "=" => ln == rn,
                            "!=" => ln != rn,
                            ">" => ln >  rn,
                            "<" => ln <  rn,
                            _ => false
                        };
                    }
                    else
                    {
                        // string compare for '=' / '!=' only (case-insensitive)
                        conditionMet = op switch
                        {
                            "=" => string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase),
                            "!=" => !string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase),
                            _ => false
                        };
                    }

                    return conditionMet ? inner : string.Empty;
                }

                return string.Empty;
            });
        }




        private string GetPath(List<string> paths, int index)
        {
            // -1 index return empty string
            if (index == -1 || index >= paths.Count)
            {
                return string.Empty;
            }
            return paths[index];
        }

        public void GetTextRef(ref string outputContent, Dictionary<string, string[]> DictionaryTextRef)
        {
            // Split outputContent into lines and remove empty lines
            var lines = outputContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Identify header lines
            var headerLines = new List<string>();
            int dataStartIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], @"^\d+\t")) // Line starts with numbers and a tab
                {
                    dataStartIndex = i; // First data line detected
                    break;
                }
                headerLines.Add(lines[i].TrimEnd()); // Collect header lines
            }

            // Dictionary to track unique lines by key
            var uniqueLines = new Dictionary<string, string>();

            // Process each data line
            for (int i = dataStartIndex; i < lines.Length; i++)
            {
                var parts = lines[i].Split('\t'); // Assuming tab-separated values
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(); // Extract the key (first value in the row)

                    // Check if the key exists in the DictionaryTextRef
                    if (DictionaryTextRef.TryGetValue(key, out string[] descriptions))
                    {
                        // Correct order: Danish | English | Norwegian
                        lines[i] = $"{key}\t{descriptions[1]}\t{descriptions[0]}\t{descriptions[2]}";
                    }

                    // Add to the dictionary to ensure uniqueness
                    uniqueLines[key] = lines[i];
                }
            }

            // Sort by key and combine the result
            var sortedData = uniqueLines.Values.OrderBy(line => line.Split('\t')[0]).ToList();

            // Combine all lines back into outputContent with proper formatting
            outputContent = string.Join(Environment.NewLine, headerLines.Concat(sortedData)) + Environment.NewLine;
        }


        public void InsertUpdateIlockTextList(string outputContent, List<string[]> valuesArray, string category)
        {
            using (SqlTools sqlTools = new SqlTools(_commonConfig.SqlCommonData.Server, _commonConfig.SqlCommonData.Database, _commonConfig.SqlCommonData.Username, _commonConfig.SqlCommonData.Password))
            {
                // Insert and Update textlist
                string sqlTextListName = null;
                var lines = outputContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    var columns = line.Split('\t');
                    if (columns.Length == 9)
                    {
                        sqlTextListName = columns[0];
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
                            inUse: inUseValue
                        );
                    }
                }
            }
        }
        public void DeleteIlockTextList(string SqlTextListName,List<string[]> valuesArray)
        {
            using (SqlTools sqlTools = new SqlTools(_commonConfig.SqlCommonData.Server, _commonConfig.SqlCommonData.Database, _commonConfig.SqlCommonData.Username, _commonConfig.SqlCommonData.Password))
            {
                // Delete not used rows from sql (for interlock textlists only. Column index started with 0)
                if (valuesArray != null && valuesArray.Count > 0  && SqlTextListName != null)
                {
                    for (int i = 0; i < valuesArray.Count; i++)
                    {
                        string[] column = valuesArray[i];
                        if (column.Length > 54)
                        {
                            if (column[51-1].Equals("0"))
                            {
                                sqlTools.DeleteFromTableById("TextLists", "TextList", SqlTextListName, "ID", int.Parse(column[2]) * 10 + 1);
                            }
                            if (column[52-1].Equals("0"))
                            {
                                sqlTools.DeleteFromTableById("TextLists", "TextList", SqlTextListName, "ID", int.Parse(column[2]) * 10 + 2);
                            }
                            if (column[53-1].Equals("0"))
                            {
                                sqlTools.DeleteFromTableById("TextLists", "TextList", SqlTextListName, "ID", int.Parse(column[2]) * 10 + 3);
                            }
                            if (column[54-1].Equals("0"))
                            {
                                sqlTools.DeleteFromTableById("TextLists", "TextList", SqlTextListName, "ID", int.Parse(column[2]) * 10 + 4);
                            }
                            if (column[55-1].Equals("0"))
                            {
                                sqlTools.DeleteFromTableById("TextLists", "TextList", SqlTextListName, "ID", int.Parse(column[2]) * 10 + 5);
                            }
                        }
                        else
                        {
                            HelpFunc.LogMessage($"Row {i + 1} does not have 55 elements.");
                        }
                    }
                }
            }
        }
        // This regex matches arithmetic expressions like "2700+2000" or "100-50"
        // but ONLY if they are immediately followed by a space or tab character.
        // 
        // It does not care what precedes the expression — it may be a letter, comma, or nothing.
        // However, the trailing space or tab ensures that it doesn't accidentally match embedded
        // parts of strings like "GX300-10Q4", which have no whitespace after the numeric expression.

        [GeneratedRegex(@"\d+(?:[*/+-]\d+)+(?=[ \t])")]
        private static partial Regex MyRegex();

        /// <summary>
        /// Updates the StartRow and EndRow values for a specific ObjType in the underlying dictionary
        /// </summary>
        public void UpdateObjTypeRows(string objType, int startRow, int endRow)
        {
            if (_genData.ObjTypes.TryGetValue(objType, out int[] details))
            {
                // Update the array: [Enabled, StartRow, EndRow]
                details[1] = startRow;
                details[2] = endRow;
                HelpFunc.LogMessage($"Updated ObjType '{objType}': StartRow={startRow}, EndRow={endRow}");
            }
            else
            {
                HelpFunc.LogMessage($"ObjType '{objType}' not found in configuration.");
            }
        }

        /// <summary>
        /// Saves the current GenData configuration back to the JSON file
        /// </summary>
        public void SaveGenDataConfig()
        {
            string genDataFilePath = Path.Combine(
                _commonConfig.CommonData.SharedFolderPath,
                _commonConfig.CommonData.ProjectFolderName,
                _fileNameConfig
            );

            try
            {
                // Serialize with formatting for readability
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(_genData, options);
                File.WriteAllText(genDataFilePath, json);

                HelpFunc.LogMessage($"Configuration saved successfully to: {genDataFilePath}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"Error saving configuration: {ex.Message}");
                throw;
            }
        }

    }

}