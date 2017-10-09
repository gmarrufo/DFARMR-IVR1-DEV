using System;
using GVP.MCL.Enhanced;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace DFARMR_IVR1
{
    public partial class SelectPayeeMenu1 : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        PayeesResultsCollection pRC;
        public int payeeLookUpNumber = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in SelectPayeeMenu1.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6100", "SelectPayeeMenu1", "");

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

                // Get Div Payees
                CallPayeeLookUpWebService(Session["MSA_NUMBER"].ToString());

                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("-- Password -- Entered into SelectPayeeMenu1 state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : SelectPayeeMenu1 event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in SelectPayeeMenu1.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- SelectPayeeMenu1 -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                string passwordPrompt = "";
                string promptFile = "";

                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in SelectPayeeMenu1, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from SelectPayeeMenu1", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }

                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                pt.addTTSToPrompt("If you are calling about payment information for...");
                Menu aMenu = new Menu("SelectPayeeMenu1");

                string[] arPrompts = new string[4];
                arPrompts[0] = "Sorry, I did not get that.";
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

                if (Request.QueryString["subevent"] == null)
                {
                    promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);

                    payeeLookUpNumber = pRC.Payees.Count;
                    l.writeToLog("Info : SelectPayeeMenu1 Input Options - payeeLookUpNumber = " + payeeLookUpNumber.ToString(), Logging.eventType.AppInfoEvent);

                   // if (payeeLookUpNumber < 7)
                   // {
                   //     passwordPrompt = "If you are calling about payment information for...";
                   //     pt.addTTSToPrompt(passwordPrompt);
                   // }

                    if (payeeLookUpNumber == 1)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                    }

                    if (payeeLookUpNumber == 2)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                    }

                    if (payeeLookUpNumber == 3)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[2].PayeeName.ToString() + "... Press 3");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "SelectPayeeMenu1.aspx?event=evalInput&input=3");
                    }

                    if (payeeLookUpNumber == 4)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[2].PayeeName.ToString() + "... Press 3");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt4" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[3].PayeeName.ToString() + "... Press 4");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "SelectPayeeMenu1.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "SelectPayeeMenu1.aspx?event=evalInput&input=4");
                    }

                    if (payeeLookUpNumber == 5)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[2].PayeeName.ToString() + "... Press 3");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt4" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[3].PayeeName.ToString() + "... Press 4");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt5" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[4].PayeeName.ToString() + "... Press 5");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "SelectPayeeMenu1.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "SelectPayeeMenu1.aspx?event=evalInput&input=4");
                        aMenu.addMenuChoice("5", "SelectPayeeMenu1.aspx?event=evalInput&input=5");
                    }

                    if (payeeLookUpNumber == 6)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[2].PayeeName.ToString() + "... Press 3");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt4" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[3].PayeeName.ToString() + "... Press 4");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt5" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[4].PayeeName.ToString() + "... Press 5");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt6" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[5].PayeeName.ToString() + "... Press 6");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "SelectPayeeMenu1.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "SelectPayeeMenu1.aspx?event=evalInput&input=4");
                        aMenu.addMenuChoice("5", "SelectPayeeMenu1.aspx?event=evalInput&input=5");
                        aMenu.addMenuChoice("6", "SelectPayeeMenu1.aspx?event=evalInput&input=6");
                    }

                    if (payeeLookUpNumber > 6)
                    {
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt8" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(pRC.Payees[0].PayeeName.ToString() + "... Press 1.");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[1].PayeeName.ToString() + "... Press 2");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt3" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[2].PayeeName.ToString() + "... Press 3");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt4" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[3].PayeeName.ToString() + "... Press 4");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt5" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("...For " + pRC.Payees[4].PayeeName.ToString() + "... Press 5");

                        aMenu.addMenuChoice("1", "SelectPayeeMenu1.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "SelectPayeeMenu1.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "SelectPayeeMenu1.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "SelectPayeeMenu1.aspx?event=evalInput&input=4");
                        aMenu.addMenuChoice("5", "SelectPayeeMenu1.aspx?event=evalInput&input=5");

                        pt.addTTSToPrompt("...To hear more " + TTSDictionary("payees") + ", press 8");

                        aMenu.addMenuChoice("8", "SelectPayeeMenu1.aspx?event=evalInput&input=8");
                    }

                    string promptFile9 = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "SelectPayeeMenu1_Opt9" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile9, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt("...To repeat this list, press 9.");

                    aMenu.addMenuChoice("9", "SelectPayeeMenu1.aspx?event=evalInput&input=9");
                }

                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- SelectPayeeMenu1 -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }

        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                l.writeToLog("Info : SelectPayeeMenu1 User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6100", "SelectPayeeMenu1", input);

                Play p = new Play();
                Prompt pt = new Prompt();

                payeeLookUpNumber = pRC.Payees.Count;

                switch (input)
                {
                    case "1":
                        Session["PAYEE"] = pRC.Payees[0].Payee.ToString();
                        Session["PAYEE_NAME"] = pRC.Payees[0].PayeeName.ToString();
                        l.writeToLog("Info : SelectPayeeMenu1:EvalInput:1:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                        p.NextURL = "CollectPin.aspx";
                        break;
                    case "2":
                        Session["PAYEE"] = pRC.Payees[1].Payee.ToString();
                        Session["PAYEE_NAME"] = pRC.Payees[1].PayeeName.ToString();
                        l.writeToLog("Info : SelectPayeeMenu1:EvalInput:2:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                        p.NextURL = "CollectPin.aspx";
                        break;
                    case "3":
                        if (!pRC.Payees[2].Payee.Equals(""))
                        {
                            Session["PAYEE"] = pRC.Payees[2].Payee.ToString();
                            Session["PAYEE_NAME"] = pRC.Payees[2].PayeeName.ToString();
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:3:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                            p.NextURL = "CollectPin.aspx";
                        }
                        else
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:3:Error", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }
                        break;
                    case "4":
                        if (!pRC.Payees[3].Payee.Equals(""))
                        {
                            Session["PAYEE"] = pRC.Payees[3].Payee.ToString();
                            Session["PAYEE_NAME"] = pRC.Payees[3].PayeeName.ToString();
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:4:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                            p.NextURL = "CollectPin.aspx";
                        }
                        else
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:4:Error", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }
                        break;
                    case "5":
                        if (!pRC.Payees[4].Payee.Equals(""))
                        {
                            Session["PAYEE"] = pRC.Payees[4].Payee.ToString();
                            Session["PAYEE_NAME"] = pRC.Payees[4].PayeeName.ToString();
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:5:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                            p.NextURL = "CollectPin.aspx";
                        }
                        else
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:5:Error", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }
                        break;
                    case "6":
                        if (!pRC.Payees[5].Payee.Equals(""))
                        {
                            Session["PAYEE"] = pRC.Payees[5].Payee.ToString();
                            Session["PAYEE_NAME"] = pRC.Payees[5].PayeeName.ToString();
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:6:Payee-PayeeName: " + Session["PAYEE"].ToString() + " " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                            p.NextURL = "CollectPin.aspx";
                        }
                        else
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:6:Error", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }
                        break;
                    case "8":
                        if (payeeLookUpNumber > 6)
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:8:SelectPayeeMenu2.aspx", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SelectPayeeMenu2.aspx";
                        }
                        else
                        {
                            l.writeToLog("Info : SelectPayeeMenu1:EvalInput:8:Error" + payeeLookUpNumber.ToString(), Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }
                        break;
                    case "9":
                        p.NextURL = "SelectPayeeMenu1.aspx";
                        break;
                    default:
                        p.NextURL = "SelectPayeeMenu1.aspx";
                        break;
                }

                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from SelectPayeeMenu1", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- SelectPayeeMenu1 -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("-- Password -- : 3 and out in SelectPayeeMenu1, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- SelectPayeeMenu1 -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }

        private void CallPayeeLookUpWebService(string msaNumber)
        {
            try
            {
                l.writeToLog("Info : Calling PayeeLookUpWebService.GET_DIVISION_PAYEE_DETAILS with MSA NUMBER " + msaNumber, Logging.eventType.AppInfoEvent);
                var result = GetPayees(msaNumber);

                l.writeToLog("Info : PayeeLookUpWebService.GET_DIVISION_PAYEE_DETAILS returned a result", Logging.eventType.AppInfoEvent);

                if (result.Payees.Count > 0)
                {
                    l.writeToLog("Info : " + result.Payees.Count.ToString() + " password results returned", Logging.eventType.AppInfoEvent);
                    pRC = result;
                    l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
                }
                else
                {
                    l.writeToLog("Info : Finished processing web service results", Logging.eventType.AppInfoEvent);
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- SelectPayeeMenu1 -- CallPayeeLookUpWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        public PayeesResultsCollection GetPayees(string msaNumber)
        {
            var payeesResultsCollection = new PayeesResultsCollection();

            try
            {
                l.writeToLog("Info - SelectPayeeMenu1.aspx - Entering GetPayees", Logging.eventType.AppInfoEvent);

                using (var service = new GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient())
                {
                    service.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["NewUserID"].ToString();
                    service.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"].ToString();

                    string detailStatus;

                    var detailResponse = service.Z_WS_GET_DIV_PAYEE_DETAILS(null, msaNumber, null, out detailStatus);

                    if (detailStatus == null)
                    {
                        payeesResultsCollection.Success = false;
                        payeesResultsCollection.ErrorMessage = "Server sent back an empty response while loading Payees.";
                        l.writeToLog("Info - SelectPayeeMenu1.aspx - GetPayees sent back an empty response", Logging.eventType.AppInfoEvent);
                    }
                    else
                    {
                        payeesResultsCollection.Success = true;
                        payeesResultsCollection.Payees =
                            detailResponse.Select(
                                x =>
                                    new PayeeInformation
                                    {
                                        Division = x.DIVISION,
                                        DivisionName = x.DIV_NAME,
                                        Payee = x.PAYEE,
                                        PayeeName = x.PAYEE_NAME
                                    }).ToList();

                        l.writeToLog("Info - SelectPayeeMenu1.aspx - GetPayees - Got Payees", Logging.eventType.AppInfoEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("Error : SelectPayeeMenu1.aspx - GetPayees error exception : " + ex.ToString(), Logging.eventType.AppException);

                return new PayeesResultsCollection
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }

            return payeesResultsCollection;
        }
    }
}