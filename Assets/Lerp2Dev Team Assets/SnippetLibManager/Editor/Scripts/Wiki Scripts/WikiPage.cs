using System;
using System.Collections.Generic;

public class WikiPage
{
    public static string[] AllowedSections = new string[6] { "Extensions", "Particle Library", "Programming", "Scripts", "Shaders", "Wizards" };
    public List<WikiTags> Contents = new List<WikiTags>();
    public static WikiPage Init(Type c, WikiParams p)
    {
        if (c == typeof(WikiDocument))
            return WikiDocument.GetContents(p);
        else if (c == typeof(WikiExtensions))
            return WikiExtensions.GetContents(p);
        else if(c == typeof(WikiParticleLibrary))
            return WikiParticleLibrary.GetContents(p);
        else if(c == typeof(WikiProgramming))
            return WikiProgramming.GetContents(p);
        else if (c == typeof(WikiScriptContainer))
            return WikiScriptContainer.GetContents(p);
        else if (c == typeof(WikiShaders))
            return WikiShaders.GetContents(p);
        else if (c == typeof(WikiWizards))
            return WikiWizards.GetContents(p);
        else if (c == typeof(WikiNavigation))
            return WikiNavigation.GetContents(p);
        else
            return null;
    }
}

public class WikiParams
{
    public Type Type;
    public string Url;
    public NavigationType NavType;
    public WikiParams(NavigationType nt, string u)
    {
        NavType = nt;
        Url = u;
    }
    public WikiParams(string u)
    {
        Url = u;
    }
}//60