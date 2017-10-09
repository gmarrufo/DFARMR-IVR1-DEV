using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using GVP.MCL.Enhanced;
using DFARMR_IVR1.NewDoesDivisionExist.WebReference;

namespace DFARMR_IVR1
{
    public partial class GetDivisionNumber : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        public string[] strDivisions = null;
        Log l = new Log();
        TTS breakTimeTTS_150 = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in GetDivisionNumber.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1100", "GetDivisionNumber", "");

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
                l.writeToLog("Info : Entered into GetDivisionNumber state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : GetDivisionNumber event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in GetDivisionNumber.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionNumber -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                breakTimeTTS_150 = FixCutoffTTS(150);

                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in GetDivisionNumber, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from GetDivisionNumber", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);

                    // AddPromptFileOrTTS(pt, "GetDivisionNumber_Init", true, "If yore division number contains both letters and numbers press the star key noww; otherwise, please enter the  two ore three digit division number found on yore melk check, followed by the pound sign.");
                    pt.addTTSToPrompt("If your division number contains both letters and numbers press the star !!\\pause=75\\ key now.");
                    pt.addTTSToPrompt(breakTimeTTS_150);
                    pt.addTTSToPrompt("otherwise, please enter the two");
                   // pt.addTTSToPrompt(breakTimeTTS_150);
                    pt.addTTSToPrompt("or");
                   // pt.addTTSToPrompt(breakTimeTTS_150);
                    pt.addTTSToPrompt("three digit division number found on your milk check, followed by the pound sign.");
                    pt.addTTSToPrompt(breakTimeTTS_150);
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in GetDivisionNumber", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionNumber_NM1", true, "Sorry, thatt was an in valid entry. If yore division number contains both letters and numbers press the star key noww; otherwise, please enter the two ore three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. If your division number contains both letters and numbers press the star !!\\pause=75\\ key now.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("otherwise, please enter the two");
                        //    pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("or");
                        //    pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("three digit division number found on your milk check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionNumber_NM2", true, "Sorry, thatt was an in valid entry. If yore division number contains both letters and numbers press the star key noww; otherwise, please enter the two ore three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. If your division number contains both letters and numbers press the star !!\\pause=75\\ key now.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("otherwise, please enter the two");
                   //         pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("or");
                    //        pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("three digit division number found on your milk check, followed by the pound sign.");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in GetDivisionNumber", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionNumber_NI1", true, "If yore division number contains both letters and numbers press the star key noww; otherwise, please enter the two ore three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("If your division number contains both letters and numbers press the star !!\\pause=75\\ key now.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("otherwise, please enter the two");
                       //     pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("or");
                       //     pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("three digit division number found on your milk check, followed by the pound sign.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetDivisionNumber_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetDivisionNumber_NI2", true, "Sorry, I still did not get that. If yore division number contains both letters and numbers press the star key noww; otherwise, please enter the two ore three digit division number found on yore melk check, followed by the pound sign.");
                            pt.addTTSToPrompt("Sorry, I still did not get that. If your division number contains both letters and numbers press the star !!\\pause=75\\ key " + TTSDictionary("now"));
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("otherwise, please enter the two");
                        //    pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("or");
                        //    pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("three digit division number found on your milk check, followed by the pound sign.");
                        }
                    }
                }

                Input i = new Input("GetDivisionNumberInput", true);

                /*
                i.DigitsOnly = true;
                i.MinDigits = 1;
                i.MaxDigits = 3;
                */

                i.MaxTries = 3;
                i.TermChar = '#';

                // i.setDTMFGrammar("builtin:dtmf/digits");
                i.setDTMFGrammar("~/../../MediaFiles/grammars/en/digitsStarDTMF.grxml");

                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "GetDivisionNumber.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "GetDivisionNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "GetDivisionNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "GetDivisionNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "GetDivisionNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "GetDivisionNumber.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(i.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionNumber -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string status = "";
                string input = Request.QueryString["GetDivisionNumberInput"] != null ? Request.QueryString["GetDivisionNumberInput"].ToString().Trim() : "";
                l.writeToLog("-- Division -- User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "1100", "GetDivisionNumber", input);

                Play p = new Play();
                Prompt pt = new Prompt();
                pt.BargeIn = false;

                if (input.Length == 1 && input.Equals("*"))
                {
                    status = "alphanum";
                }
                else if (input.Length < 3)
                {
                    status = CallWebService(input);
                }
                else
                {
                    status = CallNewWebService(input);
                }

                l.writeToLog("-- GetDivisionNumber -- EvalInput status : " + status, Logging.eventType.AppInfoEvent);

                switch (status)
                {
                    case "valid":
                        l.writeToLog("Info : Web Service says division is valid", Logging.eventType.AppInfoEvent);
                        Session["division"] = input;
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
                    default:
                        l.writeToLog("Error : Web Service error. Going to SystemError.aspx", Logging.eventType.AppLogAlwaysEvent);
                        p.NextURL = "SystemError.aspx";
                        break;
                }

                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from GetDivisionNumber", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetDivisionNumber -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                l.writeToLog("-- GetDivisionNumber -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                l.writeToLog("-- GetDivisionNumber -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                l.writeToLog("-- GetDivisionNumber -- CallNewWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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