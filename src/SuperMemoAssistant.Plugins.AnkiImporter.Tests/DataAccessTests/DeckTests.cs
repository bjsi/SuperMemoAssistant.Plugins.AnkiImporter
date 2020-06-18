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

    public string file = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";

    [Theory]
    [InlineData(1)]
    [InlineData(1586701137475)]
    [InlineData(1586701137476)]
    [InlineData(1586701137479)]
    [InlineData(1586701137480)]
    [InlineData(1586701137481)]
    [InlineData(1586701137482)]
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
    [InlineData(1586701137475)]
    [InlineData(1586701137479)]
    public async void GetEmptyDecksReturnsEmptyDecks(long id)
    {
      
      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);

      Assert.Empty(deck[id].Cards);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1586701137475, 0)]
    [InlineData(1586701137476, 240)]
    [InlineData(1586701137479, 0)]
    [InlineData(1586701137480, 72)]
    [InlineData(1586701137481, 72)]
    [InlineData(1586701137482, 72)]
    public async void GetDecksReturnsCorrectNumberOfCards(long id, int cards)
    {

      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);

      Assert.Equal(deck[id].Cards.Count, cards);

    }


    [Theory]
    [InlineData(1)]
    // These don't have configs for some reason
    [InlineData(1586701137475)]
    [InlineData(1586701137476)]
    [InlineData(1586701137479)]
    [InlineData(1586701137480)]
    [InlineData(1586701137481)]
    [InlineData(1586701137482)]
    public async void GetDeckByIdIncludesConfig(long id)
    {

      var db = new DataAccess(file);
      var deck = await db.GetDecksAsync(x => x.Key == id);

      Assert.NotNull(deck[id].Config);
      Assert.Equal(deck[id].Config.Id, id);

    }

    [Fact]
    public async void GetDeckDefaultDeckIsEmpty()
    {
      long id = 1;
      var db = new DataAccess(file);
      var decks = await db.GetDecksAsync(x => x.Key == id);

      Assert.Empty(decks[id].Cards);

    }

    [Theory]
    // [InlineData(1)] Default deck is empty
    // [InlineData(1586701137475)] Piano Basics is empty
    [InlineData(1586701137476)]
    //[InlineData(1586701137479)] Chord Triads is empty
    [InlineData(1586701137480)]
    [InlineData(1586701137481)]
    [InlineData(1586701137482)]
    public async void GetDeckByIdIncludesCards(long id)
    {

      var db = new DataAccess(file);
      var decks = await db.GetDecksAsync(x => x.Key == id);
      var deck = decks[id];

      Assert.NotNull(deck.Cards);
      Assert.NotEmpty(deck.Cards);

      foreach (var card in deck.Cards)
      {
        Assert.Equal(card.DeckId, deck.Id);
      }

    }

    [Theory]
    // [InlineData(1)]
    [InlineData(1586701137475)]
    [InlineData(1586701137476)]
    [InlineData(1586701137479)]
    [InlineData(1586701137480)]
    [InlineData(1586701137481)]
    [InlineData(1586701137482)]
    public async void GetDeckByIdIncludesCardsCardsIncludeDeck(long id)
    {

      var db = new DataAccess(file);
      var results = await db.GetDecksAsync(x => x.Key == id);
      var deck = results[id];

      foreach (var card in deck.Cards)
      {
        Assert.NotNull(card.Deck);
        Assert.True(deck.Id == card.Deck.Id);
      }

    }

    [Fact]
    public async void GetDecksReturnsDecks()
    {

      var db = new DataAccess(file);
      var decks = await db.GetDecksAsync();

      Assert.NotNull(decks);
      Assert.NotEmpty(decks);
      Assert.True(decks.Count == 7);
      Assert.NotNull(decks[1]);
      Assert.NotNull(decks[1586701137475]);
      Assert.NotNull(decks[1586701137476]);
      Assert.NotNull(decks[1586701137479]);
      Assert.NotNull(decks[1586701137480]);
      Assert.NotNull(decks[1586701137481]);
      Assert.NotNull(decks[1586701137482]);

    }

  }
}
