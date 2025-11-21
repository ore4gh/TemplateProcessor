using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateProcessor.Generator
{
    public class ObjTypeDetails
    {
        public int Enabled { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
    }

    public class PathVariables
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public string Rule { get; set; }
        public string Version { get; set; }
        public string ExtName { get; set; }
        public string Separator { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string RulePath { get; set; }
        // Additional properties for other tasks
        public string Common { get; set; }
        public string InputVersion { get; set; }
        public string OutputVersion { get; set; }
        public string RuleVersion { get; set; }
    }

    public class GroupList
    {
        public string Name { get; set; }
        public List<string> ObjTypes { get; set; }
        public List<string> IncludedCategories { get; set; }
    }

    public class Task
    {
        public string Name { get; set; }
        public PathVariables PathVariables { get; set; }
        public List<GroupList> GroupList { get; set; }
    }

    public class Commands
    {

        public string Name { get; set; }
        public string Category { get; set; }
        public int Enabled { get; set; }
        public string Parameters { get; set; }

    }

    public class GenData
    {
        public string ActiveObjType { get; set; }
        public string ActiveTask { get; set; }
        public Dictionary<string, int[]> ObjTypes { get; set; }
        public Dictionary<string, int> Categories { get; set; }
        public List<Task> Tasks { get; set; }
        public List<Commands> Commands { get; set; }

        // Method to get ObjTypeDetails from the dictionary
        public ObjTypeDetails GetObjTypeDetails(string objType)
        {
            if (ObjTypes.TryGetValue(objType, out int[] details) && details.Length == 3)
            {
                return new ObjTypeDetails
                {
                    Enabled = details[0],
                    StartRow = details[1],
                    EndRow = details[2]
                };
            }
            return null; // or throw an exception if preferred
        }

        // Method to get all enabled categories
        public List<string> GetEnabledCategories()
        {
            var enabledCategories = new List<string>();

            foreach (var category in Categories)
            {
                if (category.Value == 1)
                {
                    enabledCategories.Add(category.Key);
                }
            }

            return enabledCategories;
        }

        // Method to get enabled included categories for the active task and selected objType
        public List<string> GetEnabledIncludedCategoriesForActiveTask(string objType)
        {
            var includedCategories = new List<string>();

            // Loop through all tasks and gather included categories for the given objType
            foreach (var task in Tasks)
            {
                foreach (var groupList in task.GroupList)
                {
                    if (groupList.ObjTypes.Contains(objType))
                    {
                        includedCategories.AddRange(groupList.IncludedCategories);
                    }
                }
            }

            // Get enabled categories
            var enabledCategories = GetEnabledCategories();

            // Filter included categories to only include enabled ones and remove duplicates
            return includedCategories.Where(category => enabledCategories.Contains(category)).Distinct().ToList();
        }

        // Method to get paths for the active task and selected objType
        public Dictionary<string, PathTemplate> GetObjTypePathsForActiveTask(string taskName, string objType, int? enabled = null)
        {
            var task = Tasks.FirstOrDefault(t => t.Name == taskName);

            if (task == null)
            {
                HelpFunc.LogMessage($"Task '{taskName}' not found.");
                return new Dictionary<string, PathTemplate>();
            }

            var pathVariables = task.PathVariables;

            // Extract and split the semicolon-separated values
            var (versions, extNames, separators) = ExtractAndSplitPathVariables(pathVariables);

            var paths = new Dictionary<string, PathTemplate>();

            // Get enabled included categories for the active task and selected objType
            var enabledCategories = GetEnabledIncludedCategoriesForActiveTask(objType);

            foreach (var taskList in task.GroupList)
            {
                if (taskList.ObjTypes.Contains(objType))
                {
                    foreach (var categoryName in enabledCategories)
                    {
                        if (Categories.TryGetValue(categoryName, out int isEnabled) && (!enabled.HasValue || isEnabled == enabled.Value))
                        {
                            var inputPath = ReplacePlaceholders(pathVariables.InputPath, objType, categoryName, pathVariables, versions, extNames, separators);
                            var outputPath = ReplacePlaceholders(pathVariables.OutputPath, objType, categoryName, pathVariables, versions, extNames, separators);
                            var rulePath = ReplacePlaceholders(pathVariables.RulePath, objType, categoryName, pathVariables, versions, extNames, separators);

                            paths[categoryName] = new PathTemplate
                            {
                                InputPath = inputPath,
                                OutputPath = outputPath,
                                RulePath = rulePath
                            };
                        }
                    }
                }
            }

            return paths;
        }

        private (string[] versions, string[] extNames, string[] separators) ExtractAndSplitPathVariables(PathVariables pathVariables)
        {
            var versions = pathVariables.Version.Split(';');
            var extNames = pathVariables.ExtName.Split(';');
            var separators = pathVariables.Separator.Split(';');
            return (versions, extNames, separators);
        }

        private string ReplacePlaceholders(string pathTemplate, string objType, string includedCategory, PathVariables pathVariables, string[] versions, string[] extNames, string[] separators)
        {
            // Replace basic placeholders
            pathTemplate = pathTemplate
                .Replace("{ObjType}", objType)
                .Replace("{Input}", pathVariables.Input)
                .Replace("{Output}", pathVariables.Output)
                .Replace("{Rule}", pathVariables.Rule)
                .Replace("{IncludedCategory}", includedCategory);

            // Replace dynamic placeholders for Version
            for (int i = 0; i < versions.Length; i++)
            {
                pathTemplate = pathTemplate.Replace($"{{Version[{i}]}}", versions[i]);
            }

            // Replace dynamic placeholders for ExtName
            for (int i = 0; i < extNames.Length; i++)
            {
                pathTemplate = pathTemplate.Replace($"{{ExtName[{i}]}}", extNames[i]);
            }

            // Replace dynamic placeholders for Separator
            for (int i = 0; i < separators.Length; i++)
            {
                pathTemplate = pathTemplate.Replace($"{{Separator[{i}]}}", separators[i]);
            }

            return pathTemplate;
        }
        // Method to get the list of commands for a given category
        public List<string> GetCommandListForCategory(string category)
        {
            return Commands
                .Where(cmd => cmd.Category == category && cmd.Enabled == 1)
                .Select(cmd => cmd.Name)
                .ToList();
        }

        // Method to process category commands
        public void ProcessCategoryCommands(string category, string outputContent)
        {
            foreach (var cmd in Commands)
            {
                if (cmd.Category == category && cmd.Enabled == 1)
                {
                    // Execute the command
                    ExecuteCommand(cmd.Name, outputContent);
                }
            }
        }

        // Method to execute a specific command
        private void ExecuteCommand(string commandName, string outputContent)
        {
            // Implement the logic to execute the command based on its name
            switch (commandName)
            {
                case "InsertUpdateIlockTextList":

                    // External SQL command section
                    //SqlTools sqlTools = new SqlTools(_commonConfig.SqlCommonData.Server, _commonConfig.SqlCommonData.Database, _commonConfig.SqlCommonData.Username, _commonConfig.SqlCommonData.Password, _txtLog);
                    //if (category.ExternIOCommand== "InsertUpdateIlockTextList")
                    //{
                    //    InsertUpdateIlockTextList(outputContent, valuesArray, category);
                    //}



                    break;
                case "DeleteIlock5":
                    // Logic for DeleteIlock5
                    break;
                default:
                    HelpFunc.LogMessage($"Unknown command: {commandName}");
                    break;
            }
        }
        public List<string> GetCommandParameters(string commandName, string objType)
        {
            var command = Commands.FirstOrDefault(cmd => cmd.Name == commandName);
            if (command == null)
            {
                HelpFunc.LogMessage($"Command '{commandName}' not found.");
                return new List<string>();
            }

            var result = new List<string>();

            var parts = command.Parameters.Split(';');

            foreach (var part in parts)
            {
                var replacedPart = part.Replace("{ObjType}", objType);
                result.Add(replacedPart);
            }

            return result;
        }

    }

    public class PathTemplate
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string RulePath { get; set; }
    }
}