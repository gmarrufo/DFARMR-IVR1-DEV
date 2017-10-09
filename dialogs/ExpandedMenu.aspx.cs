using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class ExpandedMenu : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        TTS breakTimeTTS;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in ExpandedMenu.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4000", "ExpandedMenu", "");

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
                    l.writeToLog("Info : GVP SessionId : " + pageGVPSessionID, Logging.eventType.AppInfoEvent);
                }
                else //Getting GVP SessionId from QueryString
                {
                    l.writeToLog("Info : Found  GVPSessionId as null from session object", Logging.eventType.AppInfoEvent);
                    if (Request.QueryString["SESSIONID"] != null)
                    {
                        l.writeToLog("Info : Getting GVP SessionID from QueryString", Logging.eventType.AppInfoEvent);
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
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }
                else //Getting IIS SessionId from Session.SessionID
                {
                    l.writeToLog("Info : Found  IISSessionID as null from session object", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : Getting IISSessionID from Session.SessionID", Logging.eventType.AppInfoEvent);
                    pageIISSessionID = Session.SessionID;
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }
                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("Info : Entered into ExpandedMenu state", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Info : ExpandedMenu event " + selection, Logging.eventType.AppInfoEvent);
                switch (selection)
                {
                    case "menuOptions":
                        MenuOptions(errorCount);
                        break;
                    case "evalInput":
                        EvalInput(errorCount);
                        break;
                    default:
                        MenuOptions(errorCount);
                        break;
                }
                l.writeToLog("Application end in ExpandedMenu.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ExpandedMenu -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void MenuOptions(int errorCount)
        {
            try
            {
                l.writeToLog("Info : MenuOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);

                breakTimeTTS = FixCutoffTTS(250);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.addTTSToPrompt(breakTimeTTS);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ExpandedMenu_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // pt.addTTSToPrompt("For the most current test rizuhlts, press 1. To listen to a specific months test rizuhlts, press 2. To return to the main menu, press the star kee.");
                    pt.addTTSToPrompt("For the most current test " + TTSDictionary("results") + ", press 1. To listen to a specific months test " + TTSDictionary("results") + ", press 2. To return to the main menu, press the star " + TTSDictionary("key") + ".");
                }

                Menu aMenu = new Menu("ExpandedMenu");

                string[] arPrompts = new string[4];
                arPrompts[0] = "";
                arPrompts[1] = "Sorry, I still did not get that.";
                // arPrompts[2] = "I'm sorry that I am unable to assist you at this time. Please try your call later. Good byee.";
                arPrompts[2] = "";
                arPrompts[3] = "Sorry, that was an in valid entry.";

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
                aMenu.setPrompt(pt);
                aMenu.MaxTries = 3;
                aMenu.addMenuChoice("1", "ExpandedMenu.aspx?event=evalInput&input=1");
                aMenu.addMenuChoice("2", "ExpandedMenu.aspx?event=evalInput&input=2");
                aMenu.addMenuChoice("*", "ExpandedMenu.aspx?event=evalInput&input=*");

                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ExpandedMenu -- MenuOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                l.writeToLog("Info : User has selected : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4000", "ExpandedMenu", input);

                Play p = new Play();
                bool addPleaseWaitPrompt = false;
                switch (input)
                {
                    case "1":
                        l.writeToLog("Info : Going to Expanded Web Service Lookup", Logging.eventType.AppInfoEvent);
                        addPleaseWaitPrompt = true;
                        Session["previousMenu"] = "ExpandedMenu.aspx";
                        p.NextURL = "ExpandedWebServiceLookup.aspx";
                        break;
                    case "2":
                        l.writeToLog("Info : Going to Get Month", Logging.eventType.AppInfoEvent);
                        p.NextURL = "GetMonth.aspx";
                        break;
                    case "*":
                        l.writeToLog("Info : Going to Main Menu", Logging.eventType.AppInfoEvent);
                        p.NextURL = "MainMenu.aspx";
                        break;
                    default:
                        p.NextURL = "ExpandedMenu.aspx?event=menuOptions&subevent=NO_MATCH&errorCount=" + (errorCount + 1).ToString();
                        break;
                }
                if (addPleaseWaitPrompt)
                {
                    Prompt pt = new Prompt();
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ExpandedMenu_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "ExpandedMenu_Msg", true, "Please wait whyle we retrieve yore information.");
                    pt.addTTSToPrompt("Please wait while we " + TTSDictionary("retrieve") + " " + TTSDictionary("your") + " information.");
                    p.setPrompt(pt);
                }
                l.writeToLog("Info : Exiting from ExpandedMenu", Logging.eventType.AppInfoEvent);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ExpandedMenu -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in ExpandedMenu, going to TriesExceeded", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("-- ExpandedMenu -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }
    }
}