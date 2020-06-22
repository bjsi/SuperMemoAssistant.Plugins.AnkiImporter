#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   6/7/2020 3:11:44 AM
// Modified By:  james

#endregion


using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests
{
  public class CardTests
  {


    private static readonly string file = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    private DataAccess db { get; } = new DataAccess(file);

    [Fact]
    public async void GetCardsReturnsCards()
    {

      var cards = await db.GetCardsAsync();
      Assert.True(cards != null && cards.Count() == 456);

    }

    [Theory]
    [InlineData(1518852959231)]
    public async void GetCardEnsureDataCorrect(long id)
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == id;
      var results = await db.GetCardsAsync(filter);
      var card = results[0];

      Assert.Equal(1518852959231, card.Id);
      Assert.Equal(1518852959151, card.NoteId);
      Assert.Equal(1586701137476, card.DeckId);
      Assert.Equal(0, card.Ordinal);
      Assert.Equal(1586701137, card.LastModificationTime);
      Assert.Equal(-1, card.usn);
      Assert.Equal(0, card.Type);
      Assert.Equal(0, card.Queue);
      Assert.Equal(1, card.due);
      Assert.Equal(0, card.Interval);
      Assert.Equal(2500, card.Factor);
      Assert.Equal(0, card.Reps);
      Assert.Equal(0, card.Lapses);
      Assert.Equal(1001, card.left);
      Assert.Equal(0, card.OriginalDue);
      Assert.Equal(0, card.OriginalDeckId);
      Assert.Equal(0, card.Flags);

    }

    [Theory]
    [InlineData(1518852959231)]
    public async void GetCardIncludesNote(long id)
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == id;
      var results = await db.GetCardsAsync(filter);
      var card = results[0];

      Assert.NotNull(card.Note);
      Assert.Equal(card.NoteId, card.Note.Id);

    }

    [Fact]
    public async void GetCardsNoteNoteTypeNotNull()
    {

      var cards = await db.GetCardsAsync();

      foreach (var card in cards)
      {
        Assert.NotNull(card.Note.NoteType);
      }

    }

    [Fact]
    public async void GetCardsNoteFieldsNotNull()
    {

      var cards = await db.GetCardsAsync();

      foreach (var card in cards)
      {
        Assert.NotNull(card.Note.Fields);
      }
    }

    [Fact]
    public async void GetCardsCardTypeCorrect()
    {

      var cards = await db.GetCardsAsync();

      foreach (var card in cards)
      {
        // All cards in the Piano Basics collection are normal
        // TODO: Need to add cloze tests
        Assert.Equal(card.CardType, CardType.Normal);
      }
    }
  }
}
