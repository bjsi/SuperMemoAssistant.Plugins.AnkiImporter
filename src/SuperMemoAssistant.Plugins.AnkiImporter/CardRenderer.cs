using Anotar.Serilog;
using Stubble.Core;
using Stubble.Core.Builders;
using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter
{

  public enum TemplateType
  {
    Question,
    Answer
  }

  public class CardRenderer
  {
    private Card Card { get; set; }
    public CardRenderer(Card card)
    {
      Card = card;
    }

    /// <summary>
    /// Create the stubble html render for card content.
    /// TODO: Add type:hint:tts filters.
    /// {{Tags}} = The note's tags
    /// {{ Type }} = the Note's Model name
    /// {{ Deck }} = the card's deck
    /// {{ The card's subdeck }} 
    /// {{ Card }} = the type of the card???
    /// {{ FrontSide }} = the instantiated qfmt (only valid on the back side)
    /// </summary>
    /// <returns></returns>
    public StubbleVisitorRenderer CreateRenderer(TemplateType Template)
    {
      var stubble = new StubbleBuilder()
      .Configure(settings =>
      {
        settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
        {
          // Splits the {{ cloze:field }} handlebars in anki templates
          string realKey = key.Split(':')?.Last();
          var dict = val as Dictionary<string, string>;
          if (key.Contains("cloze"))
          {
            return Template == TemplateType.Question
              ? CreateClozeQuestion(dict[realKey])
              : CreateClozeAnswer(dict[realKey]);
          }
          return dict[key];
        })
        // allow unescaped html
        .SetEncodingFunction(x => x);
      }).Build();
      return stubble;
    }

    public string CreateClozeQuestion(string cloze)
    {
      // 3 capture groups: (1: cloze number), (2: text), (3: hint)
      Regex regex = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");
      string questionString = string.Empty;

      if (string.IsNullOrEmpty(cloze))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return questionString;
      }

      // Search for the cloze
      Match match = regex.Match(cloze);
      if (!match.Success || match.Groups.Count < 3)
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze regex didn't match");
        return questionString;
      }

      int prevIndex = 0;
      while (match.Success && match.Groups.Count >= 3)
      {
        // Get cloze information
        int clozeNumber = int.Parse(match.Groups[1].Value);
        int cardClozeNumber = Card.Ordinal + 1;
        string clozeText = match.Groups[2].Value;
        string clozeHint = null;
        int matchStart = match.Index;
        int matchEnd = match.Index + match.Length;

        // Get cloze hint if exists
        if (match.Groups.Count >= 4)
        {
          clozeHint = match.Groups[3].Value;
        }

        if (clozeNumber == cardClozeNumber)
        {
          questionString += cloze.Substring(prevIndex, matchStart - prevIndex);
          questionString += "<span class=\"cloze\">[";
          // Add hint or ...
          questionString += string.IsNullOrEmpty(clozeHint)
            ? "..."
            : clozeHint;
          questionString += "]</span>";
          prevIndex = matchEnd;
        }
        else
        {
          questionString += cloze.Substring(prevIndex, matchStart - prevIndex);
          questionString += clozeText;
          prevIndex = matchEnd;
        }
        match = match.NextMatch();
      }

      // If questionstring not null, add the end part.
      if (!string.IsNullOrEmpty(questionString))
      {
        questionString += cloze.Substring(prevIndex);
      }
      return questionString;
    }

    public string CreateClozeAnswer(string cloze)
    {
      // 3 capture groups: (1: cloze number), (2: text), (3: hint)
      Regex regex = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");
      Match match = regex.Match(cloze);
      bool matched = true;
      List<string> answerList = new List<string>();

      while (matched)
      {
        if (match.Success && match.Groups.Count >= 3)
        {
          int clozeNumber = int.Parse(match.Groups[1].Value);
          int cardClozeNumber = Card.Ordinal + 1;

          // If the cloze number == cardOrdinal + 1,
          // add the answer to the answerList 
          if (clozeNumber == cardClozeNumber)
          {
            answerList.Add(match.Groups[2].Value);
          }
          match = match.NextMatch();
        }
        else
        {
          matched = false;
        }
      }

      // Create the answerString
      string answerString = string.Empty;
      if (answerList != null && answerList.Count > 0)
      {
        int i = 1;
        foreach (var answer in answerList)
        {
          answerString += $"{i}: {answer}\n";
          i++;
        }
      }

      return answerString?.Trim();
    }

    public string AddCssStyling(string content)
    {
      string css = Card.Note.NoteType.CSS;
      if (!string.IsNullOrEmpty(css))
      {
        content = $"<html><style>{css}</style><body><div class=\"card\">{content}</div></body></html>";
      }
      return content;
    }

    public string RenderQuestion()
    {
      var renderer = CreateRenderer(TemplateType.Question);
      var question = renderer.Render(Card.Template.QuestionFormat, Card.Note.Fields);
      question = AddCssStyling(question);
      return question;
    }

    public string RenderAnswer()
    {
      var renderer = CreateRenderer(TemplateType.Answer);
      var answer = renderer.Render(Card.Template.AnswerFormat, Card.Note.Fields);
      answer = AddCssStyling(answer);
      return answer;
    }
  }
}
