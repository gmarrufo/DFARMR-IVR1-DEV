namespace DFARMR_IVR1.utils
{
    public class GetPasswordXML
    {
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:sap-com:document:sap:rfc:functions")]
        [System.Xml.Serialization.XmlRootAttribute("Z_WS_DOES_PASSWORD_EXIST_MSA.Response", Namespace = "urn:sap-com:document:sap:rfc:functions", IsNullable = false)]
        public partial class Z_WS_DOES_PASSWORD_EXIST_MSAResponse
        {
            private uint e_MSAField;
            private ushort e_PINField;
            private byte e_STATUSField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public uint E_MSA
            {
                get
                {
                    return this.e_MSAField;
                }
                set
                {
                    this.e_MSAField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public ushort E_PIN
            {
                get
                {
                    return this.e_PINField;
                }
                set
                {
                    this.e_PINField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public byte E_STATUS
            {
                get
                {
                    return this.e_STATUSField;
                }
                set
                {
                    this.e_STATUSField = value;
                }
            }
        }
    }
}