using System;
using System.Net;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewDoesDivisionExist.WebReference;

namespace DFARMR_IVR1
{
    public partial class MultipleDivisionsMenu : BaseDialog
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
                l.writeToLog("Application start in MultipleDivisionsMenu.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1108", "MultipleDivisionsMenu", "");

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
                l.writeToLog("Info : Entered into MultipleDivisionsMenu state", Logging.eventType.AppInfoEvent);

                string selection = "no event";

                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                int errorCount = 0;
                string divisions = "";
                string divCount = "";
                if (Request.QueryString["errorCount"] != null)
                {
                    errorCount = Int16.Parse(Request.QueryString["errorCount"].ToString().Trim());
                }
                if (Request.QueryString["divisions"] != null)
                {
                    divisions = Request.QueryString["divisions"].ToString().Trim();
                }
                if (Request.QueryString["divCount"] != null)
                {
                    divCount = Request.QueryString["divCount"].ToString().Trim();
                }

                l.writeToLog("Info : MultipleDivisionsMenu event " + selection, Logging.eventType.AppInfoEvent);
                switch (selection)
                {
                    case "inputOptions":
                        InputOptions(errorCount);
                        break;
                    case "evalInput":
                        EvalInput(errorCount, divisions, divCount);
                        break;
                    default:
                        InputOptions(errorCount);
                        break;
                }
                l.writeToLog("Application end in MultipleDivisionsMenu.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MultipleDivisionMenu -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in MultipleDivisionsMenu, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from MultipleDivisionsMenu", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Init", true, "If you are calling for division number...");
                    pt.addTTSToPrompt("If you are calling for division number...");
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in MultipleDivisionsMenu", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_NM1", true, "Sorry, thatt was an in valid entry.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_NM2", true, "Sorry, thatt was an in valid entry");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in MultipleDivisionsMenu", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_NI1", true, "Sorry, thatt was an in valid entry.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_NI2", true, "Sorry, I still did not get that.");
                            pt.addTTSToPrompt("Sorry, I still did not get that.");
                        }
                    }
                }

                string sGetDivs = Session["divisions"].ToString();
                string sDivs = Session["divCount"].ToString();

                l.writeToLog(sGetDivs, Logging.eventType.AppInfoEvent);

                if (sGetDivs != null && sDivs != null)
                {
                    string[] strDivisions = new string[Int32.Parse(sDivs)];
                    strDivisions = sGetDivs.Split('~');

                    if (strDivisions.Length > 0 && !strDivisions[0].Equals(""))
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt1", true, strDivisions[0].ToString() + "... Press 1.");
                        pt.addTTSToPrompt(strDivisions[0].ToString() + "... Press 1.");
                    }

                    if (strDivisions.Length > 1 && !strDivisions[1].Equals(""))
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt2", true, "...For division number" + strDivisions[1].ToString() + "... Press 2");
                        pt.addTTSToPrompt("...For division number" + strDivisions[1].ToString() + "... Press 2");
                    }

                    if (strDivisions.Length > 2 && !strDivisions[2].Equals(""))
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt3", true, "...For division number" + strDivisions[2].ToString() + "... Press 3");
                        pt.addTTSToPrompt("...For division number" + strDivisions[2].ToString() + "... Press 3");
                    }

                    if (strDivisions.Length > 3 && !strDivisions[3].Equals(""))
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt4" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt4", true, "...For division number" + strDivisions[3].ToString() + "... Press 4");
                        pt.addTTSToPrompt("...For division number" + strDivisions[3].ToString() + "... Press 4");
                    }

                    if (strDivisions.Length > 4 && !strDivisions[4].Equals(""))
                    {
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt5" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt5", true, "...For division number" + strDivisions[4].ToString() + "... Press 5");
                        pt.addTTSToPrompt("...For division number" + strDivisions[4].ToString() + "... Press 5");
                    }

                    string promptFile9 = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_Opt9" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile9, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_Opt9", true, "...If your division number was not listed, press 9.");
                    pt.addTTSToPrompt("...If " + TTSDictionary("your") + " division number was not listed, press 9.");

                    Input i = new Input("MultipleDivisionsMenuInput", true);
                    i.DigitsOnly = true;
                    i.MinDigits = 1;
                    i.MaxDigits = 3;
                    i.MaxTries = 3;
                    i.TermChar = '#';
                    i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                    i.NextURL = "MultipleDivisionsMenu.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString() + "&divisions=" + sGetDivs + "&divCount=" + strDivisions.Length;
                    i.setPrompt(pt);
                    i.defaultconfirmation = Input.confirmationMode.never;
                    i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "MultipleDivisionsMenu.aspx?event=inputOptions", errorCount));
                    i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "MultipleDivisionsMenu.aspx?event=inputOptions", errorCount));
                    i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "MultipleDivisionsMenu.aspx?event=inputOptions", errorCount));
                    i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "MultipleDivisionsMenu.aspx?event=inputOptions", errorCount));
                    i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "MultipleDivisionsMenu.aspx?event=inputOptions", errorCount));
                    i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    i.setValidationScript(Input.validationType.digits);
                    Response.Write(i.getVXML());
                }
                else
                {
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
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MultipleDivisionMenu -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount, string divisions, string divCount)
        {
            try
            {
                l.writeToLog("Inside Eval Input", Logging.eventType.AppInfoEvent);
                l.writeToLog("********" + divisions + "********" + divCount, Logging.eventType.AppInfoEvent);

                string status = null;
                string input = Request.QueryString["MultipleDivisionsMenuInput"] != null ? Request.QueryString["MultipleDivisionsMenuInput"].ToString().Trim() : "";

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1108", "MultipleDivisionsMenu", input);

                string sGetDivs = Session["divisions"].ToString();
                string sDivs = Session["divCount"].ToString();

                string selectedDivision = "";

                l.writeToLog(sGetDivs, Logging.eventType.AppInfoEvent);
                string[] strDivisions = new string[Int32.Parse(sDivs)];
                strDivisions = sGetDivs.Split('~');

                l.writeToLog("-- Division -- User has entered : " + input, Logging.eventType.AppInfoEvent);

                Play p = new Play();
                Prompt pt = new Prompt();

                if (input.Equals("1"))
                {
                    status = CallNewWebService(strDivisions[0]);
                    selectedDivision = strDivisions[0];
                }
                else if (input.Equals("2"))
                {
                    status = CallNewWebService(strDivisions[1]);
                    selectedDivision = strDivisions[1];
                }
                else if (input.Equals("3"))
                {
                    status = CallNewWebService(strDivisions[2]);
                    selectedDivision = strDivisions[2];
                }
                else if (input.Equals("4"))
                {
                    status = CallNewWebService(strDivisions[3]);
                    selectedDivision = strDivisions[3];
                }
                else if (input.Equals("5"))
                {
                    status = CallNewWebService(strDivisions[4]);
                    selectedDivision = strDivisions[4];
                }
                else if (input.Equals("9"))
                {
                    status = "alphanum";
                }
                else if (input.Equals("#") || input.Equals("") || input.Equals("6") || input.Equals("7") || input.Equals("8"))
                {
                    status = "noinput_nomatch";
                }
                else
                {
                    status = "noinput_nomatch";
                }

                l.writeToLog("-- Division -- EvalInput status : " + status, Logging.eventType.AppInfoEvent);
                switch (status)
                {
                    case "valid":
                        l.writeToLog("Info : Web Service says division is valid", Logging.eventType.AppInfoEvent);
                        Session["division"] = selectedDivision;
                        errorCount = 0;
                        p.NextURL = "GetProducerNumber.aspx";
                        break;
                    case "invalid":
                        l.writeToLog("Info : Web Service says division is not valid", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        pt.addTTSToPrompt(input);
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetDivisionNumber_Msg", true, "is not a valid division number.");
                        pt.addTTSToPrompt("is not a valid division number.");
                        p.NextURL = "GetDivisionNumber.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    case "alphanum":
                        p.NextURL = "GetDivisionAlphaNum.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    case "noinput_nomatch":
                        l.writeToLog("Info : User entered an invalid value", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        pt.addTTSToPrompt(input);
                        string promptFile1 = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MultipleDivisionsMenu_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile1, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "MultipleDivisionsMenu_NM1", true, "Sorry, thatt was an in valid entry.");
                        pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry.");
                        p.NextURL = "GetDivisionAlphaNum.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    default:
                        l.writeToLog("Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppInfoEvent);
                        p.NextURL = "SystemError.aspx";
                        break;
                }
                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from MultipleDivisionsMenu", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MultipleDivisionMenu -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in GetDivisionNumber, going to TriesExceeded", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("-- MultipleDivisionMenu -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                // string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><DivisionValidation xmlns=\"http://www.atsmilk.com/DivisionValidation\" version=\"1.0\"><DivisionNumber>45</DivisionNumber><DoesDivisionExist>1</DoesDivisionExist></DivisionValidation>";
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
                l.writeToLog("-- MultipleDivisionMenu -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                l.writeToLog("-- MultipleDivisionMenu -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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