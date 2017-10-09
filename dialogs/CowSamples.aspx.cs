using System;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewGetTestResults.WebReference;

namespace DFARMR_IVR1
{
    public partial class CowSamples : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in CowSamples.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "5000", "CowSamples", "");

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
                    l.writeToLog("Info : Found  IISSessionID as null from session object", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : Getting IISSessionID from Session.SessionID", Logging.eventType.AppInfoEvent);
                    pageIISSessionID = Session.SessionID;
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }

                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("Info : ----------   Entered into   CowSamples      state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                l.writeToLog("Info : CowSamples event " + selection, Logging.eventType.AppInfoEvent);
                if (selection.Equals("inputOptions"))
                {
                    l.writeToLog("Info : Caller selected 1 to hear up to next 5 results", Logging.eventType.AppInfoEvent);
                    InputOptions(false);
                }
                else if (selection.Equals("returnToMain"))
                {
                    l.writeToLog("Info : Didn't hit dtmf 1 so returning to main menu", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "MainMenu.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from CowSamples", Logging.eventType.AppInfoEvent);
                    Session["cowSamplesResults"] = null;
                    Response.Write(p.getVXML());
                    return;
                }
                else
                {
                    Session["cowSamplesResults"] = null;
                    InputOptions(true);
                }
                l.writeToLog("Application end in CowSamples.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CowSamples -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(bool callWebService)
        {
            try
            {
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);

                Session["nodeType"] = Logging.nodeDataType.Unknown;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "5000", "CowSamples", "Calling Web Service");

                if (callWebService)
                {
                    string status;
                    string divisionNumber = Session["division"].ToString();
                    if (divisionNumber.Length == 3)
                    {
                        status = CallNewWebService();
                    }
                    else
                    {
                        status = CallWebService();
                    }

                    Play p = new Play();
                    switch (status)
                    {
                        case "samplesFound":
                            l.writeToLog("Info : Web Service returned at least 1 cow sample", Logging.eventType.AppInfoEvent);
                            break;
                        case "noResults":
                            l.writeToLog("Info : Web Service didn't return any cow samples. Going to MainMenu.aspx", Logging.eventType.AppInfoEvent);
                            p.NextURL = "MainMenu.aspx";
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CowSamples_Msg1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "CowSamples_Msg1", true, "No test rizuhlts were found for the period 1 month prior to the current date.");
                            pt.addTTSToPrompt("No test results were found for the period 1 month prior to the current date.");
                            p.setPrompt(pt);
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                            l.writeToLog("Info : Exiting from CowSamples", Logging.eventType.AppInfoEvent);
                            Response.Write(p.getVXML());
                            return;
                        default:
                            l.writeToLog("Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppErrorEvent);
                            p.NextURL = "SystemError.aspx";
                            AddSilenceToPrompt(pt);
                            p.setPrompt(pt);
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                            l.writeToLog("Info : Exiting from CowSamples", Logging.eventType.AppInfoEvent);
                            Response.Write(p.getVXML());
                            return;
                    }
                }
                l.writeToLog(" -- Cow Samples -- Pre Queue", Logging.eventType.AppInfoEvent);

                Queue<WebServiceResult> cowSamples = (Queue<WebServiceResult>)Session["cowSamplesResults"];

                l.writeToLog(" -- Cow Samples -- After Queue", Logging.eventType.AppInfoEvent);
                int resultsToPlay = 0;
                while (cowSamples.Count > 0 && resultsToPlay < 5)
                {
                    WebServiceResult aCowSample = cowSamples.Dequeue();
                    l.writeToLog(" -- Cow Samples -- After Dequeue", Logging.eventType.AppInfoEvent);

                    string ttsSample = aCowSample.GetPromptText();
                    
                    // pt = aCowSample.GetPromptText();
                    
                    l.writeToLog("Debug : Adding TTS cow sample:" + ttsSample, Logging.eventType.AppLogAlwaysEvent);
                    pt.addTTSToPrompt(ttsSample);

                    resultsToPlay++;
                }
                if (cowSamples.Count == 0)
                {
                    Play p = new Play();
                    l.writeToLog("Info : No more cow samples. Going to MainMenu.aspx", Logging.eventType.AppInfoEvent);
                    p.NextURL = "MainMenu.aspx";
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CowSamples_Msg2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "CowSamples_Msg2", true, "No more rizuhlts are available. Returning to the main menu.");
                    pt.addTTSToPrompt("No more results are available. Returning to the main menu.");
                    p.setPrompt(pt);
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from CowSamples", Logging.eventType.AppInfoEvent);
                    Session["cowSamplesResults"] = null;
                    Response.Write(p.getVXML());
                }
                else
                {
                    GVP.MCL.Enhanced.Menu aMenu = new GVP.MCL.Enhanced.Menu("CowSamplesMenu");
                    aMenu.addMenuChoice("1", "CowSamples.aspx?event=inputOptions");
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CowSamples_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "CowSamples_Init", true, "To hear up to the next 5 rizuhlts, press 1. To return to the main menu, press any other kee.");
                    pt.addTTSToPrompt("To hear up to the next 5 results, press 1. To return to the main menu, press any other key.");
                    aMenu.setPrompt(pt);
                    aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "CowSamples.aspx?event=returnToMain"));
                    aMenu.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "CowSamples.aspx?event=returnToMain"));
                    aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "CowSamples.aspx?event=returnToMain"));
                    aMenu.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "CowSamples.aspx?event=returnToMain"));
                    aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "CowSamples.aspx?event=inputOptions"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Getting input to see if continue with more cow samples", Logging.eventType.AppInfoEvent);
                    Response.Write(aMenu.getVXML());
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CowSamples -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private CatchEvent ErrorHandling(int iCount, string strCatchType, string strNextURL)
        {
            CatchEvent e = new CatchEvent(iCount, strCatchType);

            try
            {
                e.Reprompt = false;
                if (strCatchType.Equals(CatchEvent.NO_MATCH))
                {
                    e.NextURL = strNextURL + "&subevent=" + CatchEvent.NO_MATCH;
                }
                else
                {
                    e.NextURL = strNextURL + "&subevent=" + CatchEvent.NO_INPUT;
                }
            }
            catch(Exception ex)
            {
                l.writeToLog("-- CowSamples -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }

            return e;
        }

        private string CallWebService()
        {
            DerivedProducerCowSamplesService ws = new DerivedProducerCowSamplesService();
            string status;

            try
            {
                string divisionNumber = Session["division"].ToString();
                string producerNumber = Session["producer"].ToString();
                l.writeToLog("Info : Calling ProducerCowSamplesService.retrieveProducerCowSamples with periodType 03, divisionNumber " + divisionNumber + " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.retrieveProducerCowSamples("3", divisionNumber, producerNumber);
                l.writeToLog("Debug : ProducerCowSamplesService.retrieveProducerCowSamples returned " + result, Logging.eventType.AppLogAlwaysEvent);
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/ProducerCowSamples");
                XmlNodeList aList = aDoc.SelectNodes("//a:Sample", nsmgr);
                Queue<WebServiceResult> cowSamples = new Queue<WebServiceResult>(aList.Count + 1);
                foreach (XmlNode aNode in aList)
                {
                    XmlNodeList childNodes = aNode.ChildNodes;
                    WebServiceResult aCowSample = new WebServiceResult(childNodes.Count + 1);
                    foreach (XmlNode childNode in childNodes)
                    {
                        aCowSample.Add(childNode.Attributes["name"].Value, childNode.Attributes["value"].Value);
                    }
                    cowSamples.Enqueue(aCowSample);
                }
                if (cowSamples.Count > 0)
                {
                    l.writeToLog("Info : " + cowSamples.Count + " cow samples returned", Logging.eventType.AppInfoEvent);
                    Session["cowSamplesResults"] = cowSamples;
                    status = "samplesFound";
                }
                else
                {
                    status = "noResults";
                }
                l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CowSamples -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                status = "error";
            }
            finally
            {
                if (ws != null)
                {
                    try
                    {
                        ws.Dispose();
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            return status;
        }
        private string CallNewWebService()
        {
            Z_WS_GET_TEST_RESULTSService newWS = new Z_WS_GET_TEST_RESULTSService();
            string status = "";

            try
            {
                string divisionNumber = Session["division"].ToString();
                string producerNumber = Session["producer"].ToString();
                int[] dayInMonths = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                string name, val;
                string[] Months = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                string insert = "";
                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();
                l.writeToLog("-- Cow Samples -- Info : Calling Z_WS_GET_TEST_RESULTS " + divisionNumber + "   " + producerNumber, Logging.eventType.AppInfoEvent);

                /*
                if (Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"]) > 0)
                    insert = @"&lt;break time=&quot;" + System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"].Trim() + @"ms&quot;/&gt;";
                */

                l.writeToLog("-- CowSamples -- Silence insert is " + insert, Logging.eventType.AppInfoEvent);

                newWS.Url = strWebServiceURL;
                newWS.UseDefaultCredentials = false;
                newWS.PreAuthenticate = true;
                l.writeToLog("-- Cow Samples -- URL is " + newWS.Url.ToString(), Logging.eventType.AppInfoEvent);

                CredentialCache cred = new CredentialCache();
                NetworkCredential netCred = new NetworkCredential(NewUserID, NewPasswd);
                cred.Add(new Uri(strWebServiceURL), "Basic", netCred);
                newWS.Credentials = cred;
                newWS.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);

                l.writeToLog("-- Cow Samples -- WebServiceURL " + strWebServiceURL, Logging.eventType.AppInfoEvent);

                string result = "0";
                string reportID = "003"; //IVR - Cow Samples
                string end_date = "";
                string str_date = "";
                string today;
                string lastMonthDate;

                //  -- Type 3 is  1 month prior to current date
                DateTime now = DateTime.Now;
                string myMonth = now.Month.ToString();
                string myDay = now.Day.ToString();
                if (myMonth.Length == 1) myMonth = "0" + myMonth;
                if (myDay.Length == 1) myDay = "0" + myDay;
                today = now.Year.ToString() + "-" + myMonth + "-" + myDay;

                DateTime LMonth = now.AddDays(-dayInMonths[now.Month]);
                string myLastMonth = LMonth.Month.ToString();
                string myLastDay = LMonth.Day.ToString();

                if (myLastMonth.Length == 1) myLastMonth = "0" + myLastMonth;
                if (myLastDay.Length == 1) myLastDay = "0" + myLastDay;
                lastMonthDate = LMonth.Year.ToString() + "-" + myLastMonth + "-" + myLastDay;

                l.writeToLog("-- Cow Samples -- today " + today + " lastMonthDate " + lastMonthDate, Logging.eventType.AppInfoEvent);
                str_date = lastMonthDate;
                end_date = today;

                // ZMVRU_OUTPUT[] ress;
                ZMVRU_OUTPUT_NET[] ress;

                l.writeToLog("-- Cow Samples -- Z_WS_GET_TEST_RESULTS with " +
                    ", reportID " + reportID +
                    ", startDate " + str_date +
                    ", endDate " + end_date +
                    ", divisionNumber " + divisionNumber +
                    " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                // Start Testing parameters
                //    str_date = "2014-01-01";
                // End Testing parameter

                result = newWS.Z_WS_GET_TEST_RESULTS(divisionNumber, end_date, producerNumber, reportID, str_date, out ress);
                l.writeToLog("-- Cow Samples -- ZMVRU_OUTPUT WebService Call returned " + result, Logging.eventType.AppInfoEvent);

                int resl = ress.Length;
                l.writeToLog("-- Cow Samples -- ress length " + resl, Logging.eventType.AppInfoEvent);

                if (result.Equals("1"))
                {
                    int cnt = 0;
                    int counter = 0;

                    Queue<WebServiceResult> cowSamples = new Queue<WebServiceResult>(resl + 1);

                    cnt = 0;
                    for (int row = 0; row < resl; row++)
                    {
                        string ProcessBuf = "";
                        var testResult = ress[row].PICKUP;

                        WebServiceResult aCowSample = new WebServiceResult(counter);

                        foreach (var item in testResult)
                        {
                            string desc = "";
                            string oldval = "";
                            int trimCheck;

                            val = item.FIELD.VALUE.Trim();
                            name = item.FIELD.NAME.Trim();
                            desc = item.FIELD.TESTDESC;
                            oldval = val;

                            trimCheck = 0;

                            switch (name.Trim()) // Do I need to trim the zeroes in the string value 
                            {
                                case "LABEL": trimCheck = 1; break;
                                case "BFT": trimCheck = 1; break;
                                case "PRO": trimCheck = 1; break;
                                case "LAC": trimCheck = 1; break;
                                case "SNF": trimCheck = 1; break;
                                case "OSOL": trimCheck = 1; break;
                                case "SCC": trimCheck = 1; break;
                                case "SPC": trimCheck = 1; break;
                                case "PI COUNT": trimCheck = 1; break;
                                case "LPC": trimCheck = 1; break;
                                case "FRZP": trimCheck = 1; break;
                                case "MUN": trimCheck = 1; break;
                                case "COLI": trimCheck = 1; break;  // specific to CowSamples
                                default: trimCheck = 0; break;
                            }

                            if (trimCheck == 1 && val.Length > 1)
                            {
                                while (val.StartsWith("0") && !val.StartsWith("0."))
                                {
                                    string temp;
                                    if (val.Length <= 1) break;
                                    temp = val.Substring(1, val.Length - 1);
                                    val = temp;
                                }
                                if (val.Contains("."))
                                {
                                    while (val.EndsWith("0") && !val.EndsWith(".0"))
                                    {
                                        string temp;
                                        if (val.Length <= 2) break;
                                        temp = val.Substring(0, val.Length - 1);
                                        val = temp;
                                    }
                                }
                                if (val.Equals("0.0") || val.Equals(".0") || val.Equals("0.")) val = "0";
                            }

                            // checking
                            ProcessBuf += "\n\rchecking " + name + " " + val;

                            //  only add items that have a value other than zero or that do not have a value of "#"
                            if (val.Equals("0") || val.Equals("#") || val.Length < 1) //02 20 15
                            {
                                ProcessBuf += "   --->> OMMITTING ";
                                continue;
                            }

                            switch (name.Trim())
                            {
                                case "TEST DATE":
                                    try
                                    { // convert pickup date from yyyymmdd to spoken month, space, day
                                        int iDay = Convert.ToInt16(val.Substring(6, 2));
                                        int iMonth = Convert.ToInt16(val.Substring(4, 2));
                                        string newVal = Months[iMonth] + " " + iDay.ToString();
                                        val = newVal;
                                    }
                                    catch { }
                                    break;
                                case "TEMP":
                                    val = val + " Degrees";
                                    break;
                                default:
                                    break;
                            }

                            aCowSample.Add(desc + insert, val + insert);

                            // if (cnt < 20)
                            ProcessBuf += "\n\rCow Samples  " + row + "," + cnt + "  " + name + " , " + oldval + "  " + desc + insert + " , " + val + insert;
                            cnt++;
                        }

                        l.writeToLog(ProcessBuf + "\n\r", Logging.eventType.AppInfoEvent);
                        ProcessBuf = "";
                        counter++;
                        cowSamples.Enqueue(aCowSample);
                    }
                    if (cowSamples.Count > 0)
                    {
                        l.writeToLog("-- Cow Samples -- " + cowSamples.Count + " test results returned", Logging.eventType.AppInfoEvent);
                        Session["cowSamplesResults"] = cowSamples;
                        status = "samplesFound";
                    }
                    else
                    {
                        status = "noResults";
                    }
                    l.writeToLog("-- Cow Samples -- Finished processing web service results", Logging.eventType.AppInfoEvent);
                }
                else if (result.Equals("0"))
                {
                    status = "noResults";
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CowSamples -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
            finally
            {
                if (newWS != null)
                {
                    newWS.Dispose();
                }
            }
            l.writeToLog("-- Cow Samples -- Return status is " + status, Logging.eventType.AppInfoEvent);
            return status;
        }
    }
}