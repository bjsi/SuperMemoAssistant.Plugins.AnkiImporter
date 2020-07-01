using Anotar.Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Rendering
{
  public partial class Renderer
  {
    public string CreateClozeAnswer(string key, Dictionary<string, string> fieldContentMap)
    {

      if (!fieldContentMap.TryGetValue(key, out var fieldContent))
      {
        LogTo.Error("Failed to CreateClozeAnswer because fieldContent is null");
        return string.Empty;
      }

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

      fieldContentMap[key] = answerString;

      return answerString?.Trim();
    }

    public string CreateClozeQuestion(string key, Dictionary<string, string> fieldContentMap)
    {

      string question = string.Empty;

      if (!fieldContentMap.TryGetValue(key, out var fieldContent))
      {
        LogTo.Error("Failed to CreateClozeQuestion because cloze is null");
        return question;
      }

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

      fieldContentMap[key] = question;

      return question;
    }
  }
}
