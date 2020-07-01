using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Rendering
{
  public class TemplateKeyHandler
  {

    // The predicate to match the key against
    public Func<string, bool> Pattern { get; set; }

    // TODO: mutate the actual content map so it updates the content as
    // you pass the request along the handlers
    public Func<string, Dictionary<string, string>, string> Action { get; set; }

    // Next handler in the chain or null
    public TemplateKeyHandler Next { get; set; }

    // Should the chain continue after this handler?
    public bool TerminatesChain { get; set; }

    // Recurse through the chain
    public string HandleRequest(string TemplateKey, Dictionary<string, string> FieldContentMap, string currentResult)
    {

      if (Pattern(TemplateKey))
      {

        string output = Action(TemplateKey, FieldContentMap);
        currentResult = output;

        if (TerminatesChain)
        {
          return currentResult;
        }

      }

      if (Next == null)
        return currentResult;

      return Next.HandleRequest(TemplateKey, FieldContentMap, currentResult);
    }
  }
}
