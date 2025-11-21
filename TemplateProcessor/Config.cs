using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TemplateProcessor
{
    // Class for holding individual commands in a template
    public class Command
    {
        public string? Name { get; set; }
        public string? Script { get; set; }
        public string? Parameter { get; set; }
        public int Enabled { get; set; }
    }

    // Class for holding individual templates in a task
    public class Template
    {
        public string Name { get; set; }
        public string? Version { get; set; }
        public int Enabled { get; set; }
        public List<Command>? Commands { get; set; }

        public bool ShouldSerializeCommands()
        {
            // Only serialize Commands if it is not null and has elements
            return Commands != null && Commands.Count > 0;
        }
    }

    // Class for holding individual tasks
    public class Task
    {
        public string? ObjType { get; set; }
        public int Enabled { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public List<Template>? Templates { get; set; }

        // Constructor
        public Task()
        {
            Templates = new List<Template>();
        }
    }
    // Class for holding the overall generator data
    public class GeneratorData
    {
        public string? ActiveObjType { get; set; }
        public List<Task>? Tasks { get; set; }

        // Constructor
        public GeneratorData()
        {
            Tasks = new List<Task>();
        }
    }


}