using SuperMemoAssistant.Plugins.AnkiImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests
{
  public class NoteTypeTests
  {

    public string file = @"C:\Users\james\Desktop\Anki\temp\User 1\collection.anki2";

    [Theory]
    [InlineData(1518831358666)]
    public async void GetNoteTypeEnsureValid(long id)
    {

      var db = new DataAccess(file);
      var noteTypes = await db.GetNoteTypesAsync();
      var noteType = noteTypes[id];

      Assert.NotNull(noteType);
      Assert.Equal(id, noteType.Id);
      Assert.NotNull(noteType.CSS);
      Assert.NotNull(noteType.DeckId);
      Assert.NotNull(noteType.Fields);
      Assert.NotNull(noteType.LastModificationTime);
      Assert.NotNull(noteType.LatexPost);
      Assert.NotNull(noteType.LatexPre);
      Assert.NotNull(noteType.Name);
      Assert.NotNull(noteType.Tags);
      Assert.NotNull(noteType.Templates);
      Assert.NotNull(noteType.Type);

    }
  }
}
