using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests
{
  public class DeckTreeDictionaryTests
  {


    // Filtering

    [Fact]
    public void FilteredDeckTreeReturnsFiltered()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = true };
      var grandchild = new Deck() { Name = "parent::child::grandchild", ToImport = true};

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      var filtered = tree.Filtered;
      Assert.NotNull(filtered);
      Assert.Throws<KeyNotFoundException>(() => filtered["parent"]);
      Assert.NotNull(filtered["parent::child"]);
      Assert.NotNull(filtered["parent::child"].ChildDecks["parent::child::grandchild"]);
    }

    public void FilterEmptyReturnsEmpty()
    {
      var tree = new DeckTreeDictionary();
      var filtered = tree.Filtered;
      Assert.Empty(filtered);
    }

    // AncestorIsToImport
    public void AncestorIsToImportReturnsFalse()
    {
      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = true };
      var grandchild = new Deck() { Name = "parent::child::grandchild", ToImport = true};

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      Assert.False(tree.AncestorIsToImport(child.Parentname));
      Assert.True(tree.AncestorIsToImport(grandchild.Parentname));

    }

    // Deck Level sorting

    [Fact]
    public void SortByLevelSortsByLevel()
    {
      var decks = new List<Deck>();
      decks.Add(new Deck() { Name = "parent::child", ToImport = true });
      decks.Add(new Deck() { Name = "parent", ToImport = false });
      decks.Add(new Deck() { Name = "parent::child::grandchild", ToImport = true });
      DeckTreeDictionary.SortByLevel(decks);

      Assert.True(decks[0].Name == "parent");
      Assert.True(decks[1].Name == "parent::child");
      Assert.True(decks[2].Name == "parent::child::grandchild");
    }

    [Fact]
    public void SortByLevelOnNullDoesntThrow()
    {

      List<Deck> decks = null;
      DeckTreeDictionary.SortByLevel(decks);
      Assert.Equal(decks, null);
     
    }

    [Fact]
    public void SortByLevelOnEmptyReturnsEmpty()
    {
      List<Deck> decks = new List<Deck>();
      DeckTreeDictionary.SortByLevel(decks);
      Assert.NotNull(decks);
      Assert.Empty(decks);
    }

    [Fact]
    public void FindDeckReturnsDeck()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent" };
      var child = new Deck() { Name = "parent::child" };
      var grandchild = new Deck() { Name = "parent::child::grandchild" };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      // Find parent
      var p = tree.Find("parent");
      Assert.Equal(parent, p);
      // Find child
      var c = tree.Find("parent::child");
      Assert.Equal(child, c);

      var g = tree.Find("parent::child::grandchild");
      Assert.Equal(grandchild, g);
    }

    [Fact]
    public void DeckTreeReturnsDeckTree()
    {
      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent" };
      var child = new Deck() { Name = "parent::child" };
      var grandchild = new Deck() { Name = "parent::child::grandchild" };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      var p = tree["parent"];
      Assert.Equal(parent, p);
      var c = tree["parent"].ChildDecks["parent::child"];
      Assert.Equal(child, c);
      var g = tree["parent"].ChildDecks["parent::child"].ChildDecks["parent::child::grandchild"];
    }
  }
}
