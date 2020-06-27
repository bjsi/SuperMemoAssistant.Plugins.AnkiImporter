using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using SuperMemoAssistant.Plugins.AnkiImporter.Rendering;
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

    private static readonly string file = @"C:\Users\polit\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";
    private DataAccess db { get; } = new DataAccess(file);

    [Fact]
    public void CreateRendererReturnsRenderer()
    {

      var card = new Card { Ordinal = 0 };
      var renderer = new Renderer(card).Create(TemplateType.Question);
      Assert.NotNull(renderer);

    }

    [Fact]
    public void CreateRendererHandleNullCard()
    {

      var renderer = new Renderer(null).Create(TemplateType.Question);
      Assert.Null(renderer);

    }

    [Fact]
    public async void RenderCardQuestionReturnsCorrect()
    {
      
      Expression<Func<Card, bool>> filter = (c) => c.Id == 1518852959231;
      var results = await db.GetCardsAsync(filter);
      var card = results.First();

      var renderer = new Renderer(card).Create(TemplateType.Question);

      string expected = "";
      string actual = renderer.Render(card.Template.QuestionFormat, card.Note.Fields);

      Assert.Equal(expected, actual);

    }

    [Fact]
    public async void RenderCardAnswerReturnsCorrect()
    {

      Expression<Func<Card, bool>> filter = (c) => c.Id == 1518852959231;
      var results = await db.GetCardsAsync(filter);
      var card = results.First();

      var renderer = new Renderer(card).Create(TemplateType.Question);

      string expected = "";
      string actual = renderer.Render(card.Template.AnswerFormat, card.Note.Fields);

      Assert.Equal(expected, actual);

    }

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

    // TODO: Gets rendered with a bunch of escaped newlines and tabs which messes up the styling of the html
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
