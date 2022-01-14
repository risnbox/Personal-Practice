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
        public class Postback//綠界回傳參數類型
        {
            public string CheckMacValue { get; set; }
            public string TradeNo { get; set; }
            public string MerchantTradeNo { get; set; }
            public string PaymentDate { get; set; }
        }
        public class Paydata
        {
            //參數詳情請至綠界API文件https://www.ecpay.com.tw/Content/files/ecpay_011.pdf
            public string MerchantID = "2000214";
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

            public override string ToString()//按照綠界規範排序字串
            {
                string str = "ChoosePayment=" + ChoosePayment + "&ClientBackURL="+ ClientBackURL + "&EncryptType=" + EncryptType + "&ItemName=" + ItemName + "&MerchantID=" + MerchantID +
                    "&MerchantTradeDate=" + MerchantTradeDate + "&MerchantTradeNo=" + MerchantTradeNo + "&PaymentType=" + PaymentType + "&ReturnURL=" +
                    ReturnURL + "&TotalAmount=" + TotalAmount + "&TradeDesc=" + TradeDesc;
                return str;
            }
        }
        public string CreateCheckMacValue(Paydata Pay)//綠界加密方式
        {
            string body = Pay.ToString();//透過覆寫方法將參數依照A-Z排序後轉為字串
            string data = string.Format("HashKey=5294y06JbISpM5x9&{0}&HashIV=v77hoKGq4kWxNNIS", body);
            data = HttpUtility.UrlEncode(data).ToLower();
            byte[] result = Encoding.Default.GetBytes(data);//將字串轉為byte後使用SHA256加密
            using (SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider())
            {
                result = sha.ComputeHash(result);
            }
            string str = "";
            for (int i = 0; i < result.Length; i++)
            {
                str += result[i].ToString("X2");//ToString("X2") 為C#中的字串格式控制符，X為十六進位，2為每次都是兩位數
            }
            return str.ToUpper();
        }
        public class Postdata//WebAPI無法解析POST參數，須建立參考否則將顯示該控制器無此動作
        {
            public string Gid { get; set; }//GoogleUserId
            public string email { get; set; }
            public string name { get; set; }
            public string gender { get; set; }//嘗試存取使用者公開資料，若使用者未公開或未填則null
            public string phone { get; set; }//嘗試存取使用者公開資料，若使用者未公開或未填則null
            public string id_token { get; set; }//使用後端存取才有用
            public string psw { get; set; }
        }
        public class Fbdata
        {
            public string Name { get; set; }
            public string Fbid { get; set; }
            public string Email { get; set; }
        }
    }
}