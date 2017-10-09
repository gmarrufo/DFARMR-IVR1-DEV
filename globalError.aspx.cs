using System;
using System.Configuration;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class globalError : baseClass
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strEvent = Request.QueryString["_event"] != null ? Request.QueryString["_event"].ToString().Trim() : "";
            Log l = new Log();

            if (strEvent.Contains("connection.disconnect"))
            {
                GVP.MCL.Enhanced.Disconnect d = new GVP.MCL.Enhanced.Disconnect();
                d.IncludeErrorCatchBlock = false;
                Response.Write(d.getVXML());
            }
            else
            {
                // Logging l = new Logging();
                System.Diagnostics.Tracing.EventLevel loggingLevel = System.Diagnostics.Tracing.EventLevel.LogAlways;
                // l.enableEventLogging(ConfigurationManager.AppSettings["tenantName"].ToString(), ConfigurationManager.ConnectionStrings["csEntLogging"].ToString(), loggingLevel);
                string sessionID = Session["sessionID"].ToString();
                // l.logEvent(sessionID, "Page begin in globalError.aspx", Logging.eventType.PageBegin);
                // l.logEvent(sessionID, "begin " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeBegin);
                strEvent = Request.QueryString["_event"] != null ? Request.QueryString["_event"].ToString().Trim() : "";
                string strMessage = Request.QueryString["_message"] != null ? Request.QueryString["_message"].ToString().Trim() : "";
                string strUrl = Request.QueryString["_url"] != null ? Request.QueryString["_url"].ToString().Trim() : "";
                // l.logEvent(sessionID, "_event: " + strEvent, Logging.eventType.AppInfoEvent);
                // l.logEvent(sessionID, "_message: " + strMessage, Logging.eventType.AppInfoEvent);
                // l.logEvent(sessionID, "_url: " + strUrl, Logging.eventType.AppInfoEvent);

                l.writeToLog("-- globalError -- Page_Load Event,Message,URL --> values: " + strEvent + "," + strMessage + "," + strUrl, Logging.eventType.AppException);

                Disconnect d = new Disconnect();
                Prompt p = new Prompt();
                p.addVoiceFileToPrompt(ConfigurationManager.AppSettings["voiceFilesURL"] + "9999.vox", true, "Im sorry, we are currently experiencing technical difficulties. Please call back later. Thank you for calling Good Byee.");
                p.BargeIn = false;
                p.TimeOut = 0;
                d.setPrompt(p);
                d.IncludeErrorCatchBlock = false;
                Response.Write(d.getVXML());
                // l.logEvent(sessionID, "end " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeEnd);
                // l.logEvent(sessionID, "Page end in globalError.aspx", Logging.eventType.PageEnd);
                // l.logEvent(sessionID, "Application end in globalError.aspx", Logging.eventType.AppEnd);
                // l.disposeEventLogging();
            }
        }
    }
}