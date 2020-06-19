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
  public class NoteTests
  {
    public string file = @"C:\Users\james\source\repos\AnkiImporter\src\SuperMemoAssistant.Plugins.AnkiImporter.Tests\Fixture\TestCollection\User 1\collection.anki2";

    [Fact]
    public async void GetNotesReturnsNotes()
    {

      var db = new DataAccess(file);
      var notes = await db.GetNotesAsync();
      Assert.True(notes != null && notes.Count > 0);

    }

    [Fact]
    public async void GetNoteIncludesNoteType()
    {

      var db = new DataAccess(file);
      var notes = await db.GetNotesAsync();
      foreach (var note in notes)
      {
        Assert.NotNull(note.NoteType);
      }

    }

    [Theory]
    [InlineData(1518852959151)]
    public async void GetNotesReturnsCorrectData(long id)
    {

      var db = new DataAccess(file);
      Expression<Func<Note, bool>> filter = (c) => c.Id == id;
      var notes = await db.GetNotesAsync(filter);
      var note = notes?.FirstOrDefault();

      Assert.NotNull(note);
      Assert.Equal(1518852959151, note.Id);
      Assert.Equal("IR/:C=)_g(", note.Guid);
      Assert.Equal(1518831358666, note.NoteTypeId);
      Assert.Equal(1519280080, note.LastModificationTime);
      Assert.Equal(-1, note.usn);
      Assert.Equal(" note ", note.Tags);
      Assert.Equal("001 - B5 Treble ClefB5<img src=\"Note-Treble-B5.png\" /><img src=\"KB_B5.png\">[sound:Piano.mf.B5.mp3]", note.FieldString);
      Assert.Equal(1606889653, note.Checksum);
      Assert.Equal(0, note.Flags);

    }

    // TODO: Improve this test
    [Fact]
    public async void NoteFieldsNotNull()
    {

      var db = new DataAccess(file);
      var notes = await db.GetNotesAsync();

      foreach (var note in notes)
      {
        var fields = note.Fields;
        Assert.NotNull(fields);
      }
    }
  }
}
