using Anotar.Serilog;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.DeckImporter
{

  public class AnkiDeckImportOptions
  {
    public bool AsConcept { get; set; } = false;
  }

  public class AnkiDeckBuilder
  {

    public Deck Deck { get; }
    public AnkiDeckImportOptions Options { get; }

    public AnkiDeckBuilder(Deck deck)
    {
      this.Deck = deck;
    }

    public AnkiDeckBuilder(Deck deck, AnkiDeckImportOptions opts)
    {
      this.Deck = deck;
      this.Options = opts;
    }

    public ElementBuilder CreateElementBuilder()
    {

      if (Deck.IsNull())
      {
        LogTo.Warning("Failed to CreateElementBuilder because Deck is null");
        return null;
      }

      ElementType type = Options.AsConcept
        ? ElementType.ConceptGroup
        : ElementType.Topic;

      return new ElementBuilder(
        type,
        Array.Empty<ContentBase>()
      )
      .DoNotDisplay();

    }
  }
}
