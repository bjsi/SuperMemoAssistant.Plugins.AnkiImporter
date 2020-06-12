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
