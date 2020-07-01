using System;


namespace SuperMemoAssistant.Plugins.AnkiImporter.UI
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new ImporterWdw();
            app.InitializeComponent();
            app.Run();

        }
    }
}