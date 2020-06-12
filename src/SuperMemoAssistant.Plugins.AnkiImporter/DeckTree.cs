using Anotar.Serilog;
using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter
{

  public class DeckTreeDictionary : Dictionary<string, Deck>, INotifyPropertyChanged
  {

    // For XAML
    private bool _ToImport { get; set; } = false;
    public bool ToImport
    {
      get { return this._ToImport; }
      set
      {
        if (value != this._ToImport)
        {
          this._ToImport = value;
          NotifyPropertyChanged(nameof(ToImport));
        }
      }
    }

    public DeckTreeDictionary(Dictionary<long, Deck> decks = null)
    {
      if (decks == null || decks.Count == 0)
        return;
      
      var Decks = decks.Values.ToList();

      SortByLevel(Decks);
      foreach (var deck in Decks)
      {
        if (deck.IsRoot)
        {
          this.Add(deck.Name, deck);
        }
        else
        {
          string parentName = deck.Parentname;
          var parentDeck = this.Find(parentName);
          if (parentDeck == null)
          {
            LogTo.Error($"Failed to find parent deck {parentName}");
            continue;
          }
          parentDeck.ChildDecks.Add(deck.Name, deck);
        }
      }
    }

    /// <summary>
    /// Sorts a list of decks by deck level
    /// </summary>
    /// <param name="decks"></param>
    private static void SortByLevel(List<Deck> decks)
    {
      decks.Sort((x, y) => (x.Level).CompareTo(y.Level));
    }

    /// <summary>
    /// Attempts to find a deck in the tree according to its name.
    /// </summary>
    /// <param name="childName"></param>
    /// <returns></returns>
    public Deck Find(string name)
    {
      Deck target = null;

      // Root deck
      if (!name.Contains("::"))
      {
        if (!this.TryGetValue(name, out target))
          LogTo.Warning($"Failed to Find deck {name} in DeckTreeDictionary");
      }
      else
      {

        // Start by getting the root deck
        List<string> pathToDeck = DeckNameEx.GetNamePath(name);
        if (!this.TryGetValue(pathToDeck[0], out var cur))
        {
          LogTo.Error($"Failed to find deck {name} in deck tree because root doesn't exist");
          return null;
        }

        // Remove the root path
        pathToDeck.RemoveAt(0);

        // Iterate through child decks along the path to target
        foreach (string path in pathToDeck)
        {
          cur.ChildDecks.TryGetValue(path, out cur);
        }
        target = cur;
      }
      return target;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
