using HtmlSerializer;
using System.Text.RegularExpressions;


var html = await Load("http://ufi.ztedevice.com/index.html#home");
html = new Regex("[\\r\\n\\t]").Replace(html, "");
html = new Regex("\\s{2,}").Replace(html, " ");

var htmlLines = new Regex("<(.*?)>").Split(html).Where(s => s.Length > 0 && !s.Equals(" ")).ToArray();

HtmlElement root = new HtmlElement();
root.Name = "html";

CreateTree(htmlLines.Skip(2).ToList(), root);

//Selector selector = Selector.ToSelector("input.width190#promptInput");
//Selector selector = Selector.ToSelector("div.row");
Selector selector = Selector.ToSelector("div.row img");

var result = root.FindElements(selector);
result.ToList().ForEach(x => Console.WriteLine(x));

static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

static void CreateTree(List<string> lines, HtmlElement current)
{
    foreach (var line in lines)
    {
        if (line.Split(' ')[0].Equals("/html"))
            break;
        if (line[0] == '/' && line[1] != '/' && line[1] != '*')
        {
            current = current.Parent;
            continue;
        }
        string name = line.Split(' ')[0];
        if (!HtmlHelper.Helper.HtmlTags.Contains(name) && !HtmlHelper.Helper.HtmlVoidTags.Contains(name) && !line.EndsWith('/'))
        {
            current.InnerHtml += line;
            continue;
        }
        HtmlElement element = new HtmlElement();
        element.Name = name;
        var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
        foreach (var a in attributes)
        {
            var attribute = a.ToString().Split('=');
           attribute[1] = attribute[1].Replace("\"", "");
            if (attribute[0].ToLower().Equals("class"))
                element.Classes.AddRange(attribute[1].Split(' '));
            else if (attribute[0].ToLower().Equals("id"))
                element.Id = attribute[1];
            else
                element.Attributes.Add(attribute[0], attribute[1]);
        }
        current.Children.Add(element);
        element.Parent = current;
        if (!HtmlHelper.Helper.HtmlVoidTags.Contains(name) && !line.EndsWith('/'))
            current = element;
    }
}