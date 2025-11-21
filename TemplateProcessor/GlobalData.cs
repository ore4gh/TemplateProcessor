using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateProcessor
{
    public class GlobalData
    {
        private static readonly Lazy<GlobalData> lazyInstance = new Lazy<GlobalData>(() => new GlobalData());

        // Private constructor to prevent instance creation
        private GlobalData() { }

        // Public property to get the singleton instance
        public static GlobalData Instance => lazyInstance.Value;

        // Global properties
        public CommonData CommonData { get; private set; }
        public SqlCommonData SqlCommonData { get; private set; }
        public SqlServerVerdo SqlServerVerdo { get; private set; }

        // Path configurations
        public List<string> ExternInputSource { get; set; } = new List<string>();
        public List<string> ExternOutputSource { get; set; } = new List<string>();
        public List<string> ImportPath { get; set; } = new List<string>();
        public List<string> TemplatePath { get; set; } = new List<string>();
        public List<string> RulePath { get; set; } = new List<string>();
        public List<string> OutputPath { get; set; } = new List<string>();

        public void SetGlobalConfig(CommonConfig config)
        {
            this.CommonData = config.CommonData;
            this.SqlCommonData = config.SqlCommonData;
            this.SqlServerVerdo = config.SqlServerVerdo;
        }

        public void SetPathsConfig(PathsConfig pathsConfig)
        {
            this.ExternInputSource = pathsConfig.Pathes.GetExternInputSource();
            this.ExternOutputSource = pathsConfig.Pathes.GetExternOutputSource();
            this.ImportPath = pathsConfig.Pathes.GetImportPaths();
            this.TemplatePath = pathsConfig.Pathes.GetTemplatePaths();
            this.RulePath = pathsConfig.Pathes.GetRulePaths();
            this.OutputPath = pathsConfig.Pathes.GetOutputPaths();
        }
    }
}