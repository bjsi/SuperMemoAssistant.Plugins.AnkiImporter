using Stubble.Core;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anotar.Serilog;
using System.Text.RegularExpressions;
using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;

// See here for template / field info https://docs.ankiweb.net/#/templates/intro
// See here for special field eg {{ Tags }} info https://docs.ankiweb.net/#/templates/fields

namespace SuperMemoAssistant.Plugins.AnkiImporter.Rendering
{

  public enum TemplateType
  {
    Question,
    Answer
  }

  public class FieldRenderOptions
  {
    // Extract pictures from fields into their own components or leave them
    public bool ExtractPictures = true;
  }

  public partial class Renderer
  {

    // 3 capture groups: (1: cloze number), (2: text), (3: hint)
    public static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");

    // The card to be rendered.
    private Card Card { get; }

    // The first question handler in the render chain
    private TemplateKeyHandler FirstQuestionHandler { get; set; }

    // The first answer handler in the render chain
    private TemplateKeyHandler FirstAnswerHandler { get; set; }

    // Rendered
    public Dictionary<string, List<ContentBase>> RenderedAnswerFieldContentMap { get; set; } = new Dictionary<string, List<ContentBase>>();
    public Dictionary<string, List<ContentBase>> RenderedQuestionFieldContentMap { get; set; } = new Dictionary<string, List<ContentBase>>();

    public Renderer(Card Card)
    {
      this.Card = Card;
      FirstAnswerHandler = SetupAnswerHandlers();
      FirstQuestionHandler = SetupQuestionHandlers();
    }

    private TemplateKeyHandler SetupQuestionHandlers()
    {

      var QBaseHandlers = CreateBaseRenderChain();
      var FirstQBaseHandler = QBaseHandlers.Item1;
      var LastQBaseHanlder = QBaseHandlers.Item2;

      LastQBaseHanlder.Next = CreateQuestionRenderChain();

      return FirstQBaseHandler;

    }

    private TemplateKeyHandler SetupAnswerHandlers()
    {

      var ABaseHandlers = CreateBaseRenderChain();
      var FirstABaseHandler = ABaseHandlers.Item1;
      var LastABaseHandler = ABaseHandlers.Item2;

      LastABaseHandler.Next = CreateAnswerRenderChain();

      return FirstABaseHandler;
    }

    /// <summary>
    /// Create the stubble html render for card content.
    /// TODO: {{ hint: }} 
    /// TODO: {{ tts: }} filters.
    /// TODO: {{ type: }} the field content should be typed in (spelling component)
    /// TODO: {{ Back }}
    /// </summary>
    /// <returns>Renderer or Null</returns>
    public StubbleVisitorRenderer Create(TemplateType templateType)
    {

      if (this.Card == null)
      {
        LogTo.Warning("Failed to Create Renderer because card was null");
        return null;
      }

      var firstHandler = templateType == TemplateType.Question
        ? FirstQuestionHandler
        : FirstAnswerHandler;

      // Require a special value getter because  of anki's custom mustache templating.
      // The fieldContentMap is a mapping between field names and content,
      // but the template variables contain other information such as whether
      // the field should be formatted like a cloze, whether there is a hint and so on.

      return new StubbleBuilder()
      .Configure(settings =>
      {

        // stubble parses the anki card template parsing the strings between double braces {{ }}
        // to the custom value getter, TODO: give an example

        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {

          // The actual name of the field always comes last
          // Filters like cloze and hint are prepended to the field name, separated by colons :

          string fieldName = key
            ?.Split(':')
            ?.Last();

          if (string.IsNullOrEmpty(fieldName))
          {
            LogTo.Warning("fieldName passed to stubble custom value getter was null or empty");
            return string.Empty;
          }

          // A mapping from field names to field content
          var fieldContentMap = val as Dictionary<string, string>;
          if (fieldContentMap == null || fieldContentMap.Count == 0)
          {
            LogTo.Warning("FieldContentMap passed to stubble custom value getter was null or empty");
            return string.Empty;
          }

          string output = firstHandler.HandleRequest(key, fieldContentMap, string.Empty);

          if (string.IsNullOrEmpty(output))
            fieldContentMap.TryGetValue(fieldName, out output);

          output = output ?? string.Empty;

          //if (templateType == TemplateType.Question)
          //  RenderedQuestionFieldContentMap[fieldName] = CreateSMComponents(output, TemplateType.Question);
          //else
          //  RenderedAnswerFieldContentMap[fieldName] = CreateSMComponents(output, TemplateType.Answer);

          return output ?? string.Empty;

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();

    }

    private List<ContentBase> CreateSMComponents(string output, TemplateType type)
    {
      throw new NotImplementedException();
    }
  }
}
