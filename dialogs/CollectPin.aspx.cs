using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using GVP.MCL.Enhanced;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DFARMR_IVR1.GetQueryViewData;
using System.ServiceModel;

namespace DFARMR_IVR1
{
    public partial class CollectPin : BaseDialog
    {
        PayeesResultsCollection pRC;
        public string pageGVPSessionID = "";
        public string pageIISSessionID = "";
        Log l = new Log();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Get Div Payees
                if (Session["PAYEE"] == null)
                {
                    CallPayeeLookUpWebService(Session["MSA_NUMBER"].ToString());
                }
                GlobalSettings.ttsName = GlobalSettings.TTSNameType.SAMANTHA;
                l.writeToLog("Application start in CollectPin.aspx", Logging.eventType.AppBegin);

                Session["nodeType"] = Logging.nodeDataType.Menu;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6300", "Collect PIN", "");

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
                l.writeToLog("-- Password -- Entered into CollectPin state", Logging.eventType.AppInfoEvent);

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
                l.writeToLog("Info : CollectPin event " + selection, Logging.eventType.AppInfoEvent);
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
                l.writeToLog("Application end in CollectPin.aspx", Logging.eventType.AppEnd);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CollectPin -- Page_Load fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void InputOptions(int errorCount)
        {
            try
            {
                string passwordPrompt = "";

                l.writeToLog("Info : InputOptions (" + errorCount.ToString() + ")", Logging.eventType.AppInfoEvent);
                if (errorCount >= 3)
                {
                    l.writeToLog("Info : 3 and out in CollectPin, going to TriesExceeded", Logging.eventType.AppInfoEvent);
                    Play p = new Play();
                    p.NextURL = "TriesExceeded.aspx";
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                    p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                    l.writeToLog("Info : Exiting from CollectPin", Logging.eventType.AppInfoEvent);
                    Response.Write(p.getVXML());
                    return;
                }
                Prompt pt = new Prompt();
                pt.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                if (Request.QueryString["subevent"] == null)
                {
                    string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CollectPin_Init" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                    l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                    // passwordPrompt = "Please enter yore 4 digit PIN followed by the pound sign.";
                    passwordPrompt = "Please enter " + TTSDictionary("your") + " 4 digit PIN followed by the pound sign.";
                    // AddPromptFileOrTTS(pt, "CollectPin_Init", true, passwordPrompt);
                    pt.addTTSToPrompt(passwordPrompt);
                }
                else
                {
                    if (Request.QueryString["subevent"] == CatchEvent.NO_MATCH)
                    {
                        l.writeToLog("Info : NO_MATCH in CollectPin", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CollectPin_NM1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // passwordPrompt = "Sorry, thatt was an in valid entry. Please enter yore payment information access PIN followed by the pound sign.";
                            passwordPrompt = "Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " payment information access PIN followed by the pound sign.";
                            // AddPromptFileOrTTS(pt, "CollectPin_NM1", true, passwordPrompt);
                            pt.addTTSToPrompt(passwordPrompt);
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CollectPin_NM2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // passwordPrompt = "Sorry, thatt was an in valid entry. Please enter yore payment information access PIN followed by the pound sign.";
                            passwordPrompt = "Sorry, that was an " + TTSDictionary("invalid") + " entry. Please enter " + TTSDictionary("your") + " payment information access PIN followed by the pound sign.";
                            // AddPromptFileOrTTS(pt, "CollectPin_NM2", true, passwordPrompt);
                            pt.addTTSToPrompt(passwordPrompt);
                        }
                    }
                    else
                    {
                        l.writeToLog("Info : NO_INPUT in CollectPin", Logging.eventType.AppInfoEvent);
                        if (errorCount == 1)
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CollectPin_NI1" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // passwordPrompt = "Please enter yore payment information access PIN followed by the pound sign.";
                            passwordPrompt = "Please enter " + TTSDictionary("your") + " payment information access PIN followed by the pound sign.";
                            // AddPromptFileOrTTS(pt, "CollectPin_NI1", true, passwordPrompt);
                            pt.addTTSToPrompt(passwordPrompt);
                        }
                        else
                        {
                            string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "CollectPin_NI2" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                            l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                            // passwordPrompt = "Please enter yore payment information access PIN followed by the pound sign.";
                            passwordPrompt = "Please enter " + TTSDictionary("your") + " payment information access PIN followed by the pound sign.";
                            // AddPromptFileOrTTS(pt, "CollectPin_NI2", true, "Sorry, I still did not get that. " + passwordPrompt);
                            pt.addTTSToPrompt("Sorry, I still did not get that. " + passwordPrompt);
                        }
                    }
                }
                Input i = new Input("CollectPinInput", true);
                i.DigitsOnly = true;
                i.MinDigits = 4;
                i.MaxDigits = 4;
                i.MaxTries = 3;
                i.TermChar = '#';
                i.InterDigitTimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                i.NextURL = "CollectPin.aspx?event=evalInput" + "&errorCount=" + errorCount.ToString();
                i.setPrompt(pt);
                i.defaultconfirmation = Input.confirmationMode.never;
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_MATCH, "CollectPin.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_MATCH, "CollectPin.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.NO_INPUT, "CollectPin.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(2, CatchEvent.NO_INPUT, "CollectPin.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(ErrorHandling(1, CatchEvent.MAX_TRIES, "CollectPin.aspx?event=inputOptions", errorCount));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                i.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
               // i.setValidationScript(Input.validationType.naturalnumbers);
                Response.Write(i.getVXML());
              //  l.writeToLog(i.getVXML(), Logging.eventType.AppInfoEvent);
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CollectPin -- InputOptions fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
        }

