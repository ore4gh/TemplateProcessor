using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TemplateProcessor
{
    public class CommonConfig
    {
        public CommonData CommonData { get; set; }
        public SqlCommonData SqlCommonData { get; set; }
        public SqlServerVerdo SqlServerVerdo { get; set; }

        public static CommonConfig LoadConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {filePath}");
            }

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<CommonConfig>(json);
        }
    }

    public class CommonData
    {
        public string SharedInputDrive { get; set; }
        public string SharedFolderPath { get; set; }
        public string ProjectFolderName { get; set; }
        public string ExcelDataFileName { get; set; }
    }

    public class SqlCommonData
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SqlServerVerdo : SqlCommonData
    {
    }

    public class AlarmNumber
    {
        public string ShortDescAct { get; set; }
        public int Db1NrAct { get; set; }
        public int Db2NrAct { get; set; }
        public int TagStartNrAct { get; set; }
        public int DescStartNrAct { get; set; }
        public int AlmStartNrAct { get; set; }
        public int AlmCountAct { get; set; }
        public int StaStartNrAct { get; set; }
        public int StaCountAct { get; set; }
        public string ShortDescNew { get; set; }
        public int Db1NrNew { get; set; }
        public int Db2NrNew { get; set; }
        public int TagStartNrNew { get; set; }
        public int DescStartNrNew { get; set; }
        public int AlmStartNrNew { get; set; }
        public int AlmCountNew { get; set; }
        public int StaStartNrNew { get; set; }
        public int StaCountNew { get; set; }
    }

    public class DataLoader
    {
        public Dictionary<string, AlarmNumber> DataDict { get; private set; }

        public DataLoader(string filePath)
        {
            DataDict = LoadData(filePath);
        }

        private Dictionary<string, AlarmNumber> LoadData(string filePath)
        {
            var dataDict = new Dictionary<string, AlarmNumber>();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length < 2)
                throw new Exception("File does not contain enough data");

            for (int i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split('\t');
                var alarmNumber = new AlarmNumber
                {
                    ShortDescAct = string.IsNullOrEmpty(values[1]) || values[1] == "0" ? "__EMPTY__" : values[1],
                    Db1NrAct = string.IsNullOrEmpty(values[2]) || values[2] == "0" ? -999 : int.Parse(values[2]),
                    Db2NrAct = string.IsNullOrEmpty(values[3]) || values[3] == "0" ? -999 : int.Parse(values[3]),
                    TagStartNrAct = string.IsNullOrEmpty(values[4]) || values[4] == "0" ? -999 : int.Parse(values[4]),
                    DescStartNrAct = string.IsNullOrEmpty(values[5]) || values[5] == "0" ? -999 : int.Parse(values[5]),
                    AlmStartNrAct = string.IsNullOrEmpty(values[6]) || values[6] == "0" ? -999 : int.Parse(values[6]),
                    AlmCountAct = string.IsNullOrEmpty(values[7]) || values[7] == "0" ? -999 : int.Parse(values[7]),
                    StaStartNrAct = string.IsNullOrEmpty(values[8]) || values[8] == "0" ? -999 : int.Parse(values[8]),
                    StaCountAct = string.IsNullOrEmpty(values[9]) || values[9] == "0" ? -999 : int.Parse(values[9]),
                    ShortDescNew = string.IsNullOrEmpty(values[10]) || values[10] == "0" ? "__EMPTY__" : values[10],
                    Db1NrNew = string.IsNullOrEmpty(values[11]) || values[11] == "0" ? -999 : int.Parse(values[11]),
                    Db2NrNew = string.IsNullOrEmpty(values[12]) || values[12] == "0" ? -999 : int.Parse(values[12]),
                    TagStartNrNew = string.IsNullOrEmpty(values[13]) || values[13] == "0" ? -999 : int.Parse(values[13]),
                    DescStartNrNew = string.IsNullOrEmpty(values[14]) || values[14] == "0" ? -999 : int.Parse(values[14]),
                    AlmStartNrNew = string.IsNullOrEmpty(values[15]) || values[15] == "0" ? -999 : int.Parse(values[15]),
                    AlmCountNew = string.IsNullOrEmpty(values[16]) || values[16] == "0" ? -999 : int.Parse(values[16]),
                    StaStartNrNew = string.IsNullOrEmpty(values[17]) || values[17] == "0" ? -999 : int.Parse(values[17]),
                    StaCountNew = string.IsNullOrEmpty(values[18]) || values[18] == "0" ? -999 : int.Parse(values[18])
                };
                dataDict[values[0]] = alarmNumber;
            }

            return dataDict;
        }
    }
}