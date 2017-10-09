﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace DFARMR_IVR1.NewDoesDivisionExist.WebReference {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="Z_WS_DOES_DIVISION_EXISTBinding", Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class Z_WS_DOES_DIVISION_EXISTService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback Z_WS_DOES_DIVISION_EXISTOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Z_WS_DOES_DIVISION_EXISTService() {
            this.Url = global::DFARMR_IVR1.Properties.Settings.Default.DFARMR_IVR1_DOES_PASSWORD_EXIST_Z_WS_DOES_PASSWORD_EXIST_MSAService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event Z_WS_DOES_DIVISION_EXISTCompletedEventHandler Z_WS_DOES_DIVISION_EXISTCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.sap.com/Z_WS_DOES_DIVISION_EXIST", RequestNamespace="urn:sap-com:document:sap:rfc:functions", ResponseElementName="Z_WS_DOES_DIVISION_EXIST.Response", ResponseNamespace="urn:sap-com:document:sap:rfc:functions", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("E_STATUS", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Z_WS_DOES_DIVISION_EXIST([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string I_DIVISION) {
            object[] results = this.Invoke("Z_WS_DOES_DIVISION_EXIST", new object[] {
                        I_DIVISION});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Z_WS_DOES_DIVISION_EXISTAsync(string I_DIVISION) {
            this.Z_WS_DOES_DIVISION_EXISTAsync(I_DIVISION, null);
        }
        
        /// <remarks/>
        public void Z_WS_DOES_DIVISION_EXISTAsync(string I_DIVISION, object userState) {
            if ((this.Z_WS_DOES_DIVISION_EXISTOperationCompleted == null)) {
                this.Z_WS_DOES_DIVISION_EXISTOperationCompleted = new System.Threading.SendOrPostCallback(this.OnZ_WS_DOES_DIVISION_EXISTOperationCompleted);
            }
            this.InvokeAsync("Z_WS_DOES_DIVISION_EXIST", new object[] {
                        I_DIVISION}, this.Z_WS_DOES_DIVISION_EXISTOperationCompleted, userState);
        }
        
        private void OnZ_WS_DOES_DIVISION_EXISTOperationCompleted(object arg) {
            if ((this.Z_WS_DOES_DIVISION_EXISTCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Z_WS_DOES_DIVISION_EXISTCompleted(this, new Z_WS_DOES_DIVISION_EXISTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void Z_WS_DOES_DIVISION_EXISTCompletedEventHandler(object sender, Z_WS_DOES_DIVISION_EXISTCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Z_WS_DOES_DIVISION_EXISTCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Z_WS_DOES_DIVISION_EXISTCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
}

#pragma warning restore 1591