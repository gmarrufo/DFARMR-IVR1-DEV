﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DFARMR_IVR1.GetDivPayeeDetails {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType")]
    public interface Z_WS_GET_DIV_PAYEE_DETAILSPortType {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.sap.com/Z_WS_GET_DIV_PAYEE_DETAILS", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSResponse Z_WS_GET_DIV_PAYEE_DETAILS(DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.sap.com/Z_WS_GET_DIV_PAYEE_DETAILS", ReplyAction="*")]
        System.Threading.Tasks.Task<DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSResponse> Z_WS_GET_DIV_PAYEE_DETAILSAsync(DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1087.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZDIV_PAYEE_DETAILS : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string dIVISIONField;
        
        private string dIV_NAMEField;
        
        private string pAYEEField;
        
        private string pAYEE_NAMEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string DIVISION {
            get {
                return this.dIVISIONField;
            }
            set {
                this.dIVISIONField = value;
                this.RaisePropertyChanged("DIVISION");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string DIV_NAME {
            get {
                return this.dIV_NAMEField;
            }
            set {
                this.dIV_NAMEField = value;
                this.RaisePropertyChanged("DIV_NAME");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PAYEE {
            get {
                return this.pAYEEField;
            }
            set {
                this.pAYEEField = value;
                this.RaisePropertyChanged("PAYEE");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string PAYEE_NAME {
            get {
                return this.pAYEE_NAMEField;
            }
            set {
                this.pAYEE_NAMEField = value;
                this.RaisePropertyChanged("PAYEE_NAME");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Z_WS_GET_DIV_PAYEE_DETAILS", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class Z_WS_GET_DIV_PAYEE_DETAILSRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string I_LOGINID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string I_MSA;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string I_PAYEE;
        
        public Z_WS_GET_DIV_PAYEE_DETAILSRequest() {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSRequest(string I_LOGINID, string I_MSA, string I_PAYEE) {
            this.I_LOGINID = I_LOGINID;
            this.I_MSA = I_MSA;
            this.I_PAYEE = I_PAYEE;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Z_WS_GET_DIV_PAYEE_DETAILS.Response", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class Z_WS_GET_DIV_PAYEE_DETAILSResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public DFARMR_IVR1.GetDivPayeeDetails.ZDIV_PAYEE_DETAILS[] E_DIV_PAYEE;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string E_STATUS;
        
        public Z_WS_GET_DIV_PAYEE_DETAILSResponse() {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSResponse(DFARMR_IVR1.GetDivPayeeDetails.ZDIV_PAYEE_DETAILS[] E_DIV_PAYEE, string E_STATUS) {
            this.E_DIV_PAYEE = E_DIV_PAYEE;
            this.E_STATUS = E_STATUS;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Z_WS_GET_DIV_PAYEE_DETAILSPortTypeChannel : DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient : System.ServiceModel.ClientBase<DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType>, DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType {
        
        public Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient() {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Z_WS_GET_DIV_PAYEE_DETAILSPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSResponse DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType.Z_WS_GET_DIV_PAYEE_DETAILS(DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest request) {
            return base.Channel.Z_WS_GET_DIV_PAYEE_DETAILS(request);
        }
        
        public DFARMR_IVR1.GetDivPayeeDetails.ZDIV_PAYEE_DETAILS[] Z_WS_GET_DIV_PAYEE_DETAILS(string I_LOGINID, string I_MSA, string I_PAYEE, out string E_STATUS) {
            DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest inValue = new DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest();
            inValue.I_LOGINID = I_LOGINID;
            inValue.I_MSA = I_MSA;
            inValue.I_PAYEE = I_PAYEE;
            DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSResponse retVal = ((DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSPortType)(this)).Z_WS_GET_DIV_PAYEE_DETAILS(inValue);
            E_STATUS = retVal.E_STATUS;
            return retVal.E_DIV_PAYEE;
        }
        
        public System.Threading.Tasks.Task<DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSResponse> Z_WS_GET_DIV_PAYEE_DETAILSAsync(DFARMR_IVR1.GetDivPayeeDetails.Z_WS_GET_DIV_PAYEE_DETAILSRequest request) {
            return base.Channel.Z_WS_GET_DIV_PAYEE_DETAILSAsync(request);
        }
    }
}
