using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Models
{
  [Alias("notes")]
  public class Note
  {
    [Alias("id")]
    public long Id { get; set; }

    [Alias("guid")]
    public string Guid { get; set; }

    [Alias("mid")]
    public long NoteTypeId { get; set; }

    [Alias("mod")]
    public long LastModificationTime { get; set; }

    // TODO
    [Alias("usn")]
    public int usn { get; set; }

    [Alias("tags")]
    public string Tags { get; set; }

    [Alias("flds")]
    public string FieldString { get; set; }

    [Alias("csum")]
    public long Checksum { get; set; }

    [Alias("flags")]
    public int Flags { get; set; }

    private Dictionary<string, string> _fields { get; set; }
    public Dictionary<string, string> Fields 
    { get
      {
        if (_fields == null)
        {
          var fieldContentMap = new Dictionary<string, string>();
          var values = FieldString.Split('\x1f');
          var keys = NoteType.Fields;
          keys.ForEach(k => fieldContentMap.Add(k.Name, values[k.Ordinal]));
          _fields = fieldContentMap;
        }
        return _fields;
      }
    }

    public NoteType NoteType { get; set; }
  }
}
