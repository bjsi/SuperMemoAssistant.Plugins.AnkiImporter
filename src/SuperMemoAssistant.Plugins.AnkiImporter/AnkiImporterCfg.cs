using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SuperMemoAssistant.Plugins.AnkiImporter
{

  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
     IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
            "Cancel",
            IsCancel = true)]
  [DialogAction("save",
            "Save",
            IsDefault = true,
            Validates = true)]
  public class AnkiImporterCfg : CfgBase<AnkiImporterCfg>, INotifyPropertyChangedEx
  {

    [Field(Name = "Testing?")]
    public bool Testing { get; set; } = true;

    [Field(Name = "Anki collection database")]
    public string AnkiCollectionDB { get; set; }

    [Field(Name = "Anki Collection Media Directory")]
    public string AnkiCollectionMediaDir { get; set; }

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "Anki Importer";
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
