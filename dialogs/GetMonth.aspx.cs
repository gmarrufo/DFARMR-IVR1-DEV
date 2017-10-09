using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class GetMonth : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        TTS breakTimeTTS_150 = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in GetMonth.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2220", "CollectMonth", "");

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
                l.writeToLog("Info : Entered into GetMonth state", Logging.eventType.AppInfoEvent);
                l.writeToLog("-- Get Month --", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : GetMonth event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in GetMonth.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetMonth -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                breakTimeTTS_150 = FixCutoffTTS(150);

                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "GetMonth_Init", true, "Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star kee.");
                    pt.addTTSToPrompt("Please enter the 2 digit month using the keypad.");
                    pt.addTTSToPrompt(breakTimeTTS_150);
                    pt.addTTSToPrompt("For example, March would be entered as zero three. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in GetMonth", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetMonth_NM1", true, "Sorry, thatt was an in valid entry. Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star kee.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter the 2 digit month using the keypad.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("For example, March would be entered as zero three. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetMonth_NM2", true, "Sorry, thatt was an in valid entry. Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star kee.");
                            pt.addTTSToPrompt("Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in GetMonth", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetMonth_NI1", true, "Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star kee.");
                            pt.addTTSToPrompt("Please enter the 2 digit month using the keypad.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("For example, March would be entered as zero three. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "GetMonth_NI2", true, "Sorry, I still did not get that. Please enter the 2 digit month using the keypad. For example, March would be entered as zero three. To return to the previous menu, press the star kee.");
                            pt.addTTSToPrompt("Sorry, I still did not get that. Please enter the 2 digit month using the keypad.");
                            pt.addTTSToPrompt(breakTimeTTS_150);
                            pt.addTTSToPrompt("For example, March would be entered as zero three. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                        }
                    }
                }

                Input i = new Input("GetMonthInput", true);

                /*
                i.DigitsOnly = true;
                i.MinDigits = 2;
                i.MaxDigits = 2;
                */

                i.MaxTries = 3;
                i.TermChar = '#';

                i.setDTMFGrammar("~/../../MediaFiles/grammars/en/2digitMonthStarDTMF.grxml");

                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "GetMonth.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "GetMonth.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "GetMonth.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "GetMonth.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "GetMonth.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "GetMonth.aspx?event=inputOptions", errorCount));
                // i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));

                // string removeAdditionOfNuanceDigitsGrammar = i.getVXML();
                // int removalIndex = removeAdditionOfNuanceDigitsGrammar.IndexOf("type=\"digits?minlength=0;maxlength=-1\"");
                // removeAdditionOfNuanceDigitsGrammar = removeAdditionOfNuanceDigitsGrammar.Remove(removalIndex, 38);

                // i.setValidationScript(Input.validationType.digits);

                Response.Write(i.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetMonth -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["GetMonthInput"] != null ? Request.QueryString["GetMonthInput"].ToString().Trim() : "";
                l.writeToLog("Info : User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2220", "CollectMonth", input);

                Play p = new Play();
                if (input.Equals("*"))
                {
                    p.NextURL = "ExpandedMenu.aspx";
                }
                else
                {
                    Session["previousMenu"] = "GetMonth.aspx";
                    p.NextURL = "ExpandedWebServiceLookup.aspx?month=" + input;
                    Prompt pt = new Prompt();
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "GetMonth_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "GetMonth_Msg", true, "Please wait whyle we retrieve yore information.");
                    pt.addTTSToPrompt("Please wait while we " + TTSDictionary("retrieve") + " " + TTSDictionary("your") + " information.");
                    p.setPrompt(pt);
                }
             //   p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from GetMonth", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- GetMonth -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in GetMonth, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- GetMonth -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }
    }
}