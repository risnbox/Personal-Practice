using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Asp.net_Exercise.Models
{
    [MetadataType(typeof(SMeta))]
    public partial class Store
    {

    }
    public partial class SMeta
    {
        public string StoreName { get; set; }
        public int StoreId { get; set; }
        public string StoreAddress { get; set; }
        public string StoreTelNo { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Store> Member_Store { get; set; }
    }
    [MetadataType(typeof(P_C_TMeta))]
    public partial class Prod_Class_Type
    {

    }
    public partial class P_C_TMeta
    {
        [JsonIgnore]
        public virtual Class Class { get; set; }
        [JsonIgnore]
        public virtual Type Type { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}