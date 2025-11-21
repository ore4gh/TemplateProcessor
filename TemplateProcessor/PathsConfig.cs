using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateProcessor
{
    public class PathsConfig
    {
        public Pathes Pathes { get; set; }

        public static PathsConfig LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {filePath}. Shared folder is not enabled");
            }

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<PathsConfig>(json);
        }
    }

    public class Pathes
    {
        public List<string> ExternInputSource { get; set; }
        public List<string> ExternOutputSource { get; set; }
        public List<string> ImportPath { get; set; }
        public List<string> TemplatePath { get; set; }
        public List<string> RulePath { get; set; }
        public List<string> OutputPath { get; set; }

        public List<string> GetExternInputSource()
        {
            return ExternInputSource ?? new List<string>();
        }
        public List<string> GetExternOutputSource()
        {
            return ExternOutputSource ?? new List<string>();
        }
        public List<string> GetImportPaths()
        {
            return ImportPath ?? new List<string>();
        }

        public List<string> GetTemplatePaths()
        {
            return TemplatePath ?? new List<string>();
        }

        public List<string> GetRulePaths()
        {
            return RulePath ?? new List<string>();
        }

        public List<string> GetOutputPaths()
        {
            return OutputPath ?? new List<string>();
        }

        public (List<string> RulePaths, List<string> ImportPaths, List<string> OutputPaths, List<string> TemplatePaths) ProcessPath(string objType, string templateName, string templateVersion, string templateComment)
        {
            var rulePaths = new List<string>();
            var importPaths = new List<string>();
            var outputPaths = new List<string>();
            var templatePaths = new List<string>();

             foreach (var rule in RulePath)
             {
                 string rulePath = rule
                     .Replace("{ObjType}", objType)
                     .Replace("{Name}", templateName)
                     .Replace("{Comment}", templateComment)
                     .Replace("{Version}", templateVersion);
                 rulePaths.Add(rulePath);
             }

             foreach (var import in ImportPath)
             {
                 string importPath = import
                     .Replace("{ObjType}", objType)
                     .Replace("{Name}", templateName)
                     .Replace("{Comment}", templateComment)
                     .Replace("{Version}", templateVersion);
                 importPaths.Add(importPath);
             }

             foreach (var output in OutputPath)
             {
                 string outputPath = output
                     .Replace("{ObjType}", objType)
                     .Replace("{Name}", templateName)
                     .Replace("{Comment}", templateComment)
                     .Replace("{Version}", templateVersion);
                 outputPaths.Add(outputPath);
             }

             foreach (var template in TemplatePath)
             {
                 string templatePath = template
                     .Replace("{ObjType}", objType)
                     .Replace("{Name}", templateName)
                     .Replace("{Comment}", templateComment)
                     .Replace("{Version}", templateVersion);
                 templatePaths.Add(templatePath);
             }

            return (rulePaths, importPaths, outputPaths, templatePaths);
        }
    }
}