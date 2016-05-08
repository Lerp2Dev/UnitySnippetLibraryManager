using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

public class WikiListTag : WikiTags
{
    public List<WikiMixedContent> ListContents = new List<WikiMixedContent>();
    public WikiListTag(HtmlNodeCollection lis, bool en)
    {
        GetList(lis, en);
    }

    public void GetList(HtmlNodeCollection lis, bool en)
    {
        int i = 1;
        foreach (HtmlNode li in lis)
        {
            string dinner = (en ? i + "." : "") + HtmlEntity.DeEntitize(li.InnerHtml);
            var wmc = new WikiMixedContent(WikiContent.ContentMultiFormatter(dinner));
            var d = WikiContent.GetLinks(dinner);
            if(d != null && d.Count > 0)
                wmc.Links = wmc.Links.Union(d).ToDictionary(k => k.Key, v => v.Value);
            ListContents.Add(wmc);
            ++i;
        }
    }
}
