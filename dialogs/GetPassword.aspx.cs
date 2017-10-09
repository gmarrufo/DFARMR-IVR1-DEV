using System;
using System.Net;
using GVP.MCL.Enhanced;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using DFARMR_IVR1.DOES_PASSWORD_EXIST;

namespace DFARMR_IVR1
{
    public partial class GetPassword : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        TTS breakTimeTTS_150;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in GetPassword.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1300", "GetPassword", "");

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
                        // Log.Write(pageGVPSessionID, "Info : GVP SessionID from QueryString is null");
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
                l.writeToLog("-- Password -- Entered into GetPassword state", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Info : GetPassword event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in GetPassword.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetPassword -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                string divisionNumber = Session["division"].ToString();
                int divNumLen = divisionNumber.Length;
                string promptFile = "";

                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in GetPassword, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from GetPassword", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }

                breakTimeTTS_150 = FixCutoffTTS(150);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);

                if (Request.QueryString["subevent"] == null)
                {
                    promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    if (divNumLen < 3)  // Legacy
                    {
                        // AddPromptFileOrTTS(pt, "GetPassword_Init", true, "Please enter yore 4 digit password followed by the pound sign");
                        // AddPromptFileOrTTS(pt, "GetPassword_Init", true, "Yore default password is the last 4 digits of yore tax i d or social security number.");
                        pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                        pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " " + TTSDictionary("tax id"));
                        pt.addTTSToPrompt(breakTimeTTS_150);
                        pt.addTTSToPrompt("or");
                        pt.addTTSToPrompt(TTSDictionary("social") + " security number.");
                    }
                    else // SAP
                    {
                        // AddPromptFileOrTTS(pt, "GetPassword_Init", true, "Please enter yore 4 digit password followed by the pound sign");
                        // AddPromptFileOrTTS(pt, "GetPassword_Init", true, "Yore default password is the last 4 digits of yore registration number.");
                        pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                        pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " registration number.");
                    }
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in GetPassword", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            if (divNumLen < 3)  // Legacy
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NM1", true, "Sorry, thatt was an in valid entry. Please enter yore 4 digit password followed by the pound sign.");
                                // AddPromptFileOrTTS(pt, "GetPassword_NM1", true, "Yore default password is the last 4 digits of yore tax i d or social security number.");
                                pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign.");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " " + TTSDictionary("tax id"));
                                pt.addTTSToPrompt(breakTimeTTS_150);
                                pt.addTTSToPrompt("or");
                                pt.addTTSToPrompt(TTSDictionary("social") + " security number.");
                            }
                            else // SAP
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NM1", true, "Sorry, thatt was an in valid entry. Please enter yore 4 digit password followed by the pound sign.");
                                // AddPromptFileOrTTS(pt, "GetPassword_NM1", true, "Yore default password is the last 4 digits of yore registration number.");
                                pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign.");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " registration number.");
                            }
                        }
                        else
                        {
                            if (divNumLen < 3)  // Legacy
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NM2", true, "Sorry, thatt was an in valid entry. Please enter yore 4 digit password followed by the pound sign.");
                                // AddPromptFileOrTTS(pt, "GetPassword_NM2", true, "Yore default password is the last 4 digits of yore tax i d or social security number.");
                                pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign.");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " " + TTSDictionary("tax id"));
                                pt.addTTSToPrompt(breakTimeTTS_150);
                                pt.addTTSToPrompt("or");
                                pt.addTTSToPrompt(TTSDictionary("social") + " security number.");
                            }
                            else // SAP
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NM2", true, "Sorry, thatt was an in valid entry. Please enter yore 4 digit password followed by the pound sign.");
                                // AddPromptFileOrTTS(pt, "GetPassword_NM2", true, "Yore default password is the last 4 digits of yore registration number.");
                                pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign.");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " registration number.");
                            }
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in GetPassword", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            if (divNumLen < 3)  // Legacy
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NI1", true, "Please enter yore 4 digit password followed by the pound sign");
                                // AddPromptFileOrTTS(pt, "GetPassword_NI1", true, "Yore default password is the last 4 digits of yore tax i d or social security number.");
                                pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " " + TTSDictionary("tax id"));
                                pt.addTTSToPrompt(breakTimeTTS_150);
                                pt.addTTSToPrompt("or");
                                pt.addTTSToPrompt(TTSDictionary("social") + " security number.");
                            }
                            else // SAP
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NI1", true, "Please enter yore 4 digit password followed by the pound sign");
                                // AddPromptFileOrTTS(pt, "GetPassword_NI1", true, "Yore default password is the last 4 digits of yore registration number.");
                                pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " registration number.");
                            }
                        }
                        else
                        {
                            if (divNumLen < 3)  // Legacy
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NI2", true, "Please enter yore 4 digit password followed by the pound sign");
                                // AddPromptFileOrTTS(pt, "GetPassword_NI2", true, "Yore default password is the last 4 digits of yore tax i d or social security number.");
                                pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " " + TTSDictionary("tax id"));
                                pt.addTTSToPrompt(breakTimeTTS_150);
                                pt.addTTSToPrompt("or");
                                pt.addTTSToPrompt(TTSDictionary("social") + " security number.");
                            }
                            else // SAP
                            {
                                promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "GetPassword_NI2", true, "Please enter yore 4 digit password followed by the pound sign");
                                // AddPromptFileOrTTS(pt, "GetPassword_NI2", true, "Yore default password is the last 4 digits of yore registration number.");
                                pt.addTTSToPrompt("Please enter " + TTSDictionary("your") + " 4 digit password followed by the pound sign");
                                pt.addTTSToPrompt(TTSDictionary("your") + " default password is the last 4 digits of " + TTSDictionary("your") + " registration number.");
                            }
                        }
                    }
                }

                Input i = new Input("GetPasswordInput", true);
                i.DigitsOnly = true;
                i.MinDigits = 1;
                i.MaxDigits = 4;
                i.MaxTries = 3;
                i.TermChar = '#';
                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "GetPassword.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "GetPassword.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "GetPassword.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "GetPassword.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "GetPassword.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "GetPassword.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                i.setValidationScript(Input.validationType.digits);
                Response.Write(i.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetPassword -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string status;
                string divisionNumber = "";
                string producerNumber = "";
                string input = Request.QueryString["GetPasswordInput"] != null ? Request.QueryString["GetPasswordInput"].ToString().Trim() : "";

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1300", "GetPassword", input);

                if (Session["division"] != null && Session["producer"] != null)
                {
                    divisionNumber = Session["division"].ToString();
                    producerNumber = Session["producer"].ToString();
                }

                l.writeToLog("Info : Get Password User has entered : " + input, Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : GetPassword.aspx Division is : " + divisionNumber, Logging.eventType.AppInfoEvent);
                l.writeToLog("Info : GetPassword.aspx Producer is : " + producerNumber, Logging.eventType.AppInfoEvent);

                Play p = new Play();
                Prompt pt = new Prompt();

                if (divisionNumber.Length < 3)
                {
                    status = CallWebService(divisionNumber, producerNumber, input);
                }
                else
                {
                    status = CallNewWebService(divisionNumber, producerNumber, input);
                }

                switch (status)
                {
                    case "valid":
                        l.writeToLog("-- Password -- Web Service says password is valid", Logging.eventType.AppInfoEvent);
                        errorCount = 0;
                        p.NextURL = "MainMenu.aspx";
                        break;
                    case "invalid":
                        l.writeToLog("-- Password -- Web Service says password is not valid", Logging.eventType.AppInfoEvent);
                        errorCount++;
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_Msg1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetPassword_Msg1", true, "I’m sorry. The password you entered does not match whut we have stored for division ");
                        pt.addTTSToPrompt("I’m sorry. The password you entered does not match what we have stored for division ");
                        pt.addTTSToPrompt(Session["division"].ToString());
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetPassword_Msg2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "GetPassword_Msg2", true, "and producer ");
                        pt.addTTSToPrompt("and producer ");

                        //They want it to say the digits individually
                        string producer = Session["producer"].ToString();
                        for (int i = 0, length = producer.Length; i < length; i++)
                        {
                            pt.addTTSToPrompt(producer[i].ToString() + " ");
                        }

                        p.NextURL = "GetPassword.aspx?event=inputOptions" + "&errorCount=" + errorCount.ToString();
                        break;
                    default:
                        l.writeToLog("-- Password -- Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppErrorEvent);
                        p.NextURL = "SystemError.aspx";
                        break;
                }
                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from GetPassword", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetPassword -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("-- Password -- : 3 and out in GetPassword, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- GetPassword -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }
       
        private string CallWebService(string divisionNumber, string producerNumber, string password)
        {
            DerivedProducerPasswordValidationService ws = new DerivedProducerPasswordValidationService();

            try
            {
                l.writeToLog("Info : Calling ProducerPasswordValidationService.doesProducerPasswordExist with divisionNumber " + divisionNumber + " and producerNumber " + producerNumber, Logging.eventType.AppInfoEvent);

                ws.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);
                string result = ws.doesProducerPasswordExist(divisionNumber, producerNumber, password);
                l.writeToLog("Info : ProducerPasswordValidationService.doesProducerPasswordExist returned " + result, Logging.eventType.AppInfoEvent);
                string exists = null;
                XmlDocument aDoc = new XmlDocument();
                aDoc.LoadXml(result);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(aDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.atsmilk.com/ProducerPasswordValidation");
                XmlNode aNode = aDoc.SelectSingleNode("//a:DoesProducerPasswordExist", nsmgr);
                exists = aNode.InnerText;
                if (exists.Equals("1"))
                    return "valid";
                else
                    return "invalid";
            }
            catch (Exception ex)
            {
                l.writeToLog("Error : Got error while accessing ProducerPasswordValidationService.doesProducerPasswordExist web service : " + ex.ToString(), Logging.eventType.AppInfoEvent);
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

        private string CallNewWebService(string divisionNumber, string producerNumber, string password)
        {
            Z_WS_DOES_PASSWORD_EXIST_MSAService newWS = new Z_WS_DOES_PASSWORD_EXIST_MSAService();

            try
            {
                l.writeToLog("-- Password -- Calling Z_WS_DOES_PASSWORD_EXISTService with producerNumber " + producerNumber + " with divisionNumber " + divisionNumber + " with password " + password + ".", Logging.eventType.AppInfoEvent);

                string strWebServiceURL = System.Configuration.ConfigurationManager.AppSettings["NewWebServiceURL"].Trim();
                string NewUserID = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].Trim();
                string NewPasswd = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].Trim();
                l.writeToLog("-- Password -- Login credentials " + NewUserID + "," + NewPasswd, Logging.eventType.AppInfoEvent);

                newWS.Url = strWebServiceURL;
                newWS.UseDefaultCredentials = false;
                newWS.PreAuthenticate = true;
                l.writeToLog("-- Password -- URL is " + newWS.Url.ToString(), Logging.eventType.AppInfoEvent);
                if (newWS.UseDefaultCredentials == false) l.writeToLog("-- Password -- UseDefaultCredentials is FALSE ", Logging.eventType.AppInfoEvent);
                else l.writeToLog("-- Password -- UseDefaultCredentials is TRUE ", Logging.eventType.AppInfoEvent);

                CredentialCache cred = new CredentialCache();
                NetworkCredential netCred = new NetworkCredential(NewUserID, NewPasswd);
                cred.Add(new Uri(strWebServiceURL), "Basic", netCred);
                newWS.Credentials = cred;
                newWS.Timeout = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["WebServicesTimeout"]);

                // Out vars
                string E_PIN = "";
                string E_STATUS = "";

                string result = newWS.Z_WS_DOES_PASSWORD_EXIST_MSA(divisionNumber, password, producerNumber, out E_PIN, out E_STATUS);

                l.writeToLog("-- Password -- Z_WS_DOES_PASSWORD_EXIST returned " + result, Logging.eventType.AppInfoEvent);
                l.writeToLog("-- E_PIN -- is " + E_PIN, Logging.eventType.AppInfoEvent);
                l.writeToLog("-- E_STATUS -- is " + E_STATUS, Logging.eventType.AppInfoEvent);

                if(E_STATUS.Equals("1"))
                {
                    Session["MSA_NUMBER"] = result;
                    Session["passwordResults"] = E_PIN + "~" + E_STATUS;
                    l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : GetPassword.aspx - MSA_NUMBER = " + Session["MSA_NUMBER"].ToString(), Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : GetPassword.aspx - Password Results = " + Session["passwordResults"].ToString(), Logging.eventType.AppInfoEvent);
                    return "valid";
                }
                else
                {
                    l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
                    return "invalid";
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- Password -- Error : Got error while accessing Z_WS_DOES_PASSWORD_EXIST web service : " + ex.ToString(), Logging.eventType.AppInfoEvent);
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