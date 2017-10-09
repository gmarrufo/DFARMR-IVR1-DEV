using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class Testing : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            l.writeToLog("Application start in Testing.aspx", Logging.eventType.AppBegin);

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

            }
            catch (Exception ex)
            {
                l.writeToLog("Error : Got error while reading GVP and IIS SessionID values from session object, Message : " + ex.StackTrace, Logging.eventType.AppException);
            }
            Session["language"] = "english";
            Log.GetValuesFromSession(pageGVPSessionID);
            try
            {
                l.writeToLog("Info : Entered into Testing state", Logging.eventType.AppInfoEvent);
                PlayTesting();
            }
            catch (Exception ex)
            {
                l.writeToLog("Error : Error in Testing page load, Message : " + ex.ToString(), Logging.eventType.AppException);
            }

            l.writeToLog("Application end in Testing.aspx", Logging.eventType.AppEnd);
        }

        private void PlayTesting()
        {
            Prompt pt = new Prompt();
            Session["division"] = "02";
            Session["producer"] = "1234";
            Play p = new Play();
            string insert = @"&lt;break time=&quot;" + System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"].Trim() + @"ms&quot;/&gt;";
            pt.addTTSToPrompt("We are in testing and here comes a pause" + insert + " did you hear it?");
            p.NextURL = "MainMenu.aspx";
            p.setPrompt(pt);
            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
            l.writeToLog("Info : Exiting from Testing", Logging.eventType.AppInfoEvent);
            Response.Write(p.getVXML());
        }
    }
}