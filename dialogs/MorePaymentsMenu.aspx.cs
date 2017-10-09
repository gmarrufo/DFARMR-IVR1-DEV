using System;
using GVP.MCL.Enhanced;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace DFARMR_IVR1
{
    public partial class MorePaymentsMenu : BaseDialog
    {
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();
        PayeesResultsCollection pRC;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in MorePaymentsMenu.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6600", "MorePaymentsMenu", "");

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
                l.writeToLog("-- Password -- Entered into MorePaymentsMenu state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : MorePaymentsMenu event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in MorePaymentsMenu.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MorePaymentsMenu -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions()
        {
            try
            {
                string passwordPrompt = "";
                Prompt pt = new Prompt();
                int iPayeeCount = 0;

                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                pt.BargeIn = true;
                pt.TimeOut = 3;

                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MorePaymentsMenu_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    passwordPrompt = "To repeat that information, press 1";
                    pt.addTTSToPrompt(passwordPrompt);
                }

                if (pRC != null)
                {
                    iPayeeCount = pRC.Payees.Count;
                }

                l.writeToLog("Info : MorePaymentsMenu:InputOptions:PayeeCount: " + iPayeeCount.ToString(), Logging.eventType.AppInfoEvent);

                if (iPayeeCount > 1)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MorePaymentsMenu_NewPayee" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    pt.addTTSToPrompt("To select another payee, press 2.");
                }
                else
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "MorePaymentsMenu_End" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                }
                pt.addTTSToPrompt("or press any other key to return to the main menu.");

                Menu aMenu = new Menu("MainMenu");

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

                CatchEvent ceNoInput1 = aMenu.getCatchEvent(pMaxErrorEndCall, 1, CatchEvent.NO_INPUT, "MainMenu.aspx", false, false);
                CatchEvent ceNoMatch1 = aMenu.getCatchEvent(pMaxErrorEndCall, 1, CatchEvent.NO_MATCH, "MainMenu.aspx", false, false);
                CatchEvent ceMaxTries = aMenu.getCatchEvent(pMaxErrorEndCall, 1, CatchEvent.MAX_TRIES, "MainMenu.aspx", false, false);

                aMenu.addCatchBlock(ceNoInput1);
                aMenu.addCatchBlock(ceNoMatch1);
                aMenu.addCatchBlock(ceMaxTries);
                aMenu.setPrompt(pt);
                aMenu.MaxTries = 3;
                aMenu.addMenuChoice("0", "MorePaymentsMenu.aspx?event=evalInput&input=0");
                aMenu.addMenuChoice("1", "MorePaymentsMenu.aspx?event=evalInput&input=1");
                aMenu.addMenuChoice("2", "MorePaymentsMenu.aspx?event=evalInput&input=2");
                aMenu.addMenuChoice("3", "MorePaymentsMenu.aspx?event=evalInput&input=3");
                aMenu.addMenuChoice("4", "MorePaymentsMenu.aspx?event=evalInput&input=4");
                aMenu.addMenuChoice("5", "MorePaymentsMenu.aspx?event=evalInput&input=5");
                aMenu.addMenuChoice("6", "MorePaymentsMenu.aspx?event=evalInput&input=6");
                aMenu.addMenuChoice("7", "MorePaymentsMenu.aspx?event=evalInput&input=7");
                aMenu.addMenuChoice("8", "MorePaymentsMenu.aspx?event=evalInput&input=8");
                aMenu.addMenuChoice("9", "MorePaymentsMenu.aspx?event=evalInput&input=9");
                aMenu.addMenuChoice("*", "MorePaymentsMenu.aspx?event=evalInput&input=*");
                aMenu.addMenuChoice("#", "MorePaymentsMenu.aspx?event=evalInput&input=#");
                Response.Write(aMenu.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MorePaymentsMenu -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput()
        {
            try
            {
                string input = Request.QueryString["input"] != null ? Request.QueryString["input"].ToString().Trim() : "";
                l.writeToLog("Info : MorePaymentsMenu User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6600", "MorePaymentsMenu", input);

                Play p = new Play();
                Prompt pt = new Prompt();

                switch (input)
                {
                    case "1":
                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoIntro_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:Here is the most current payment information for ...", Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt("Here is the most current payment information for...");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PayeeName_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:PayeeName: " + Session["PAYEE_NAME"].ToString(), Logging.eventType.AppInfoEvent);
                        pt.addTTSToPrompt(Session["PAYEE_NAME"].ToString());

                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:Payment dated...", Logging.eventType.AppInfoEvent);
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoDate_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt("Payment dated...");

                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:PaymentDate: " + Session["PaymentDate"].ToString(), Logging.eventType.AppInfoEvent);
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Date_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt(Session["PaymentDate"].ToString());

                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:Document Type... ", Logging.eventType.AppInfoEvent);
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoType_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt("Document type...");

                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:Document type: " + Session["ZSTUBTYPE"].ToString(), Logging.eventType.AppInfoEvent);
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Type_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt(Session["ZSTUBTYPE"].ToString());

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoAmmt_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt("Payment Amount...");

                        l.writeToLog("Info : MorePaymentsMenu:EvalInput:1:Amount: " + Session["Amount"].ToString(), Logging.eventType.AppInfoEvent);
                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Amount_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        pt.addTTSToPrompt(Session["Amount"].ToString());

                        p.NextURL = "MorePaymentsMenu.aspx";

                        break;
                    case "2":
                        p.NextURL = "SelectPayeeMenu1.aspx";
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
                l.writeToLog("Info : Exiting from MorePaymentsMenu", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MorePaymentsMenu -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
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
                l.writeToLog("-- MorePaymentsMenu -- CallPayeeLookUpWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        public PayeesResultsCollection GetPayees(string msaNumber)
        {
            var payeesResultsCollection = new PayeesResultsCollection();

            try
            {
                l.writeToLog("Info - MorePaymentsMenu.aspx - Entering GetPayees", Logging.eventType.AppInfoEvent);

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
                        l.writeToLog("Info - MorePaymentsMenu.aspx - GetPayees sent back an empty response", Logging.eventType.AppInfoEvent);
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

                        l.writeToLog("Info - MorePaymentsMenu.aspx - GetPayees - Got Payees", Logging.eventType.AppInfoEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- MorePaymentsMenu -- GetPayees fails --> exception" + ex.ToString(), Logging.eventType.AppException);

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