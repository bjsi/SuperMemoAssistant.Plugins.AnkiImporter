using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperMemoAssistant.Plugins.AnkiImporter.UI
{
  /// <summary>
  /// Interaction logic for ImporterWdw.xaml
  /// </summary>
  public partial class ImporterWdw : Window
  {
    public DeckTreeDictionary Trees { get; set; }

    public ImporterWdw(DeckTreeDictionary tree)
    {
      Trees = tree;
      InitializeComponent();
      Closing += ImporterWdw_Closing;
      DataContext = this;
    }

    private void ImporterWdw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Svc<AnkiImporterPlugin>.Plugin.CurrentInstance = null;
    }

    private void RefreshSMKT()
    {
      tv2.ItemsSource = Trees.Filtered.Values;
    }

    /// <summary>
    /// Intercepts the checkbox mouse click to implement custom checking behaviour.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      var checkBox = ((CheckBox)sender);
      var deck = ((Deck)checkBox.Tag);
      var pathToParent = DeckNameEx.GetNamePath(deck.Parentname);

      if (checkBox.IsChecked == true)
      {
        if (Trees.AncestorIsToImport(deck.Parentname))
        {
          // Uncheck from this deck's root until this deck's parent
          Trees.SetToImportOverRange(pathToParent, false);
        }
        else
        {
          // Uncheck this deck down through the hierarchy to the leaf decks
          deck.RecursivelySetToImport(false);
        }
      }
      else
      {
        // Check from this deck down through the hierarchy to the leaf decks
        deck.RecursivelySetToImport(true);
      }

      RefreshSMKT();
      e.Handled = true;
    }
  }
}
