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

namespace DFARMR_IVR1.GET_QUERY_VIEW_DATA {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="RRW3_GET_QUERY_VIEW_DATABinding", Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRW3_GET_QUERY_VIEW_DATAService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback RRW3_GET_QUERY_VIEW_DATAOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RRW3_GET_QUERY_VIEW_DATAService() {
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
        public event RRW3_GET_QUERY_VIEW_DATACompletedEventHandler RRW3_GET_QUERY_VIEW_DATACompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.sap.com/RRW3_GET_QUERY_VIEW_DATA", RequestNamespace="urn:sap-com:document:sap:rfc:functions", ResponseElementName="RRW3_GET_QUERY_VIEW_DATA.Response", ResponseNamespace="urn:sap-com:document:sap:rfc:functions", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("E_AXIS_DATA", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RRWS_SX_AXIS_DATA[] RRW3_GET_QUERY_VIEW_DATA([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string I_INFOPROVIDER, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string I_QUERY, [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)] W3QUERY[] I_T_PARAMETER, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string I_VIEW_ID, [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)] out RRWS_SX_AXIS_INFO[] E_AXIS_INFO, [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)] out RRWS_S_CELL[] E_CELL_DATA, [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)] out RRWS_S_TEXT_SYMBOLS[] E_TXT_SYMBOLS) {
            object[] results = this.Invoke("RRW3_GET_QUERY_VIEW_DATA", new object[] {
                        I_INFOPROVIDER,
                        I_QUERY,
                        I_T_PARAMETER,
                        I_VIEW_ID});
            E_AXIS_INFO = ((RRWS_SX_AXIS_INFO[])(results[1]));
            E_CELL_DATA = ((RRWS_S_CELL[])(results[2]));
            E_TXT_SYMBOLS = ((RRWS_S_TEXT_SYMBOLS[])(results[3]));
            return ((RRWS_SX_AXIS_DATA[])(results[0]));
        }
        
        /// <remarks/>
        public void RRW3_GET_QUERY_VIEW_DATAAsync(string I_INFOPROVIDER, string I_QUERY, W3QUERY[] I_T_PARAMETER, string I_VIEW_ID) {
            this.RRW3_GET_QUERY_VIEW_DATAAsync(I_INFOPROVIDER, I_QUERY, I_T_PARAMETER, I_VIEW_ID, null);
        }
        
