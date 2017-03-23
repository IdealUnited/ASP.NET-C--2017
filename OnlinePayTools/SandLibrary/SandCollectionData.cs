using System;
using System.Collections.Generic;
using System.Text;
using ComLibrary;

namespace SandLibrary
{
    class SandCollectionData
    {
        string version;

        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        string productId;

        public string ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        string tranTime;

        public string TranTime
        {
            get { return tranTime; }
            set { tranTime = value; }
        }
        string orderCode;

        public string OrderCode
        {
            get { return orderCode; }
            set { orderCode = value; }
        }
        string tranAmt;

        public string TranAmt
        {
            get { return tranAmt; }
            set { tranAmt = value; }
        }
        string currencyCode;

        public string CurrencyCode
        {
            get { return currencyCode; }
            set { currencyCode = value; }
        }
        string accAttr;

        public string AccAttr
        {
            get { return accAttr; }
            set { accAttr = value; }
        }
        string accType;

        public string AccType
        {
            get { return accType; }
            set { accType = value; }
        }
        string accNo;

        public string AccNo
        {
            get { return accNo; }
            set { accNo = value; }
        }
        string accName;

        public string AccName
        {
            get { return accName; }
            set { accName = value; }
        }
        string bankName;

        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }
        string provNo;

        public string ProvNo
        {
            get { return provNo; }
            set { provNo = value; }
        }
        string cityNo;

        public string CityNo
        {
            get { return cityNo; }
            set { cityNo = value; }
        }
        string certType;

        public string CertType
        {
            get { return certType; }
            set { certType = value; }
        }
        string certNo;

        public string CertNo
        {
            get { return certNo; }
            set { certNo = value; }
        }
        string cardId;

        public string CardId
        {
            get { return cardId; }
            set { cardId = value; }
        }
        string phone;

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        string bankInsCode;

        public string BankInsCode
        {
            get { return bankInsCode; }
            set { bankInsCode = value; }
        }
        string purpose;

        public string Purpose
        {
            get { return purpose; }
            set { purpose = value; }
        }
        string reqReserved;

        public string ReqReserved
        {
            get { return reqReserved; }
            set { reqReserved = value; }
        }
        string extend;

        public string Extend
        {
            get { return extend; }
            set { extend = value; }
        }

        string toJson() {
            return JsonUtil.ObjectToJson(this);
        }
    }
}
