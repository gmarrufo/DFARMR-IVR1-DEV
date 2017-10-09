using System;
using System.Configuration;
using GVP.MCL.Enhanced;

public partial class hold : baseClass
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Logging l = new Logging();
        string strLoggingLevel = "Error";
        System.Diagnostics.Tracing.EventLevel loggingLevel = System.Diagnostics.Tracing.EventLevel.Error;
        if (Cache["loggingLevel" + Session["sessionID"].ToString()] != null)
        {
            strLoggingLevel = Cache["loggingLevel" + Session["sessionID"].ToString()].ToString();
        }
        loggingLevel = getEventLevel(strLoggingLevel);
        l.enableEventLogging(ConfigurationManager.AppSettings["tenantName"].ToString(), ConfigurationManager.ConnectionStrings["csEntLogging"].ToString(), loggingLevel);
        string sessionID = Session["sessionID"].ToString();
        l.logEvent(sessionID, "Page begin in hold.aspx", Logging.eventType.PageBegin);
        l.logEvent(sessionID, "begin " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeBegin);
        Play pl = new Play();
        Prompt p = new Prompt();
        p.addVoiceFileToPrompt(ConfigurationManager.AppSettings["voiceFilesURL"] + "ring.vox", true);
        p.TimeOut = 20;
        pl.setPrompt(p);
        Response.Write(pl.getVXML());
        l.logEvent(sessionID, "end " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeEnd);
        l.logEvent(sessionID, "Page end in hold.aspx", Logging.eventType.PageEnd);
        l.disposeEventLogging();
    }
}