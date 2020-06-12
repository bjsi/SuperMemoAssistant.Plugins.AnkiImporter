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
  }
}
