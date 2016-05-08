using HtmlAgilityPack;

public class WikiCodeTag : WikiTags
{
    public ScriptType CodeType;
    public string Code;
    public string RawCode;
    public WikiCodeTag(ScriptType st, string c, string rc)
    {
        CodeType = st;
        Code = HtmlEntity.DeEntitize(c);
        RawCode = HtmlEntity.DeEntitize(rc);
    }
}