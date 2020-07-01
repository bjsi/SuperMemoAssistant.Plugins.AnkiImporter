using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Rendering
{
  public partial class Renderer
  {

    /// <summary>
    /// Base render chain shared by Question and Answer.
    /// </summary>
    private Tuple<TemplateKeyHandler, TemplateKeyHandler> CreateBaseRenderChain()
    {

      var tagsHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "Tags",
        Action = (x, y) => Card.Note.Tags,
        TerminatesChain = false
      };

      var typeHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "Type",
        Action = (x, y) => Card.Note.NoteType.Name,
        TerminatesChain = false
      };

      tagsHandler.Next = typeHandler;

      var subdeckHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "Subdeck",
        Action = (x, y) => Card.Deck.Basename,
        TerminatesChain = false
      };

      tagsHandler.Next = subdeckHandler;

      var deckHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "Deck",
        Action = (x, y) => Card.Deck.Name,
        TerminatesChain = false
      };

      subdeckHandler.Next = deckHandler;

      var cardHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "Card",
        // TODO: Check this is correct
        Action = (x, y) => Card.Template.Name,
        TerminatesChain = false
      };

      deckHandler.Next = cardHandler;

      return new Tuple<TemplateKeyHandler, TemplateKeyHandler>(tagsHandler, cardHandler);
    }

    private TemplateKeyHandler CreateQuestionRenderChain()
    {

      var clozeHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key.Split(':').Any(x => x == "cloze"),
        Action = (key, fieldContentMap) => CreateClozeQuestion(key, fieldContentMap),
        TerminatesChain = false
      };

      return clozeHandler;

    }

    private TemplateKeyHandler CreateAnswerRenderChain()
    {

      var clozeHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key.Split(':').Any(x => x == "cloze"),
        Action = (key, fieldContentMap) => CreateClozeAnswer(key, fieldContentMap),
        TerminatesChain = false
      };

      var frontSideHandler = new TemplateKeyHandler
      {
        Pattern = key => string.IsNullOrEmpty(key) ? false : key == "FrontSide",
        Action = (x, y) => Card.Question,
        TerminatesChain = false
      };

      clozeHandler.Next = frontSideHandler;

      return clozeHandler;

    }
  }
}
