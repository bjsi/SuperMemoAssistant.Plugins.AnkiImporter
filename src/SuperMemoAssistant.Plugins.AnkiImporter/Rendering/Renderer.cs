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

// See here for template / field info https://docs.ankiweb.net/#/templates/intro
// See here for special field eg {{ Tags }} info https://docs.ankiweb.net/#/templates/fields

namespace SuperMemoAssistant.Plugins.AnkiImporter.Rendering
{

  // Type aliases
  using Pattern = Func<string, bool>;
  using Transformation = Func<string, string>;
  using PatternTransformationList = List<Tuple<Func<string, bool>, Func<string, string>>>;

  public class Renderer
  {

    /// <summary>
    /// Matches clozes in a field content string
    /// 3 capture groups: (1: cloze number), (2: text), (3: hint)
    /// </summary>
    public static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");

    /// <summary>
    /// The card to be rendered.
    /// </summary>
    private Card Card { get; }

    public Renderer(Card Card)
    {
      this.Card = Card;
      this.QPatternTransList.AddRange(baseList);
      this.APatternTransList.AddRange(baseList);
    }

    /// <summary>
    /// baseList is shared by the question and answer lists
    /// ///baseList is shared by the question and answer lists.
    /// </summary>
    private PatternTransformationList baseList => new PatternTransformationList
    {

      // eg. {{ Tags }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "Tags"),
        new Transformation( _ => Card.Note.Tags)
      ),

      // eg. {{ Type }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "Type"),
        new Transformation( _ => Card.Note.NoteType.Name)
      ),

      // eg. {{ Subdeck }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "Subdeck"),
        // TODO: Check this is correct...
        new Transformation( _ => Card.Deck.Basename)
      ),

      // eg. {{ Deck }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "Deck"),
        new Transformation( _ => Card.Deck.Name)
      ),

      // eg. {{ Card }} TODO: Check this is correct
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "Card"),
        new Transformation( _ => Card.Template.Name)
      )

    };

    /// <summary>
    /// PatternTransformationList for Question Templates
    /// </summary>
    public PatternTransformationList QPatternTransList => new PatternTransformationList
    {

      // TODO: {{ hint:cloze:Text }}
      // eg. {{ cloze:Text }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key.Split(':').Any(x => x == "cloze")),
        new Transformation(fieldContent => CreateClozeQuestion(fieldContent))
      ),

    };

    /// <summary>
    /// PatternTransformationList for Answer Templates
    /// </summary>
    public PatternTransformationList APatternTransList => new PatternTransformationList
    {

      // TODO: {{ hint:cloze:Text }}
      // eg. {{ cloze:Text }}
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key.Split(':').Any(x => x == "cloze")),
        new Transformation(fieldContent => CreateClozeAnswer(fieldContent))
      ),

      // eg. {{ FrontSide }}
      // Returns the rendered Question side
      new Tuple<Pattern, Transformation>(
        new Pattern(key => string.IsNullOrEmpty(key) ? false : key == "FrontSide"),
        new Transformation( _ => Card.Question)
      )

    };

    public string CreateClozeAnswer(string fieldContent)
    {

      if (string.IsNullOrEmpty(fieldContent))
      {
        LogTo.Error("Failed to CreateClozeAnswer because fieldContent is null");
        return string.Empty;
      }

      Match match = ClozeRegex.Match(fieldContent);
      List<string> answerList = new List<string>();

      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze");
          continue;
        }

        int cardClozeNumber = Card.Ordinal + 1;

        // If the cloze number == cardOrdinal + 1,
        // add the answer to the answerList 
        if (clozeNumber == cardClozeNumber)
        {
          answerList.Add(match.Groups[2].Value);
        }
        match = match.NextMatch();
      }

      // Create the answerString
      string answerString = string.Empty;
      if (answerList != null && answerList.Count > 0)
      {

        // Create a list of answers
        for (int i = 0; i < answerList.Count; i++)
        {
          answerString += $"{i + 1}: {answerList[i]}";
        }

      }

      return answerString?.Trim();
    }

    public string CreateClozeQuestion(string fieldContent)
    {

      string question = string.Empty;

      if (string.IsNullOrEmpty(fieldContent))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return question;
      }

      // Search for the cloze
      Match match = ClozeRegex.Match(fieldContent);
      if (!match.Success || match.Groups.Count < 3)
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze regex didn't match");
        return question;
      }

      int prevIndex = 0;
      while (match.Success && match.Groups.Count >= 3)
      {

        int clozeNumber;
        if (!int.TryParse(match.Groups[1].Value, out clozeNumber))
        {
          LogTo.Error("Failed to parse clozeNumber from cloze.");
          continue;
        }

        int cardClozeNumber = Card.Ordinal + 1;
        string clozeText = match.Groups[2].Value;
        int matchStart = match.Index;
        int matchEnd = match.Index + match.Length;

        // Get cloze hint if exists
        string clozeHint = null;
        if (match.Groups.Count >= 4)
        {
          clozeHint = match.Groups[3].Value;
        }

        if (clozeNumber == cardClozeNumber)
        {
          question += fieldContent.Substring(prevIndex, matchStart - prevIndex);
          question += "<span class=\"cloze\">[";
          // Add hint or ...
          question += string.IsNullOrEmpty(clozeHint)
            ? "..."
            : clozeHint;
          question += "]</span>";
          prevIndex = matchEnd;
        }
        else
        {
          question += fieldContent.Substring(prevIndex, matchStart - prevIndex);
          question += clozeText;
          prevIndex = matchEnd;
        }
        match = match.NextMatch();
      }

      // If questionstring not null, add the end part.
      if (!string.IsNullOrEmpty(question))
      {
        question += fieldContent.Substring(prevIndex);
      }
      return question;
    }

    /// <summary>
    /// Create the stubble html render for card content.
    /// TODO: Add type:hint:tts filters.
    /// TODO: Add {{Tags}} = The note's tags, {{ Type }} = the Note's Model name, {{ Deck }} = the card's deck, {{ The card's subdeck }}, {{ Card }} = the type of the card???
    /// TODO: {{ type: }} the field content should be typed in (spelling component)
    /// TODO: {{ Back }}
    /// TODO: {{ FrontSide }} = the instantiated qfmt (only valid on the back side)
    /// </summary>
    /// <returns>Renderer or Null</returns>
    public StubbleVisitorRenderer Create(TemplateType templateType)
    {

      if (this.Card == null)
      {
        LogTo.Warning("Failed to Create Renderer because card was null");
        return null;
      }

      // TODO: document
      var patternTransformationList = templateType == TemplateType.Question
        ? QPatternTransList
        : APatternTransList;

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

          foreach (var patternAction in patternTransformationList)
          {

            // The first pattern that matches the key will execute the transformation function on the fieldContent

            var pattern = patternAction.Item1;
            var transformation = patternAction.Item2;

            if (pattern(key))
            {

              if (!fieldContentMap.TryGetValue(fieldName, out var fieldContent))
              {
                LogTo.Warning($"Stubble custom value getter failed to find {fieldName} in fieldContentMap");
                return string.Empty;
              }

              return transformation(fieldContent);
            }

          }

          fieldContentMap.TryGetValue(fieldName, out var ret);
          return ret ?? string.Empty;

        })
        .SetEncodingFunction(x => x); // allow unescaped html
      }).Build();
    }
  }
}
