using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;
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

    private static readonly string file = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    private DataAccess db { get; } = new DataAccess(file);

    // Filtering
    [Fact]
    public void FilteredTreeReturnsFilteredTree()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = true };
      var grandchild = new Deck() { Name = "parent::child::grandchild", ToImport = true };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);
      var filtered = tree.Filtered;

      Assert.NotNull(filtered);
      Assert.Throws<KeyNotFoundException>(() => filtered["parent"]);
      Assert.Throws<KeyNotFoundException>(() => filtered["parent::child::grandchild"]);
      Assert.NotNull(filtered["parent::child"]);
      Assert.NotNull(filtered["parent::child"].ChildDecks["parent::child::grandchild"]);

    }

    [Fact]
    public async void GetDecksFilteredTreeBeginsEmpty()
    {

      var decks = await db.GetDecksAsync();
      var tree = new DeckTreeDictionary(decks);

      Assert.NotNull(tree);
      Assert.Empty(tree.Filtered);

    }

    public void FilteredTreeBeginsEmpty()
    {

      var tree = new DeckTreeDictionary();
      var filtered = tree.Filtered;

      Assert.Empty(filtered);

    }

    // AncestorIsToImport
    public void AncestorToImportReturnsFalse()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = true };
      var grandchild = new Deck() { Name = "parent::child::grandchild", ToImport = true };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      Assert.False(tree.AncestorToImportIsTrue(child.Parentname));
      Assert.True(tree.AncestorToImportIsTrue(grandchild.Parentname));

    }

    public void AncestorToImportReturnsTrue()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent", ToImport = true };
      var child = new Deck() { Name = "parent::child", ToImport = false };
      var grandchild = new Deck() { Name = "parent::child::grandchild", ToImport = false };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      Assert.True(tree.AncestorToImportIsTrue(child.Parentname));
      Assert.True(tree.AncestorToImportIsTrue(grandchild.Parentname));

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

    // Find Decks
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

      var p = tree.Find("parent");
      Assert.Equal(parent, p);

      var c = tree.Find("parent::child");
      Assert.Equal(child, c);

      var g = tree.Find("parent::child::grandchild");
      Assert.Equal(grandchild, g);

    }

    [Fact]
    public void FindNonExistentDeckReturnsNull()
    {

      var decks = new Dictionary<long, Deck>();
      var parent = new Deck() { Name = "parent" };
      var child = new Deck() { Name = "parent::child" };
      var grandchild = new Deck() { Name = "parent::child::grandchild" };

      decks.Add(1, parent);
      decks.Add(2, child);
      decks.Add(3, grandchild);

      var tree = new DeckTreeDictionary(decks);

      var p = tree.Find("james");
      Assert.Null(p);

      var c = tree.Find("ben");
      Assert.Null(c);

      var g = tree.Find("luke");
      Assert.Null(g);

    }

    [Fact]
    public void CreateDeckTreeReturnsDeckTree()
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
      Assert.Equal(grandchild, g);

    }

    [Fact]
    public async void DeckAllCardsReturnsCorrectNumberOfCards()
    {

      var decks = await db.GetDecksAsync();
      var tree = new DeckTreeDictionary(decks);

      Assert.Equal(0, tree["Default"].AllCards.Count);
      Assert.Equal(456, tree["Piano Basics"].AllCards.Count);

      Assert.Equal(240, tree["Piano Basics"].ChildDecks["Piano Basics::Notes"].AllCards.Count);
      Assert.Equal(216, tree["Piano Basics"].ChildDecks["Piano Basics::Chord Triads"].AllCards.Count);

      Assert.Equal(72, tree["Piano Basics"].ChildDecks["Piano Basics::Chord Triads"].ChildDecks["Piano Basics::Chord Triads::1st Inversion"].AllCards.Count);
      Assert.Equal(72, tree["Piano Basics"].ChildDecks["Piano Basics::Chord Triads"].ChildDecks["Piano Basics::Chord Triads::2nd Inversion"].AllCards.Count);
      Assert.Equal(72, tree["Piano Basics"].ChildDecks["Piano Basics::Chord Triads"].ChildDecks["Piano Basics::Chord Triads::Root Chords"].AllCards.Count);
      
    }
  }
}
