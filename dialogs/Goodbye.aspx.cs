using System;
using GVP.MCL.Enhanced;
using DFARMR_IVR1;

namespace DFARMR_IVR1
{
    public partial class Goodbye : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        DFARMR_IVR1.Log l = new DFARMR_IVR1.Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in Goodbye.aspx", Logging.eventType.AppBegin);

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
                    l.writeToLog("Info : GVP SessionId : " + pageGVPSessionID, Logging.eventType.AppInfoEvent);
                }
                else //Getting GVP SessionId from QueryString
                {
                    l.writeToLog("Info : Found  GVPSessionId as null from session object", Logging.eventType.AppInfoEvent);
                    if (Request.QueryString["SESSIONID"] != null)
                    {
                        l.writeToLog("Info : Getting GVP SessionID from QueryString", Logging.eventType.AppInfoEvent);
                        pageGVPSessionID = Request.QueryString["SESSIONID"].ToString().Trim();
                        pageGVPSessionID = pageGVPSessionID.Replace("{", " ");
                        pageGVPSessionID = pageGVPSessionID.Replace("}", " ");
                    }
                    else
                    {
                        l.writeToLog("Info : GVP SessionID from QueryString is null", Logging.eventType.AppInfoEvent);
                    }
                }
                if (Session["IISSessionID"] != null)//Getting IIS SessionId from session object
                {
                    pageIISSessionID = Session["IISSessionID"].ToString();
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }
                else //Getting IIS SessionId from Session.SessionID
                {
                    l.writeToLog("Info : Found  IISSessionID as null from session object", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : Getting IISSessionID from Session.SessionID", Logging.eventType.AppInfoEvent);
                    pageIISSessionID = Session.SessionID;
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }

                DFARMR_IVR1.Log.GetValuesFromSession(pageGVPSessionID);
                string queryDisconnect = Request.QueryString["event"];
                Disconnect d = new Disconnect();
                string catchEventLogs = Request.QueryString["catchEventLogs"];
                string callDataEvent = Request.QueryString["callDataEvent"];
                string callDataEventMaxTries = Request.QueryString["callDataEventMaxTries"];
                DFARMR_IVR1.Log.PrintErrorCounterLogs(catchEventLogs, callDataEvent, callDataEventMaxTries);

                if (queryDisconnect == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Goodbye_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    Prompt pt = new Prompt();
                    pt.BargeIn = false;
                    // AddPromptFileOrTTS(pt, "Goodbye_Msg", true, "Thank you for calling yore Member information system. Good byee.");
                    pt.addTTSToPrompt("Thank you for calling " + TTSDictionary("your") + " Member information system. " + TTSDictionary("Goodbye") + ".");
                    d.setPrompt(pt);
                }
                l.writeToLog("Info : Disconnecting the session Succesfully", Logging.eventType.AppInfoEvent);
                pageGVPSessionID = null;
                Response.Write(d.getVXML());

                l.writeToLog("Application end in Goodbye.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Goodbye -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
    }
}