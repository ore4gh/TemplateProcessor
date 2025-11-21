using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TemplateProcessor.SearchReplace
{
    public class PathTemplate
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string RulePath { get; set; }
    }

    public class SearchReplaceEngine
    {
        private SearchReplaceData _searchReplaceData;
        private readonly string _fileNameConfig;
        private readonly CommonConfig _commonConfig;
        private readonly Dictionary<string, AlarmNumber> _alarmNumbers;

        public SearchReplaceEngine(string fileNameConfig, CommonConfig commonConfig, Dictionary<string, AlarmNumber> alarmNumbers)
        {
            _fileNameConfig = fileNameConfig;
            _commonConfig = commonConfig;
            _alarmNumbers = alarmNumbers;

            InitializeTextReplacementData();
        }

        private void InitializeTextReplacementData()
        {
            string searchReplaceFilePath = Path.Combine(
                _commonConfig.CommonData.SharedFolderPath,
                _commonConfig.CommonData.ProjectFolderName,
                _fileNameConfig
            );

            if (File.Exists(searchReplaceFilePath))
            {
                string json = File.ReadAllText(searchReplaceFilePath);
                _searchReplaceData = JsonSerializer.Deserialize<SearchReplaceData>(json);
            }
            else
            {
                // Handle file not found scenario
                throw new FileNotFoundException("SearchReplaceData file not found.", searchReplaceFilePath);
            }
        }

        public SearchReplaceData GetSearchReplaceData()
        {
            return _searchReplaceData;
        }
        public void UpdateActiveObjType(string activeObjType)
        {
            if (_searchReplaceData != null)
            {
                _searchReplaceData.ActiveObjType = activeObjType;
                SaveConfig();
            }
        }

        public void SaveConfig()
        {
            string searchReplaceFilePath = Path.Combine(
                _commonConfig.CommonData.SharedFolderPath,
                _commonConfig.CommonData.ProjectFolderName,
                _fileNameConfig
            );

            string json = JsonSerializer.Serialize(_searchReplaceData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(searchReplaceFilePath, json);
        }
        private (string[] versions, string[] extNames, string[] separators) ExtractAndSplitPathVariables(PathVariables pathVariables)
        {
            var versions = pathVariables.Version.Split(';');
            var extNames = pathVariables.ExtName.Split(';');
            var separators = pathVariables.Separator.Split(';');
            return (versions, extNames, separators);
        }

        public void ProcessSearchReplace(string objType)
        {
            if (_searchReplaceData == null)
            {
                HelpFunc.LogMessage("No data loaded for search-replace process.");
                return;
            }

            // Select all existing objTypes in active category
            var objTypes = GetObjTypesForCategory(_searchReplaceData.ActiveTask);

            // Log or process the ObjTypes
            foreach (var type in objTypes)
            {
                HelpFunc.LogMessage($"ObjType: {type}");
            }

            // Get properties of included categories for the active category and objType
            var includedCategories = GetObjTypePropertiesIncludedCategories(_searchReplaceData.ActiveTask, objType, 1, "Common");

            // Get the PathVariables from the active category's path variables
            var activeTask = _searchReplaceData.Tasks.FirstOrDefault(t => t.Name == _searchReplaceData.ActiveTask);
            if (activeTask == null)
            {
                HelpFunc.LogMessage($"Active task '{_searchReplaceData.ActiveTask}' not found.");
                return;
            }
            var pathVariables = activeTask.PathVariables;

            // Extract and split the semicolon-separated values
            var (versions, extNames, separators) = ExtractAndSplitPathVariables(pathVariables);

            foreach (var category in includedCategories)
            {
                List<LineItem> combinedLines = new List<LineItem>();
                var categoryName = category.Key;
                var pathTemplate = category.Value;

                // Use TryGetValue to access the category from the dictionary
                if (!_searchReplaceData.Categories.TryGetValue(categoryName, out int isEnabled))
                {
                    HelpFunc.LogMessage($"Category '{categoryName}' not found.");
                    continue;
                }

                foreach (var rulePath in pathTemplate.RulePath.Split(';'))
                {
                    string ruleUpdatedPath = ReplacePlaceholders(rulePath, objType, categoryName, pathVariables, versions, extNames, separators);
                    string ruleFullPath = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName, ruleUpdatedPath);

                    List<LineItem> lines = LineProcessor.ReadSearchReplacePairs(ruleFullPath, _alarmNumbers, objType);
                    combinedLines.AddRange(lines);
                }

                string inputUpdatedPath = ReplacePlaceholders(pathTemplate.InputPath, objType, categoryName, pathVariables, versions, extNames, separators);
                string outputUpdatedPath = ReplacePlaceholders(pathTemplate.OutputPath, objType, categoryName, pathVariables, versions, extNames, separators);

                string inputFullPath = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName, inputUpdatedPath);
                string outputFullPath = Path.Combine(_commonConfig.CommonData.SharedFolderPath, _commonConfig.CommonData.ProjectFolderName, outputUpdatedPath);

                var processor = new LineProcessor(versions[1], _alarmNumbers); // Assuming you want to use the second version
                processor.PerformSearchReplace(inputFullPath, outputFullPath, combinedLines);
            }
        }

        public List<string> GetObjTypesForCategory(string categoryName)
        {
            if (_searchReplaceData == null)
            {
                HelpFunc.LogMessage("No data loaded.");
                return new List<string>();
            }

            var category = _searchReplaceData.Tasks.FirstOrDefault(task => task.Name == categoryName);

            if (category == null)
            {
                HelpFunc.LogMessage($"Category '{categoryName}' not found.");
                return new List<string>();
            }

            var objTypes = new List<string>();
            foreach (var taskList in category.TaskList)
            {
                objTypes.AddRange(taskList.ObjTypes);
            }

            // Remove duplicates
            return objTypes.Distinct().ToList();
        }

        public Dictionary<string, PathTemplate> GetObjTypePropertiesIncludedCategories(string taskName, string objType, int? enabled = null, string commonFolder = "")
        {
            if (_searchReplaceData == null)
            {
                HelpFunc.LogMessage("No data loaded.");
                return new Dictionary<string, PathTemplate>();
            }

            var task = _searchReplaceData.Tasks.FirstOrDefault(t => t.Name == taskName);

            if (task == null)
            {
                HelpFunc.LogMessage($"Task '{taskName}' not found.");
                return new Dictionary<string, PathTemplate>();
            }

            var pathVariables = task.PathVariables;

            // Extract and split the semicolon-separated values
            var (versions, extNames, separators) = ExtractAndSplitPathVariables(pathVariables);

            var includedCategories = new Dictionary<string, PathTemplate>();

            foreach (var taskList in task.TaskList)
            {
                if (taskList.ObjTypes.Contains(objType))
                {
                    foreach (var categoryName in taskList.IncludedCategories)
                    {
                        if (_searchReplaceData.Categories.TryGetValue(categoryName, out int isEnabled) && (!enabled.HasValue || isEnabled == enabled.Value))
                        {
                            var inputPath = ReplacePlaceholders(pathVariables.InputPath, objType, categoryName, pathVariables, versions, extNames, separators);
                            var outputPath = ReplacePlaceholders(pathVariables.OutputPath, objType, categoryName, pathVariables, versions, extNames, separators);
                            var rulePath = ReplacePlaceholders(pathVariables.RulePath, objType, categoryName, pathVariables, versions, extNames, separators);

                            includedCategories[categoryName] = new PathTemplate
                            {
                                InputPath = inputPath,
                                OutputPath = outputPath,
                                RulePath = rulePath
                            };
                        }
                    }
                }
            }

            return includedCategories;
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
    }
}