        /// <remarks/>
        public void RRW3_GET_QUERY_VIEW_DATAAsync(string I_INFOPROVIDER, string I_QUERY, W3QUERY[] I_T_PARAMETER, string I_VIEW_ID, object userState) {
            if ((this.RRW3_GET_QUERY_VIEW_DATAOperationCompleted == null)) {
                this.RRW3_GET_QUERY_VIEW_DATAOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRRW3_GET_QUERY_VIEW_DATAOperationCompleted);
            }
            this.InvokeAsync("RRW3_GET_QUERY_VIEW_DATA", new object[] {
                        I_INFOPROVIDER,
                        I_QUERY,
                        I_T_PARAMETER,
                        I_VIEW_ID}, this.RRW3_GET_QUERY_VIEW_DATAOperationCompleted, userState);
        }
        
        private void OnRRW3_GET_QUERY_VIEW_DATAOperationCompleted(object arg) {
            if ((this.RRW3_GET_QUERY_VIEW_DATACompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RRW3_GET_QUERY_VIEW_DATACompleted(this, new RRW3_GET_QUERY_VIEW_DATACompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class W3QUERY {
        
        private string nAMEField;
        
        private string vALUEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NAME {
            get {
                return this.nAMEField;
            }
            set {
                this.nAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VALUE {
            get {
                return this.vALUEField;
            }
            set {
                this.vALUEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_S_TEXT_SYMBOLS {
        
        private string sYM_TYPEField;
        
        private string sYM_TIMEDEPField;
        
        private string sYM_NAMEField;
        
        private string sYM_FSField;
        
        private string sYM_BEGIN_GROUPField;
        
        private string sYM_CAPTIONField;
        
        private string sYM_VALUE_TYPEField;
        
        private string sYM_VALUEField;
        
        private string sYM_OUTPUTLENField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_TYPE {
            get {
                return this.sYM_TYPEField;
            }
            set {
                this.sYM_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_TIMEDEP {
            get {
                return this.sYM_TIMEDEPField;
            }
            set {
                this.sYM_TIMEDEPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_NAME {
            get {
                return this.sYM_NAMEField;
            }
            set {
                this.sYM_NAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_FS {
            get {
                return this.sYM_FSField;
            }
            set {
                this.sYM_FSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_BEGIN_GROUP {
            get {
                return this.sYM_BEGIN_GROUPField;
            }
            set {
                this.sYM_BEGIN_GROUPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_CAPTION {
            get {
                return this.sYM_CAPTIONField;
            }
            set {
                this.sYM_CAPTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_VALUE_TYPE {
            get {
                return this.sYM_VALUE_TYPEField;
            }
            set {
                this.sYM_VALUE_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_VALUE {
            get {
                return this.sYM_VALUEField;
            }
            set {
                this.sYM_VALUEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SYM_OUTPUTLEN {
            get {
                return this.sYM_OUTPUTLENField;
            }
            set {
                this.sYM_OUTPUTLENField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_S_CELL {
        
        private string cELL_ORDINALField;
        
        private string vALUEField;
        
        private string fORMATTED_VALUEField;
        
        private string vALUE_TYPEField;
        
        private string cURRENCYField;
        
        private string uNITField;
        
        private string mWKZField;
        
        private string nUM_SCALEField;
        
        private string nUM_PRECField;
        
        private string cELL_STATUSField;
        
        private string bACK_COLORField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CELL_ORDINAL {
            get {
                return this.cELL_ORDINALField;
            }
            set {
                this.cELL_ORDINALField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VALUE {
            get {
                return this.vALUEField;
            }
            set {
                this.vALUEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FORMATTED_VALUE {
            get {
                return this.fORMATTED_VALUEField;
            }
            set {
                this.fORMATTED_VALUEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VALUE_TYPE {
            get {
                return this.vALUE_TYPEField;
            }
            set {
                this.vALUE_TYPEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CURRENCY {
            get {
                return this.cURRENCYField;
            }
            set {
                this.cURRENCYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UNIT {
            get {
                return this.uNITField;
            }
            set {
                this.uNITField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MWKZ {
            get {
                return this.mWKZField;
            }
            set {
                this.mWKZField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NUM_SCALE {
            get {
                return this.nUM_SCALEField;
            }
            set {
                this.nUM_SCALEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NUM_PREC {
            get {
                return this.nUM_PRECField;
            }
            set {
                this.nUM_PRECField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CELL_STATUS {
            get {
                return this.cELL_STATUSField;
            }
            set {
                this.cELL_STATUSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BACK_COLOR {
            get {
                return this.bACK_COLORField;
            }
            set {
                this.bACK_COLORField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_S_ATTRINM {
        
        private string aTTRINMField;
        
        private string cAPTIONField;
        
        private string cHAPRSNTField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ATTRINM {
            get {
                return this.aTTRINMField;
            }
            set {
                this.aTTRINMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAPTION {
            get {
                return this.cAPTIONField;
            }
            set {
                this.cAPTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHAPRSNT {
            get {
                return this.cHAPRSNTField;
            }
            set {
                this.cHAPRSNTField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_SX_AXIS_CHARS {
        
        private string cHANMField;
        
        private string hIENMField;
        
        private string vERSIONField;
        
        private string dATETOField;
        
        private string cAPTIONField;
        
        private string cHAPRSNTField;
        
        private string cHATYPField;
        
        private RRWS_S_ATTRINM[] aTTRINMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHANM {
            get {
                return this.cHANMField;
            }
            set {
                this.cHANMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string HIENM {
            get {
                return this.hIENMField;
            }
            set {
                this.hIENMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VERSION {
            get {
                return this.vERSIONField;
            }
            set {
                this.vERSIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DATETO {
            get {
                return this.dATETOField;
            }
            set {
                this.dATETOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAPTION {
            get {
                return this.cAPTIONField;
            }
            set {
                this.cAPTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHAPRSNT {
            get {
                return this.cHAPRSNTField;
            }
            set {
                this.cHAPRSNTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHATYP {
            get {
                return this.cHATYPField;
            }
            set {
                this.cHATYPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RRWS_S_ATTRINM[] ATTRINM {
            get {
                return this.aTTRINMField;
            }
            set {
                this.aTTRINMField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_SX_AXIS_INFO {
        
        private string aXISField;
        
        private int nCHARSField;
        
        private bool nCHARSFieldSpecified;
        
        private int nCOORDSField;
        
        private bool nCOORDSFieldSpecified;
        
        private RRWS_SX_AXIS_CHARS[] cHARSField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AXIS {
            get {
                return this.aXISField;
            }
            set {
                this.aXISField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int NCHARS {
            get {
                return this.nCHARSField;
            }
            set {
                this.nCHARSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NCHARSSpecified {
            get {
                return this.nCHARSFieldSpecified;
            }
            set {
                this.nCHARSFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int NCOORDS {
            get {
                return this.nCOORDSField;
            }
            set {
                this.nCOORDSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool NCOORDSSpecified {
            get {
                return this.nCOORDSFieldSpecified;
            }
            set {
                this.nCOORDSFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RRWS_SX_AXIS_CHARS[] CHARS {
            get {
                return this.cHARSField;
            }
            set {
                this.cHARSField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_S_ATTRIBUTES {
        
        private string aTTRINMField;
        
        private string cAPTIONField;
        
        private string aTTRIVLField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ATTRINM {
            get {
                return this.aTTRINMField;
            }
            set {
                this.aTTRINMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAPTION {
            get {
                return this.cAPTIONField;
            }
            set {
                this.cAPTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ATTRIVL {
            get {
                return this.aTTRIVLField;
            }
            set {
                this.aTTRIVLField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_SX_TUPLE {
        
        private string tUPLE_ORDINALField;
        
        private string cHANMField;
        
        private string cAPTIONField;
        
        private string cHAVLField;
        
        private string cHAVL_EXTField;
        
        private string nIOBJNMField;
        
        private string tLEVELField;
        
        private string dRILLSTATEField;
        
        private string oPTField;
        
        private string sIGNField;
        
        private RRWS_S_ATTRIBUTES[] aTTRIBUTESField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TUPLE_ORDINAL {
            get {
                return this.tUPLE_ORDINALField;
            }
            set {
                this.tUPLE_ORDINALField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHANM {
            get {
                return this.cHANMField;
            }
            set {
                this.cHANMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CAPTION {
            get {
                return this.cAPTIONField;
            }
            set {
                this.cAPTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHAVL {
            get {
                return this.cHAVLField;
            }
            set {
                this.cHAVLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CHAVL_EXT {
            get {
                return this.cHAVL_EXTField;
            }
            set {
                this.cHAVL_EXTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NIOBJNM {
            get {
                return this.nIOBJNMField;
            }
            set {
                this.nIOBJNMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TLEVEL {
            get {
                return this.tLEVELField;
            }
            set {
                this.tLEVELField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DRILLSTATE {
            get {
                return this.dRILLSTATEField;
            }
            set {
                this.dRILLSTATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string OPT {
            get {
                return this.oPTField;
            }
            set {
                this.oPTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SIGN {
            get {
                return this.sIGNField;
            }
            set {
                this.sIGNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RRWS_S_ATTRIBUTES[] ATTRIBUTES {
            get {
                return this.aTTRIBUTESField;
            }
            set {
                this.aTTRIBUTESField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class RRWS_SX_AXIS_DATA {
        
        private string aXISField;
        
        private RRWS_SX_TUPLE[] sETField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AXIS {
            get {
                return this.aXISField;
            }
            set {
                this.aXISField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public RRWS_SX_TUPLE[] SET {
            get {
                return this.sETField;
            }
            set {
                this.sETField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    public delegate void RRW3_GET_QUERY_VIEW_DATACompletedEventHandler(object sender, RRW3_GET_QUERY_VIEW_DATACompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2053.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RRW3_GET_QUERY_VIEW_DATACompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RRW3_GET_QUERY_VIEW_DATACompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public RRWS_SX_AXIS_DATA[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RRWS_SX_AXIS_DATA[])(this.results[0]));
            }
        }
        
        /// <remarks/>
        public RRWS_SX_AXIS_INFO[] E_AXIS_INFO {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RRWS_SX_AXIS_INFO[])(this.results[1]));
            }
        }
        
        /// <remarks/>
        public RRWS_S_CELL[] E_CELL_DATA {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RRWS_S_CELL[])(this.results[2]));
            }
        }
        
        /// <remarks/>
        public RRWS_S_TEXT_SYMBOLS[] E_TXT_SYMBOLS {
            get {
                this.RaiseExceptionIfNecessary();
                return ((RRWS_S_TEXT_SYMBOLS[])(this.results[3]));
            }
        }
    }
}

#pragma warning restore 1591