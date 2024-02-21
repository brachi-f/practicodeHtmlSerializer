using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace HtmlSerializer
{
    public class HtmlHelper
    {
        private static readonly HtmlHelper _helper = new HtmlHelper();
        public static HtmlHelper Helper =>_helper;
        private HtmlHelper()
        {
            var tags = File.ReadAllText("HtmlTags.json");
            var voidTags = File.ReadAllText("HtmlVoidTags.json");
            HtmlTags = JsonSerializer.Deserialize<List<string>>(tags);
            HtmlVoidTags = JsonSerializer.Deserialize<List<string>>(voidTags);
        }
        public List<string> HtmlTags { get;}
        public List<string> HtmlVoidTags { get;}
    }
}
