using System;
using System.Collections.Generic;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class ComponentOrQuality : BaseDialog
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
                l.writeToLog("Application start in ComponentOrQuality.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4200", "ComponentOrQuality", "");

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
                l.writeToLog("-- ComponentOrQuality -- Entered into ComponentOrQuality state", Logging.eventType.AppInfoEvent);

                string selection = "no event";
                if (Request.QueryString["event"] != null)
                {
                    selection = Request.QueryString["event"].Trim();
                }
                int errorCount = 0;
                if (Request.QueryString["errorCount"] != null)
                {
                    errorCount = Int16.Parse(Request.QueryString["errorCount"].Trim());
                }
                l.writeToLog("-- ComponentOrQuality -- ComponentOrQuality event " + selection, Logging.eventType.AppInfoEvent);
                switch (selection)
                {
                    case "menuOptions":
                        MenuOptions(errorCount);
                        break;
                    case "evalInput":
                        EvalInput(errorCount);
                        break;
                    default:
                        List<string> qualityTypes = (List<string>)Session["ExpandedQualityTypes"];
                        if (qualityTypes.Count == 0)
                        {
                            l.writeToLog("Info : Going to ComponentOrQualityColumns to play component results since no quality results exist", Logging.eventType.AppInfoEvent);
                            Play p = new Play();
                            p.NextURL = "ComponentOrQualityColumns.aspx?category=" + WebServiceResult.COMPONENT;
                            l.writeToLog("Info : Exiting from ComponentOrQuality", Logging.eventType.AppInfoEvent);
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                            p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                            Response.Write(p.getVXML());
                        }
                        else
                        {
                            MenuOptions(errorCount);
                        }
                        break;
                }
                l.writeToLog("Application end in ComponentOrQuality.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQuality -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void MenuOptions(int errorCount)
        {
            try
            {
                l.writeToLog("-- ComponentOrQuality -- MenuOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);

                breakTimeTTS = FixCutoffTTS(250);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.addTTSToPrompt(breakTimeTTS);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentorQuality_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("-- ComponentOrQuality -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // pt.addTTSToPrompt("For component test rizuhlts, press 1. For quality test rizuhlts, press 2. To return to the previous menu, press the star kee.");
                    pt.addTTSToPrompt("For component test " + TTSDictionary("results") + ", press 1. For quality test " + TTSDictionary("results") + ", press 2. To return to the previous menu, press the star " + TTSDictionary("key") + ".");
                }

                Menu aMenu = new Menu("ComponentOrQuality");

                string[] arPrompts = new string[4];
                arPrompts[0] = "";
                arPrompts[1] = "Sorry, I still did not get that.";
                // arPrompts[2] = "I'm sorry that I am unable to assist you at this time. Please try " + TTSDictionary("your") + " call later." + TTSDictionary("Goodbye") + ".";
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
                aMenu.addMenuChoice("1", "ComponentOrQuality.aspx?event=evalInput&input=1");
                aMenu.addMenuChoice("2", "ComponentOrQuality.aspx?event=evalInput&input=2");
                aMenu.addMenuChoice("*", "ComponentOrQuality.aspx?event=evalInput&input=*");

                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQuality -- MenuOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].Trim() : "";
                l.writeToLog("-- ComponentOrQuality -- User has selected : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4200", "ComponentOrQuality", input);

                Play p = new Play();
                List<string> columnTypes;
                switch (input)
                {
                    case "1":
                        columnTypes = (List<string>)Session["ExpandedComponentTypes"];
                        if (columnTypes.Count == 0)
                        {
                            l.writeToLog("-- ComponentOrQuality -- No component results found, asking again to see if they want quality results", Logging.eventType.AppInfoEvent);
                            Prompt pt = new Prompt();
                            pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                            p.NextURL = "ComponentOrQuality.aspx";
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentorQuality_Msg1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("-- ComponentOrQuality -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "ComponentorQuality_Msg1", true, "No component test rizuhlts were found for that time fraym.");
                            pt.addTTSToPrompt("No component test " + TTSDictionary("results") + " were found for that time " + TTSDictionary("frame") + ".");
                            p.setPrompt(pt);
                        }
                        else
                        {
                            l.writeToLog("-- ComponentOrQuality -- Going to ComponentOrQualityColumns to play component results", Logging.eventType.AppInfoEvent);
                            Session["ExpandedCategory"] = WebServiceResult.COMPONENT;
                            p.NextURL = "ComponentOrQualityColumns.aspx";
                        }
                        break;
                    case "2":
                        columnTypes = (List<string>)Session["ExpandedQualityTypes"];
                        if (columnTypes.Count == 0)
                        {
                            l.writeToLog("-- ComponentOrQuality -- No quality results found, asking again to see if they want component results", Logging.eventType.AppInfoEvent);
                            Prompt pt = new Prompt();
                            pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                            p.NextURL = "ComponentOrQuality.aspx";
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentorQuality_Msg2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("-- ComponentOrQuality -- Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "ComponentorQuality_Msg2", true, "No quality test rizuhlts were found for that time fraym.");
                            pt.addTTSToPrompt("No quality test " + TTSDictionary("results") + " were found for that time " + TTSDictionary("frame") + ".");
                            p.setPrompt(pt);
                        }
                        else
                        {
                            l.writeToLog("-- ComponentOrQuality -- Going to ComponentOrQualityColumns to play quality results", Logging.eventType.AppInfoEvent);
                            Session["ExpandedCategory"] = WebServiceResult.QUALITY;
                            p.NextURL = "ComponentOrQualityColumns.aspx";
                        }
                        break;
                    case "*":
                        string previousMenu = Session["previousMenu"] != null ? Session["previousMenu"].ToString() : "ExpandedMenu.aspx";
                        l.writeToLog("Info : Going to " + previousMenu, Logging.eventType.AppInfoEvent);
                        p.NextURL = previousMenu;
                        break;
                    default:
                        p.NextURL = "ComponentOrQuality.aspx?event=menuOptions&subevent=NO_MATCH&errorCount=" + (errorCount + 1).ToString();
                        break;
                }
                l.writeToLog("-- ComponentOrQuality -- Exiting from ComponentOrQuality", Logging.eventType.AppInfoEvent);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQuality -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("-- ComponentOrQuality -- 3 and out in ComponentOrQuality, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- ComponentOrQuality -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }
    }
}