using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace TemplateProcessor
{
    public class DiscoveredSheetsConfig
    {
        public DateTime DiscoveredDate { get; set; }
        public string ExcelFileName { get; set; }
        public List<SheetInfo> Sheets { get; set; }

        /// <summary>
        /// Read all sheet names from the Excel file
        /// </summary>
        public static List<string> GetExcelSheetNames(string excelFilePath)
        {
            var sheetNames = new List<string>();

            try
            {
                using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        foreach (var worksheet in workbook.Worksheets)
                        {
                            sheetNames.Add(worksheet.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading Excel sheet names: {ex.Message}", ex);
            }

            return sheetNames;
        }

        /// <summary>
        /// Discover sheets from Excel and save/update JSON file
        /// If file exists, update with new sheets and preserve existing ComponentGroup values
        /// </summary>
        public static void DiscoverAndSaveSheets(CommonConfig config)
        {
            try
            {
                // Build paths (same as Generator.cs)
                string excelSourcePath = Path.Combine(
                    config.CommonData.SharedInputDrive,
                    config.CommonData.ExcelDataFileName
                );

                string projectFolder = Path.Combine(
                    config.CommonData.SharedFolderPath,
                    config.CommonData.ProjectFolderName
                );

                string excelDestinationPath = Path.Combine(
                    projectFolder,
                    "Tmp_" + config.CommonData.ExcelDataFileName
                );

                HelpFunc.LogMessage($"=== Starting Sheet Discovery ===");
                HelpFunc.LogMessage($"Source: {excelSourcePath}");
                HelpFunc.LogMessage($"Destination: {excelDestinationPath}");

                // Copy Excel file (so we can read it even if original is open)
                HelpFunc.CopyExcelFile(excelSourcePath, excelDestinationPath);

                // Check if Tmp_ file exists
                if (!File.Exists(excelDestinationPath))
                {
                    throw new Exception($"Failed to copy Excel file to: {excelDestinationPath}");
                }

                // Get sheet names from Tmp_ file
                var sheetNames = GetExcelSheetNames(excelDestinationPath);

                // Output path for JSON
                string outputPath = Path.Combine(projectFolder, "discoveredSheets.json");

                DiscoveredSheetsConfig discoveredConfig;

                // Check if file already exists
                if (File.Exists(outputPath))
                {
                    // Load existing config
                    var existingJson = File.ReadAllText(outputPath);
                    var existingConfig = JsonConvert.DeserializeObject<DiscoveredSheetsConfig>(existingJson);

                    // Create dictionary of existing sheets for quick lookup
                    var existingSheets = existingConfig.Sheets.ToDictionary(s => s.SheetName);

                    // Update config
                    discoveredConfig = new DiscoveredSheetsConfig
                    {
                        DiscoveredDate = DateTime.Now,
                        ExcelFileName = config.CommonData.ExcelDataFileName,
                        Sheets = new List<SheetInfo>()
                    };

                    // Add all sheets from Excel
                    foreach (var sheetName in sheetNames)
                    {
                        if (existingSheets.ContainsKey(sheetName))
                        {
                            // Keep existing sheet with its ComponentGroup value
                            discoveredConfig.Sheets.Add(existingSheets[sheetName]);
                        }
                        else
                        {
                            // Add new sheet with empty ComponentGroup
                            discoveredConfig.Sheets.Add(new SheetInfo
                            {
                                SheetName = sheetName,
                                IsEnabled = false,
                                ComponentGroup = ""
                            });
                        }
                    }

                    HelpFunc.LogMessage($"Updated discoveredSheets.json with {sheetNames.Count} sheets (preserved existing ComponentGroup values)");
                }
                else
                {
                    // Create new config
                    discoveredConfig = new DiscoveredSheetsConfig
                    {
                        DiscoveredDate = DateTime.Now,
                        ExcelFileName = config.CommonData.ExcelDataFileName,
                        Sheets = sheetNames.Select(name => new SheetInfo
                        {
                            SheetName = name,
                            IsEnabled = false,
                            ComponentGroup = ""
                        }).ToList()
                    };

                    HelpFunc.LogMessage($"Created discoveredSheets.json with {sheetNames.Count} sheets");
                }

                // Save to JSON
                string json = JsonConvert.SerializeObject(discoveredConfig, Formatting.Indented);
                File.WriteAllText(outputPath, json);

                HelpFunc.LogMessage($"=== Sheet Discovery Completed ===");
                HelpFunc.LogMessage($"Sheets found: {sheetNames.Count}");
                HelpFunc.LogMessage($"Saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage($"=== Sheet Discovery Error ===");
                HelpFunc.LogMessage($"Error: {ex.Message}");
                HelpFunc.LogMessage($"Stack: {ex.StackTrace}");
            }
        }
    }

    public class SheetInfo
    {
        public string SheetName { get; set; }
        public bool IsEnabled { get; set; }
        public string ComponentGroup { get; set; }
    }
}
