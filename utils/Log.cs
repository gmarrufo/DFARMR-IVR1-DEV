using System;
using System.Web;
using GVP.MCL.Enhanced;
using System.Configuration;
using System.Data.SqlClient;

namespace DFARMR_IVR1
{
    public class Log : System.Web.UI.Page
    {
        private string sURLParams;
        private string sVoiceFileDir = String.Empty;

        // WriteToLog
        public void writeToLog(string msg, Logging.eventType eT)
        {
            Logging l = new Logging();
            System.Diagnostics.Tracing.EventLevel loggingLevel = System.Diagnostics.Tracing.EventLevel.LogAlways;
            string strLoggingLevel = "";
            try
            {
                string sessionIDinit = Request.QueryString["session.connection.uuid"] != null ? Request.QueryString["session.connection.uuid"].ToString().Trim() : null;
                if (sessionIDinit != null)
                {
                    Session["sessionID"] = sessionIDinit;
                }
                else if (Session["sessionID"] == null && sessionIDinit == null)
                {
                    Session["sessionID"] = 0;
                }
                if (Cache["loggingLevel" + Session["sessionID"].ToString()] == null)
                {
                    SqlConnection SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["csEntLogging"].ToString());
                    SqlConnection.Open();
                    SqlCommand SqlCommand = new SqlCommand("SELECT loggingLevel from [Configuration] where tenantName='" + ConfigurationManager.AppSettings["tenantName"].ToString() + "'", SqlConnection);
                    SqlDataReader myDataReader = SqlCommand.ExecuteReader();
                    while (myDataReader.Read())
                    {
                        strLoggingLevel = myDataReader["loggingLevel"].ToString();
                    }
                    Cache.Insert("loggingLevel" + Session["sessionID"].ToString(), strLoggingLevel, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
                    myDataReader.Close();
                    SqlConnection.Close();
                }
                else
                {
                    strLoggingLevel = Cache["loggingLevel" + Session["sessionID"].ToString()].ToString();
                }
                loggingLevel = getEventLevel(strLoggingLevel);
                Session["loggingLevel"] = loggingLevel;
            }
            catch { }

            l.enableEventLogging(ConfigurationManager.AppSettings["tenantName"].ToString(), ConfigurationManager.ConnectionStrings["csEntLogging"].ToString(), loggingLevel);
            string sessionID = Session["sessionID"].ToString();

            l.logEvent(sessionID, msg, eT);

            l.disposeEventLogging();
        }

