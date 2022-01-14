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
        public partial class SMeta
        {
            public string StoreName { get; set; }
            public int StoreId { get; set; }
            public string StoreAddress { get; set; }
            public string StoreTelNo { get; set; }
            [JsonIgnore]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
            public virtual ICollection<Member_Store> Member_Store { get; set; }
            [JsonIgnore]
            public virtual ICollection<Order> Order { get; set; }
        }
    }
    [MetadataType(typeof(P_TMeta))]
    public partial class Prod_Type
    {
        public partial class P_TMeta
        {
            [JsonIgnore]
            public virtual Type Type { get; set; }
            [JsonIgnore]
            public virtual Product Product { get; set; }
        }
    }

    [MetadataType(typeof(MetaImg))]
    public partial class Img
    {
        public partial class MetaImg
        {
            [JsonIgnore]
            public virtual ICollection<Prod_Img> Prod_Img { get; set; }
        }
    }
    [MetadataType(typeof(MetaProd))]
    public partial class Product
    {
        public partial class MetaProd
        {
            [JsonIgnore]
            public virtual ICollection<Prod_Img> Prod_Img { get; set; }
            [JsonIgnore]
            public virtual ICollection<Prod_Type> Prod_Type { get; set; }
            [JsonIgnore]
            public virtual ICollection<Keep> Keep { get; set; }
            [JsonIgnore]
            public virtual ICollection<ProdFeature> ProdFeature { get; set; }
        }
    }

    [MetadataType(typeof(MetaCar))]
    public partial class ShoppingCar
    {
        public partial class MetaCar
        {
            [JsonIgnore]
            public virtual Member Member { get; set; }
        }
    }
    [MetadataType(typeof(MetaQty))]
    public partial class Quantity
    {
        public partial class MetaQty
        {
            [JsonIgnore]
            public virtual ShoppingCar ShoppingCar { get; set; }
            [JsonIgnore]
            public virtual ProdFeature ProdFeature { get; set; }
        }
    }
    [MetadataType(typeof(MetaColor))]
    public partial class Color
    {
        public class MetaColor
        {
            [JsonIgnore]
            public virtual ICollection<ProdFeature> ProdFeature { get; set; }
        }
    }
    [MetadataType(typeof(MetaSize))]
    public partial class Size
    {
        public class MetaSize
        {
            [JsonIgnore]
            public virtual ICollection<ProdFeature> ProdFeature { get; set; }
        }
    }
}