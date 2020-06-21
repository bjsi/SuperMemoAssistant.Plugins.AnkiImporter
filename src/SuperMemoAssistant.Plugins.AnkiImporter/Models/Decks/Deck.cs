﻿using Newtonsoft.Json;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Models.Decks
{

  /// <summary>
  /// Represents an Anki Deck.
  /// TODO: Consider 
  /// </summary>
  public class Deck : INotifyPropertyChanged
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("collapsed")]
    public bool IsCollapsed { get; set; }

    [JsonProperty("conf")]
    public int ConfigId { get; set; }

    [JsonProperty("desc")]
    public string Description { get; set; }

    // TODO
    [JsonProperty("dyn")]
    public int dyn { get; set; }

    // TODO
    [JsonProperty("extendNew")]
    public int extendNew { get; set; }

    // TODO
    [JsonProperty("extendRev")]
    public int extendRev { get; set; }

    // TODO
    [JsonProperty("lrnToday")]
    public List<int> lrnToday { get; set; }

    [JsonProperty("mod")] 
    public long LastModificationTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // TODO
    [JsonProperty("newToday")]
    public List<int> newToday { get; set; }

    // TODO
    [JsonProperty("revToday")]
    public List<int> revToday { get; set; }

    [JsonProperty("timeToday")]
    public List<int> timeToday { get; set; }

    // TODO
    [JsonProperty("usn")]
    public int usn { get; set; }

    /// <summary>
    /// Decks that are children of the current deck.
    /// </summary>
    public DeckTreeDictionary ChildDecks { get; set; } = new DeckTreeDictionary();

    /// <summary>
    /// Cards that are members of this deck. Does NOT include subdecks!
    /// </summary>
    public List<Card> Cards { get; set; } = new List<Card>();

    /// <summary>
    /// Deck config options
    /// </summary>
    public DeckConfig Config { get; set; }

    /// <summary>
    /// Recursively set the ToImport value for decks in the hierarchy.
    /// </summary>
    /// <param name="ToImportValue">True if user wants to import else False</param>
    public void RecursivelySetToImport(bool ToImportValue)
    {
      ToImport = ToImportValue;
      if (ChildDecks != null && ChildDecks.Count > 0)
      {
        foreach (KeyValuePair<string, Deck> keyValuePair in ChildDecks)
        {
          var child = keyValuePair.Value;
          child.RecursivelySetToImport(ToImportValue);
        }
      }
    }

    /// <summary>
    /// Called recursively to get the first selected deck (the one closest to the root)
    /// for the path between and including the current deck -> leaf decks
    /// </summary>
    /// <returns>The selected deck closest to the root or null if there are no selected decks</returns>
    public Deck GetSelectedDeck()
    {

      Deck selectedDeck = null;

      // If this deck is ToImport, return it
      if (this.ToImport)
      {
        selectedDeck = this;
      }
      // Otherwise, recursively check child decks
      else
      {
        if (this.ChildDecks == null || this.ChildDecks.Count == 0)
          return selectedDeck;

        foreach (KeyValuePair<string, Deck> childDeck in this.ChildDecks)
        {
          var deck = childDeck.Value;
          selectedDeck = deck.GetSelectedDeck();
          if (selectedDeck != null)
            break;
        }
      }

      return selectedDeck;
    }

    public int Level { 
      get 
      {
        return DeckNameEx.Level(Name);
      } 
    }

    /// <summary>
    /// Get all child deck cards recursively, beginning at and including the current deck.
    /// TODO: Refactor these two methods into one?
    /// </summary>
    public List<Card> AllCards 
    {
      get
      {
        List<Card> allCards = new List<Card>();
        allCards.AddRange(Cards);
        foreach (KeyValuePair<string, Deck> keyValuePair in ChildDecks)
        {
          var deck = keyValuePair.Value;
          allCards.AddRange(deck.RecursivelyGetCards());
        }
        return allCards;
      }
    }

    private List<Card> RecursivelyGetCards()
    {

      List<Card> ret = new List<Card>();
      ret.AddRange(Cards);
      foreach(KeyValuePair<string, Deck> keyValuePair in ChildDecks)
      {
        var deck = keyValuePair.Value;
        ret.AddRange(deck.RecursivelyGetCards());
      }
      return ret;
    }

    public bool IsRoot { 
      get 
      {
        return DeckNameEx.IsRoot(Name);
      } 
    }

    public string Basename { get 
      {
        return DeckNameEx.Basename(Name);
      } 
    }

    public string Parentname 
    { get 
      {
        return DeckNameEx.Parentname(Name);
      } 
    }

    public List<string> GetDeckPath()
    {
      return DeckNameEx.GetNamePath(Name);
    }

    // For UI
    private bool _ToImport { get; set; } = false;
    public bool ToImport
    {
      get { return this._ToImport; }
      set
      {
        if (value != this._ToImport)
        {
          this._ToImport = value;
          NotifyPropertyChanged(nameof(ToImport));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }

  public class DeckConfig
  {

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("autoplay")]
    public bool Autoplay { get; set; }

    // TODO
    [JsonProperty("dyn")]
    public bool dyn { get; set; }

    // TODO
    [JsonProperty("lapse")]
    public Dictionary<string, object> lapse { get; set; }

    // TODO
    [JsonProperty("maxTaken")]
    public int maxTaken { get; set; }

    [JsonProperty("mod")]
    public long LastModificationTime { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // TODO
    [JsonProperty("new")]
    public Dictionary<string, object> New { get; set; }

    [JsonProperty("replayq")]
    public bool ReplayQuestion { get; set; }

    // TODO
    [JsonProperty("rev")]
    public Dictionary<string, object> rev { get; set; }

    // TODO
    [JsonProperty("timer")]
    public int Timer { get; set; }

    // TODO
    [JsonProperty("usn")]
    public int usn { get; set; }
  }
}