        private void EvalInput(int errorCount)
        {
            try
            {
                string status = "";
                string input = Request.QueryString["CollectPinInput"] != null ? Request.QueryString["CollectPinInput"].ToString().Trim() : "";
                l.writeToLog("Info : CollectPin User has entered : " + input, Logging.eventType.AppInfoEvent);

                Session["nodeType"] = Logging.nodeDataType.CED;
                l.reportERM((Logging.nodeDataType)Session["nodeType"], "6300", "Collect PIN", input);

                Play p = new Play();
                Prompt pt = new Prompt();

                string[] passResults = Session["passwordResults"].ToString().Split('~');
                l.writeToLog("Info : passResults[0].ToString(): " + passResults[0].ToString(), Logging.eventType.AppInfoEvent);

                if (input.Equals(passResults[0].ToString()))
                {
                    if (Session["PAYEE"] == null)
                    {
                        status = CallWebService(Convert.ToInt32(pRC.Payees[0].Payee));
                        l.writeToLog("Info : pRC.Payees[0].Payee: " + pRC.Payees[0].Payee, Logging.eventType.AppInfoEvent);

                    }
                    else
                    {
                        status = CallWebService(Convert.ToInt32(Session["PAYEE"].ToString()));
                        l.writeToLog("Info : Session[PAYEE].ToString(): " + Session["PAYEE"].ToString(), Logging.eventType.AppInfoEvent);
                    }
                }
                else
                {
                    status = "invalid";
                }
                l.writeToLog("status: " + status, Logging.eventType.AppInfoEvent);
                switch (status)
                {
                    case "valid":
                        if (Session["PAYEE_NAME"] == null)
                        {
                            Session["PAYEE_NAME"] = pRC.Payees[0].PayeeName.ToString();
                        }
                            l.writeToLog("Info : CollectPin Entering EvalInput = VALID ", Logging.eventType.AppInfoEvent);

                        string promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoIntro_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "PaymentInfoIntro_Msg", true, "Here is the most current payment information for...");
                        pt.addTTSToPrompt("Here is the most current payment information for...");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PayeeName_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "PayeeName_Msg", true, Session["PAYEE_NAME"].ToString());
                        pt.addTTSToPrompt(Session["PAYEE_NAME"].ToString());

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoDate_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "PaymentInfoDate_Msg", true, "Payment dated...");
                        pt.addTTSToPrompt("Payment dated...");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Date_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "Date_Msg", true, Session["PaymentDate"].ToString());
                        pt.addTTSToPrompt(Session["PaymentDate"].ToString());

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoType_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "PaymentInfoType_Msg", true, "Document type...");
                        pt.addTTSToPrompt("Document type...");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Type_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "Type_Msg", true, Session["ZSTUBTYPE"].ToString());
                        pt.addTTSToPrompt(Session["ZSTUBTYPE"].ToString());

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "PaymentInfoAmmt_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "PaymentInfoAmmt_Msg", true, "Payment Amount...");
                        pt.addTTSToPrompt("Payment Amount...");

                        promptFile = System.Configuration.ConfigurationManager.AppSettings["PromptsLocation"] + "Amount_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                        l.writeToLog("Info : Playing " + promptFile, Logging.eventType.AppInfoEvent);
                        // AddPromptFileOrTTS(pt, "Amount_Msg", true, Session["Amount"].ToString());
                        pt.addTTSToPrompt(Session["Amount"].ToString());

                        p.NextURL = "MorePaymentsMenu.aspx";

                        break;
                    case "invalid":
                        p.NextURL = "SystemError.aspx";
                        break;
                    default:
                        p.NextURL = "SystemError.aspx";
                        break;
                }

                AddSilenceToPrompt(pt);
                p.setPrompt(pt);
                p.addCatchBlock(getCatchEvent(1, CatchEvent.ERROR, "SystemError.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.CONNECTION_HANG_UP, "Goodbye.aspx"));
                p.addCatchBlock(getCatchEvent(1, CatchEvent.PHONE_HANG_UP, "Goodbye.aspx"));
                l.writeToLog("Info : Exiting from CollectPin", Logging.eventType.AppInfoEvent);
                Response.Write(p.getVXML());
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CollectPin -- EvalInput fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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
                    l.writeToLog("-- Password -- : 3 and out in CollectPin, going to TriesExceeded", Logging.eventType.AppErrorEvent);
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
                l.writeToLog("-- CollectPin -- ErrorHandling fails --> exception" + ex.ToString(), Logging.eventType.AppException);
            }
            return e;
        }

        private string CallWebService(int payeeID)
        {
            try
            {
                l.writeToLog("CollectPin.aspx - PayeeID is: " + payeeID.ToString(), Logging.eventType.AppInfoEvent);

                PaymentResultCollectionModel result = GetPayments(payeeID);

                l.writeToLog("CollectPin.aspx - GetPaymentsService.GET_QUERY_VIEW_DATA returned a result ", Logging.eventType.AppInfoEvent);

                List<DateTime> dates = new List<DateTime>();
                List<PaymentModel> pm = result.Payments;
                int iPM = pm.Count;

                l.writeToLog("Info : CollectPin.aspx Payments Count = " + iPM.ToString(), Logging.eventType.AppInfoEvent);

                if (iPM > 0)
                {
                    foreach (PaymentModel mp in pm)
                    {
                        dates.Add(Convert.ToDateTime(mp.PaymentDate));
                    }

                    var HighestDate = dates.Max();
                    string sTemp = HighestDate.ToString();
                    string[] sTempArray = sTemp.Split(' ');
                    Session["PaymentDate"] = sTempArray[0].ToString();
                    l.writeToLog("CollectPin.aspx - Session[PaymentDate] = " + Session["PaymentDate"].ToString(), Logging.eventType.AppInfoEvent);

                    foreach (PaymentModel mp in pm)
                    {
                        var PaymentDateCheckVS = Convert.ToDateTime(mp.PaymentDate);
                        string sTempCheckVS = PaymentDateCheckVS.ToString();
                        string[] sTempArrayCheckVS = sTempCheckVS.Split(' ');
                        l.writeToLog("CollectPin.aspx mp.PaymentDate VS sTempArray[0]: " + sTempArrayCheckVS[0].ToString() + " - " + sTempArray[0].ToString(), Logging.eventType.AppInfoEvent);

                        if (sTempArrayCheckVS[0].Equals(sTempArray[0].ToString()))
                        {
                            Session["ZSTUBTYPE"] = mp.Description.ToString();
                            Session["Amount"] = mp.Amount.ToString();
                            Session["PaymentDate"] = mp.PaymentDate.ToString();
                            l.writeToLog("Info : CollectPin.aspx - mp.Description = " + mp.Description, Logging.eventType.AppInfoEvent);
                            l.writeToLog("Info : CollectPin.aspx - mp.Amount = " + mp.Amount, Logging.eventType.AppInfoEvent);
                            l.writeToLog("Info : CollectPin.aspx - mp.PaymentDate = " + mp.PaymentDate, Logging.eventType.AppInfoEvent);
                        }
                    }

                    l.writeToLog("Info : GetPaymentsService.GET_QUERY_VIEW_DATA Sessions: ZSTUBTYPE, Amount and PaymentDate are: " + Session["ZSTUBTYPE"].ToString() + " - " + Session["Amount"].ToString() + " - " + Session["PaymentDate"], Logging.eventType.AppInfoEvent);
                }

                /*
                for (int i = 0; i < iPM; i++)
                {
                    Session["PaymentDate"] = pm[i].PaymentDate;
                    Session["ZSTUBTYPE"] = pm[i].Description.ToString();
                    Session["Amount"] = pm[i].Amount.ToString();
                }
                
                // Add dates to List for comparizon
                for (int i = 0; i < iPM; i++)
                {
                    dates.Add(pm[i].PaymentDate);
                    l.writeToLog("Info : CollectPin.aspx Date List = " + pm[i].PaymentDate, Logging.eventType.AppInfoEvent);
                }

                // Compares to today closest
                var allDates = dates.Select(DateTime.Parse).OrderBy(d => d).ToList();
                var inputDate = DateTime.Today;

                var closestDate = inputDate >= allDates.Last()
                    ? allDates.Last()
                    : inputDate <= allDates.First()
                        ? allDates.First()
                        : allDates.First(d => d >= inputDate);

                // Session Pay Date
                string sTemp = closestDate.ToString();
                string[] sTempArray = sTemp.Split(' ');
                Session["PaymentDate"] = sTempArray[0].ToString();

                l.writeToLog("Info : GetPaymentsService.GET_QUERY_VIEW_DATA Session[PaymentDate] = " + Session["PaymentDate"].ToString() , Logging.eventType.AppInfoEvent);

                // Get ZSTUBTYPE and Amount
             
                for (int i = 0; i < iPM; i++)
                {
                    if(pm[i].PaymentDate.Equals(Session["PaymentDate"].ToString()) && !pm[i].DocumentType.Equals(""))
                    {
                        Session["ZSTUBTYPE"] = pm[i].Description.ToString();
                        Session["Amount"] = pm[i].Amount.ToString();
                    }
                }

                // Get Ordinal and Caption
                // var ordinal = "";

                for (int i = 0; i < iPM; i++)
                {
                    l.writeToLog("Info : CollectPin.aspx - PaymentDate + DocumentType  = " + pm[i].PaymentDate + " - " + pm[i].DocumentType, Logging.eventType.AppInfoEvent);

                    if (pm[i].PaymentDate.Equals(Session["PaymentDate"].ToString()) && pm[i].DocumentType.Equals("ZSTUBTYPE"))
                    {
                        Session["ZSTUBTYPE"] = pm[i].Description.ToString();
                        // ordinal = e_SET[i].TUPLE_ORDINAL.ToString();
                        Session["Amount"] = pm[i].Amount.ToString();
                    }
                }

                // Obtain the amount based on ordinal
               
                var e_amounts = (DFARMR.utils.GetPaymentsXML.E_CELL_DATA)serializer.Deserialize(stream);
                int iItems = e_amounts.item.Length;

                for (int i = 0; i < iItems; i++)
                {
                    if (e_amounts.item[i].CELL_ORDINAL.Equals(ordinal))
                    {
                        Session["Amount"] = e_amounts.item[i].VALUE.ToString();
                    }
                }
                */

                if (result.Result.Success && iPM > 0)
                {
                    return "valid";
                }
                else
                {
                    return "invalid";
                }
            }
            catch (Exception ex)
            {
                l.writeToLog("-- CollectPin -- CallWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
                return "error";
            }
        }

        public PaymentResultCollectionModel GetPayments(int payeeID)
        {
            // DateTime low_date = DateTime.Today.AddDays(-20);
            DateTime low_date = DateTime.Today.AddDays(-300);
            string sLow_Date = low_date.ToShortDateString();

            DateTime high_date = DateTime.Today.AddDays(2);
            string sHigh_Date = high_date.ToShortDateString();

            var client = new GetQueryViewData.RRW3_GET_QUERY_VIEW_DATAPortTypeClient();
            
            try
            {
                l.writeToLog("Info : Entering GetPayments Method in CollectPin", Logging.eventType.AppInfoEvent);

                RRWS_SX_AXIS_INFO[] axisInfo = null;
                RRWS_S_CELL[] cellData = null;
                RRWS_S_TEXT_SYMBOLS[] textSymbols = null;

                client.ClientCredentials.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["NewUserID"];
                client.ClientCredentials.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["NewPasswd"];
                client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                l.writeToLog("CollectPin.aspx - Values to pass: " + payeeID.ToString() + " - " + sLow_Date + " - " + sHigh_Date, Logging.eventType.AppInfoEvent);

                var data = client.RRW3_GET_QUERY_VIEW_DATA(
                    "ZFIAP_M01",
                    "ZZFIAP_M01_Q1597_WS",
                    new[]
                    {
                        new DFARMR_IVR1.GetQueryViewData.W3QUERY {NAME = "VAR_NAME_1", VALUE = "ZCM_MM_PAYEE"},
                        new DFARMR_IVR1.GetQueryViewData.W3QUERY {NAME = "VAR_VALUE_EXT_1", VALUE = payeeID.ToString()},
                        new DFARMR_IVR1.GetQueryViewData.W3QUERY {NAME = "VAR_NAME_2", VALUE = "ZCM_OI_CALDATE"},
                        new DFARMR_IVR1.GetQueryViewData.W3QUERY
                        {
                            NAME = "VAR_VALUE_LOW_EXT_2",
                            VALUE = sLow_Date
                        },
                        new DFARMR_IVR1.GetQueryViewData.W3QUERY
                        {
                            NAME = "VAR_VALUE_HIGH_EXT_2",
                            VALUE = sHigh_Date
                        }
                    },
                    "",
                    out axisInfo,
                    out cellData,
                    out textSymbols);

                var columns = axisInfo.Single(x => x.AXIS.Equals("001")).CHARS;
                var cells = data.Single(x => x.AXIS.Equals("001")).SET;
                var columnsTotal = columns.Length;
                var rowsTotal = cells.Length / columns.Length;

                var payDateIndex = Array.FindIndex(columns, x => x.CHANM.Equals("0PAY_DATE"));
                var paymentNumberIndex = Array.FindIndex(columns, x => x.CHANM.Equals("ZPAYMTNUM"));
                var payPeriodIndex = Array.FindIndex(columns, x => x.CHANM.Equals("0IN_PERIOD"));
                var descriptionIndex = Array.FindIndex(columns, x => x.CHANM.Equals("ZSTUBTYPE"));
                var checkNumberIndex = Array.FindIndex(columns, x => x.CHANM.Equals("0CHEQUE"));
                var companyCodeIndex = Array.FindIndex(columns, x => x.CHANM.Equals("0COMP_CODE"));

                var results = new PaymentResultCollectionModel();
                results.Result.Success = true;
                results.Payments = new List<PaymentModel>(rowsTotal);

                var totalAmount = 0m;
                for (var i = 0; i < rowsTotal; i++)
                {
                    var row = cells.Skip(i * columnsTotal).Take(columnsTotal).ToArray();
                    var rowModel = new PaymentModel
                    {
                        Amount = cellData[i].FORMATTED_VALUE.Replace("$ ", string.Empty),
                        PaymentDate = row[payDateIndex].CHAVL_EXT,
                        PaymentNumber = row[paymentNumberIndex].CHAVL.TrimStart('0'),
                        CheckNumber = row[checkNumberIndex].CHAVL.TrimStart('0'),
                        PayPeriod = row[payPeriodIndex].CHAVL,
                        DocumentType = row[descriptionIndex].CHAVL,
                        Description = row[descriptionIndex].CAPTION,
                        CompanyCode = row[companyCodeIndex].CHAVL
                    };
                    results.Payments.Add(rowModel);

                    // Make sure PaymentDate has a valid date or the orderby below that parses it will complain

                    DateTime dummyDate;
                    if (!DateTime.TryParse(rowModel.PaymentDate, out dummyDate))
                        rowModel.PaymentDate = DateTime.MinValue.ToString("MM/dd/yyyy");

                    decimal rowAmount;
                    if (decimal.TryParse(rowModel.Amount, out rowAmount))
                        totalAmount += rowAmount;
                }

                results.Payments = results.Payments.OrderByDescending(x => DateTime.Parse(x.PaymentDate)).ToList();
                results.TotalAmount = totalAmount.ToString("0,0.00");

                l.writeToLog("Info : Exit GetPayments Method in CollectPin", Logging.eventType.AppInfoEvent);

                return results;
            }
            catch (FaultException fe)
            {
                // l.writeToLog("CollectPin.aspx - FaultException: " + fe.InnerException.Message, Logging.eventType.AppInfoEvent);

                if (fe.Message.Equals("NO_APPLICABLE_DATA"))
                {
                    // This is the SAP query's weird way of saying there are no search results
                    l.writeToLog("GetPayments: GET_QUERY_VIEW_DATA call complete. (No results) ({0}, {1}, log id {2})",Logging.eventType.AppInfoEvent);

                    return new PaymentResultCollectionModel
                    {
                        Result = new ResultModel()
                        {
                            Success = true,
                        },
                        Payments = new List<PaymentModel>()
                    };
                }

                return new PaymentResultCollectionModel
                {
                    Result = new ResultModel()
                    {
                        Success = false,
                        ErrorMessage = fe.Message
                    },
                };
            }
            catch (Exception e)
            {
                l.writeToLog("-- CollectPin -- GetPayments fails --> exception" + e.ToString(), Logging.eventType.AppException);
                return new PaymentResultCollectionModel
                {
                    Result = new ResultModel()
                    {
                        Success = false,
                        ErrorMessage = e.Message
                    },
                };
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
                l.writeToLog("-- SelectPayeeMenu1 -- CallPayeeLookUpWebService fails --> exception" + ex.ToString(), Logging.eventType.AppException);
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