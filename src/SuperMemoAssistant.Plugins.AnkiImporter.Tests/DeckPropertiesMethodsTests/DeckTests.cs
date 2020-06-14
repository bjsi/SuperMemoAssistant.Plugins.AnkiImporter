using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests.DeckPropertiesMethodsTests
{
  public class DeckTests
  {
    [Theory]
    [InlineData("parent::child::grandchild", "grandchild")]
    [InlineData("parent", "parent")]
    // TODO:
    [InlineData("", null)]
    [InlineData(null, null)]
    public void BasenameReturnsBasename(string name, string expected)
    {
      string actual = DeckNameEx.Basename(name);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("parent::child::grandchild", 2)]
    [InlineData("parent", 0)]
    // TODO:
    [InlineData("", -1)]
    [InlineData(null, -1)]
    public void LevelReturnsLevel(string name, int expected)
    {
      int actual = DeckNameEx.Level(name);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("parent::child::grandchild", "parent::child")]
    [InlineData("parent", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void ParentnameReturnsParentname(string name, string expected)
    {
      string actual = DeckNameEx.Parentname(name);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetNamePathReturnsPath()
    {
      string name = "parent::child::grandchild";
      var expected = new List<string>()
      {
        "parent",
        "parent::child",
        "parent::child::grandchild"
      };
      var actual = DeckNameEx.GetNamePath(name);
      Assert.Equal(expected, actual);

      name = "parent";
      expected = new List<string>()
      {
        "parent"
      };
      actual = DeckNameEx.GetNamePath(name);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetNamePathNullReturnsNull()
    {
      string name = null;
      var output = DeckNameEx.GetNamePath(name);
      Assert.Null(output);
    }

    [Fact]
    public void GetSelectedDeckReturnsDeck()
    {

      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = true };
      parent.ChildDecks.Add(child.Name, child);
      Assert.Equal(child, parent.GetSelectedDeck());

      parent = new Deck() { Name = "parent", ToImport = false };
      child = new Deck() { Name = "parent::child", ToImport = false };
      parent.ChildDecks.Add(child.Name, child);

      Assert.Null(parent.GetSelectedDeck());

    }

    [Fact]
    public void RecursivelySetToImportSetsToImport()
    {
      var parent = new Deck() { Name = "parent", ToImport = false };
      var child = new Deck() { Name = "parent::child", ToImport = false };
      parent.ChildDecks.Add(child.Name, child);
      parent.RecursivelySetToImport(true);
      Assert.True(parent.ToImport);
      Assert.True(child.ToImport);

      parent.RecursivelySetToImport(false);
      Assert.False(parent.ToImport);
      Assert.False(child.ToImport);

    }

  }
}
