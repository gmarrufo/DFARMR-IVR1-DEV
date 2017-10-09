using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class SystemError : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in SystemError.aspx", Logging.eventType.AppBegin);

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
                string strMessage = Request.QueryString["_message"] != null ? Request.QueryString["_message"].ToString().Trim() : "";
                string strURL = Request.QueryString["_url"] != null ? Request.QueryString["_url"].ToString().Trim() : "";

                l.writeToLog("-- SystemError -- Page_Load Event,Message,URL --> values: " + strEvent + "," + strMessage + "," + strURL, Logging.eventType.AppException);

                if (strEvent.Contains("connection.disconnect"))
                {
                    GVP.MCL.Enhanced.Disconnect d = new GVP.MCL.Enhanced.Disconnect();
                    d.IncludeErrorCatchBlock = false;
                    Response.Write(d.getVXML());
                }
                else
                {
                    string queryDisconnect = Request.QueryString["event"];
                    Disconnect d = new Disconnect();
                    string catchEventLogs = Request.QueryString["catchEventLogs"];
                    string callDataEvent = Request.QueryString["callDataEvent"];
                    string callDataEventMaxTries = Request.QueryString["callDataEventMaxTries"];
                    Log.PrintErrorCounterLogs(catchEventLogs, callDataEvent, callDataEventMaxTries);

                    if (queryDisconnect == null)
                    {
                        Play p = new Play();
                        Prompt pt = new Prompt();
                        pt.BargeIn = false;
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SystemError_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "SystemError_Msg", true, "I’m sorry. We are unable to process yore request at this time. Please call again later.");
                        pt.addTTSToPrompt("I’m sorry. We are unable to process " + TTSDictionary("your") + " request at this time. Please call again later.");
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Goodbye_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "Goodbye_Msg", true, "Thank you for calling yore Member information system. Good byee.");
                        pt.addTTSToPrompt("Thank you for calling " + TTSDictionary("your") + " Member information system. " + TTSDictionary("Goodbye") + ".");
                        d.setPrompt(pt);
                    }
                    l.writeToLog("Info : Disconnecting the session Succesfully", Logging.eventType.AppInfoEvent);
                    pageGVPSessionID = null;
                    Response.Write(d.getVXML());
                }

                l.writeToLog("Application end in SystemError.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- SystemError -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }    
    }
}