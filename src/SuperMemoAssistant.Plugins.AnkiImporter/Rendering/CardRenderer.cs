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

    ///// <summary>
    ///// Create the stubble html render for card content.
    ///// TODO: Add type:hint:tts filters.
    ///// {{Tags}} = The note's tags
    ///// {{ Type }} = the Note's Model name
    ///// {{ Deck }} = the card's deck
    ///// {{ The card's subdeck }} 
    ///// {{ Card }} = the type of the card???
    ///// {{ FrontSide }} = the instantiated qfmt (only valid on the back side)
    ///// </summary>
    ///// <returns></returns>
    //public StubbleVisitorRenderer CreateRenderer(TemplateType Template)
    //{
    //  var stubble = new StubbleBuilder()
    //  .Configure(settings =>
    //  {
    //    settings.AddValueGetter(typeof(Dictionary<string, string>), (val, key, ignoreCase) =>
    //    {
    //      // Splits the {{ cloze:field }} handlebars in anki templates
    //      string realKey = key.Split(':')?.Last();
    //      var dict = val as Dictionary<string, string>;
    //      if (key.Contains("cloze"))
    //      {
    //        return Template == TemplateType.Question
    //          ? CreateClozeQuestion(dict[realKey])
    //          : CreateClozeAnswer(dict[realKey]);
    //      }
    //      return dict[key];
    //    })
    //    // allow unescaped html
    //    .SetEncodingFunction(x => x);
    //  }).Build();
    //  return stubble;
    //}
  }
}
