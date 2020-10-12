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
        public class prod_img
        {
            public Product prod { get; set; }
            public Img img { get; set; }
            public void Addd(Product P,Img M)
            {
                prod = P;
                img = M;
            }
        }
        public class Paydata
        {
            //參數詳情請至綠界API文件https://www.ecpay.com.tw/Content/files/ecpay_011.pdf
            public string MerchantID = "2000132";//6
            public string MerchantTradeNo { get; set; }//8
            public string MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//7
            public string PaymentType = "aio";//9
            public string TotalAmount { get; set; }//11
            public string TradeDesc = "金流測試";//12
            public string ItemName { get; set; }//5
            public string ReturnURL = "https://asptest.ml:45678/cart/VerifyPay";//10
            public string ChoosePayment = "Credit";//2
            public string CheckMacValue { get; set; }//1
            public string ClientBackURL = "https://asptest.ml:45678/member/orderview";//3
            public string EncryptType = "1";//4
            public void CreateCheckMacValue(obj.Paydata Pay)
            {
                string body = Pay.ToString();
                string data = string.Format("HashKey=5294y06JbISpM5x9&{0}&HashIV=v77hoKGq4kWxNNIS", body);
                data = HttpUtility.UrlEncode(data).ToLower();
                byte[] result = Encoding.Default.GetBytes(data);
                using (SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider())
                {
                    result = sha.ComputeHash(result);
                }
                string str = "";
                for(int i = 0; i < result.Length; i++)
                {
                    str += result[i].ToString("X2");
                }
                CheckMacValue = str.ToUpper();
            }
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
    }
}