using SuperMemoAssistant.Plugins.AnkiImporter.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SuperMemoAssistant.Plugins.AnkiImporter.Tests.RenderingTests
{
  public class RendererTests
  {

    [Fact]
    public void CreateRendererReturnsRenderer()
    {

      var renderer = new Renderer(0).Create(TemplateType.Question);
      Assert.NotNull(renderer);

    }
  }
}
