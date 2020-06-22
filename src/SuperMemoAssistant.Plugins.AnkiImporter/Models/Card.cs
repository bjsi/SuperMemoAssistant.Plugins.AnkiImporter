using ServiceStack.DataAnnotations;
using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;
using SuperMemoAssistant.Plugins.AnkiImporter.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Models
{
  public enum CardType
  {
    Cloze,
    Normal
  }

  [Alias("cards")]
  public class Card
  {
    [Alias("id")]
    public long Id { get; set; }

    [Alias("nid")]
    public long NoteId { get; set; }

    [Alias("did")]
    public long DeckId { get; set; }

    [Alias("ord")]
    public int Ordinal { get; set; }

    [Alias("mod")]
    public long LastModificationTime { get; set; }

    // TODO
    [Alias("usn")]
    public int usn { get; set; }

    [Alias("type")]
    public int Type { get; set; }

    [Alias("queue")]
    public int Queue { get; set; }

    // TODO
    [Alias("due")]
    public int due { get; set; }

    [Alias("ivl")]
    public int Interval { get; set; }

    [Alias("factor")]
    public int Factor { get; set; }

    [Alias("reps")]
    public int Reps { get; set; }

    [Alias("lapses")]
    public int Lapses { get; set; }

    // TODO:
    [Alias("left")]
    public int left { get; set; }

    [Alias("odue")]
    public int OriginalDue { get; set; }

    [Alias("odid")]
    public int OriginalDeckId { get; set; }

    [Alias("flags")]
    public int Flags { get; set; }

    public Template Template
    {
      get
      {
        // TODO: Check this is correct
        return CardType == CardType.Cloze
          ? Note.NoteType.Templates.Where(t => t.Ordinal == 0).First()
          : Note.NoteType.Templates.Where(t => t.Ordinal == Ordinal).First();
      }
    }

    public CardType CardType {
      get
      {
        return Note.NoteType.Type == 1
          ? CardType.Cloze
          : CardType.Normal;
      }
    }

    /// <summary>
    /// Rendering result cached in private field.
    /// </summary>
    private string _question { get;  set; }

    /// <summary>
    /// Renders the question and returns it
    /// </summary>
    public string Question
    {
      get
      {

        if (string.IsNullOrEmpty(_question))
        {
          var renderer = new Renderer(this).Create(TemplateType.Question);
          string question = renderer.Render(Template.QuestionFormat, Note.Fields);
          _question = $"<html><style>{Note.NoteType.CSS}</style><body><div class=\"card\">{question}</div></body></html>";
        }

        return _question;
      }
    }

    /// <summary>
    /// Rendering result cached in private field.
    /// </summary>
    private string _answer { get; set; }
    public string Answer { 
      get 
      { 

        if (string.IsNullOrEmpty(_answer))
        {
          var renderer = new Renderer(this).Create(TemplateType.Answer);
          string answer = renderer.Render(Template.AnswerFormat, Note.Fields);
          _answer = $"<html><style>{Note.NoteType.CSS}</style><body><div class=\"card\">{answer}</div></body></html>";
        }

        return _answer;
      } 
    }

    // Relationships
    [Reference]
    public Note Note { get; set; }

    public Deck Deck { get; set; }

  }
}
