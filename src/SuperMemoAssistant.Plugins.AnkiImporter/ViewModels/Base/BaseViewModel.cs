using Newtonsoft.Json;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.ViewModels
{
  public abstract class BaseViewModel : INotifyPropertyChangedEx
  {

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

  }
}
