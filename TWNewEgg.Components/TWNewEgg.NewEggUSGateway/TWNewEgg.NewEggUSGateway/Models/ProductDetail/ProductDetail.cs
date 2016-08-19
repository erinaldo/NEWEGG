using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NewEggService.Models
{
    public class ProductDetail
    {
        public ProductDetailBasic Basic { get; set; }
        public ProductDetailAdditional Additional { get; set; }
        public ProductDetailCoremetricsInfo CoremetricsInfo { get; set; }
    }
}
