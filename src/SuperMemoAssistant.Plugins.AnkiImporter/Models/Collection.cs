using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Models
{ 

  /// <summary>
  /// Represents an Anki Collection
  /// </summary>
  [Alias("col")]
  public class Collection
  {

    [Alias("id")]
    public long Id { get; set; }

    [Alias("crt")]
    public long CreatedAt { get; set; }

    [Alias("mod")]
    public long LastModificationTime { get; set; }

    // TODO
    [Alias("scm")]
    public long scm { get; set; }

    // TODO
    [Alias("ver")]
    public int ver { get; set; }

    [Alias("dty")]
    public int dty { get; set; }

    // TODO
    [Alias("usn")]
    public int usn { get; set; }

    // TODO
    [Alias("ls")]
    public int ls { get; set; }

    [Alias("conf")]
    public string Config { get; set; }

    [Alias("models")]
    public string NoteTypes { get; set; }

    [Alias("dconf")]
    public string DeckConfigs { get; set; }

    [Alias("decks")]
    public string Decks { get; set; }

    [Alias("tags")]
    public string Tags { get; set; }
  }
}
