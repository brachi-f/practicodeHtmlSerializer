using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Child { get; set; }
        public Selector Parent { get; set; }

        public static Selector ToSelector(string query)
        {
            Selector rootSelector = new Selector();
            Selector tmp = rootSelector;
            var queryLevels = query.Split(' ');
            foreach (var level in queryLevels)
            {
                var parts = new Regex("(?=[#\\.])").Split(level).Where(p => p.Length > 0).ToList();
                parts.ForEach(p =>
                {
                    if (p.StartsWith('#'))
                        tmp.Id = p.Substring(1);
                    else if (p.StartsWith('.'))
                        tmp.Classes.Add(p.Substring(1));
                    else if (HtmlHelper.Helper.HtmlTags.Contains(p) || HtmlHelper.Helper.HtmlVoidTags.Contains(p))
                        tmp.TagName = p;
                });
                if (!queryLevels.Last().Equals(level))
                {
                    Selector selector = new Selector();
                    selector.Parent = tmp;
                    tmp.Child = selector;
                    tmp = selector;
                }
            }
            return rootSelector;
        }
    }
}
