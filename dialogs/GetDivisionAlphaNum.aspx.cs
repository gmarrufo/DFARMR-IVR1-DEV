using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.Web.UI.WebControls;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewDoesDivisionExist.WebReference;
using System.Linq;

namespace DFARMR_IVR1
{
    public partial class GetDivisionAlphaNum : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        public string[] strDivisions = null;
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in GetDivisionAlphaNum.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1102", "GetDivisionAlphaNum", "");

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
                l.writeToLog("Info : Entered into GetDivisionAlphaNum state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                int errorCount = 0;
                if (Request.QueryString["errorCount"] != null)
                {
                    errorCount = Int16.Parse(Request.QueryString["errorCount"].ToString().Trim());
                }
                l.writeToLog("Info : GetDivisionAlphaNum event " + selection, Logging.eventType.AppInfoEvent);
                switch (selection)
                {
                    case "inputOptions":
                        InputOptions(errorCount);
                        break;
                    case "evalInput":
                        EvalInput(errorCount);
                        break;
                    default:
                        InputOptions(errorCount);
                        break;
                }
                l.writeToLog("Application end in GetDivisionAlphaNum.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in GetDivisionAlphaNum, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from GetDivisionAlphaNum", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_Init", true, "Using yore telephone key pad, please enter the three digit division number found on yore melk check, followed by the pound sign.");
                    pt.addTTSToPrompt("Using " + TTSDictionary("your") + " telephone key pad, please enter the three digit division number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in GetDivisionAlphaNum", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_NM1", true, "Sorry, thatt was an in valid entry. Using yore telephone key pad, please enter the three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an in valid entry. Using " + TTSDictionary("your") + " telephone key pad, please enter the three digit division number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GGetDivisionAlphaNum_NM2", true, "Sorry, thatt was an in valid entry. Using yore telephone key pad, please enter the three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an in valid entry. Using " + TTSDictionary("your") + " telephone key pad, please enter the three digit division number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in GetDivisionAlphaNum", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_NI1", true, "Using yore telephone key pad, please enter the three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Using " + TTSDictionary("your") + " telephone key pad, please enter the three digit division number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_NI2", true, "Sorry, I still did not get that. Using yore telephone key pad, please enter the three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, I still did not get that. Using " + TTSDictionary("your") + " telephone key pad, please enter the three digit division number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                    }
                }

                Input i = new Input("GetDivisionAlphaNumInput", true);
                i.DigitsOnly = true;
                i.MinDigits = 1;
                i.MaxDigits = 3;
                i.MaxTries = 3;
                i.TermChar = '#';
                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "GetDivisionAlphaNum.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "GetDivisionAlphaNum.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "GetDivisionAlphaNum.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "GetDivisionAlphaNum.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "GetDivisionAlphaNum.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "GetDivisionAlphaNum.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                i.setValidationScript(Input.validationType.digits);
                Response.Write(i.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string status = null;
                string input = Request.QueryString["GetDivisionAlphaNumInput"] != null ? Request.QueryString["GetDivisionAlphaNumInput"].ToString().Trim() : "";

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1102", "GetDivisionAlphaNum", input);

                // Grab file check if exists
                string filePath = System.Configuration.ConfigurationManager.AppSettings["DivisionsLocation"] + "DTMFtoALPHA" + System.Configuration.ConfigurationManager.AppSettings["TextFileType"];

                l.writeToLog("-- GetDivisionAlphaNum -- Division File is " + filePath, Logging.eventType.AppInfoEvent);

                if (File.Exists(Server.MapPath(filePath)))
                {
                    l.writeToLog("-- GetDivisionAlphaNum -- File Exists Continue", Logging.eventType.AppInfoEvent);

                    // Open file and read by input
                    List<string> readLines = File.ReadAllLines(Server.MapPath(filePath)).ToList();

                    System.Collections.Hashtable objTable = new System.Collections.Hashtable();

                    foreach (string line in readLines)
                    {
                        string key = line.Split(',')[0];
                        string val = line.Split(',')[1];

                        if (!objTable.ContainsKey(key))
                        {
                            objTable.Add(key, val);
                        }
                    }

                    int iCounter = 0;

                    foreach (var key in objTable.Keys)
                    {
                        if (objTable[key].Equals(input))
                        {
                            iCounter = iCounter + 1;
                        }
                    }

                    if (iCounter > 0)
                    {
                        strDivisions = new string[iCounter];
                        iCounter = 0;

                        foreach (var key in objTable.Keys)
                        {
                            if (objTable[key].Equals(input))
                            {
                                strDivisions[iCounter] = key.ToString();
                                iCounter = iCounter + 1;
                                l.writeToLog("{0}{1}" + key + "," + objTable[key], Logging.eventType.AppInfoEvent);
                            }
                        }
                    }
                }
                else
                {
                    l.writeToLog("-- GetDivisionAlphaNum -- File doesn't exist - WHAT TO DO????", Logging.eventType.AppInfoEvent);
                }

                l.writeToLog("-- DivisionAlphaNum -- User has entered : " + input, Logging.eventType.AppInfoEvent);

                Play p = new Play();
                Prompt pt = new Prompt();

                pt.BargeIn = false;

                if (strDivisions != null)
                {
                    // Single Division Found
                    if (strDivisions.Length == 1)
                    {
                        if (input.Length < 3)
                        {
                            status = CallWebService(strDivisions[0]);
                        }
                        else
                        {
                            status = CallNewWebService(strDivisions[0]);
                        }
                    }
                    // Multiple Divisions Found
                    else if (strDivisions.Length > 1)
                    {
                        status = "multipledivs";
                    }
                }
                else
                {
                    // No Divisions Found less than 3 attempts
                    if (errorCount < 3)
                    {
                        status = "nodivless3";
                    }
                    // No Divisions Found max attempts
                    else if (errorCount > 3)
                    {
                        l.writeToLog("Info : 3 and out in GetDivisionAlphaNum, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                        Play p1 = new Play();
                        p1.NextURL = "TriesExceeded.aspx";
                        p1.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                        p1.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                        p1.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                        l.writeToLog("Info : Exiting from GetDivisionAlphaNum", Logging.eventType.AppInfoEvent);
                        Response.Write(p1.getVXML());
                        return;
                    }
                }

                l.writeToLog("-- Division -- EvalInput status : " + status, Logging.eventType.AppInfoEvent);
                switch (status)
                {
                    case "valid":
                        l.writeToLog("Info : Web Service says division is valid", Logging.eventType.AppInfoEvent);
                        Session["division"] = strDivisions[0];
                        errorCount = 0;
                        p.NextURL = "GetProducerNumber.aspx";
                        break;
                    case "invalid":
                        l.writeToLog("Info : Web Service says division is not valid", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        pt.addTTSToPrompt(input);
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_Msg", true, "is not a valid division number.");
                        pt.addTTSToPrompt("is not a valid division number.");
                        p.NextURL = "GetDivisionAlphaNum.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    case "nodivless3":
                        l.writeToLog("Info : No divisions less than 3 attempts", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        pt.addTTSToPrompt(input);
                        string promptFileA = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionAlphaNum_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFileA, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetDivisionAlphaNum_Msg", true, "is not a valid division number.");
                        pt.addTTSToPrompt("is not a valid division number.");
                        p.NextURL = "GetDivisionAlphaNum.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    case "multipledivs":
                        string strDivsList = "";
                        foreach (string s in strDivisions)
                        {
                            strDivsList = strDivsList + s + "~";
                        }

                        l.writeToLog("-- Division -- strDivsList : " + strDivsList, Logging.eventType.AppInfoEvent);
                        l.writeToLog("-- Division -- divCount : " + strDivisions.Length, Logging.eventType.AppInfoEvent);

                        Session["divisions"] = strDivsList;
                        Session["divCount"] = strDivisions.Length;

                        p.NextURL = "MultipleDivisionsMenu.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString() + "&divisions=" + strDivsList + "&divCount=" + strDivisions.Length;
                        break;
                    default:
                        l.writeToLog("Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppException);
                        p.NextURL = "SystemError.aspx";
                        break;
                }

                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from GetDivisionAlphaNum", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private CatchEvent ErrorHandling(int iCount, string strCatchType, string strNextURL, int errorCount)
        {
            CatchEvent e = new CatchEvent(iCount, strCatchType);

            try
            {
                Prompt pt = new Prompt();
                pt.BargeIn = false;
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                e.Reprompt = false;
                AddSilenceToPrompt(pt);
                e.setPrompt(pt);
                errorCount++;
                if (errorCount == 3)
                {
                    l.writeToLog("Info : 3 and out in GetDivisionAlphaNum, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                }
                if (strCatchType.Equals(CatchEvent.NO_MATCH))
                {
                    if (errorCount == 3)
                    {
                        e.NextURL = "TriesExceeded.aspx";
                    }
                    else
                    {
                        e.NextURL = strNextURL + "&errorCount=" + errorCount.ToString() + "&subevent=" + CatchEvent.NO_MATCH;
                    }
                }
                else
                {
                    if (errorCount == 3)
                    {
                        e.NextURL = "TriesExceeded.aspx";
                    }
                    else
                    {
                        e.NextURL = strNextURL + "&errorCount=" + errorCount.ToString() + "&subevent=" + CatchEvent.NO_INPUT;
                    }
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }

        private string CallWebService(string divisionNumber)
        {
            DerivedDivisionValidationService ws = new DerivedDivisionValidationService();

            try
            {
                l.writeToLog("Info : Calling DivisionValidationService.doesDivisonExist with divisionNumber " + divisionNumber, Logging.eventType.AppInfoEvent);
                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.doesDivisionExist(divisionNumber);
                //string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><DivisionValidation xmlns=\"http://www.atsmilk.com/DivisionValidation\" version=\"1.0\"><DivisionNumber>45</DivisionNumber><DoesDivisionExist>1</DoesDivisionExist></DivisionValidation>";
                l.writeToLog("Info : DivisionValidationService.doesDivisionExist returned " + result, Logging.eventType.AppInfoEvent);
                string exists = null;
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/DivisionValidation");
                XmlNode aNode = aDoc.SelectSingleNode("//a:DoesDivisionExist", nsmgr);
                exists = aNode.InnerText;
                if (exists.Equals("1"))
                    return "valid";
                else
                    return "invalid";
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
            finally
            {
                if (ws != null)
                {
                    ws.Dispose();
                }
            }
        }
        private string CallNewWebService(string divisionNumber)
        {
            Z_WS_DOES_DIVISION_EXISTService newWS = new Z_WS_DOES_DIVISION_EXISTService();

            try
            {
                l.writeToLog("-- Division -- Calling NEW Z_WS_DOES_DIVISION_EXISTService.doesDivisonExist with divisionNumber " + divisionNumber, Logging.eventType.AppInfoEvent);
                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();
                l.writeToLog("-- Division -- Login credentials " + NewUserID + "," + NewPasswd + "," + strWebServiceURL, Logging.eventType.AppInfoEvent);

                newWS.Url = strWebServiceURL;
                newWS.UseDefaultCredentials = false;
                newWS.PreAuthenticate = true;
                l.writeToLog("-- Division -- URL is " + newWS.Url.ToString(), Logging.eventType.AppInfoEvent);
                if (newWS.UseDefaultCredentials == false) l.writeToLog("-- Division -- UseDefaultCredentials is FALSE ", Logging.eventType.AppInfoEvent);
                else l.writeToLog("-- Division -- UseDefaultCredentials is TRUE ", Logging.eventType.AppInfoEvent);

                CredentialCache cred = new CredentialCache();
                NetworkCredential netCred = new NetworkCredential(NewUserID, NewPasswd);
                cred.Add(new Uri(strWebServiceURL), "Basic", netCred);
                newWS.Credentials = cred;
                newWS.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);

                string result = newWS.Z_WS_DOES_DIVISION_EXIST(divisionNumber.Trim()).Trim();
                l.writeToLog("-- Division -- Z_WS_DOES_DIVISION_EXIST returned " + result, Logging.eventType.AppInfoEvent);
                if (result.Equals("1"))
                    return "valid";
                else
                    return "invalid";
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionAlphaNum -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
            finally
            {
                if (newWS != null)
                {
                    newWS.Dispose();
                }
            }
        }
    }
}