using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class GlobalError : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
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

                Log.GetValuesFromSession(pageGVPSessionID);

                string strEvent = Request.QueryString["_event"] != null ? Request.QueryString["_event"].ToString().Trim() : "";
                if (strEvent.Contains("connection.disconnect"))
                {
                    GVP.MCL.Enhanced.Disconnect d = new GVP.MCL.Enhanced.Disconnect();
                    d.IncludeErrorCatchBlock = false;
                    Response.Write(d.getVXML());
                }
                else
                {
                    l.writeToLog("Application start in GlobalError.aspx", Logging.eventType.AppBegin);

                    string queryDisconnect = Request.QueryString["event"];
                   
                    string catchEventLogs = Request.QueryString["catchEventLogs"];
                    string callDataEvent = Request.QueryString["callDataEvent"];
                    string callDataEventMaxTries = Request.QueryString["callDataEventMaxTries"];

                    Log.PrintErrorCounterLogs(catchEventLogs, callDataEvent, callDataEventMaxTries);

                    Disconnect d = new Disconnect();

                    if (queryDisconnect == null)
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SystemError_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        Play p = new Play();
                        Prompt pt = new Prompt();
                        pt.BargeIn = false;
                        // AddPromptFileOrTTS(pt, "SystemError_Msg", true, "I'm sorry, but we are unable to process yur request at this time. Please call again later. Thank you for calling yur Member Information System. Good byee.");
                        pt.addTTSToPrompt("I'm sorry, but we are unable to process " + TTSDictionary("your") + " request at this time. Please call again later. Thank you for calling " + TTSDictionary("your") + " Member Information System." + TTSDictionary("Goodbye") + ".");
                        d.setPrompt(pt);
                    }
                    l.writeToLog("Info : Disconnecting the session Succesfully", Logging.eventType.AppInfoEvent);

                    pageGVPSessionID = null;
                    Response.Write(d.getVXML());

                    l.writeToLog("Application end in GlobalError.aspx", Logging.eventType.AppEnd);
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GlobalError -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
    }
}