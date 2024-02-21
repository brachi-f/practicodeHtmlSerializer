using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();
        public override string ToString()
        {
            string attributes = "";
            if (this.Id != null)
                attributes += $" id=\"{this.Id}\" ";
            if (this.Classes.Count > 0)
                attributes += $" class=\"{string.Join(' ', this.Classes)}\" ";
            if (this.Attributes.Count > 0)
                foreach (var item in this.Attributes)
                    attributes += " " + item.Key + "=" + "\"" + item.Value + "\" ";
            if (HtmlHelper.Helper.HtmlVoidTags.Contains(Name))
                attributes = $"<{this.Name} {attributes} />";
            else
                attributes = $"<{Name} {attributes}>{InnerHtml}</{Name}>";
            return attributes;
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();
                if (this != current)
                    yield return current;

                foreach (HtmlElement child in current.Children)
                    queue.Enqueue(child);
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement tmp = this;
            while (tmp != null)
            {
                yield return tmp;
                tmp = tmp.Parent;
            }
        }
        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            HashSet<HtmlElement> list = new HashSet<HtmlElement>();
            this.Descendants().ToList().ForEach(d => d.findElements(selector, list));
            return list;
        }
        private void findElements(Selector selector, HashSet<HtmlElement> elements)
        {
            if (!this.match(selector))
                return;
            if (selector.Child == null)
                elements.Add(this);
            else
                this.Descendants().ToList().ForEach(d => d.findElements(selector.Child, elements));

        }
        private bool match(Selector selector) =>
            (selector.Id is null || (this.Id is not null && this.Id.Equals(selector.Id)))
                && (selector.TagName is null || this.Name.Equals(selector.TagName))
                && (selector.Classes.Intersect(Classes).Count() == selector.Classes.Count());
    }
}
