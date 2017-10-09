using System;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewGetTestResults.WebReference;

namespace DFARMR_IVR1
{
    public partial class FastResults : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in FastResults.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "3000", "FastResults", "");

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
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
                }
                else //Getting IIS SessionId from Session.SessionID
                {
                    l.writeToLog("Info : Found  IISSessionID as null from session object", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : Getting IISSessionID from Session.SessionID", Logging.eventType.AppInfoEvent);
                    pageIISSessionID = Session.SessionID;
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }

                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("Info : Entered into -----   FastResults   ----- state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                l.writeToLog("Info : FastResults event " + selection, Logging.eventType.AppInfoEvent);
                if (selection.Equals("inputOptions"))
                {
                    string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                    l.writeToLog("Info : User has selected : " + input, Logging.eventType.AppInfoEvent);

                    if (input.Equals("1"))
                    {
                        l.writeToLog("Info : Caller selected 1 to hear up to next 5 results", Logging.eventType.AppInfoEvent);
                        InputOptions(false);
                    }
                }
                else if (selection.Equals("returnToMain"))
                {
                    l.writeToLog("Info : Didn't hit dtmf 1 so returning to main menu", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "MainMenu.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from FastResults", Logging.eventType.AppInfoEvent);
                    Session["fastResults"] = null;
                    Response.Write(p.getVXML());
                    return;
                }
                else
                {
                    Session["fastResults"] = null;
                    InputOptions(true);
                }
                l.writeToLog("Application end in FastResults.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- FastResults -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(bool callWebService)
        {
            try
            {
                // Prompt p1R = new Prompt();
                // Prompt p2R = new Prompt();
                // Prompt p3R = new Prompt();
                // Prompt p4R = new Prompt();
                // Prompt p5R = new Prompt();
                Prompt pt = new Prompt();
                // p1R.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                // p2R.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                // p3R.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                // p4R.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                // p5R.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);

                Session["nodeType"] = Logging.nodeDataType.Unknown;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "3000", "FastResults", "Calling Web Services");

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

                    l.writeToLog("-- Fast Results -- InputOptions WebService returned : " + status, Logging.eventType.AppInfoEvent);

                    Play p = new Play();
                    switch (status)
                    {
                        case "resultsFound":
                            l.writeToLog("-- Fast Results -- resultsFound : Web Service returned at least 1 test result", Logging.eventType.AppInfoEvent);
                            break;
                        case "noResults":
                            l.writeToLog("-- Fast Results -- noResults : Web Service didn't return any test results. Going to MainMenu.aspx", Logging.eventType.AppInfoEvent);
                            p.NextURL = "MainMenu.aspx";
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "FastResults_Msg1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            pt.addTTSToPrompt("No test results were found for the prior and current month.");
                            p.setPrompt(pt);
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                            l.writeToLog("Info : Exiting from FastResults", Logging.eventType.AppInfoEvent);
                            Response.Write(p.getVXML());
                            return;
                        default:
                            l.writeToLog("-- Fast Results -- default (error) : Web Service error. Going to SystemError.aspx", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                            AddSilenceToPrompt(pt);
                            p.setPrompt(pt);
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                            l.writeToLog("Info : Exiting from FastResults", Logging.eventType.AppInfoEvent);
                            Response.Write(p.getVXML());
                            return;
                    }
                }

                Queue<WebServiceResult> testResults = (Queue<WebServiceResult>)Session["fastResults"];

                int resultsToPlay = 0;

                l.writeToLog("-- Fast Results -- noResults testResults.Count " + testResults.Count, Logging.eventType.AppInfoEvent);

                while (testResults.Count > 0 && resultsToPlay < 5)
                {
                    WebServiceResult aTestResult = testResults.Dequeue();

                    string ttsSample = aTestResult.GetPromptText();
                    l.writeToLog("FastResults.aspx - Test Result: " + ttsSample, Logging.eventType.AppInfoEvent);

                    // pt = aTestResult.GetPromptText();
                    /*
                    switch (resultsToPlay)
                    {
                        case 0:
                            p1R = aTestResult.GetPromptText();
                            l.writeToLog("FastResults.aspx - Test Result 1: " + p1R.getVXML(), Logging.eventType.AppInfoEvent);
                            break;
                        case 1:
                            p2R = aTestResult.GetPromptText();
                            l.writeToLog("FastResults.aspx - Test Result 2: " + p2R.getVXML(), Logging.eventType.AppInfoEvent);
                            break;
                        case 2:
                            p3R = aTestResult.GetPromptText();
                            l.writeToLog("FastResults.aspx - Test Result 3: " + p3R.getVXML(), Logging.eventType.AppInfoEvent);
                            break;
                        case 3:
                            p4R = aTestResult.GetPromptText();
                            l.writeToLog("FastResults.aspx - Test Result 4: " + p4R.getVXML(), Logging.eventType.AppInfoEvent);
                            break;
                        case 4:
                            p5R = aTestResult.GetPromptText();
                            l.writeToLog("FastResults.aspx - Test Result 5: " + p5R.getVXML(), Logging.eventType.AppInfoEvent);
                            break;
                    }
                    */

                    // l.writeToLog("Debug : Adding TTS test result:" + ttsSample, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt(ttsSample);

                    resultsToPlay++;
                }

                if (testResults.Count == 0)
                {
                    Play p = new Play();
                    l.writeToLog("Info : No more test results. Going to MainMenu.aspx", Logging.eventType.AppInfoEvent);
                    p.NextURL = "MainMenu.aspx";
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "FastResults_Msg2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt("No more results are available. Returning to the main menu.");
                    p.setPrompt(pt);
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from FastResults", Logging.eventType.AppInfoEvent);
                    Session["fastResults"] = null;
                    Response.Write(p.getVXML());
                }
                else
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "FastResults_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.BargeIn = true;
                    pt.TimeOut = 3;

                    pt.addTTSToPrompt("To hear up to the next 5 rizuhlts, press 1. To return to the main menu, press any other kee.");

                    Menu aMenu = new Menu("FastResultsMenu");

                    string[] arPrompts = new string[5];
                    arPrompts[0] = "Sorry, I didn't hear you.";
                    arPrompts[1] = "Sorry, I still didn't hear you.";
                    // arPrompts[2] = "I'm sorry that I am unable to assist you at this time. Please try " + TTSDictionary("your") + " call later. Good byee.";
                    arPrompts[2] = "";
                    arPrompts[3] = "Sorry, I didn't understand.";
                    arPrompts[4] = "Sorry, I still didn't understand.";

                    Prompt pNI1 = new Prompt();
                    pNI1.addTTSToPrompt(arPrompts[0]);
                    pNI1.TimeOut = 0;
                    pNI1.BargeIn = true;

                    Prompt pNI2 = new Prompt();
                    pNI2.addTTSToPrompt(arPrompts[1]);
                    pNI2.TimeOut = 0;
                    pNI2.BargeIn = true;

                    Prompt pMaxErrorEndCall = new Prompt();
                    pMaxErrorEndCall.addTTSToPrompt(arPrompts[2]);
                    pMaxErrorEndCall.TimeOut = 0;
                    pMaxErrorEndCall.BargeIn = true;

                    Prompt pNM1 = new Prompt();
                    pNM1.addTTSToPrompt(arPrompts[3]);
                    pNM1.TimeOut = 0;
                    pNM1.BargeIn = true;

                    Prompt pNM2 = new Prompt();
                    pNM2.addTTSToPrompt(arPrompts[3]);
                    pNM2.TimeOut = 0;
                    pNM2.BargeIn = true;

                    CatchEvent ceNoInput1 = aMenu.getCatchEvent(pNI1, 1, CatchEvent.NO_INPUT, "", true, false);
                    CatchEvent ceNoInput2 = aMenu.getCatchEvent(pNI2, 2, CatchEvent.NO_INPUT, "", true, false);
                    CatchEvent ceNoInput3 = aMenu.getCatchEvent(pMaxErrorEndCall, 3, CatchEvent.NO_INPUT, "TriesExceeded.aspx", false, true);
                    CatchEvent ceNoMatch1 = aMenu.getCatchEvent(pNM1, 1, CatchEvent.NO_MATCH, "", true, false);
                    CatchEvent ceNoMatch2 = aMenu.getCatchEvent(pNM2, 2, CatchEvent.NO_MATCH, "", true, false);
                    CatchEvent ceNoMatch3 = aMenu.getCatchEvent(pMaxErrorEndCall, 3, CatchEvent.NO_MATCH, "TriesExceeded.aspx", false, true);
                    CatchEvent ceMaxTries = aMenu.getCatchEvent(pMaxErrorEndCall, 1, CatchEvent.MAX_TRIES, "TriesExceeded.aspx", false, true);

                    aMenu.addCatchBlock(ceNoInput1);
                    aMenu.addCatchBlock(ceNoInput2);
                    aMenu.addCatchBlock(ceNoInput3);
                    aMenu.addCatchBlock(ceNoMatch1);
                    aMenu.addCatchBlock(ceNoMatch2);
                    aMenu.addCatchBlock(ceNoMatch3);
                    aMenu.addCatchBlock(ceMaxTries);

                    /*
                    aMenu.setPrompt(p1R);
                    aMenu.setPrompt(p2R);
                    aMenu.setPrompt(p3R);
                    aMenu.setPrompt(p4R);
                    aMenu.setPrompt(p5R);
                    */

                    aMenu.setPrompt(pt);
                    aMenu.MaxTries = 3;
                    aMenu.addMenuChoice("0", "FastResults.aspx?event=returnToMain&input=0");
                    aMenu.addMenuChoice("1", "FastResults.aspx?event=inputOptions&input=1");
                    aMenu.addMenuChoice("2", "FastResults.aspx?event=returnToMain&input=2");
                    aMenu.addMenuChoice("3", "FastResults.aspx?event=returnToMain&input=3");
                    aMenu.addMenuChoice("4", "FastResults.aspx?event=returnToMain&input=4");
                    aMenu.addMenuChoice("5", "FastResults.aspx?event=returnToMain&input=5");
                    aMenu.addMenuChoice("6", "FastResults.aspx?event=returnToMain&input=6");
                    aMenu.addMenuChoice("7", "FastResults.aspx?event=returnToMain&input=7");
                    aMenu.addMenuChoice("8", "FastResults.aspx?event=returnToMain&input=8");
                    aMenu.addMenuChoice("9", "FastResults.aspx?event=returnToMain&input=9");
                    aMenu.addMenuChoice("*", "FastResults.aspx?event=returnToMain&input=*");
                    aMenu.addMenuChoice("#", "FastResults.aspx?event=returnToMain&input=#");
                    l.writeToLog("Info : Getting input to see if continue with more fast results", Logging.eventType.AppInfoEvent);

                    // l.writeToLog("FastResults : InputOptions GetVXML :" + aMenu.getVXML(), Logging.eventType.AppInfoEvent);

                    Response.Write(aMenu.getVXML());
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- FastResults -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
            catch (Exception ex)
            {
                l.writeToLog("-- FastResults -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }

        private string CallWebService()
        {
            DerivedProducerWeightsAndTestsService ws = new DerivedProducerWeightsAndTestsService();
            string status;
            try
            {
                string divisionNumber = Session["division"].ToString();
                string producerNumber = Session["producer"].ToString();
                l.writeToLog("Info : Calling ProducerWeightsAndTestsService.retrieveProducerWeightsAndTests with periodType 4, profileType 1, divisionNumber " + divisionNumber + " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);
                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.retrieveProducerWeightsAndTests("4", "1", "", "", divisionNumber, producerNumber);
                result = result.Replace("&amp;lt;break time=&amp;quot;250ms&amp;quot;/&amp;gt;", "");
                l.writeToLog("Debug : ProducerWeightsAndTestsService.retrieveProducerWeightsAndTests returned " + result, Logging.eventType.AppLogAlwaysEvent);
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/ProducerWeightsAndTests");
                XmlNodeList aList = aDoc.SelectNodes("//a:Pickup", nsmgr);
                Queue<WebServiceResult> testResults = new Queue<WebServiceResult>(aList.Count + 1);
                foreach (XmlNode aNode in aList)
                {
                    XmlNodeList childNodes = aNode.ChildNodes;
                    WebServiceResult aTestResult = new WebServiceResult(childNodes.Count + 1);
                    foreach (XmlNode childNode in childNodes)
                    {
                        string name = childNode.Attributes["name"].Value;
                        string value = childNode.Attributes["value"].Value;
                        aTestResult.Add(name, value);
                    }
                    testResults.Enqueue(aTestResult);
                }
                if (testResults.Count > 0)
                {
                    l.writeToLog("Info : " + testResults.Count + " test results returned", Logging.eventType.AppInfoEvent);
                    Session["fastResults"] = testResults;
                    status = "resultsFound";
                }
                else
                {
                    status = "noResults";
                }
                l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- FastResults -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                string OutBuf = "";
                string ProcessBuf = "";
                string divisionNumber = Session["division"].ToString();
                string producerNumber = Session["producer"].ToString();

                l.writeToLog("-----------------------------------------", Logging.eventType.AppInfoEvent);
                l.writeToLog("------- FAST RESULTS -- 2/23/2015 -------", Logging.eventType.AppInfoEvent);
                l.writeToLog("-----------------------------------------", Logging.eventType.AppInfoEvent);

                l.writeToLog("-- FAST RESULTS -- Info : Calling Z_WS_GET_TEST_RESULTS " + divisionNumber + "   " + producerNumber, Logging.eventType.AppInfoEvent);
                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();

                int insertINT = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"]);
                string insert = "";


                if (insertINT > 0)
                    insert = @" !!\pause=" + System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"].Trim() + @"\ ";
                   

                l.writeToLog("-- Producer -- Silence insert is " + insert, Logging.eventType.AppInfoEvent);

                newWS.Url = strWebServiceURL;
                newWS.UseDefaultCredentials = false;
                newWS.PreAuthenticate = true;
                l.writeToLog("-- Producer -- URL is " + newWS.Url.ToString(), Logging.eventType.AppInfoEvent);
                if (newWS.UseDefaultCredentials == false) l.writeToLog("-- Producer -- UseDefaultCredentials is FALSE ", Logging.eventType.AppInfoEvent);
                else l.writeToLog("-- Producer -- UseDefaultCredentials is TRUE ", Logging.eventType.AppInfoEvent);

                CredentialCache cred = new CredentialCache();
                NetworkCredential netCred = new NetworkCredential(NewUserID, NewPasswd);
                cred.Add(new Uri(strWebServiceURL), "Basic", netCred);
                newWS.Credentials = cred;
                newWS.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);

                l.writeToLog("-- Fast Results -- WebServiceURL " + strWebServiceURL, Logging.eventType.AppInfoEvent);
                string result = "0";
                // string reportID = "001"; //IVR - Fast
                string reportID = "002"; //IVR - Fast

                // Type 4 date -> Prior and Current Month
                string end_date = "2014-05-09";
                string str_date = "2014-04-01";
                string today;
                string lastMonthDate;

                DateTime now = DateTime.Now;
                string myMonth = now.Month.ToString();
                string myDay = now.Day.ToString();

                if (myMonth.Length == 1) myMonth = "0" + myMonth;
                if (myDay.Length == 1) myDay = "0" + myDay;
                today = now.Year.ToString() + "-" + myMonth + "-" + myDay;
                int LastMonth = Convert.ToInt16(now.Month.ToString()) - 1;
                if (LastMonth == 0)
                {
                    int lastYear = Convert.ToInt16(now.Year.ToString()) - 1;
                    lastMonthDate = lastYear.ToString() + "-12-01";
                }
                else
                {
                    if (LastMonth < 10) lastMonthDate = now.Year.ToString() + "-0" + LastMonth.ToString() + "-01";
                    else lastMonthDate = now.Year.ToString() + "-" + LastMonth.ToString() + "-01";
                }
                l.writeToLog("-- Fast Results -- today " + today + " lastMonthDate " + lastMonthDate, Logging.eventType.AppInfoEvent);
                str_date = lastMonthDate;
                end_date = today;

                // ZMVRU_OUTPUT[] ress;
                ZMVRU_OUTPUT_NET[] ress;

                l.writeToLog("-- Fast Results  : Calling New Z_WS_GET_TEST_RESULTS with " +
                    ", reportID " + reportID +
                    ", startDate " + str_date +
                    ", endDate " + end_date +
                    ", divisionNumber " + divisionNumber +
                    " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                result = newWS.Z_WS_GET_TEST_RESULTS(divisionNumber, end_date, producerNumber, reportID, str_date, out ress);
                l.writeToLog("-- Fast Results -- ZMVRU_OUTPUT WebService Call returned " + result, Logging.eventType.AppInfoEvent);

                int resl = ress.Length;
                l.writeToLog("--- Fast Results -- ress length " + resl, Logging.eventType.AppInfoEvent);

                if (result.Equals("1"))
                {
                    string name, val, desc;

                    Queue<WebServiceResult> testResults = new Queue<WebServiceResult>(resl + 1);

                    int cnt = 0;
                    int counter = 0;
                    string[] Months = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                    cnt = 0;
                    for (int row = 0; row < resl; row++)
                    {
                        string oldval;

                        var testResult = ress[row].PICKUP;

                        WebServiceResult aTestResult = new WebServiceResult(counter);

                        foreach (var item in testResult)
                        {
                            int trimCheck;

                            val = item.FIELD.VALUE.Trim();
                            name = item.FIELD.NAME;
                            desc = item.FIELD.TESTDESC;
                            oldval = val;
                            trimCheck = 0;

                            switch (name.Trim()) // Do I need to trim the zeroes in the string value 
                            {
                                case "BFT": trimCheck = 1; break;
                                case "BF": trimCheck = 1; break;
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
                                case "SED": trimCheck = 1; break;
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
                                case "PICKUP DATE":
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
                                case "TICKET":
                                    try
                                    {
                                        if (val.Length > 0)
                                        {
                                            string tempVal = "";
                                            for (int k = 0; k < val.Length; k++)
                                                tempVal = tempVal + val.Substring(k, 1) + " ";
                                            val = tempVal;
                                        }
                                    }
                                    catch { }
                                    break;
                                case "PICKUP ID":
                                    try
                                    {
                                        if (val.Length > 0)
                                        {
                                            string tempVal = "";
                                            for (int k = 0; k < val.Length; k++)
                                                tempVal = tempVal + val.Substring(k, 1) + " ";
                                            val = tempVal;
                                        }
                                    }
                                    catch { }
                                    break;
                                case "TANK":
                                    try
                                    {
                                        if (val.Length > 0)
                                            val = insert + Convert.ToInt16(val).ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }

                            if (desc.Contains("PROTEIN"))
                            {
                                desc = TTSDictionary(desc);
                            }

                            if (desc.Contains("INHIBITOR"))
                            {
                                desc = TTSDictionary(desc);
                            }

                            val = val + insert;
                            desc = desc + insert;
                            
                            aTestResult.Add(desc, val);

                            if (cnt == 0)
                            {
                                OutBuf = "-- Fast Results -- \n\r" + row + "," + cnt + "  " + name + " , " + oldval + " --> " + desc + " , " + val;
                            }
                            else
                            {
                                OutBuf += "\n\r" + row + "," + cnt + "  " + name + " , " + oldval + " --> " + desc + " , " + val;
                            }
                            cnt++;
                        }

                        l.writeToLog(ProcessBuf + "\n\r" + OutBuf, Logging.eventType.AppInfoEvent);
                        cnt = 0;
                        OutBuf = "";
                        ProcessBuf = "";
                        counter++;
                        testResults.Enqueue(aTestResult);
                    }
                    if (testResults.Count > 0)
                    {
                        l.writeToLog("-- Fast Results -- " + testResults.Count + " test results returned", Logging.eventType.AppInfoEvent);
                        Session["fastResults"] = testResults;
                        status = "resultsFound";
                    }
                    else
                    {
                        status = "noResults";
                    }
                    l.writeToLog("-- Fast Results -- Finished processing web service results", Logging.eventType.AppInfoEvent);
                }
                else if (result.Equals("0"))
                {
                    status = "noResults";
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- FastResults -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
            finally
            {
                if (newWS != null)
                {
                    newWS.Dispose();
                }
            }
            l.writeToLog("-- Fast Results -- Return status is " + status, Logging.eventType.AppInfoEvent);
            return status;
        }
    }
}