namespace DFARMR_IVR1.utils
{
    public class GetPaymentsXML
    {
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class E_AXIS_DATA
        {
            private E_AXIS_DATAItem[] itemField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public E_AXIS_DATAItem[] item
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
        public partial class E_AXIS_DATAItem
        {
            private byte aXISField;
            private E_AXIS_DATAItemItem[] sETField;

            /// <remarks/>
            public byte AXIS
            {
                get
                {
                    return this.aXISField;
                }
                set
                {
                    this.aXISField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
            public E_AXIS_DATAItemItem[] SET
            {
                get
                {
                    return this.sETField;
                }
                set
                {
                    this.sETField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class E_AXIS_DATAItemItem
        {
            private byte tUPLE_ORDINALField;
            private string cHANMField;
            private string cAPTIONField;
            private string cHAVLField;
            private string cHAVL_EXTField;
            private object nIOBJNMField;
            private byte tLEVELField;
            private object dRILLSTATEField;
            private object oPTField;
            private object sIGNField;
            private object aTTRIBUTESField;

            /// <remarks/>
            public byte TUPLE_ORDINAL
            {
                get
                {
                    return this.tUPLE_ORDINALField;
                }
                set
                {
                    this.tUPLE_ORDINALField = value;
                }
            }

            /// <remarks/>
            public string CHANM
            {
                get
                {
                    return this.cHANMField;
                }
                set
                {
                    this.cHANMField = value;
                }
            }

            /// <remarks/>
            public string CAPTION
            {
                get
                {
                    return this.cAPTIONField;
                }
                set
                {
                    this.cAPTIONField = value;
                }
            }

            /// <remarks/>
            public string CHAVL
            {
                get
                {
                    return this.cHAVLField;
                }
                set
                {
                    this.cHAVLField = value;
                }
            }

            /// <remarks/>
            public string CHAVL_EXT
            {
                get
                {
                    return this.cHAVL_EXTField;
                }
                set
                {
                    this.cHAVL_EXTField = value;
                }
            }

            /// <remarks/>
            public object NIOBJNM
            {
                get
                {
                    return this.nIOBJNMField;
                }
                set
                {
                    this.nIOBJNMField = value;
                }
            }

            /// <remarks/>
            public byte TLEVEL
            {
                get
                {
                    return this.tLEVELField;
                }
                set
                {
                    this.tLEVELField = value;
                }
            }

            /// <remarks/>
            public object DRILLSTATE
            {
                get
                {
                    return this.dRILLSTATEField;
                }
                set
                {
                    this.dRILLSTATEField = value;
                }
            }

            /// <remarks/>
            public object OPT
            {
                get
                {
                    return this.oPTField;
                }
                set
                {
                    this.oPTField = value;
                }
            }

            /// <remarks/>
            public object SIGN
            {
                get
                {
                    return this.sIGNField;
                }
                set
                {
                    this.sIGNField = value;
                }
            }

            /// <remarks/>
            public object ATTRIBUTES
            {
                get
                {
                    return this.aTTRIBUTESField;
                }
                set
                {
                    this.aTTRIBUTESField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class E_AXIS_INFO
        {
            private E_AXIS_INFOItem[] itemField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public E_AXIS_INFOItem[] item
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
        public partial class E_AXIS_INFOItem
        {
            private byte aXISField;
            private byte nCHARSField;
            private byte nCOORDSField;
            private E_AXIS_INFOItemItem[] cHARSField;

            /// <remarks/>
            public byte AXIS
            {
                get
                {
                    return this.aXISField;
                }
                set
                {
                    this.aXISField = value;
                }
            }

            /// <remarks/>
            public byte NCHARS
            {
                get
                {
                    return this.nCHARSField;
                }
                set
                {
                    this.nCHARSField = value;
                }
            }

            /// <remarks/>
            public byte NCOORDS
            {
                get
                {
                    return this.nCOORDSField;
                }
                set
                {
                    this.nCOORDSField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
            public E_AXIS_INFOItemItem[] CHARS
            {
                get
                {
                    return this.cHARSField;
                }
                set
                {
                    this.cHARSField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class E_AXIS_INFOItemItem
        {
            private string cHANMField;
            private object hIENMField;
            private object vERSIONField;
            private string dATETOField;
            private string cAPTIONField;
            private byte cHAPRSNTField;
            private byte cHATYPField;
            private object aTTRINMField;

            /// <remarks/>
            public string CHANM
            {
                get
                {
                    return this.cHANMField;
                }
                set
                {
                    this.cHANMField = value;
                }
            }

            /// <remarks/>
            public object HIENM
            {
                get
                {
                    return this.hIENMField;
                }
                set
                {
                    this.hIENMField = value;
                }
            }

            /// <remarks/>
            public object VERSION
            {
                get
                {
                    return this.vERSIONField;
                }
                set
                {
                    this.vERSIONField = value;
                }
            }

            /// <remarks/>
            public string DATETO
            {
                get
                {
                    return this.dATETOField;
                }
                set
                {
                    this.dATETOField = value;
                }
            }

            /// <remarks/>
            public string CAPTION
            {
                get
                {
                    return this.cAPTIONField;
                }
                set
                {
                    this.cAPTIONField = value;
                }
            }

            /// <remarks/>
            public byte CHAPRSNT
            {
                get
                {
                    return this.cHAPRSNTField;
                }
                set
                {
                    this.cHAPRSNTField = value;
                }
            }

            /// <remarks/>
            public byte CHATYP
            {
                get
                {
                    return this.cHATYPField;
                }
                set
                {
                    this.cHATYPField = value;
                }
            }

            /// <remarks/>
            public object ATTRINM
            {
                get
                {
                    return this.aTTRINMField;
                }
                set
                {
                    this.aTTRINMField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class E_CELL_DATA
        {
            private E_CELL_DATAItem[] itemField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public E_CELL_DATAItem[] item
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
        public partial class E_CELL_DATAItem
        {
            private byte cELL_ORDINALField;
            private decimal vALUEField;
            private string fORMATTED_VALUEField;
            private object vALUE_TYPEField;
            private string cURRENCYField;
            private object uNITField;
            private string mWKZField;
            private byte nUM_SCALEField;
            private byte nUM_PRECField;
            private object cELL_STATUSField;
            private byte bACK_COLORField;

            /// <remarks/>
            public byte CELL_ORDINAL
            {
                get
                {
                    return this.cELL_ORDINALField;
                }
                set
                {
                    this.cELL_ORDINALField = value;
                }
            }

            /// <remarks/>
            public decimal VALUE
            {
                get
                {
                    return this.vALUEField;
                }
                set
                {
                    this.vALUEField = value;
                }
            }

            /// <remarks/>
            public string FORMATTED_VALUE
            {
                get
                {
                    return this.fORMATTED_VALUEField;
                }
                set
                {
                    this.fORMATTED_VALUEField = value;
                }
            }

            /// <remarks/>
            public object VALUE_TYPE
            {
                get
                {
                    return this.vALUE_TYPEField;
                }
                set
                {
                    this.vALUE_TYPEField = value;
                }
            }

            /// <remarks/>
            public string CURRENCY
            {
                get
                {
                    return this.cURRENCYField;
                }
                set
                {
                    this.cURRENCYField = value;
                }
            }

            /// <remarks/>
            public object UNIT
            {
                get
                {
                    return this.uNITField;
                }
                set
                {
                    this.uNITField = value;
                }
            }

            /// <remarks/>
            public string MWKZ
            {
                get
                {
                    return this.mWKZField;
                }
                set
                {
                    this.mWKZField = value;
                }
            }

            /// <remarks/>
            public byte NUM_SCALE
            {
                get
                {
                    return this.nUM_SCALEField;
                }
                set
                {
                    this.nUM_SCALEField = value;
                }
            }

            /// <remarks/>
            public byte NUM_PREC
            {
                get
                {
                    return this.nUM_PRECField;
                }
                set
                {
                    this.nUM_PRECField = value;
                }
            }

            /// <remarks/>
            public object CELL_STATUS
            {
                get
                {
                    return this.cELL_STATUSField;
                }
                set
                {
                    this.cELL_STATUSField = value;
                }
            }

            /// <remarks/>
            public byte BACK_COLOR
            {
                get
                {
                    return this.bACK_COLORField;
                }
                set
                {
                    this.bACK_COLORField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class E_TXT_SYMBOLS
        {
            private E_TXT_SYMBOLSItem[] itemField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public E_TXT_SYMBOLSItem[] item
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
        public partial class E_TXT_SYMBOLSItem
        {
            private string sYM_TYPEField;
            private object sYM_TIMEDEPField;
            private string sYM_NAMEField;
            private byte sYM_FSField;
            private string sYM_BEGIN_GROUPField;
            private string sYM_CAPTIONField;
            private string sYM_VALUE_TYPEField;
            private string sYM_VALUEField;
            private byte sYM_OUTPUTLENField;

            /// <remarks/>
            public string SYM_TYPE
            {
                get
                {
                    return this.sYM_TYPEField;
                }
                set
                {
                    this.sYM_TYPEField = value;
                }
            }

            /// <remarks/>
            public object SYM_TIMEDEP
            {
                get
                {
                    return this.sYM_TIMEDEPField;
                }
                set
                {
                    this.sYM_TIMEDEPField = value;
                }
            }

            /// <remarks/>
            public string SYM_NAME
            {
                get
                {
                    return this.sYM_NAMEField;
                }
                set
                {
                    this.sYM_NAMEField = value;
                }
            }

            /// <remarks/>
            public byte SYM_FS
            {
                get
                {
                    return this.sYM_FSField;
                }
                set
                {
                    this.sYM_FSField = value;
                }
            }

            /// <remarks/>
            public string SYM_BEGIN_GROUP
            {
                get
                {
                    return this.sYM_BEGIN_GROUPField;
                }
                set
                {
                    this.sYM_BEGIN_GROUPField = value;
                }
            }

            /// <remarks/>
            public string SYM_CAPTION
            {
                get
                {
                    return this.sYM_CAPTIONField;
                }
                set
                {
                    this.sYM_CAPTIONField = value;
                }
            }

            /// <remarks/>
            public string SYM_VALUE_TYPE
            {
                get
                {
                    return this.sYM_VALUE_TYPEField;
                }
                set
                {
                    this.sYM_VALUE_TYPEField = value;
                }
            }

            /// <remarks/>
            public string SYM_VALUE
            {
                get
                {
                    return this.sYM_VALUEField;
                }
                set
                {
                    this.sYM_VALUEField = value;
                }
            }

            /// <remarks/>
            public byte SYM_OUTPUTLEN
            {
                get
                {
                    return this.sYM_OUTPUTLENField;
                }
                set
                {
                    this.sYM_OUTPUTLENField = value;
                }
            }
        }
    }
}