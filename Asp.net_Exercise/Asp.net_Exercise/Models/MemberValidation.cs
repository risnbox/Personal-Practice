using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Asp.net_Exercise.Models;

namespace Asp.net_Exercise.Models
{
    /*
    使用System.CompnentModel.DataAnnotations命名空間內的MetadataType將驗證規則寫在另一個類別檔
    用意是避免更新ADO.Net時將驗證規則覆蓋掉導致重複動作,MetadataType再使用時要記得Typeof(),要記得加上partial(部分)這樣才能跟資料實體同步
    */
    [MetadataType(typeof(MemberValidation))]
    public partial class Member
    {
        public class MemberValidation
        {
            public int Id { get; set; }
            [DisplayName("姓名")]
            [Required(ErrorMessage = "姓名為必填")]
            public string Name { get; set; }
            [DisplayName("信箱")]
            [Required(ErrorMessage = "信箱為必填")]
            [EmailAddress]
            public string Email { get; set; }
            [DisplayName("手機")]
            [RegularExpression(@"^\s*09{1}[0-9]{8}\s*$", ErrorMessage = "手機格式錯誤")]
            public string Phone { get; set; }
            [DisplayName("密碼")]
            [RegularExpression(@"^\s*(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,20}\s*$", ErrorMessage = "密碼必須包含大小寫字母及數字各1,長度為6~20位數")]
            public string Password { get; set; }
            [DisplayName("姓別")]
            public string Gender { get; set; }
            public DateTime? Joindate
            {
                get { return Joindate; }
                set { Joindate = DateTime.Now; }
            }
            [JsonIgnore]
            public virtual ICollection<ShoppingCar> ShoppingCar { get; set; }
            [JsonIgnore]
            public virtual ICollection<Keep> Keep { get; set; }
            [JsonIgnore]
            public virtual ICollection<Member_Store> Member_Store { get; set; }
            [JsonIgnore]
            public virtual ICollection<Order> Order { get; set; }
        }
    }

    [MetadataType(typeof(OrderValidation))]
    public partial class Order
    {
        public class OrderValidation
        {
            [Required(ErrorMessage = "收件人為必填")]
            public string Name { get; set; }
            [Required(ErrorMessage = "信箱為必填")]
            public string Email { get; set; }
            [Required(ErrorMessage = "手機為必填")]
            [RegularExpression(@"^\s*09{1}[0-9]{8}\s*$", ErrorMessage = "手機格式錯誤")]
            public int Phone { get; set; }
            [JsonIgnore]
            public virtual Member Member { get; set; }
            [JsonIgnore]
            public virtual Store Store { get; set; }
        }
    }
    public class EditPsw
    {
        [Required]
        public string oldpsw { get; set; }
        [Required]
        [RegularExpression(@"^\s*(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,20}\s*$", ErrorMessage = "密碼必須包含大小寫字母及數字各1,長度為6~20位數")]
        public string newpsw { get; set; }
        [Required]
        [Compare("newpsw",ErrorMessage ="確認密碼輸入不一致")]
        public string check { get; set; }
    }

    //使用enum列舉型別來建立下拉式選單需要的項目
    public enum GenderSelect
    {
        Male,
        Female
    }
    public enum CitySelect
    {
        台北市,
        基隆市,
        新北市,
        連江縣,
        宜蘭縣,
        新竹市,
        新竹縣,
        桃園市,
        苗栗縣,
        台中市,
        彰化縣,
        南投縣,
        嘉義市,
        嘉義縣,
        雲林縣,
        台南市,
        高雄市,
        澎湖縣,
        金門縣,
        屏東縣,
        台東縣,
        花蓮縣
    }
    public enum TypeSelect
    {
        請選擇=0,
        women=1,
        man=2,
        kids=3
    }
    //複製JSON字串再利用VisualStudio內建功能 選擇性貼上=>JSON格式
    //此為首層結構,通常不會用到此層
    public class Jsclass
    {
        public data1[] Property1 { get; set; }
    }

    public class data1//city
    {
        public data2[] districts { get; set; }
        public string name { get; set; }
    }

    public class data2//town
    {
        public string zip { get; set; }
        public string name { get; set; }
        public data3[] districts { get; set; }
    }

    public class data3
    {
        public string zip { get; set; }
        public string name { get; set; }
    }

}