//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Asp.net_Exercise.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShoppingCar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShoppingCar()
        {
            this.Cart_Detail = new HashSet<Cart_Detail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Userid { get; set; }
        public string Pay { get; set; }
        public string Guid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart_Detail> Cart_Detail { get; set; }
        public virtual Member Member { get; set; }
    }
}