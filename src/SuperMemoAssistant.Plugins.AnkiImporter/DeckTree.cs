using Anotar.Serilog;
using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter
{

  /// <summary>
  /// TODO: Document in detail
  /// </summary>
  public class DeckTreeDictionary : ObservableDictionary<string, Deck> 
  {

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

    public DeckTreeDictionary Filtered 
    { 
      get 
      {
        var filteredTrees = new DeckTreeDictionary();
        foreach (KeyValuePair<string, Deck> keyValuePair in this)
        {
          var rootDeck = keyValuePair.Value;
          var selectedDeck = rootDeck.GetSelectedDeck();
          if (selectedDeck != null)
            filteredTrees.Add(selectedDeck.Name, selectedDeck);
        }
        return filteredTrees;
      }
    }

    /// <summary>
    /// Returns true if an ancestor between the root and the parentName has true for the ToImport property.
    /// </summary>
    public bool AncestorToImportIsTrue(string parentName)
    {

      List<string> pathToParent = DeckNameEx.GetNamePath(parentName);
      if (pathToParent != null && pathToParent.Count > 0)
      {
        var rootDeck = this[pathToParent[0]];
        if (rootDeck.ToImport)
          return true;

        pathToParent.RemoveAt(0);
        var currentDeck = rootDeck;
        foreach (var deck in pathToParent)
        {
          if (currentDeck.ChildDecks[deck].ToImport)
          {
            return true;
          }
          currentDeck = currentDeck.ChildDecks[deck];
        }
      }

      return false;

    }

    /// <summary>
    /// Set the ToImportValue over a range of decks.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ToImportValue"></param>
    public void SetToImportOverRange(List<string> path, bool ToImportValue)
    {
      if (path == null || path.Count == 0)
        return;

      Deck root = this[path[0]];
      root.ToImport = ToImportValue;
      path.RemoveAt(0);
      var currentPathValue = root;
      foreach (var node in path)
      {
        currentPathValue.ChildDecks[node].ToImport = ToImportValue;
        currentPathValue = currentPathValue.ChildDecks[node];
      }
    }

    /// <summary>
    /// Sorts a list of decks by deck level
    /// </summary>
    /// <param name="decks"></param>
    public static void SortByLevel(List<Deck> decks)
    {
      decks?.Sort((x, y) => (x.Level).CompareTo(y.Level));
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
  }
}
