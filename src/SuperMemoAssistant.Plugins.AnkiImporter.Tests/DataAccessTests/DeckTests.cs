using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests
{
  public class DeckTests
  {

    public string file = @"C:\Users\james\Desktop\Anki\temp\User 1\collection.anki2";

    [Theory]
    [InlineData(1)]
    public async void GetDeckByIdReturnsDeck(long id)
    {

      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);
      Assert.NotNull(deck);
      Assert.Equal(id, deck[id].Id);
      Assert.Equal(1, deck.Count);

    }

    [Theory]
    [InlineData(1)]
    public async void GetDeckByIdIncludesConfig(long id)
    {
      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);
      Assert.NotNull(deck[id].Config);
    }
    
    [Theory]
    [InlineData(1)]
    public async void GetDeckByIdIncludesCards(long id)
    {
      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);
      Assert.NotNull(deck[id].Cards);
    }
    

    [Fact]
    public async void GetDecksReturnsDecks()
    {
      var db = new DataAccess(file);
      var decks = await db.GetDecksAsync();
      Assert.NotNull(decks);
      Assert.NotEmpty(decks);
    }
  }
}
