using Stubble.Core;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anotar.Serilog;
using System.Text.RegularExpressions;

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
    private static Regex ClozeRegex { get; } = new Regex(@"\{\{c(\d+)::(.*?)(?:::(.*?))?\}\}");

    /// <summary>
    /// The ordinal of the card.
    /// </summary>
    private int Ordinal { get; }

    public Renderer(int Ordinal)
    {
      this.Ordinal = Ordinal;
    }

    /// <summary>
    /// PatternTransformationList for Question Templates
    /// </summary>
    public PatternTransformationList QPatternTransList => new PatternTransformationList
    {
      new Tuple<Pattern, Transformation>(
        new Pattern(x => string.IsNullOrEmpty(x) ? false : x.Contains("cloze")),
        new Transformation(x => CreateClozeQuestion(x))
      )
    };

    /// <summary>
    /// PatternTransformationList for Answer Templates
    /// </summary>
    public PatternTransformationList APatternTransList => new PatternTransformationList
    {
      new Tuple<Pattern, Transformation>(
        new Pattern(x => string.IsNullOrEmpty(x) ? false : x.Contains("cloze")),
        new Transformation(x => CreateClozeAnswer(x))
      )
    };

    public string CreateClozeAnswer(string fieldContent)
    {
      Match match = ClozeRegex.Match(fieldContent);
      bool matched = true;
      List<string> answerList = new List<string>();

      while (matched)
      {
        if (match.Success && match.Groups.Count >= 3)
        {
          int clozeNumber = int.Parse(match.Groups[1].Value);
          int cardClozeNumber = Ordinal + 1;

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
        // Get cloze information
        int clozeNumber = int.Parse(match.Groups[1].Value);
        int cardClozeNumber = Ordinal + 1;
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
    /// {{ FrontSide }} = the instantiated qfmt (only valid on the back side)
    /// </summary>
    /// <returns></returns>
    public StubbleVisitorRenderer Create(TemplateType templateType)
    {

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
