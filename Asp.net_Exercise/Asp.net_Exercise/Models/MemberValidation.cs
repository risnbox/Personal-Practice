using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Asp.net_Exercise.Models
{
    /*
    使用System.CompnentModel.DataAnnotations命名空間內的MetadataType將驗證規則寫在另一個類別檔
    用意是避免更新ADO.Net時將驗證規則覆蓋掉導致重複動作,MetadataType再使用時要記得Typeof(),要記得加上partial(部分)這樣才能跟資料實體同步
    */
    [MetadataType(typeof(MemberValidation))]
    public partial class Member
    {

    }
    public partial class MemberValidation
    {
        public int Id { get; set; }
        [DisplayName("姓名")]
        [Required(ErrorMessage ="姓名為必填")]
        public string Name { get; set; }
        [DisplayName("信箱")]
        [Required(ErrorMessage ="信箱為必填")]
        [EmailAddress]
        public string Email { get; set; }
        [DisplayName("手機")]
        [Required(ErrorMessage ="手機為必填")]
        //關於正規表達式相關內容至Google書籤內有相關可用外掛!
        [RegularExpression(@"^\s*09{1}[0-9]{8}\s*$",ErrorMessage ="手機格式錯誤")]
        public string Phone { get; set; }
        [DisplayName("密碼")]
        [Required(ErrorMessage ="密碼為必填")]
        //關於正規表達式相關內容至Google書籤內有相關可用外掛!
        [RegularExpression(@"^\s*(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,20}\s*$",ErrorMessage ="密碼必須包含大小寫字母及數字各1,長度為6~20位數")]
        public string Password { get; set; }
        [DisplayName("姓別")]
        public string Gender { get; set; }
        [DisplayName("地址")]
        [Required(ErrorMessage = "地址為必填")]
        public string Address { get; set; }
    }
    //使用enum列舉型別來建立下拉式選單需要的項目
    public enum GenderSelect
    {
        Male,
        Female
    }
}