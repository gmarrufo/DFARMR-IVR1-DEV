//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5446
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Web.Services3.Security.Tokens;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="DivisionValidationSoapBinding", Namespace="http://service.validation.milk.ats.com")]
public partial class DivisionValidationService : Microsoft.Web.Services3.WebServicesClientProtocol {
    
    private System.Threading.SendOrPostCallback doesDivisionExistOperationCompleted;
    
    /// <remarks/>
    public DivisionValidationService() {
        string strWebserviceUrl = ConfigurationManager.AppSettings["WebServiceURL"].ToString();
        this.Url = strWebserviceUrl + "DivisionValidation";
#pragma warning disable CS0618 // 'SoapContext.Security' is obsolete: 'SoapContext.Security is obsolete. Consider deriving from SendSecurityFilter or ReceiveSecurityFilter and creating a custom policy assertion that generates these filters.'
        RequestSoapContext.Security.Tokens.Add(new UsernameToken("ivrwebsvc", "V&XT4X@L", PasswordOption.SendPlainText));
#pragma warning restore CS0618 // 'SoapContext.Security' is obsolete: 'SoapContext.Security is obsolete. Consider deriving from SendSecurityFilter or ReceiveSecurityFilter and creating a custom policy assertion that generates these filters.'
    }
    
    /// <remarks/>
    public event doesDivisionExistCompletedEventHandler doesDivisionExistCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://service.validation.milk.ats.com", ResponseNamespace="http://service.validation.milk.ats.com", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("doesDivisionExistReturn", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public string doesDivisionExist([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)] string divisionNumber) {
        object[] results = this.Invoke("doesDivisionExist", new object[] {
                    divisionNumber});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BegindoesDivisionExist(string divisionNumber, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("doesDivisionExist", new object[] {
                    divisionNumber}, callback, asyncState);
    }
    
    /// <remarks/>
    public string EnddoesDivisionExist(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void doesDivisionExistAsync(string divisionNumber) {
        this.doesDivisionExistAsync(divisionNumber, null);
    }
    
    /// <remarks/>
    public void doesDivisionExistAsync(string divisionNumber, object userState) {
        if ((this.doesDivisionExistOperationCompleted == null)) {
            this.doesDivisionExistOperationCompleted = new System.Threading.SendOrPostCallback(this.OndoesDivisionExistOperationCompleted);
        }
        this.InvokeAsync("doesDivisionExist", new object[] {
                    divisionNumber}, this.doesDivisionExistOperationCompleted, userState);
    }
    
    private void OndoesDivisionExistOperationCompleted(object arg) {
        if ((this.doesDivisionExistCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.doesDivisionExistCompleted(this, new doesDivisionExistCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
public delegate void doesDivisionExistCompletedEventHandler(object sender, doesDivisionExistCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class doesDivisionExistCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal doesDivisionExistCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}
