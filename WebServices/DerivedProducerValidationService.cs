﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace DFARMR_IVR1
{
    public class DerivedProducerValidationService : ProducerValidationService
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri);
            webRequest.KeepAlive = false;
            webRequest.PreAuthenticate = true;
            return webRequest;
        }
    }
}