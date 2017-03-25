using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComLibrary
{
    public class BaseResponse
    {
        public string orderId;
        public string bankOrderNo;
        public DateTime completeTime;
        public string returnCode;
        public string returnMsg;
        public string respCode;
        public string respMsg;
        public string status;
        public BaseResponse() { }
        public BaseResponse(string orderId, string bankOrderNo, DateTime completeTime,string returnCode,string returnMsg,string respCode,string respMsg,string status)
        {
            this.orderId = orderId;
            this.bankOrderNo = bankOrderNo;
            this.completeTime = completeTime;
            this.returnCode = returnCode;
            this.returnMsg = returnMsg;
            this.respCode = respCode;
            this.respMsg = respMsg;
            this.status = status;
        }
    }
}
