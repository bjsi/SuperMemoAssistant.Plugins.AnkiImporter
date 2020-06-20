#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   6/7/2020 3:11:36 AM
// Modified By:  james

#endregion


namespace SuperMemoAssistant.Plugins.AnkiImporter
{
  using System.Diagnostics.CodeAnalysis;
  using System.Windows.Input;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.HotKeys;
  using SuperMemoAssistant.Services.IO.Keyboard;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Services.UI.Configuration;
  using SuperMemoAssistant.Sys.IO.Devices;
  using System.Collections.Generic;
  using SuperMemoAssistant.Plugins.AnkiImporter.Models;
  using System.IO;
  using System.Threading.Tasks;
  using System.Windows;
  using SuperMemoAssistant.Plugins.AnkiImporter.UI;
  using SuperMemoAssistant.Extensions;
  using Anotar.Serilog;
  using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class AnkiImporterPlugin : SentrySMAPluginBase<AnkiImporterPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public AnkiImporterPlugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "AnkiImporter";

    /// <inheritdoc />
    public override bool HasSettings => true;
    public AnkiImporterCfg Config { get; set; }
    public ImporterWdw CurrentInstance { get; set; }
    public string TestAnkiCollectionDB { get; } = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    public string TestAnkiCollectionMediaDir { get; } = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.media\";

    #endregion


    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<AnkiImporterCfg>() ?? new AnkiImporterCfg();

      // TODO: Fix this
      if (Config.Testing)
      {
        Config.AnkiCollectionDB = TestAnkiCollectionDB;
        Config.AnkiCollectionMediaDir = TestAnkiCollectionMediaDir;
      }
    }


    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {

      LoadConfig();

      Svc.HotKeyManager
         .RegisterGlobal(
           "AnkiImporter",
           "Open AnkiImporter Window",
           HotKeyScopes.SMBrowser,
           new HotKey(Key.I, KeyModifiers.CtrlAltShift),
           OpenAnkiImporter
     );

    }

    /// <summary>
    /// Get anki decks from an anki collection database.
    /// </summary>
    /// <returns></returns>
    private async Task<Dictionary<long, Deck>> GetDecksAsync(string database)
    {
      var db = new DataAccess(database);
      var decks = await db.GetDecksAsync();
      return decks;
    }

    /// <summary>
    /// Get Decks from the database and launch the importer window.
    /// </summary>
    private async void OpenAnkiImporter()
    {

      var database = Config.AnkiCollectionDB;

      if (!File.Exists(database))
      {
        Popups.ShowAlert($"Anki database \"{database}\" does not exist", "Failed to open AnkiImporter window");
        return;
      }

      var decks = await GetDecksAsync(database);
      if (decks == null || decks.Count == 0)
      {
        Popups.ShowAlert($"Attempt to get decks from database \"{database}\"returned null or empty.", "Failed to open AnkiImporter window");
        return;
      }

      var trees = new DeckTreeDictionary(decks);
      OpenAnkiImporterWdw(trees);
    }

    /// <summary>
    /// Open an AnkiImporterWdw instance.
    /// </summary>
    /// <param name="trees"></param>
    private void OpenAnkiImporterWdw(DeckTreeDictionary trees)
    {

      if (trees == null || trees.Count == 0)
      {
        LogTo.Error("Attempted to OpenAnkiImporterWdw with a null or empty DeckTreeDictionary object");
        return;
      }

      if (CurrentInstance != null)
        return;

      Application.Current.Dispatcher.Invoke(() =>
      {
        var wdw = new ImporterWdw(trees);
        CurrentInstance = wdw;
        wdw.ShowAndActivate();
      });

    }

    /// <inheritdoc />
    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(HotKeyManager.Instance, Config);
    }

    #endregion

    #region Methods

    #endregion
  }
}
