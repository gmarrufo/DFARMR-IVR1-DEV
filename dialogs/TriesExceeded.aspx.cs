using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class TriesExceeded : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in TriesExceeded.aspx", Logging.eventType.AppBegin);

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
                l.writeToLog("Info : Entered into TriesExceeded state", Logging.eventType.AppInfoEvent);
                PlayTriesExceeded();

                l.writeToLog("Application end in TriesExceeded.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- TriesExceeded -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void PlayTriesExceeded()
        {
            try
            {
                Play p = new Play();
                Prompt pt = new Prompt();
                string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "TriesExceeded_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                // AddPromptFileOrTTS(pt, "TriesExceeded_Msg", true, "The maximum number of retries has been exceeded.");
                pt.addTTSToPrompt("The maximum number of retries has been exceeded.");
                p.NextURL = "Goodbye.aspx";
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from TriesExceeded", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- TriesExceeded -- PlayTriesExceeded fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
    }
}