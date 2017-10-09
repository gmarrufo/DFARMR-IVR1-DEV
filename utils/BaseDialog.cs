using System;
using System.IO;
using GVP.MCL.Enhanced;
using System.Collections.Generic;

namespace DFARMR_IVR1
{
    public class BaseDialog : System.Web.UI.Page
    {
        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        public BaseDialog()
            : base()
        {
        }

        protected CatchEvent getCatchEvent(int iCount, string strCatchType, string strNextURL)
        {
            Prompt p = new Prompt();
            p.BargeIn = false;
            p.TimeOut = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
            string catchEventLogs = "";
            string callDataEvent = "";
            CatchEvent e = new CatchEvent(iCount, strCatchType);

            if (strCatchType.Equals(CatchEvent.ERROR))
            {
                catchEventLogs = "Info : Playing SystemError_Msg Prompt|";
                callDataEvent = "MenuError|" + callDataEvent + "|" + "ErrApp_Disconnect|" + "CDE_ErrApp_Disconnect|" + "Invalid|" + "";
                DateTime dateTime = DateTime.Now;
                // p.addVoiceFileToPrompt(System.Configuration.ConfigurationManager.AppSettings["PromptsLocationRoot"] + "SystemError_Msg" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"], true, "I'm sorry, but we are unable to process " + TTSDictionary("your") + " request at this time. Please call again later. Thank you for calling " + TTSDictionary("your") + " Member Information System. Good byee.");
                p.addTTSToPrompt("I’m sorry. We are unable to process " + TTSDictionary("your") + " request at this time. Please call again later.");
                e.setPrompt(p);
                e.NextURL = strNextURL + "?event=globalerror" + "&callDataEvent=" + callDataEvent + "&catchEventLogs=" + catchEventLogs;
            }
            else
            {
                e.NextURL = strNextURL + "?event=globalerror";
            }
            e.Disconnect = false;
            e.Reprompt = false;
            return e;
        }

        protected void AddSilenceToPrompt(Prompt aPrompt)
        {
            aPrompt.addVoiceFileToPrompt(System.Configuration.ConfigurationManager.AppSettings["PromptsLocationRoot"] + "silence" + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"], false, "");
        }

        protected void AddPromptFileOrTTS(Prompt aPrompt, string promptname, bool refresh, string tts)
        {
            if (CheckIfPromptExists(promptname))
            {
                aPrompt.addVoiceFileToPrompt(System.Configuration.ConfigurationManager.AppSettings["PromptsLocationRoot"] + promptname + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"], refresh);
            }
            else
            {
                aPrompt.addTTSToPrompt(tts);
            }
        }

        protected bool CheckIfPromptExists(string promptname)
        {
            try
            {
                string filePath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["PromptsLocationRoot"]) + promptname + System.Configuration.ConfigurationManager.AppSettings["SpeechFileType"];
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                //assume doesn't exist if exception
            }
            return false;
        }

        protected TTS FixCutoffTTS(int breakTime)
        {
            // The platform cuts off the TTS for some prompts. Add a break and it stops.
            // return ttsText + "<break time=\"200ms\"/>";

            TTS ttsBreak = new TTS();
            ttsBreak.silence = breakTime;
            return ttsBreak;
        }

        protected string TTSDictionary(string strInput)
        {
            string name = "";
            string nameTmp = strInput.ToLower();

            if (nameTmp.Equals("protein"))
            {
                     name = "PROTEEN";
            }
            else if(nameTmp.Equals("goodbye"))
            {
                name = "GOODBYEE";
            }
            else if(nameTmp.Equals("milk"))
            {
                //name = "MELK";
                name = "milk";
            }
            else if (nameTmp.Equals("invalid"))
            {
                name = "IN VALID";
            }
            else if (nameTmp.Equals("retrieve"))
            {
               // name = "RETREEEVE";
                name = "retrieve";
            }
            else if (nameTmp.Equals("tax id"))
            {
                name = "TAX I D";
            }
            else if (nameTmp.Equals("now"))
            {
                name = "now";
            }
            else if (nameTmp.Equals("key"))
            {
                name = "KEE";
            }
            else if (nameTmp.Equals("results"))
            {
                //name = "RIZUHLTS";
                name = "results";
            }
            else if (nameTmp.Equals("frame"))
            {
                name = "FRAYM";
            }
            else if (nameTmp.Equals("your"))
            {
                //name = "YORE";
                name = "your";
            }
            else if (nameTmp.Equals("social"))
            {
                name = "SOO SHEL";
            }
            else if (nameTmp.Equals("payees"))
            {
                name = "PAY YEES";
            }
            else if (nameTmp.Equals("inhibitor"))
            {
                name = "inhibetter";
            }
            return name; 
        }
    }

    public class PaymentResultCollectionModel
    {
        public ResultModel Result { get; set; }
        public List<PaymentModel> Payments { get; set; }
        public string TotalAmount { get; set; }

        public PaymentResultCollectionModel()
        {
            Result = new ResultModel();
            Payments = new List<PaymentModel>();
        }
    }

    public class PaymentModel
    {
        public string PaymentDate { get; set; }
        public string PaymentNumber { get; set; }
        public string CheckNumber { get; set; }
        public string Amount { get; set; }
        public string PayPeriod { get; set; }
        public string DocumentType { get; set; }
        public string Description { get; set; }
        public string CompanyCode { get; set; }
        public string ProductionStatementLink { get; set; }
        public string QualityMailerLink { get; set; }
    }

    public class ResultModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public ResultModel()
        {
            Success = false;
            ErrorMessage = null;
        }
    }

    public class PayeesResultsCollection
    {
        //public string LoginId { get; set; }
        public List<PayeeInformation> Payees { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }

        public PayeesResultsCollection()
        {
            Payees = new List<PayeeInformation>();
        }
    }

    public class PayeeInformation
    {
        public string Division { get; set; }
        public string DivisionName { get; set; }
        public string Payee { get; set; }
        public string PayeeName { get; set; }
    }
}