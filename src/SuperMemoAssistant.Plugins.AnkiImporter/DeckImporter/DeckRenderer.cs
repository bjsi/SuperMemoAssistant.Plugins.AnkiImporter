using Anotar.Serilog;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Types;
using SuperMemoAssistant.Plugins.AnkiImporter.CardRendering;
using SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.DeckImporter
{

  public class DeckImporterOptions
  {
  }

  public class DeckImporter
  {

    public DeckTreeDictionary DeckTree { get; }
    public DeckImporterOptions Options { get; }

    public DeckImporter(DeckTreeDictionary tree)
    {
      this.DeckTree = tree;
    }

    public DeckImporter(DeckTreeDictionary tree, DeckImporterOptions opts)
    {
      this.DeckTree = tree;
      this.Options = opts;
    }

    private IElement ImportDeck(Deck deck)
    {

      if (deck.IsNull())
      {
        LogTo.Warning("Failed to import deck because it was null");
        return null;
      }

      var builder = new AnkiDeckBuilder(deck);
      var elemBuilder = builder.CreateElementBuilder();
      return CreateSMElement(elemBuilder);

    }

    private IElement ImportDeck(Deck deck, IElement parent)
    {

      if (deck.IsNull())
      {
        LogTo.Warning("Failed to import deck because it was null");
        return null;
      }

      var builder = new AnkiDeckBuilder(deck);
      var elemBuilder = builder.CreateElementBuilder();
      return CreateSMElement(elemBuilder);

    }

    public void Import()
    {

      if (DeckTree.IsNull() || !DeckTree.Any())
      {
        LogTo.Warning("Failed to import because DeckTree was null or empty");
        return;
      }



      foreach (KeyValuePair<string, Deck> tree in DeckTree)
      {

        var name = tree.Key;
        var deck = tree.Value;
        LogTo.Debug($"Importing deck {name}");

        // Recurse deck tree
        var cur = deck;
        while (!cur.IsNull())
        {

          var deckElement = ImportDeck(cur);
          if (deckElement.IsNull())
          {
            LogTo.Error("Failed to import deck because deckElement was null");
            return;
          }

          foreach (var card in deck.Cards)
          {

          }
        }
      }
    }

    public IElement CreateSMElement(ElementBuilder builder)
    {

      Svc.SM.Registry.Element.Add(
        out var results,
        ElemCreationFlags.CreateSubfolders,
        builder
      );

      var result = results?.FirstOrDefault();
      return result.IsNull()
        ? null
        : Svc.SM.Registry.Element[result.ElementId];

    }
  }
}
