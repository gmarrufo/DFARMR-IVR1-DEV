using System;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class NoPinMenu : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in NoPinMenu.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6020", "NoPinMenu", "");

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
                l.writeToLog("-- Password -- Entered into NoPinMenu state", Logging.eventType.AppInfoEvent);
                string selection = "no event";

                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].ToString().Trim();
                }
                l.writeToLog("Info : NoPinMenu event " + selection, Logging.eventType.AppInfoEvent);
                switch (selection)
                {
                    case "inputOptions":
                        InputOptions();
                        break;
                    case "evalInput":
                        EvalInput();
                        break;
                    default:
                        InputOptions();
                        break;
                }
                l.writeToLog("Application end in NoPinMenu.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- NoPinMenu -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions()
        {
            try
            {
                string passwordPrompt = "";
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "NoPinMenu_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    passwordPrompt = "To repeat that, press 1, or press any other key to return to the main menu.";
                    // AddPromptFileOrTTS(pt, "NoPinMenu_Init", true, passwordPrompt);
                    pt.addTTSToPrompt(passwordPrompt);
                }

                Menu aMenu = new Menu("ExpandedMenu");

                string[] arPrompts = new string[4];
                arPrompts[0] = "";
                arPrompts[1] = "";
                arPrompts[2] = "";
                arPrompts[3] = "";

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

                CatchEvent ceNoInput1 = aMenu.getCatchEvent(pNI1, 1, CatchEvent.NO_INPUT, "MainMenu.aspx", true, false);
                CatchEvent ceNoInput2 = aMenu.getCatchEvent(pNI2, 2, CatchEvent.NO_INPUT, "MainMenu.aspx", true, false);
                CatchEvent ceNoInput3 = aMenu.getCatchEvent(pMaxErrorEndCall, 3, CatchEvent.NO_INPUT, "TriesExceeded.aspx", false, true);
                CatchEvent ceNoMatch1 = aMenu.getCatchEvent(pNM1, 1, CatchEvent.NO_MATCH, "MainMenu.aspx", true, false);
                CatchEvent ceNoMatch2 = aMenu.getCatchEvent(pNM2, 2, CatchEvent.NO_MATCH, "MainMenu.aspx", true, false);
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

                aMenu.addMenuChoice("1", "NoPinMenu.aspx?event=evalInput&input=1");

                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- NoPinMenu -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput()
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                l.writeToLog("Info : NoPinMenu User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6020", "NoPinMenu", input);

                Play p = new Play();
                Prompt pt = new Prompt();

                switch (input)
                {
                    case "1":
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "NoPin_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "NoPin_Msg", true, "I am sorry you must first set up PIN number to access payment information. Please contact your council office for assistance.");
                        pt.addTTSToPrompt("I am sorry you must first set up PIN number to access payment information. Please contact " + TTSDictionary("your") + " council office for assistance.");
                        p.setPrompt(pt);
                        p.NextURL = "NoPinMenu.aspx";
                        break;
                    default:
                        p.NextURL = "MainMenu.aspx";
                        break;
                }

                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from NoPinMenu", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- NoPinMenu -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }
    }
}