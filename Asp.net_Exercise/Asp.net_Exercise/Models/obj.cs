using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace Asp.net_Exercise.Models
{
    public class obj
    {
        public class Postback
        {
            public string CheckMacValue { get; set; }
            public string TradeNo { get; set; }
            public string MerchantTradeNo { get; set; }
            public string PaymentDate { get; set; }
        }
        public class Paydata
        {
            //參數詳情請至綠界API文件https://www.ecpay.com.tw/Content/files/ecpay_011.pdf
            public string MerchantID = "2000132";
            public string MerchantTradeNo { get; set; }
            public string MerchantTradeDate = DateTime.UtcNow.AddHours(08).ToString("yyyy/MM/dd HH:mm:ss");//由於主機在雲端所以取得國際時間在換算時區
            public string PaymentType = "aio";
            public string TotalAmount { get; set; }
            public string TradeDesc = "金流測試";
            public string ItemName { get; set; }
            public string ReturnURL = "https://aspnetexercise.azurewebsites.net/cart/VerifyPay"; 
            public string ChoosePayment = "Credit";
            public string CheckMacValue { get; set; }
            public string ClientBackURL = "https://aspnetexercise.azurewebsites.net/cart/RedirectView";
            public string EncryptType = "1";

            public override string ToString()
            {
                string str = "ChoosePayment=" + ChoosePayment + "&ClientBackURL="+ ClientBackURL + "&EncryptType=" + EncryptType + "&ItemName=" + ItemName + "&MerchantID=" + MerchantID +
                    "&MerchantTradeDate=" + MerchantTradeDate + "&MerchantTradeNo=" + MerchantTradeNo + "&PaymentType=" + PaymentType + "&ReturnURL=" +
                    ReturnURL + "&TotalAmount=" + TotalAmount + "&TradeDesc=" + TradeDesc;
                return str;
            }
            public string ToUrl()
            {
                string str = "ChoosePayment=" + ChoosePayment + "&ClientBackURL=" + ClientBackURL + "&EncryptType=" + EncryptType + "&ItemName=" + ItemName + "&MerchantID=" + MerchantID +
                    "&MerchantTradeDate=" + MerchantTradeDate + "&MerchantTradeNo=" + MerchantTradeNo + "&PaymentType=" + PaymentType + "&ReturnURL=" +
                    ReturnURL + "&TotalAmount=" + TotalAmount + "&TradeDesc=" + TradeDesc + "&CheckMacValue=" + CheckMacValue;
                return str;
            }
        }
        public class SearchOrder
        {
            public string MerchantID = "2000132";
            public string MerchantTradeNo { get; set; }
            public string TimeStamp { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            public string CheckMacValue { get; set; }
        }
        public string CreateCheckMacValue(Paydata Pay)//綠界加密方式
        {
            string body = Pay.ToString();//透過覆寫方法將參數依照A-Z排序後轉為字串
            string data = string.Format("HashKey=5294y06JbISpM5x9&{0}&HashIV=v77hoKGq4kWxNNIS", body);
            data = HttpUtility.UrlEncode(data).ToLower();
            byte[] result = Encoding.Default.GetBytes(data);
            using (SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider())
            {
                result = sha.ComputeHash(result);
            }
            string str = "";
            for (int i = 0; i < result.Length; i++)
            {
                str += result[i].ToString("X2");
            }
            return str.ToUpper();
        }
    }
}