using System;
using System.Collections.Generic;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class ComponentOrQualityPlayback : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in ComponentOrQualityPlayback.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4300", "ComponentOrQualityPlayBack", "");

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
                }
                else //Getting GVP SessionId from QueryString
                {
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
                }
                else //Getting IIS SessionId from Session.SessionID
                {
                    pageIISSessionID = Session.SessionID;
                }

                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("-- Expanded -- Entered into ComponentOrQualityPlayback state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                l.writeToLog("-- Expanded -- ComponentOrQualityPlayback event " + selection, Logging.eventType.AppInfoEvent);
                if (selection.Equals("inputOptions"))
                {
                    l.writeToLog("Info : Caller selected 1 to hear up to next 5 results", Logging.eventType.AppInfoEvent);
                    BuildAndPlayPrompt(false);
                }
                else if (selection.Equals("returnToPrevious"))
                {
                    l.writeToLog("-- Expanded -- Didn't hit dtmf 1 so returning to previous menu", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "ComponentOrQualityColumns.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("-- Expanded -- Exiting from ComponentOrQualityPlayback", Logging.eventType.AppInfoEvent);
                    Session["PlaybackResults"] = null;
                    Response.Write(p.getVXML());
                    return;
                }
                else
                {
                    Session["PlaybackResults"] = null;
                    BuildAndPlayPrompt(true);
                }
                l.writeToLog("Application end in ComponentOrQualityPlayback.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityPlayback -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void BuildAndPlayPrompt(bool firstTime)
        {
            try
            {
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (firstTime)
                {
                    if (!BuildResultsQueue())
                    {
                        Play p = new Play();
                        l.writeToLog("-- Expanded -- No results found at all. Going to ComponentOrQualityColumns.aspx", Logging.eventType.AppInfoEvent);
                        p.NextURL = "ComponentOrQualityColumns.aspx";
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentorQualityPlayback_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("-- Expanded -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "ComponentorQualityPlayback_Msg", true, "No more rizuhlts are available. Returning to previous menu.");
                        pt.addTTSToPrompt("No more results are available. Returning to previous menu.");
                        p.setPrompt(pt);
                        p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                        p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                        p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                        Session["PlaybackResults"] = null;
                        l.writeToLog("-- Expanded -- Exiting from ComponentOrQualityPlayback", Logging.eventType.AppInfoEvent);
                        Response.Write(p.getVXML());
                        return;
                    }
                }
                Queue<WebServiceResult> testResults = (Queue<WebServiceResult>)Session["PlaybackResults"];
                string category = Session["ExpandedCategory"] != null ? (string)Session["ExpandedCategory"] : WebServiceResult.COMPONENT;
                string type = Session["ExpandedType"] != null ? (string)Session["ExpandedType"] : "";
                int resultsToPlay = 0;
                while (testResults.Count > 0 && resultsToPlay < 5)
                {
                    WebServiceResult aTestResult = testResults.Dequeue();
                    string ttsSample = aTestResult.GetPromptText(category, type).Replace("INHIBITOR", TTSDictionary("INHIBITOR"));

                    // pt = aTestResult.GetPromptText(category, type);

                    l.writeToLog("-- Expanded -- Adding TTS test result: category = " + category + ", type =  " + type, Logging.eventType.AppInfoEvent);
                    // l.writeToLog("-- Expanded -- Adding TTS test result: ttsSample = " + ttsSample, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt(ttsSample);

                    resultsToPlay++;
                }
                if (testResults.Count == 0)
                {
                    Play p = new Play();
                    l.writeToLog("-- Expanded -- No more test results. Going to ComponentOrQualityColumns.aspx", Logging.eventType.AppInfoEvent);
                    p.NextURL = "ComponentOrQualityColumns.aspx";
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentorQualityPlayback_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("-- Expanded -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "ComponentorQualityPlayback_Msg", true, "No more rizuhlts are available. Returning to previous menu");
                    pt.addTTSToPrompt("No more results are available. Returning to previous menu");
                    p.setPrompt(pt);
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    Session["PlaybackResults"] = null;
                    l.writeToLog("Info : Exiting from ComponentOrQualityPlayback", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                }
                else
                {
                    GVP.MCL.Enhanced.Menu aMenu = new GVP.MCL.Enhanced.Menu("ComponentOrQualityPlaybackMenu");
                    aMenu.addMenuChoice("1", "ComponentOrQualityPlayback.aspx?event=inputOptions");
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentOrQualityPlayback_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("-- Expanded -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "ComponentOrQualityPlayback_Init", true, "To hear up to the next 5 rizuhlts, press 1. To return to the previous menu, press any other kee.");
                    pt.addTTSToPrompt("To hear up to the next 5 results, press 1. To return to the previous menu, press any other key.");
                    aMenu.setPrompt(pt);
                    aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "ComponentOrQualityColumns.aspx?event=returnToPrevious"));
                    aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "ComponentOrQualityColumns.aspx?event=returnToPrevious"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Getting input to see if continue with more expanded results", Logging.eventType.AppInfoEvent);
                    Response.Write(aMenu.getVXML());
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityPlayback -- BuildAndPlayPrompt fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private CatchEvent ErrorHandling(int iCount, string strCatchType, string strNextURL)
        {
            CatchEvent e = new CatchEvent(iCount, strCatchType);
            e.Reprompt = false;
            e.NextURL = strNextURL;
            return e;
        }

        private bool BuildResultsQueue()
        {
            int numberOfResults = 0;

            try
            {
                l.writeToLog("-- Expanded -- ComponentOrQualityPlayback BuildResultsQueue", Logging.eventType.AppInfoEvent);
                string category = Session["ExpandedCategory"] != null ? (string)Session["ExpandedCategory"] : WebServiceResult.COMPONENT;
                string type = Session["ExpandedType"] != null ? (string)Session["ExpandedType"] : "";
                l.writeToLog("-- Expanded -- Building results queue for category " + category + " and type " + type, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.Unknown;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4300", "ComponentOrQualityPlayBack", category + " - " + type);

                List <WebServiceResult> testResults = (List<WebServiceResult>)Session["ExpandedWebServiceLookup"];

                l.writeToLog("-- Expanded -- total test results for timeframe =" + testResults.Count, Logging.eventType.AppInfoEvent);
                Queue<WebServiceResult> resultsQueue = new Queue<WebServiceResult>(testResults.Count + 1);
                foreach (WebServiceResult aResult in testResults)
                {
                    if (aResult.HasResults(category, type))
                    {
                        resultsQueue.Enqueue(aResult);
                    }
                }
                Session["PlaybackResults"] = resultsQueue;
                numberOfResults = resultsQueue.Count;
                l.writeToLog("-- Expanded --  Found " + numberOfResults + " test results that match category/type for timeframe.", Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityPlayback -- BuildResultsQueue fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return numberOfResults > 0;
        }
    }
}