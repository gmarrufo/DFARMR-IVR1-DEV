using System;
using System.Configuration;
using System.IO;
using System.Web;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class MarketReport : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in MarketReport.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2300", "MarketReport", "");

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
                }
                else //Getting GVP SessionId from QueryString
                {
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
                l.writeToLog("Info : Entered into MarketReport state", Logging.eventType.AppInfoEvent);
                PlayMarketReport();
                l.writeToLog("Application end in MarketReport.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MarketReport -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void PlayMarketReport()
        {
            try
            {
                Play p = new Play();
                Prompt pt = new Prompt();
                string marketReportFilename = ConfigurationManager.AppSettings["MarketReportFilename"].Trim();

                Session["nodeType"] = Logging.nodeDataType.Unknown;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2300", "MarketReport", marketReportFilename);

                if (String.IsNullOrEmpty(marketReportFilename))
                {
                    l.writeToLog("Warning : MarketReportFilename is not set in web.config file, assuming marketReport.wav", Logging.eventType.AppInfoEvent);
                    marketReportFilename = "marketReport.wav";
                }

                string promptFile = System.Configuration.ConfigurationManager.AppSettings["MarketReportLocation"] + marketReportFilename;
                string promptFileName = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MarketReportLocation"]).Trim() + marketReportFilename;
                
                if (MarketReportExists(promptFileName))
                {
                    string baseURL = HttpContext.Current.Request.Url.Host;
                    promptFile = "http://" + baseURL + promptFile;
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.addVoiceFileToPrompt(promptFile, true, "No market report is available at this time.");
                }
                else
                {
                    l.writeToLog("Warning : Market Report prompt file either doesn't exist or is 0 size. filepath = " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt("No market report is available at this time.");
                }
                p.NextURL = "MainMenu.aspx";
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from MarketReport", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MarketReport -- PlayMarketReport fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private bool MarketReportExists(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("Warning : Error trying to see if market report file exists. Assuming not. Path: " + filePath + " Error:" + ex.ToString(), Logging.eventType.AppException);
            }
            return false;
        }
    }
}