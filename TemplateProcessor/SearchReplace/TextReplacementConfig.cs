using System;
using System.Collections.Generic;

namespace TemplateProcessor.SearchReplace
{
    public class ObjTypes
    {
        public Dictionary<string, int> Types { get; set; }
    }

    public class Categories
    {
        public Dictionary<string, int> CategoryList { get; set; }
    }

    public class Task
    {
        public string Name { get; set; }
        public PathVariables PathVariables { get; set; }
        public List<TaskList> TaskList { get; set; }
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
    }

    public class TaskList
    {
        public string Name { get; set; }
        public List<string> ObjTypes { get; set; }
        public List<string> IncludedCategories { get; set; }
    }

    public class SearchReplaceData
    {
        public string ActiveObjType { get; set; }
        public string ActiveTask { get; set; }
        public Dictionary<string, int> ObjTypes { get; set; }
        public Dictionary<string, int> Categories { get; set; }
        public List<Task> Tasks { get; set; }
    }
}