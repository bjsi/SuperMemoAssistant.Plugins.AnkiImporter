using Forge.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperMemoAssistant.Plugins.AnkiImporter
{
  public static class Popups
  {
    /// <summary>
    /// Show an alert window.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    public static void ShowAlert(string message, string title)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        Show.Window().For(new Alert(message, title));
      });
    }
  }
}
