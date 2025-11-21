using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateProcessor.SearchReplace
{
    public class LineItem { }

    public class SearchReplacePair : LineItem
    {
        public string Search { get; set; }
        public string Replace { get; set; }

        public SearchReplacePair(string search, string replace)
        {
            Search = search;
            Replace = replace;
        }
    }

    public class Command : LineItem
    {
        public string Name { get; set; }
        public List<string> Operands { get; set; }

        public Command(string name, List<string> operands)
        {
            Name = name;
            Operands = operands;
        }
    }

    public class LineProcessor
    {
        private readonly string templVersion;
        private readonly Dictionary<string, AlarmNumber> _alarmNumbers;

        public LineProcessor(string templVersion, Dictionary<string, AlarmNumber> alarmNumbers)
        {
            this.templVersion = templVersion;
            _alarmNumbers = alarmNumbers;
        }
        public void PerformSearchReplace(string inputFilePath, string outputFilePath, List<LineItem> pairs)
        {
            // Check if the output file exists and rename it if it does
            if (File.Exists(outputFilePath))
            {
                string directory = Path.GetDirectoryName(outputFilePath);
                string fileName = Path.GetFileName(outputFilePath);
                string newFileName = Path.Combine(directory, "old_" + fileName);

                // Ensure the new file name doesn't already exist
                int count = 1;
                while (File.Exists(newFileName))
                {
                    newFileName = Path.Combine(directory, $"old_{count}_{fileName}");
                    count++;
                }

                File.Copy(outputFilePath, newFileName);
            }

            // Ensure the directory exists
            string outputDirectory = Path.GetDirectoryName(outputFilePath);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Reading the content from the input file
            string content = File.ReadAllText(inputFilePath);

            // Performing search and replace for each pair
            foreach (var pair in pairs)
            {
                if (pair is SearchReplacePair srp)
                {
                    // Check if both search and replace strings are not null
                    if (srp.Search != null && srp.Replace != null)
                    {
                        content = content.Replace(srp.Search, srp.Replace);
                    }
                }
                else if (pair is Command command)
                {
                    // Check if none of the operands are null
                    if (command.Operands.All(operand => operand != null))
                    {
                        // Handle commands in a specific way
                        HandleCommand(command, ref content);
                    }
                }
            }

            // Split the content into lines and remove empty lines
            var lines = new List<string>(content.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            lines.RemoveAll(string.IsNullOrWhiteSpace);

            // Insert the specified lines at the correct positions
            InsertSpecifiedLines(lines);

            // Write the modified content back to the file with UTF-16 LE encoding with BOM
            using (StreamWriter writer = new StreamWriter(outputFilePath, false, new UnicodeEncoding(false, true)))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            // Ensure the StreamWriter is disposed and file closed properly
            //Console.WriteLine("Writing to output file completed.");
        }

        private void InsertSpecifiedLines(List<string> lines)
        {
            // Initialize variables to store the values that will be inserted
            string headerLine = "<ROOT>";
            string detailsLine = "<DETAILS>";
            List<string> footerLines = new List<string> { "</DETAILS>", "</ROOT>" };

            // Insert the header line at the beginning
            lines.Insert(0, headerLine);

            // Insert the details line at the correct position
            if (lines.Count >= 4) // Check if we have enough lines to insert at line 4
            {
                lines.Insert(4, detailsLine); // Index 4 is actually the 5th line, swap this with Index 3 if required
            }
            else
            {
                lines.Add(detailsLine); // Append to the end if there are not enough lines
            }

            // Add footer lines at the end
            lines.AddRange(footerLines);
        }

        private static void HandleCommand(Command command, ref string content)
        {
            if (command.Name == "Uppercase")
            {
                foreach (var operand in command.Operands)
                {
                    content = content.Replace(operand, operand.ToUpper());
                }
            }
            else if (command.Name == "InsertLinesBefore")
            {
                InsertLinesBefore(command, ref content);
            }
            else if (command.Name == "InsertLinesAfter")
            {
                InsertLinesAfter(command, ref content);
            }
            else if (command.Name == "RegexSearch")
            {
                RegexSearch(command, ref content);
            }
            // Add more command handling logic as needed
        }

        private static void RegexSearch(Command command, ref string content)
        {
            if (command.Operands.Count >= 2)
            {
                string pattern = command.Operands[0];
                string replacement = command.Operands[1];

                // Split content into lines
                var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // Create a regex with the specified pattern
                Regex regex = new Regex(pattern);

                // Replace only the matched portion in each line
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = regex.Replace(lines[i], replacement);
                }

                // Join the lines back into a single string
                content = string.Join(Environment.NewLine, lines);
            }
            else
            {
                throw new ArgumentException("RegexSearch requires at least a pattern and a replacement string.");
            }
        }

        private static void InsertLinesBefore(Command command, ref string content)
        {
            if (command.Operands.Count >= 2)
            {
                string searchTag = command.Operands[0];
                List<string> linesToInsert = command.Operands.Skip(1).ToList();

                var lines = new List<string>(content.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                // We need the loop to continue searching until the end of the list
                for (int i = 0; i < lines.Count;)
                {
                    if (lines[i].Contains(searchTag))
                    {
                        // Insert the lines before the found line (searchTag)
                        lines.InsertRange(i, linesToInsert);
                        // Skip over the inserted lines and the found line
                        i += linesToInsert.Count + 1;
                    }
                    else
                    {
                        // Otherwise, just go to the next line
                        i++;
                    }
                }

                content = string.Join(Environment.NewLine, lines);
            }
            else
            {
                throw new ArgumentException("InsertLinesBefore requires at least one search tag and one line to insert.");
            }
        }

        private static void InsertLinesAfter(Command command, ref string content)
        {
            if (command.Operands.Count >= 2)
            {
                string searchTag = command.Operands[0];
                List<string> linesToInsert = command.Operands.Skip(1).ToList();

                var lines = new List<string>(content.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                // We need the loop to continue searching until the end of the list
                for (int i = 0; i < lines.Count;)
                {
                    if (lines[i].Contains(searchTag))
                    {
                        // Insert the lines after the found line (searchTag)
                        lines.InsertRange(i + 1, linesToInsert);
                        // Skip over the found line and the newly inserted lines
                        i += linesToInsert.Count + 1;
                    }
                    else
                    {
                        // Otherwise, just go to the next line
                        i++;
                    }
                }

                content = string.Join(Environment.NewLine, lines);
            }
            else
            {
                throw new ArgumentException("InsertLinesAfter requires at least one search tag and one line to insert.");
            }
        }

        private static Command ParseCommand(string commandLine)
        {
            // Extract the command by removing the leading and trailing '@'
            string commandWithOperands = commandLine.Trim('@');
            // Corrected regex pattern to extract command and operands
            Regex regex = new Regex(@"^(?<command>[^@]+)@\""(?<operand1>.*?)(?<!\\)\""\s*,\s*\""(?<operand2>.*?)(?<!\\)\""$");
            Match match = regex.Match(commandWithOperands);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid command format.");
            }
            string command = match.Groups["command"].Value;
            string operand1 = match.Groups["operand1"].Value;
            // Split operand2 by "\n" to handle multiple lines
            string operand2 = match.Groups["operand2"].Value;
            List<string> operands = new List<string> { operand1 }.Concat(operand2.Split(new[] { @"\n" }, StringSplitOptions.None)).ToList();
            return new Command(command, operands);
        }

        public static List<LineItem> ReadSearchReplacePairs(string filePath, Dictionary<string, AlarmNumber> alarmNumbers, string activeObjType)
        {
            var lineItems = new List<LineItem>();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Ignore lines that start with //
                    if (line.TrimStart().StartsWith("//"))
                    {
                        continue;
                    }

                    // Check if the line is a command
                    if (line.TrimStart().StartsWith("@"))
                    {
                        var command = ParseCommand(line);
                        if (command != null)
                        {
                            lineItems.Add(command);
                        }
                        continue;
                    }

                    // Removing the enclosing quotes and splitting the line into search and replace parts
                    var parts = line.Trim().Trim('"').Split(new[] { "\",\"" }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        // Handle multi-line replacements by replacing \n with newline characters
                        string replace = parts[1].Replace(@"\n", Environment.NewLine);
                        lineItems.Add(new SearchReplacePair(parts[0], replace));
                    }
                }
            }

            // Replace placeholders in the search and replace pairs
            ReplacePlaceholdersInPairs(lineItems, alarmNumbers, activeObjType);

            return lineItems;
        }

        private static void ReplacePlaceholdersInPairs(List<LineItem> pairs, Dictionary<string, AlarmNumber> alarmNumbers, string activeObjType)
        {
            foreach (var pair in pairs)
            {
                if (pair is SearchReplacePair srp)
                {
                    srp.Search = ReplacePlaceholdersWithAlarmData(srp.Search, alarmNumbers, activeObjType);
                    srp.Replace = ReplacePlaceholdersWithAlarmData(srp.Replace, alarmNumbers, activeObjType);
                }
                else if (pair is Command command)
                {
                    for (int i = 0; i < command.Operands.Count; i++)
                    {
                        command.Operands[i] = ReplacePlaceholdersWithAlarmData(command.Operands[i], alarmNumbers, activeObjType);
                    }
                }
            }
        }

        private static string ReplacePlaceholdersWithAlarmData(string text, Dictionary<string, AlarmNumber> alarmNumbers, string activeObjType)
        {
            if (alarmNumbers.TryGetValue(activeObjType, out var alarmNumber))
            {
                // Define a dictionary to map placeholders to their corresponding values
                var placeholderValues = new Dictionary<string, string>
                {
                    { "{ShortDescAct}", alarmNumber.ShortDescAct },
                    { "{Db1NrAct}", alarmNumber.Db1NrAct.ToString() },
                    { "{Db2NrAct}", alarmNumber.Db2NrAct.ToString() },
                    { "{TagStartNrAct}", alarmNumber.TagStartNrAct.ToString() },
                    { "{DescStartNrAct}", alarmNumber.DescStartNrAct.ToString() },
                    { "{AlmStartNrAct}", (alarmNumber.AlmStartNrAct / 100).ToString() },
                    { "{AlmCountAct}", alarmNumber.AlmCountAct.ToString() },
                    { "{StaStartNrAct}", (alarmNumber.StaStartNrAct / 100).ToString() },
                    { "{StaCountAct}", alarmNumber.StaCountAct.ToString() },
                    { "{ShortDescNew}", alarmNumber.ShortDescNew },
                    { "{Db1NrNew}", alarmNumber.Db1NrNew.ToString() },
                    { "{Db2NrNew}", alarmNumber.Db2NrNew.ToString() },
                    { "{TagStartNrNew}", alarmNumber.TagStartNrNew.ToString() },
                    { "{DescStartNrNew}", alarmNumber.DescStartNrNew.ToString() },
                    { "{AlmStartNrNew}", (alarmNumber.AlmStartNrNew / 100).ToString() },
                    { "{AlmCountNew}", alarmNumber.AlmCountNew.ToString() },
                    { "{StaStartNrNew}", (alarmNumber.StaStartNrNew / 100).ToString() },
                    { "{StaCountNew}", alarmNumber.StaCountNew.ToString() },
                    // added new calculated placeholders for alarmNumbers>99 (SB=3800xx,3801xx-> +1)
                    { "{StaStartNr1Act}", (alarmNumber.StaStartNrAct / 100 + 1).ToString() },
                    { "{StaStartNr1New}", (alarmNumber.StaStartNrNew / 100 + 1).ToString() },

                };

                // Extract placeholders from the text
                var matches = System.Text.RegularExpressions.Regex.Matches(text, @"\{[^}]+\}");

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string placeholder = match.Value;

                    // Check if the placeholder exists in the dictionary and its value is valid
                    if (placeholderValues.TryGetValue(placeholder, out var value))
                    {
                        if (value != "__EMPTY__" && value != "-999" && value != "0")
                        {
                            // Replace the placeholder with its value
                            text = text.Replace(placeholder, value);
                        }
                    }
                }
            }
            else
            {
                HelpFunc.LogMessage($"Active object type '{activeObjType}' not found in alarm numbers.");
            }

            return text;
        }
    }
}