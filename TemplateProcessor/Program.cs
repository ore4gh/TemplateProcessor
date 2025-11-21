namespace TemplateProcessor
{
    public static class Program
    {
        public static CommonConfig Config { get; private set; }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load the configuration
            LoadConfiguration();

            // Set global data
            GlobalData.Instance.SetGlobalConfig(Config);

            MainForm mainForm = new MainForm(Config);
            Application.Run(mainForm);
        }
        private static void LoadConfiguration()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string configFilePath = Path.Combine(baseDirectory, "TemplateProcessorSettings.json");
                Config = CommonConfig.LoadConfig(configFilePath);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}