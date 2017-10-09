using System;
using System.IO;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class Intro : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in Intro.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1000", "Greeting", "");

                /*
                string seesionID = Request.QueryString["SESSIONID"].ToString().Trim();
                Session["SESSIONID"] = seesionID;
                Session["GVPSessionId"] = seesionID;
                */

                string sessionIDinit = Request.QueryString["session.connection.uuid"] != null ? Request.QueryString["session.connection.uuid"].ToString().Trim() : null;
                if (sessionIDinit != null)
                {
                    Session["sessionID"] = sessionIDinit;
                    Session["GVPSessionId"] = sessionIDinit;
                }
                else if (Session["sessionID"] == null && sessionIDinit == null)
                {
                    Session["sessionID"] = 0;
                }

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    // pageGVPSessionID = Session["GVPSessionId"].ToString();
                    l.writeToLog("Info : GVP SessionId : " + pageGVPSessionID, Logging.eventType.AppInfoEvent);
                }
                else //Getting GVP SessionId from QueryString
                {
                    l.writeToLog("Info : Found  GVPSessionId as null from session object", Logging.eventType.AppInfoEvent);
                    if (Request.QueryString["SESSIONID"] != null)
                    {
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

                Session["language"] = "english";
                Log.GetValuesFromSession(pageGVPSessionID);

                string filePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"]) + "SiteDown" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                l.writeToLog("-- Intro -- Down File is " + filePath, Logging.eventType.AppInfoEvent);
                if (File.Exists(filePath))
                {
                    l.writeToLog("-- Intro -- Entered into Play Message and Hang Up", Logging.eventType.AppInfoEvent);
                    PlayMessageAndHangUp();
                }
                else
                {
                    l.writeToLog("-- Intro -- Entered into Intro state", Logging.eventType.AppInfoEvent);
                    PlayIntro();
                }
                l.writeToLog("Application end in Intro.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Intro -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void PlayIntro()
        {
            try
            {
                l.writeToLog("-- Intro -- PlayIntro", Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.Announcement;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1000", "Greeting", "Welcome to yore Member Information System.");
               
                Play p = new Play();
                Prompt pt = new Prompt();
                string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Intro_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                l.writeToLog("-- Intro -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                // AddPromptFileOrTTS(pt, "Intro_Msg", true, "Welcome to yore Member Information System.");
                pt.addTTSToPrompt("Welcome to " + TTSDictionary("your") + " Member Information System.");
                p.NextURL = "GetDivisionNumber.aspx";
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("-- Intro -- Exiting from Intro", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Intro -- PlayIntro fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
        private void PlayMessageAndHangUp()
        {
            try
            {
                Disconnect d = new Disconnect();
                string promptFile = System.Configuration.ConfigurationManager.AppSettings["sysPromptLocation"] + "SiteDown" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                l.writeToLog("-- Intro -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                Play p = new Play();
                Prompt pt = new Prompt();
                pt.BargeIn = false;
                // pt.addVoiceFileToPrompt(promptFile, true, "I'm sorry, but we are unable to process yur request at this time. Please call again later. Thank you for calling yur Member Information System. Good byee.");
                // AddPromptFileOrTTS(pt, "SiteDown", true, "I'm sorry, but we are unable to process yur request at this time. Please call again later. Thank you for calling yur Member Information System. Goodbye.");
                pt.addVoiceFileToPrompt(promptFile, true, "I'm sorry, but we are unable to process " + TTSDictionary("your") + " request at this time. Please call again later. Thank you for calling " + TTSDictionary("your") + " Member Information System. " + TTSDictionary("Goodbye") + ".");
                d.setPrompt(pt);
                l.writeToLog("-- Intro -- Disconnecting the session Succesfully from PlayMessageAndHangUp", Logging.eventType.AppInfoEvent);
                Response.Write(d.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Intro -- PlayMessageAndHangUp fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
    }
}