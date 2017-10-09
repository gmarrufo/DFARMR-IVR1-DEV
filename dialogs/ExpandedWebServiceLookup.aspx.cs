using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewGetTestResults.WebReference;

namespace DFARMR_IVR1
{
    public partial class ExpandedWebServiceLookup : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in ExpandedWebServiceLookup.aspx", Logging.eventType.AppBegin);
                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4100", "ExpandedWebServiceLookup", "");
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
                        //Log.Write(pageGVPSessionID, "Info : GVP SessionID from QueryString is null");
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
                l.writeToLog("-- Expanded WS Lookup --Entered into ExpandedWebServiceLookup state", Logging.eventType.AppInfoEvent);
                Session["ExpandedWebServiceLookup"] = null;
                Session["ExpandedComponentTypes"] = null;
                Session["ExpandedQualityTypes"] = null;
                Session["ExpandedComponentTypesSpeak"] = null;
                Session["ExpandedQualityTypesSpeak"] = null;
                
                ProcessRequest();
                l.writeToLog("Application end in ExpandedWebServiceLookup.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Expanded WS Lookup -- Error : Got error while reading GVP and IIS SessionID values from session object, Message : " + ex.StackTrace, Logging.eventType.AppException);
            }
        }

        private void ProcessRequest()
        {
            try
            {
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                Session["nodeType"] = Logging.nodeDataType.Unknown;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4100", "ExpandedWebServiceLookup", "Calling Web Services");
                string status = "0";
                string divisionNumber = Session["division"].ToString();
                if (divisionNumber.Length == 3)
                {
                    status = CallNewWebService();
                }
                else
                {
                    status = CallWebService();
                }
                l.writeToLog("status: " + status, Logging.eventType.AppInfoEvent);

                Play p = new Play();
                switch (status)
                {
                    case "resultsFound":
                        l.writeToLog("-- Expanded WS Lookup -- Web Service returned at least 1 test result - Going into ComponentOrQuality.aspx", Logging.eventType.AppInfoEvent);
                        p.NextURL = "ComponentOrQuality.aspx";
                        AddSilenceToPrompt(pt);
                        p.setPrompt(pt);
                        break;
                    case "noResults":
                        string previousMenu = Session["previousMenu"] != null ? Session["previousMenu"].ToString() : "ExpandedMenu.aspx";
                        l.writeToLog("-- Expanded WS Lookup -- Web Service didn't return any test results. Going to " + previousMenu, Logging.eventType.AppInfoEvent);
                        p.NextURL = previousMenu;
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ExpandedWebServiceLookup_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "ExpandedWebServiceLookup_Msg", true, "No test rizuhlts were found for that time fraym.");
                        pt.addTTSToPrompt("No test results were found for that time frame.");
                        p.setPrompt(pt);
                        break;
                    default:
                        l.writeToLog("-- Expanded WS Lookup -- Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppInfoEvent);
                        p.NextURL = "SystemError.aspx";
                        AddSilenceToPrompt(pt);
                        p.setPrompt(pt);
                        break;
                }
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(p.getVXML());
                return;
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ExpandedWebServiceLookup -- ProcessRequest fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private CatchEvent ErrorHandling(int iCount, string strCatchType, string strNextURL)
        {
            CatchEvent e = new CatchEvent(iCount, strCatchType);
            e.Reprompt = false;
            e.NextURL = strNextURL;
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
                string periodType = "4"; //current and prior month
                string month = "";
                string startDate = "";
                string endDate = "";
                string profileType = "2";
                if (Request.QueryString["month"] != null)
                {
                    periodType = "0";
                    month = Request.QueryString["month"].ToString().Trim();
                    DateTime now = DateTime.Now;
                    startDate = month + "/01/" + now.Year.ToString();
                    DateTime start = new DateTime(now.Year, int.Parse(month), 1);
                    endDate = month + "/" + start.AddMonths(1).AddDays(-1).Day.ToString() + "/" + now.Year.ToString();
                }
                l.writeToLog("Info : Calling ProducerWeightsAndTestsService.retrieveProducerWeightsAndTests with periodType " + periodType
                    + ", startDate " + startDate + ", endDate " + endDate + ", profileType " + profileType + ", divisionNumber " + divisionNumber
                    + " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.retrieveProducerWeightsAndTests(periodType, profileType, startDate, endDate, divisionNumber, producerNumber);
                l.writeToLog("Debug : ProducerWeightsAndTestsService.retrieveProducerWeightsAndTests returned " + result, Logging.eventType.AppLogAlwaysEvent);
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/ProducerWeightsAndTests");
                XmlNodeList aList = aDoc.SelectNodes("//a:Pickup", nsmgr);
                List<string> componentTypes = new List<string>(9);
                List<string> qualityTypes = new List<string>(9);
                List<string> componentTypesSpeak = new List<string>(9);
                List<string> qualityTypesSpeak = new List<string>(9);

                List<WebServiceResult> testResults = new List<WebServiceResult>(aList.Count + 1);
                foreach (XmlNode aNode in aList)
                {
                    XmlNodeList childNodes = aNode.ChildNodes;
                    WebServiceResult aTestResult = new WebServiceResult(true, childNodes.Count + 1);
                    foreach (XmlNode childNode in childNodes)
                    {
                        string category = childNode.Attributes["category"].Value;
                        string spokenName = childNode.Attributes["name"].Value;
                        string name = spokenName.Substring(0, spokenName.IndexOf("&lt"));
                        if (category.Equals(WebServiceResult.COMPONENT))
                        {
                            if (!componentTypes.Contains(name))
                            {
                                componentTypesSpeak.Add(spokenName);
                                componentTypes.Add(name);
                            }
                        }
                        else if (category.Equals(WebServiceResult.QUALITY))
                        {
                            if (!qualityTypes.Contains(name))
                            {
                                qualityTypesSpeak.Add(spokenName);
                                qualityTypes.Add(name);
                            }
                        }
                        aTestResult.Add(category, name, childNode.Attributes["value"].Value);
                    }
                    testResults.Add(aTestResult);
                }
                if (testResults.Count > 0)
                {
                    l.writeToLog("Info : " + testResults.Count + " test results returned", Logging.eventType.AppInfoEvent);
                    Session["ExpandedComponentTypes"] = componentTypes;
                    Session["ExpandedQualityTypes"] = qualityTypes;
                    Session["ExpandedComponentTypesSpeak"] = componentTypesSpeak;
                    Session["ExpandedQualityTypesSpeak"] = qualityTypesSpeak;
                    Session["ExpandedWebServiceLookup"] = testResults;
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
                l.writeToLog("Error : Got error while accessing ProducerWeightsAndTestsService.retrieveProducerWeightsAndTests web service : " + ex.ToString(), Logging.eventType.AppInfoEvent);
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
            string result = "0";
            string status = "";

            try
            {
                l.writeToLog("here", Logging.eventType.AppInfoEvent);

                Z_WS_GET_TEST_RESULTSService newWS = new Z_WS_GET_TEST_RESULTSService();
                
                string OutBuf = "";
                string ProcessBuf = "";
                string divisionNumber = Session["division"].ToString();
                string producerNumber = Session["producer"].ToString();
                string[] Months = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                int insertINT = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"]);
                string insert = "";

                // if (insertINT > 0)
                //    insert = @"&lt;break time=&quot;" + System.Configuration.ConfigurationManager.AppSettings["SilenceInsert"].Trim() + @"ms&quot;/&gt;";

                // l.writeToLog("-- Expanded WS Lookup -- Silence insert is " + insert, Logging.eventType.AppInfoEvent);

                l.writeToLog("-- Expanded WS Lookup -- Calling Z_WS_GET_TEST_RESULTS " + divisionNumber + "   " + producerNumber, Logging.eventType.AppInfoEvent);

                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();

                newWS.Url = strWebServiceURL;
                newWS.UseDefaultCredentials = false;
                newWS.PreAuthenticate = true;
                l.writeToLog("-- Expanded WS Lookup -- URL is " + newWS.Url.ToString(), Logging.eventType.AppInfoEvent);
                if (newWS.UseDefaultCredentials == false) l.writeToLog("-- Expanded WS Lookup -- UseDefaultCredentials is FALSE ", Logging.eventType.AppInfoEvent);
                else l.writeToLog("-- Expanded WS Lookup -- UseDefaultCredentials is TRUE ", Logging.eventType.AppInfoEvent);
                CredentialCache cred = new CredentialCache();
                NetworkCredential netCred = new NetworkCredential(NewUserID, NewPasswd);
                cred.Add(new Uri(strWebServiceURL), "Basic", netCred);
                newWS.Credentials = cred;
                newWS.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);

                string reportID = "001"; //IVR - VRU EXPANDED
                string end_date = "2014-05-11";
                string str_date = "2013-01-01";
                string month;

                if (Request.QueryString["month"] != null)
                {
                    // periodType = "0";
                    l.writeToLog("-- Expanded WS Lookup -- by Month ", Logging.eventType.AppInfoEvent);
                    month = Request.QueryString["month"].ToString().Trim();
                    l.writeToLog("-- Requesting month = " + month, Logging.eventType.AppInfoEvent);
                    DateTime now = DateTime.Now;
                    int requestedMonth = Convert.ToInt16(month);
                    int thisMonth = Convert.ToInt16(now.Month.ToString());
                    l.writeToLog("-- thisMonth " + thisMonth + " RequestedMonth " + requestedMonth, Logging.eventType.AppInfoEvent);
                    if (requestedMonth > thisMonth)
                    {
                        int lastYear = Convert.ToInt16(now.Year.ToString()) - 1;
                        DateTime d = new DateTime(lastYear, requestedMonth, 1);
                        l.writeToLog("-- LAST Year " + lastYear.ToString() + " last year date " + d.ToString(), Logging.eventType.AppInfoEvent);
                        str_date = lastYear.ToString() + "-" + month + "-01";
                        end_date = d.Year.ToString() + "-" + month + "-" + d.AddMonths(1).AddDays(-1).Day.ToString();
                    }
                    else
                    {
                        str_date = now.Year.ToString() + "-" + month + "-01";
                        DateTime start = new DateTime(now.Year, int.Parse(month), 1);
                        end_date = now.Year.ToString() + "-" + month + "-" + start.AddMonths(1).AddDays(-1).Day.ToString();
                    }
                }
                else
                {
                    // Type 4 date -> Prior and Current Month
                    l.writeToLog("-- Expanded WS Lookup -- by Type 4 ", Logging.eventType.AppInfoEvent);
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
                    str_date = lastMonthDate;
                    end_date = today;
                }

                l.writeToLog("-- Expanded WS Lookup -- start date " + str_date + " end date " + end_date, Logging.eventType.AppInfoEvent);

                // ZMVRU_OUTPUT[] ress;
                ZMVRU_OUTPUT_NET[] ress;

                l.writeToLog("-- Expanded WS Lookup -- after new ZMVRU_OUTPUT", Logging.eventType.AppInfoEvent);

                l.writeToLog("-- Expanded WS Lookup --  : Calling New Z_WS_GET_TEST_RESULTS with " +
                    ", reportID " + reportID +
                    ", startDate " + str_date +
                    ", endDate " + end_date +
                    ", divisionNumber " + divisionNumber +
                    " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                string name, val;
                string category = "";
                string spokenName = "";

                int cnt = 0;
                int counter = 0;

                newWS.PreAuthenticate = true;

                result = newWS.Z_WS_GET_TEST_RESULTS(divisionNumber, end_date, producerNumber, reportID, str_date, out ress);
                l.writeToLog("-- Expanded WS Lookup -- ZMVRU_OUTPUT WebService Call returned " + result, Logging.eventType.AppInfoEvent);
                int resl = ress.Length;
                l.writeToLog("-- Expanded WS Lookup -- ress length " + resl, Logging.eventType.AppInfoEvent);

                List<WebServiceResult> testResults = new List<WebServiceResult>(resl + 1); // 1

                if (result.Equals("1"))
                {
                    List<string> componentTypes = new List<string>(9);
                    List<string> qualityTypes = new List<string>(9);
                    List<string> componentTypesSpeak = new List<string>(9);
                    List<string> qualityTypesSpeak = new List<string>(9);
                   
                    cnt = 0;
                    for (int row = 0; row < resl; row++)
                    {
                        var testResult = ress[row].PICKUP;
                        int ii = testResult.Length;

                        WebServiceResult aTestResult = new WebServiceResult(true, ii + 1); //2

                        foreach (var item in testResult)
                        {
                            int trimCheck;
                            category = item.FIELD.CATEGORY.Trim();
                            // name = item.FIELD.NAME.Trim();
                            name = item.FIELD.TESTDESC.Trim();
                            spokenName = item.FIELD.TESTDESC.Trim() + insert;
                            val = item.FIELD.VALUE.Trim();
                            trimCheck = 0;

                            // OutBuf += "\n\r -- CATEGORY = " + category;
                            // OutBuf += " -- NAME = " + item.FIELD.NAME.Trim();
                            // OutBuf += " -- TESTDESC = " + item.FIELD.TESTDESC.Trim();
                            // OutBuf += " -- VALUE = " + item.FIELD.VALUE.Trim();

                            switch (item.FIELD.NAME.Trim()) // Do I need to trim the zeroes in the string value 
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
                            ProcessBuf += "\n\rchecking " + item.FIELD.NAME + " " + val;

                            //  only add items that have a value other than zero or that do not have a value of "#"
                            if (val.Equals("0") || val.Equals("#") || val.Length < 1) //02 20 15
                            {
                                ProcessBuf += "   --->> OMMITTING ";
                                continue;
                            }
                            
                            switch (item.FIELD.NAME.Trim())
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
                                    name = "TANK"; // THis is necessary, TESTDESC messes up logic down the Road
                                    try
                                    {
                                        if (val.Length > 0)
                                            val = Convert.ToInt16(val).ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }

                            if (category.Equals(WebServiceResult.COMPONENT))
                            {
                                if (!componentTypes.Contains(name))
                                {
                                    componentTypesSpeak.Add(spokenName);
                                    componentTypes.Add(name);
                                    l.writeToLog("-- Expanded WS Lookup -- added COMPONENT spokenName " + spokenName + " , " + name, Logging.eventType.AppInfoEvent);
                                }
                            }
                            else if (category.Equals(WebServiceResult.QUALITY))
                            {
                                if (!qualityTypes.Contains(name))
                                {
                                    qualityTypesSpeak.Add(spokenName);
                                    qualityTypes.Add(name);
                                    l.writeToLog("-- Expanded WS Lookup -- added QUALITY spokenName " + spokenName + " , " + name, Logging.eventType.AppInfoEvent);
                                }
                            }

                            string newval = val + insert;

                            aTestResult.Add(category, name, newval); //3

                            if (cnt == 0)
                            {
                                OutBuf = "-- Expanded WS Lookup -- added \n\r" + row + "," + cnt + "  " + category + " , " + name + " , " + newval + " , spokenName " + spokenName;
                            }
                            else
                            {
                                OutBuf += "\n\r" + row + "," + cnt + "  " + category + " , " + name + " , " + newval + " , spokenName " + spokenName;
                            }
                            cnt++;
                        }

                        // l.writeToLog(ProcessBuf + "\n\r" + OutBuf, Logging.eventType.AppInfoEvent);
                        OutBuf = "";
                        ProcessBuf = "";
                        counter++;
                        testResults.Add(aTestResult);
                    }

                    if (testResults.Count > 0 && testResults != null)
                    {
                        l.writeToLog("-- Expanded WS Lookup -- " + testResults.Count + " test results returned", Logging.eventType.AppInfoEvent);
                        Session["ExpandedComponentTypes"] = componentTypes;
                        Session["ExpandedQualityTypes"] = qualityTypes;
                        Session["ExpandedComponentTypesSpeak"] = componentTypesSpeak;
                        Session["ExpandedQualityTypesSpeak"] = qualityTypesSpeak;
                        Session["ExpandedWebServiceLookup"] = testResults;
                        status = "resultsFound";
                    }
                    else
                    {
                        status = "noResults";
                    }

                    l.writeToLog("-- Expanded WS Lookup -- Finished processing web service results", Logging.eventType.AppInfoEvent);
                }
                else if (result.Equals("0"))
                {
                    status = "noResults";
                }

                l.writeToLog("-- ExpandedWebServiceLookup -- Return status is " + status, Logging.eventType.AppInfoEvent);
                return status;
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ExpandedWebServiceLookup -- Z_WS_GET_TEST_RESULTS fail --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
        }
    }
}