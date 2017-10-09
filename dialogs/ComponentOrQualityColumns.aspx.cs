using System;
using System.Collections.Generic;
using System.Text;
using GVP.MCL.Enhanced;

namespace DFARMR_IVR1
{
    public partial class ComponentOrQualityColumns : BaseDialog
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
                l.writeToLog("Application start in ComponentOrQualityColumns.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4400", "ComponentOrQualityColumns", "");

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
                l.writeToLog("Info : Entered into ComponentOrQualityColumns state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : ComponentOrQualityColumns event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in ComponentOrQualityColumns.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityColumns -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void MenuOptions(int errorCount)
        {
            try
            {
                l.writeToLog("Info : MenuOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                string category = Session["ExpandedCategory"] != null ? (string)Session["ExpandedCategory"] : WebServiceResult.COMPONENT;

                breakTimeTTS = FixCutoffTTS(250);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.addTTSToPrompt(breakTimeTTS);

                if (Request.QueryString["subevent"] != null)
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in ComponentOrQualityColumns", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentOrQualityColumns_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "ComponentOrQualityColumns_NM1", true, "Sorry, thatt was an in valid entry.");
                            pt.addTTSToPrompt("Sorry, that was an in valid entry.");
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentOrQualityColumns_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "ComponentOrQualityColumns_NM2", true, "Sorry, thatt was an in valid entry.");
                            pt.addTTSToPrompt("Sorry, that was an in valid entry.");
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in ComponentOrQualityColumns", Logging.eventType.AppInfoEvent);
                        if (errorCount > 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "ComponentOrQualityColumns_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // AddPromptFileOrTTS(pt, "ComponentOrQualityColumns_NI2", true, "Sorry, I still did not get thatt.");
                            pt.addTTSToPrompt("Sorry, I still did not get that.");
                        }
                    }
                }
                List<string> columnTypes;
                List<string> columnTypesSpeak;
                l.writeToLog("Info : Building types menu for category " + category, Logging.eventType.AppInfoEvent);
                if (category.Equals(WebServiceResult.COMPONENT))
                {
                    columnTypes = (List<string>)Session["ExpandedComponentTypes"];
                    columnTypesSpeak = (List<string>)Session["ExpandedComponentTypesSpeak"];
                }
                else
                {
                    columnTypes = (List<string>)Session["ExpandedQualityTypes"];
                    columnTypesSpeak = (List<string>)Session["ExpandedQualityTypesSpeak"];
                }
                if (columnTypes.Count > 8)
                {
                    l.writeToLog("Warning : More than 8 component or quality types so only listing first 8", Logging.eventType.AppWarningEvent);
                }
                GVP.MCL.Enhanced.Menu aMenu = new GVP.MCL.Enhanced.Menu("ComponentOrQualityColumns");
                int columnCount = columnTypes.Count < 9 ? columnTypes.Count : 8;
                for (int i = 0; i < columnCount; i++)
                {
                    string menuSelection = (i + 1).ToString();
                    StringBuilder promptText = new StringBuilder(50);
                    promptText.Append("For ");

                    string s = columnTypesSpeak[i].ToString();

                    if (s.Contains("PROTEIN"))
                    {
                        s = TTSDictionary(s);
                    }

                    if (s.Contains("INHIBITOR"))
                    {
                        s = TTSDictionary(s);
                    }

                    // promptText.Append(columnTypesSpeak[i].ToString());
                    promptText.Append(s);

                    promptText.Append(" press ");
                    promptText.Append(menuSelection);
                    promptText.Append(". ");
                    l.writeToLog("Info : Adding TTS:" + promptText.ToString(), Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt(promptText.ToString());
                    aMenu.addMenuChoice(menuSelection, "ComponentOrQualityColumns.aspx?event=evalInput&input=" + menuSelection
                                        + "&type=" + columnTypes[i]);
                }
                pt.addTTSToPrompt("To hear all of these, press 9.");
                aMenu.setPrompt(pt);
                aMenu.addMenuChoice("9", "ComponentOrQualityColumns.aspx?event=evalInput&input=9");
                aMenu.addMenuChoice("*", "ComponentOrQualityColumns.aspx?event=evalInput&input=*");

                aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "ComponentOrQualityColumns.aspx?event=menuOptions", errorCount));
                aMenu.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "ComponentOrQualityColumns.aspx?event=menuOptions", errorCount));
                aMenu.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "ComponentOrQualityColumns.aspx?event=menuOptions", errorCount));
                aMenu.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "ComponentOrQualityColumns.aspx?event=menuOptions", errorCount));
                aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                aMenu.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityColumns -- MenuOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].Trim() : "";
                string type = Request.QueryString["type"] != null ? Request.QueryString["type"].Trim() : "";
                l.writeToLog("Info : User has selected : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "4400", "ComponentOrQualityColumns", input);

                Play p = new Play();
                if (input.Equals("*"))
                {
                    l.writeToLog("Info : Going back to ComponentOrQuality", Logging.eventType.AppInfoEvent);
                    p.NextURL = "ComponentOrQuality.aspx";
                }
                else
                {
                    l.writeToLog("Info : Going to ComponentOrQualityPlayback. type=" + type, Logging.eventType.AppInfoEvent);
                    Session["ExpandedType"] = type;
                    Session["ExpandedTypeIndex"] = input;
                    p.NextURL = "ComponentOrQualityPlayback.aspx";
                }
                l.writeToLog("Info : Exiting from ComponentOrQualityColumns", Logging.eventType.AppInfoEvent);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- ComponentOrQualityColumns -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in ComponentOrQualityColumns, going to TriesExceeded", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("-- ComponentOrQualityColumns -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }
    }
}