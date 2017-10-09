using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Configuration;

namespace DFARMR_IVR1
{
    public class Global : System.Web.HttpApplication
    {
        public static volatile string PromptLocation = null;
        public static string SpeechFileType = null;
        public static string ScriptLocation = null;
        public static string Timeout = null;
        public static string WebServicesTimeout = null;
        public string GVPSessionID = null;
        public string IISSessionID = null;
        public static string ipAddress = null;
        Log l = new Log();


        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            try
            {
                NameValueCollection Settings = ConfigurationManager.AppSettings;
                PromptLocation = Settings["PromptsLocation"].Trim();
                SpeechFileType = Settings["SpeechFileType"].Trim();
                Timeout = Settings["Timeout"].Trim();
                if (String.IsNullOrEmpty(Timeout))
                {
                    Timeout = "5";
                }
                WebServicesTimeout = Settings["WebServicesTimeout"].Trim();
                if (String.IsNullOrEmpty(WebServicesTimeout))
                {
                    WebServicesTimeout = "5";
                }

                // log4net.Config.XmlConfigurator.Configure();

            }
            catch (Exception ex)
            {
                l.writeToLog("Error in Application_Start event of Global.asax file : " + ex.ToString(), GVP.MCL.Enhanced.Logging.eventType.AppException);
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

            try
            {
                Session["HostIpAddress"] = ipAddress;
                Session["PromptLocation"] = PromptLocation;
                Session["SpeechFileType"] = SpeechFileType;
                Session["Timeout"] = Timeout;
                Session["WebServicesTimeout"] = WebServicesTimeout;
                string HostNameurl = Request.Url.ToString();
                IISSessionID = Session.SessionID;
                Session["IISSessionID"] = IISSessionID;

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

                /*
                if (Request.QueryString["SESSIONID"] != null)
                {
                    GVPSessionID = Request.QueryString["SESSIONID"].ToString().Trim();
                    GVPSessionID = GVPSessionID.Replace("{", " ");
                    GVPSessionID = GVPSessionID.Replace("}", " ");
                    Session["GVPSessionId"] = GVPSessionID;

                }
                */

                l.writeToLog("Info : ***** NEW CALL REQUEST TO DFARMR_IVR1 APPLICATION *****", GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);

                if (GVPSessionID != null)
                {
                    l.writeToLog("Info : SESSIONID from Voice Browser : " + GVPSessionID, GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                }
                else
                {
                    l.writeToLog("Info : SESSIONID null from Voice Browser", GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                }
                l.writeToLog("Info : Request Host Name As :" + Request.Url.Host.ToString(), GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);

                ipAddress = Request.Url.Host.ToString();

                l.writeToLog("Info : Reading Web.config values ", GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : Prompt Location           : " + PromptLocation, GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : Speech File Type          : " + SpeechFileType, GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : Timeout	                  : " + Timeout + " seconds", GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : WebServicesTimeout        : " + WebServicesTimeout + " seconds", GVP.MCL.Enhanced.Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("Error in Session_Start event of Global.asax file, Message : " + ex.Message, GVP.MCL.Enhanced.Logging.eventType.AppException);
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}
