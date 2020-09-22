using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}