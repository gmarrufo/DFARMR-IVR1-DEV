using System;
using System.Net;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewDoesProducerExist.WebReference;

namespace DFARMR_IVR1
{
    public partial class GetProducerNumber : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in GetProducerNumber.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1200", "GetProducerNumber", "");

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
                        //  Log.Write(pageGVPSessionID, "Info : GVP SessionID from QueryString is null");
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
                l.writeToLog("-- Producer -- Entered into GetProducerNumber state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                int errorCount = 0;
                if (Request.QueryString["errorCount"] != null)
                {
                    errorCount = Int16.Parse(Request.QueryString["errorCount"].ToString().Trim());
                    l.writeToLog("GetProducerNumber - ErrorCount : " + errorCount, Logging.eventType.AppInfoEvent);
                }

                l.writeToLog("Info : GetProducerNumber event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in GetProducerNumber.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetProducerNumber -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in GetProducerNumber, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from GetProducerNumber", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }

                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt("Please enter the producer number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in GetProducerNumber", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetProducerNumber_NM1", true, "Sorry, thatt was an in valid entry. Please enter the producer number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter the producer number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetProducerNumber_NM2", true, "Sorry, thatt was an in valid entry. Please enter the producer number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter the producer number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in GetProducerNumber", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetProducerNumber_NI1", true, "Please enter the producer number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Please enter the producer number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetProducerNumber_NI2", true, "Sorry, I still did not get that. Please enter the producer number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, I still did not get that. Please enter the producer number found on " + TTSDictionary("your") + " " + TTSDictionary("milk") + " check, followed by the pound sign.");
                        }
                    }
                }

                Input i = new Input("GetProducerNumberInput", true);
                i.DigitsOnly = true;
                i.MinDigits = 1;
                i.MaxDigits = 7;
                i.MaxTries = 3;
                i.TermChar = '#';
                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "GetProducerNumber.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "GetProducerNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "GetProducerNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "GetProducerNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "GetProducerNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "GetProducerNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                i.setValidationScript(Input.validationType.digits);
                Response.Write(i.getVXML());
                l.writeToLog(i.getVXML(), Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetProducerNumber -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string status;
                string input = Request.QueryString["GetProducerNumberInput"] != null ? Request.QueryString["GetProducerNumberInput"].ToString().Trim() : "";
                string divisionNumber = Session["division"].ToString();

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1200", "GetProducerNumber", input);

                l.writeToLog("Info : Division is " + divisionNumber + "User has entered : " + input, Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : Division User has entered : " + input, Logging.eventType.AppInfoEvent);
                Play p = new Play();
                Prompt pt = new Prompt();
                if (divisionNumber.Length == 3)
                {
                    status = CallNewWebService(divisionNumber, input);
                }
                else
                {
                    status = CallWebService(divisionNumber, input);
                }
                switch (status)
                {
                    case "valid":
                        l.writeToLog("Info : Web Service says producer is valid", Logging.eventType.AppInfoEvent);
                        Session["producer"] = input;
                        errorCount = 0;
                        p.NextURL = "GetPassword.aspx";
                        break;
                    case "invalid":
                        l.writeToLog("Info : Web Service says producer is not valid", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        // They want it to say the digits individually
                        for (int i = 0, length = input.Length; i < length; i++)
                        {
                            pt.addTTSToPrompt(input[i].ToString() + " ");
                        }
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetProducerNumber_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetProducerNumber_Msg", true, "is not a valid producer number for division ");
                        pt.addTTSToPrompt("is not a valid producer number for division ");
                        pt.addTTSToPrompt(divisionNumber);
                        p.NextURL = "GetProducerNumber.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
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
                l.writeToLog("Info : Exiting from GetProducerNumber", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetProducerNumber -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in GetProducerNumber, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- GetProducerNumber -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }

        private string CallWebService(string divisionNumber, string producerNumber)
        {
            DerivedProducerValidationService ws = new DerivedProducerValidationService();

            try
            {
                l.writeToLog("Info : Calling ProducerValidationService.doesProducerExist with divisionNumber " + divisionNumber + " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);
                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.doesProducerExist(divisionNumber, producerNumber);
                // string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ProducerValidation xmlns=\"http://www.atsmilk.com/ProducerValidation\" version=\"1.0\"><DivisionNumber>01</DivisionNumber><ProducerNumber>000100</ProducerNumber><DoesProducerExist>1</DoesProducerExist></ProducerValidation>";
                l.writeToLog("Info : ProducerValidationService.doesProducerExist returned " + result, Logging.eventType.AppInfoEvent);
                string exists = null;
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/ProducerValidation");
                XmlNode aNode = aDoc.SelectSingleNode("//a:DoesProducerExist", nsmgr);
                exists = aNode.InnerText;
                if (exists.Equals("1"))
                    return "valid";
                else
                    return "invalid";
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetProducerNumber -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
        private string CallNewWebService(string divisionNumber, string producerNumber)
        {
            Z_WS_DOES_PRODUCER_EXISTService newWS = new Z_WS_DOES_PRODUCER_EXISTService();

            try
            {
                l.writeToLog("-- Producer -- Calling Z_WS_DOES_PRODUCER_EXISTService with producerNumber " + producerNumber + " with divisionNumber " + divisionNumber, Logging.eventType.AppInfoEvent);
                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();
                l.writeToLog("-- Producer -- Login credentials " + NewUserID + "," + NewPasswd, Logging.eventType.AppInfoEvent);

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

                string result = newWS.Z_WS_DOES_PRODUCER_EXIST(divisionNumber, producerNumber).Trim();

                l.writeToLog("-- Producer -- NEW Z_WS_DOES_PRODUCER_EXISTService.doesDivisonExist returned " + result, Logging.eventType.AppInfoEvent);

                if (result.Equals("1"))
                    return "valid";
                else
                    return "invalid";
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetProducerNumber -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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