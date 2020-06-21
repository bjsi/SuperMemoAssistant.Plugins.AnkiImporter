using SuperMemoAssistant.Plugins.AnkiImporter.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests.RenderingTests
{
  public class RendererTests
  {

    [Fact]
    public void CreateRendererReturnsRenderer()
    {

      var renderer = new Renderer(0).Create(TemplateType.Question);
      Assert.NotNull(renderer);

    }

    [Theory]
    [InlineData("{{c1::your cloze::your hint}}", true, 1, "your cloze", "your hint")]
    public void RegexpMatchesCloze(string input, bool matched, int card, string cloze, string hint)
    {

      var regex = Renderer.ClozeRegex;
      Match match = regex.Match(input);

      Assert.Equal(matched, match.Success);
      Assert.Equal(card, int.Parse(match.Groups[1].Value));
      Assert.Equal(cloze, match.Groups[2].Value);
      Assert.Equal(hint, match.Groups[3].Value);

    }

    [Fact]
    public void CreateClozeQuestionCreatesClozeQuestion()
    {

      var input = new Dictionary<string, string>
      {
        { "cloze", "{{c1::your cloze::your hint}}" }
      };

      string expected = "<span class=\"cloze\">[your hint]</span>";
      var renderer = new Renderer(0).Create(TemplateType.Question);
      string actual = renderer.Render("{{ cloze }}", input);
      Assert.Equal(expected, actual);

    }

    [Fact]
    public void CreateClozeAnswerCreatesClozeAnswer()
    {

      var input = new Dictionary<string, string>
      {
        { "cloze", "{{c1::your cloze::your hint}}" }
      };

      string expected = "1: your cloze";
      var renderer = new Renderer(0).Create(TemplateType.Answer);
      string actual = renderer.Render("{{ cloze }}", input);
      Assert.Equal(expected, actual);

    }
  }
}
