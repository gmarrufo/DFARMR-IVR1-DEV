using System;
using System.Configuration;
using GVP.MCL.Enhanced;

/// <summary>
/// Summary description for baseClass
/// </summary>
public class baseClass : System.Web.UI.Page
{
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    public baseClass()
        : base()
    {
    }
    protected string httpWebPage
    {
        get
        {
            int lastSlash = 0;
            int lsPlus1 = 0;
            int pageNameLen = 0;
            int fileExtension = 0;
            string strHttpWebPage = "";
            lastSlash = Request.ServerVariables["URL"].LastIndexOf("/");
            lsPlus1 = lastSlash + 1;
            fileExtension = Request.ServerVariables["URL"].IndexOf(".aspx");
            pageNameLen = (fileExtension - lastSlash) - 1;
            strHttpWebPage = Request.ServerVariables["URL"].Substring(lsPlus1, pageNameLen);

            return strHttpWebPage;
        }
    }
    protected CatchEvent getCatchEvent(string[] arPrompts, int iCount, string strCatchType, string strNextURL)
    {
        Prompt p = new Prompt();
        p.BargeIn = true;
        p.TimeOut = 3;
        for (int i = 0; i < arPrompts.Length; i++)
            if (!arPrompts[i].Equals(""))
            {
                p.addVoiceFileToPrompt(ConfigurationManager.AppSettings["voiceFilesURL" + Session["language"].ToString()] + arPrompts[i], true, arPrompts[i]);
            }
        CatchEvent e = new CatchEvent(iCount, strCatchType);
        e.setPrompt(p);
        e.Disconnect = false;
        e.Reprompt = true;
        e.NextURL = strNextURL;
        return e;
    }
    protected CatchEvent getCatchEventNoReprompt(string[] arPrompts, int iCount, string strCatchType, string strNextURL)
    {
        Prompt p = new Prompt();
        p.BargeIn = true;
        p.TimeOut = 3;
        for (int i = 0; i < arPrompts.Length; i++)
            if (!arPrompts[i].Equals(""))
            {
                p.addVoiceFileToPrompt(ConfigurationManager.AppSettings["voiceFilesURL" + Session["language"].ToString()] + arPrompts[i], true, arPrompts[i]);
            }
        CatchEvent e = new CatchEvent(iCount, strCatchType);
        e.setPrompt(p);
        e.Disconnect = false;
        e.Reprompt = false;
        e.NextURL = strNextURL;
        return e;
    }
    protected System.Diagnostics.Tracing.EventLevel getEventLevel(string strLoggingLevel)
    {
        System.Diagnostics.Tracing.EventLevel loggingLevel = System.Diagnostics.Tracing.EventLevel.Error;
        switch (strLoggingLevel)
        {
            case "LogAlways":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.LogAlways;
                break;
            case "Critical":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Critical;
                break;
            case "Error":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Error;
                break;
            case "Informational":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Informational;
                break;
            case "Verbose":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Verbose;
                break;
            case "Warning":
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Warning;
                break;
            default:
                loggingLevel = System.Diagnostics.Tracing.EventLevel.Error;
                break;
        }
        return loggingLevel;
    }
}