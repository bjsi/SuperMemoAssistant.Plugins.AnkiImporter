using SuperMemoAssistant.Plugins.AnkiImporter.CardRendering;
using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests.RenderingTests
{
  public class RendererTests
  {

    private static readonly string file = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    private DataAccess db { get; } = new DataAccess(file);

    [Fact]
    public async void CardQuestionPropertyReturnsRenderedQuestion()
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == 1518852959231;
      var results = await db.GetCardsAsync(filter);
      var card = results.First();

      string expected = "";
      string actual = card.Question;

      Assert.Equal(expected, actual);

    }

    [Fact]
    public async void CardAnswerPropertyReturnsRenderedAnswer()
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == 1518852959231;
      var results = await db.GetCardsAsync(filter);
      var card = results.First();

      string expected = "";
      string actual = card.Answer;

      Assert.Equal(expected, actual);

    }
  }
}
