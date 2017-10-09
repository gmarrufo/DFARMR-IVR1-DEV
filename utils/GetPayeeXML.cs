namespace DFARMR_IVR1.utils
{
    public class GetPayeeXML
    {
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class E_DIV_PAYEE
        {
            private E_DIV_PAYEEItem[] itemField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public E_DIV_PAYEEItem[] item
            {
                get
                {
                    return this.itemField;
                }
                set
                {
                    this.itemField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class E_DIV_PAYEEItem
        {
            private byte dIVISIONField;
            private string dIV_NAMEField;
            private uint pAYEEField;
            private string pAYEE_NAMEField;

            /// <remarks/>
            public byte DIVISION
            {
                get
                {
                    return this.dIVISIONField;
                }
                set
                {
                    this.dIVISIONField = value;
                }
            }

            /// <remarks/>
            public string DIV_NAME
            {
                get
                {
                    return this.dIV_NAMEField;
                }
                set
                {
                    this.dIV_NAMEField = value;
                }
            }

            /// <remarks/>
            public uint PAYEE
            {
                get
                {
                    return this.pAYEEField;
                }
                set
                {
                    this.pAYEEField = value;
                }
            }

            /// <remarks/>
            public string PAYEE_NAME
            {
                get
                {
                    return this.pAYEE_NAMEField;
                }
                set
                {
                    this.pAYEE_NAMEField = value;
                }
            }
        }
    }
}