using System;
using GVP.MCL.Enhanced;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DFARMR_IVR1
{
    public partial class MainMenu : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        PayeesResultsCollection pRC;
        TTS breakTimeTTS;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in MainMenu.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2000", "MainMenu", "");

                if (Session["GVPSessionId"] != null)//Getting GVP SessionId from session object
                {
                    pageGVPSessionID = Session["GVPSessionId"].ToString();
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
                }
                else //Getting IIS SessionId from Session.SessionID
                {
                    l.writeToLog("Info : Found  IISSessionID as null from session object", Logging.eventType.AppInfoEvent);
                    l.writeToLog("Info : Getting IISSessionID from Session.SessionID", Logging.eventType.AppInfoEvent);
                    pageIISSessionID = Session.SessionID;
                    l.writeToLog("Info : IIS SessionId : " + pageIISSessionID, Logging.eventType.AppInfoEvent);
                }

                // Get Div Payees
                CallPayeeLookUpWebService(Session["MSA_NUMBER"].ToString());

                Log.GetValuesFromSession(pageGVPSessionID);
                l.writeToLog("Info : Entered into MainMenu state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : Main Menu event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in MainMenu.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MainMenu -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void MenuOptions(int errorCount)
        {
            try
            {
                int iPayeeCount = 0;
                l.writeToLog("Info : MenuOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                breakTimeTTS = FixCutoffTTS(250);
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.addTTSToPrompt(breakTimeTTS);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                Menu aMenu = new Menu("MainMenu");

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

                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MainMenu_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);

                    if (pRC != null)
                    {
                        iPayeeCount = pRC.Payees.Count;
                    }

                    l.writeToLog("Info : MainMenu Menu Options Number of Payee Count is = " + iPayeeCount.ToString(), Logging.eventType.AppInfoEvent);

                    if (iPayeeCount > 0)
                    {
                        pt.addTTSToPrompt("For fast rizuhlts, press 1. For the expanded menu, press 2. To hear a markut report, press 3. For cow samples, press 4.");
                        pt.addTTSToPrompt("For recent payment information,press 5.");
                        pt.addTTSToPrompt("If you have any questions, please contact yore council office.");

                        aMenu.addMenuChoice("1", "MainMenu.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "MainMenu.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "MainMenu.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "MainMenu.aspx?event=evalInput&input=4");
                        aMenu.addMenuChoice("5", "MainMenu.aspx?event=evalInput&input=5");
                    }
                    else
                    {
                        pt.addTTSToPrompt("For fast rizuhlts, press 1. For the expanded menu, press 2. To hear a markut report, press 3. For cow samples, press 4.");
                        pt.addTTSToPrompt("If you have any questions, please contact yore council office.");

                        aMenu.addMenuChoice("1", "MainMenu.aspx?event=evalInput&input=1");
                        aMenu.addMenuChoice("2", "MainMenu.aspx?event=evalInput&input=2");
                        aMenu.addMenuChoice("3", "MainMenu.aspx?event=evalInput&input=3");
                        aMenu.addMenuChoice("4", "MainMenu.aspx?event=evalInput&input=4");
                    }
                }

                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MainMenu -- MenuOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                l.writeToLog("Info : User has selected : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "2000", "MainMenu", input);

                Play p = new Play();
                bool addPleaseWaitPrompt = false;
                int iPayeeCount = 0;
                int i = int.Parse(input);

                switch (i)
                {
                    case 1:
                        l.writeToLog("Info : Going to Fast Results", Logging.eventType.AppInfoEvent);
                        addPleaseWaitPrompt = true;
                        p.NextURL = "FastResults.aspx";
                        break;
                    case 2:
                        l.writeToLog("Info : Going to Expanded Menu", Logging.eventType.AppInfoEvent);
                        p.NextURL = "ExpandedMenu.aspx";
                        break;
                    case 3:
                        l.writeToLog("Info : Going to Market Report", Logging.eventType.AppInfoEvent);
                        p.NextURL = "MarketReport.aspx";
                        break;
                    case 4:
                        l.writeToLog("Info : Going to Cow Samples", Logging.eventType.AppInfoEvent);
                        addPleaseWaitPrompt = true;
                        p.NextURL = "CowSamples.aspx";
                        break;
                    case 5:
                        if (pRC != null)
                        {
                            iPayeeCount = pRC.Payees.Count;
                        }
                        l.writeToLog("Info : MainMenu:EvalInput:PayeeCount: " + iPayeeCount.ToString(), Logging.eventType.AppInfoEvent);

                        if (iPayeeCount > 0)
                        {
                            l.writeToLog("Info : MainMenu:EvalInput:PayeeCount > 0 ", Logging.eventType.AppInfoEvent);
                            string[] passResults = Session["passwordResults"].ToString().Split('~');
                            l.writeToLog("Info : MainMenu:EvalInput:passResults[0] " + passResults[0].ToString(), Logging.eventType.AppInfoEvent);
                            l.writeToLog("Info : MainMenu:EvalInput:passResults[1] " + passResults[1].ToString(), Logging.eventType.AppInfoEvent);

                            if (!passResults[0].Equals(""))
                            {
                                if (iPayeeCount > 1)
                                {
                                    p.NextURL = "SelectPayeeMenu1.aspx";
                                }
                                else
                                {
                                    p.NextURL = "CollectPin.aspx";
                                }
                            }
                            else
                            {
                                Prompt pt = new Prompt();
                                string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "NoPin_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                                l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                                // AddPromptFileOrTTS(pt, "NoPin_Msg", true, "I am sorry you must first set up PIN number to access payment information. Please contact your council office for assistance.");
                                pt.addTTSToPrompt("I am sorry you must first set up PIN number to access payment information. Please contact " + TTSDictionary("your") + " council office for assistance.");
                                p.setPrompt(pt);
                                p.NextURL = "NoPinMenu.aspx";
                            }
                        }
                        else
                        {
                            l.writeToLog("Info : MainMenu:EvalInput:PayeeCount < 0 ", Logging.eventType.AppInfoEvent);
                            p.NextURL = "SystemError.aspx";
                        }

                        break;
                    default:
                        errorCount++;
                        p.NextURL = "MainMenu.aspx?event=menuOptions&subevent=NO_MATCH&errorCount=" + errorCount.ToString();
                        break;
                }

                if (addPleaseWaitPrompt)
                {
                    Prompt pt = new Prompt();
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MainMenu_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // AddPromptFileOrTTS(pt, "MainMenu_Msg", true, "Please wait whyle we retrieve yore information.");
                    pt.addTTSToPrompt("Please wait while we " + TTSDictionary("retrieve") + " " + TTSDictionary("your") + " information.");
                    p.setPrompt(pt);
                }

                l.writeToLog("Info : Exiting from MainMenu - EvalInput", Logging.eventType.AppInfoEvent);

                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MainMenu -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : 3 and out in MainMenu, going to TriesExceeded", Logging.eventType.AppInfoEvent);
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
                l.writeToLog("-- MainMenu -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("Info : NO password results returned", Logging.eventType.AppInfoEvent);
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MainMenu -- CallPayeeLookUpWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        public PayeesResultsCollection GetPayees(string msaNumber)
        {
            var payeesResultsCollection = new PayeesResultsCollection();

            try
            {
                l.writeToLog("Info - MainMenu.aspx - Entering GetPayees", Logging.eventType.AppInfoEvent);

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
                        l.writeToLog("Info - MainMenu.aspx - GetPayees sent back an empty response", Logging.eventType.AppInfoEvent);
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

                        l.writeToLog("Info - MainMenu.aspx - GetPayees - Got Payees", Logging.eventType.AppInfoEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MainMenu -- GetPayees fails --> exception" + ex.ToString(), Logging.eventType.AppException);

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