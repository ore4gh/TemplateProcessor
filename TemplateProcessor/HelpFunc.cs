using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TemplateProcessor
{
    public class HelpFunc
    {
        private static TextBox? _txtLog;

        // Method to initialize the log TextBox from MainForm
        public static void Initialize(TextBox txtLog)
        {
            _txtLog = txtLog;
        }

        // Log messages
        public static void LogMessage(string message)
        {
            if (_txtLog != null)
            {
                _txtLog.AppendText($" {message}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine(message); // Fallback to console logging if _txtLog is not set
            }
        }

        public static bool SaveGeneratorData(string filePath, GeneratorData generatorData)
        {
            try
            {
                var configJson = JsonSerializer.Serialize(generatorData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, configJson);
                LogMessage("Configuration saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage($"Error saving configuration: {ex.Message}");
                return false;
            }
        }

        public static GeneratorData? LoadGeneratorDataFromJson(string filePath)
        {
            try
            {
                var genJson = File.ReadAllText(filePath);
                var GenData = JsonSerializer.Deserialize<GeneratorData>(genJson);
                LogMessage("Generator data loaded successfully.");
                return GenData;
            }
            catch (Exception ex)
            {
                LogMessage($"Error loading generator data: {ex.Message}");
                return null;
            }
        }

        public static void CopyExcelFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                if (File.Exists(sourceFilePath))
                {
                    File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                    LogMessage($"File copied from {sourceFilePath} to {destinationFilePath}");
                }
                else
                {
                    LogMessage($"Source file {sourceFilePath} does not exist.");
                }
            }
            catch (IOException ioEx)
            {
                LogMessage($"IO Exception: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                LogMessage($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Reads data from an Excel file while applying filtering rules.
        /// </summary>
        /// <param name="excelPath">The full path to the Excel file.</param>
        /// <param name="sheetName">The name of the worksheet to read from.</param>
        /// <param name="sRow">The starting row number (1-based).</param>
        /// <param name="eRow">
        /// The optional ending row number (1-based). 
        /// If null, the method will use the last row with content in the worksheet.
        /// </param>
        /// <returns>
        /// A list of string arrays, where each array represents one valid row from the Excel file.
        /// Rows are included only if:
        /// 1. Column A contains a valid integer.
        /// 2. Column A does not have a background fill color (excluding white or transparent).
        /// </returns>
        /// <remarks>
        /// This method uses ClosedXML to read content and DocumentFormat.OpenXml to detect cell background fill.
        /// Rows that do not satisfy both rules will be skipped and logged via HelpFunc.LogMessage.
        /// </remarks>

        public static List<string[]> ReadExcelFile(string excelPath, string sheetName, int sRow, int? eRow = null)
        {
            var valuesArray = new List<string[]>();

            try
            {
                // Step 1: Get all rows that have background fill in column A
                var rowsWithFill = GetRowsWithColorFillInColumnA(excelPath, sheetName);

                using (var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(sheetName);
                        int colCount = worksheet.LastColumnUsed().ColumnNumber();
                        eRow = eRow ?? worksheet.LastRowUsed().RowNumber();

                        for (int row = sRow; row <= eRow; row++)
                        {
                            var cellValue = worksheet.Cell(row, 1).GetString().Trim();

                            // Rule 1: Skip if column A is not a valid integer
                            if (!int.TryParse(cellValue, out _))
                            {
                                HelpFunc.LogMessage($"Row {row} skipped: column A is not an integer → \"{cellValue}\"");
                                continue;
                            }

                            // Rule 2: Skip if this row has background fill in column A
                            if (rowsWithFill.Contains(row))
                            {
                                HelpFunc.LogMessage($"Row {row} skipped: column A has background fill.");
                                continue;
                            }

                            // Passed both checks: read the entire row
                            string[] rowData = new string[colCount];
                            for (int col = 1; col <= colCount; col++)
                            {
                                rowData[col - 1] = worksheet.Cell(row, col).GetString();
                            }

                            valuesArray.Add(rowData);
                        }
                    }
                }

                HelpFunc.LogMessage($"Excel file \"{excelPath}\" was read successfully.");
            }
            catch (IOException ioEx)
            {
                HelpFunc.LogMessage("The file is currently open in Excel and cannot be read. Please close the file and try again.");
                HelpFunc.LogMessage($"Error: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                HelpFunc.LogMessage("An unexpected error occurred.");
                HelpFunc.LogMessage($"Error: {ex.Message}");
            }

            return valuesArray;
        }
        public static HashSet<int> GetRowsWithColorFillInColumnA(string filePath, string sheetName)
        {
            var rowsWithFill = new HashSet<int>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                var workbookPart = doc.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>()
                                .FirstOrDefault(s => s.Name == sheetName);
                if (sheet == null) return rowsWithFill;

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                var stylesPart = workbookPart.WorkbookStylesPart;
                var fills = stylesPart.Stylesheet.Fills;
                var cellFormats = stylesPart.Stylesheet.CellFormats;

                foreach (var row in sheetData.Elements<Row>())
                {
                    var cell = row.Elements<Cell>()
                                  .FirstOrDefault(c => GetColumnLetter(c.CellReference) == "A");

                    if (cell != null && cell.StyleIndex != null)
                    {
                        var format = (CellFormat)cellFormats.ElementAt((int)cell.StyleIndex.Value);

                        if (format.FillId != null)
                        {
                            Fill fill = fills.ElementAt((int)format.FillId.Value) as Fill;
                            if (fill != null && fill.PatternFill != null)
                            {
                                var patternFill = fill.PatternFill;

                                // Check for solid fill and foreground color
                                if (patternFill.PatternType != null &&
                                    patternFill.PatternType.Value == PatternValues.Solid &&
                                    patternFill.ForegroundColor?.Rgb != null)
                                {
                                    rowsWithFill.Add((int)row.RowIndex.Value);
                                }
                            }
                        }
                    }
                }
            }

            return rowsWithFill;
        }

        // Helper function to extract column letter from a cell reference (e.g., "A4" → "A")
        private static string GetColumnLetter(string cellRef)
        {
            return new string(cellRef.Where(char.IsLetter).ToArray());
        }
    }
}