        protected System.Diagnostics.Tracing.EventLevel getEventLevel(string strLoggingLevel)
        {
            System.Diagnostics.Tracing.EventLevel loggingLevel = System.Diagnostics.Tracing.EventLevel.LogAlways;
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

        protected string VoiceFileDir
        {
            get
            {
                if (Request["voiceFileDir"] == null)
                {
                    sVoiceFileDir = getEnVoxDir;
                }
                else
                {
                    if (Request["voiceFileDir"].Trim().Equals("sp") || Request["voiceFileDir"].Trim().IndexOf("sp") > 0)
                        sVoiceFileDir = getSpVoxDir;
                }
                return sVoiceFileDir;
            }
        }

        protected string URLParams
        {
            get
            {
                string SESSIONID = Request.Params["session.connection.uuid"] != null ? Request.Params["session.connection.uuid"].Trim() : Request.Params["SESSIONID"].Trim();
                string ANI = Request.Params["session.connection.remote.uri"] != null ? Request.Params["session.connection.remote.uri"].Split(':', '@')[1] : Request.Params["ANI"].Trim();
                string DID = Request.Params["session.connection.local.uri"] != null ? Request.Params["session.connection.local.uri"].Split(':', '@')[1] : Request.Params["DID"].Trim();
                sURLParams = "?SESSIONID=" + SESSIONID + "&ANI=" + ANI + "&DID=" + DID;
                return sURLParams;
            }
        }

        public string getEnVoxDir
        {
            get
            {
                int lastSlash = 0;
                lastSlash = Request.ServerVariables["URL"].LastIndexOf("/");
                string ivrurl = "http://" + Request.ServerVariables["HTTP_HOST"] + Request.ServerVariables["URL"].Substring(0, lastSlash) + "/";

                return ivrurl + ConfigurationManager.AppSettings.Get("EnVoxDir").ToString();
            }
        }

        public string getSpVoxDir
        {
            get
            {
                int lastSlash = 0;
                lastSlash = Request.ServerVariables["URL"].LastIndexOf("/");
                string ivrurl = "http://" + Request.ServerVariables["HTTP_HOST"] + Request.ServerVariables["URL"].Substring(0, lastSlash) + "/";

                return ivrurl + ConfigurationManager.AppSettings.Get("SpVoxDir").ToString();
            }
        }

        protected CatchEvent getCatchEvent(string[] arPrompts, int iCount, string strCatchType, string strNextURL)
        {
            Prompt inPrompt = new Prompt();
            inPrompt.BargeIn = true;
            inPrompt.TimeOut = 5;
            for (int i = 0; i < arPrompts.Length; i++)
            {
                inPrompt.addVoiceFileToPrompt(VoiceFileDir + arPrompts[i], true, arPrompts[i]);
            }
            CatchEvent event2 = new CatchEvent(iCount, strCatchType);
            event2.setPrompt(inPrompt);
            event2.Disconnect = false;
            event2.Reprompt = false;
            event2.NextURL = strNextURL;
            return event2;
        }

        //To print nomatch,noinput logs and to submit calldata event report
        public static void PrintErrorCounterLogs(string catchEventLogs, string callDataEvent, string callDataEventMaxTries)
        {
            //CallDataReport report = new CallDataReport();
            DateTime dateTime = DateTime.Now;
            Play play = new Play();
            string pageGVPSessionID = "";
            Log l = new Log();
            try
            {
                if (HttpContext.Current.Session["SessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = HttpContext.Current.Session["SessionId"].ToString();
                    l.writeToLog("Info : GVP SessionId : " + pageGVPSessionID, Logging.eventType.AppInfoEvent);
                }
                else
                {
                    l.writeToLog("Info : Found  GVPSessionId as null from session object", Logging.eventType.AppInfoEvent);
                }
                if (catchEventLogs != null && !catchEventLogs.Equals(""))
                {
                    string[] array = catchEventLogs.Split('|');
                    for (int count = 0; count < array.Length; count++)
                    {
                        // Log.Write(pageGVPSessionID, array[count]);
                    }
                }
                else
                {
                    l.writeToLog("Info : catchEventLogs null", Logging.eventType.AppInfoEvent);
                }

                if (callDataEvent != null && !callDataEvent.Equals(""))
                {
                    string[] array = callDataEvent.Split('|');
                    ////report.SubmitRecord(pageGVPSessionID, dateTime, array[0], array[1], array[2], array[3], array[4], array[5]);
                }
                else
                {
                    l.writeToLog("Info : callDataEvent null", Logging.eventType.AppInfoEvent);
                }
                if ((callDataEventMaxTries != null) && (callDataEventMaxTries != ""))
                {
                    string[] array = callDataEventMaxTries.Split('|');
                    if (array.Length.Equals(6))
                    {
                        ////report.SubmitRecord(pageGVPSessionID, dateTime, array[0], array[1], array[2], array[3], array[4], array[5]);
                    }
                }
                else
                {
                    l.writeToLog("Info : callDataEventMaxTries null", Logging.eventType.AppInfoEvent);
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("Info : Error in PrintErrorCounterLogs method, Message : " + ex.Message, Logging.eventType.AppInfoEvent);
            }
        }

        //To read values from session objects defined in Global.asax.cs file
        public static void GetValuesFromSession(string pageGVPSessionID)
        {
        }

        // Replace the original Debug - No longer logs into files
        public static void Debug(string strMessage)
        {
        }

        public void reportERM(Logging.nodeDataType nodeType, string nodeID, string nodeName, string nodeData)
        {
            Log l = new Log();
            string sessionID = Session["sessionID"].ToString();
            Logging lg = new Logging();

            l.writeToLog(sessionID + " begin " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeBegin);
            string eventResp = lg.logERM(Session["sessionID"].ToString(), nodeID + " - " + nodeName, nodeData, nodeType, false);

            if (eventResp == "1")
            {
                l.writeToLog(sessionID + " eventResp: " + eventResp, Logging.eventType.ReportingSuccess);
            }
            else
            {
                l.writeToLog(sessionID + " eventResp: " + eventResp, Logging.eventType.ReportingFailure);
            }

            l.writeToLog(sessionID + " end " + System.Reflection.MethodBase.GetCurrentMethod().Name + " node", Logging.eventType.NodeEnd);
        }
    }
}