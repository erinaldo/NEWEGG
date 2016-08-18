using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Product
{
    public class ComplexProduntDM
    {
        public byte byteTest { get; set; }
        public short shortTest { get; set; }
        public int intTest { get; set; }
        public long longTest { get; set; }
        public float floatTest { get; set; }
        public double doubleTest { get; set; }
        public decimal decimalTest { get; set; }
        public string stringTest { get; set; }
        public bool boolTest { get; set; }
        public DateTime dateTimeTest { get; set; }

        public Nullable<byte> byteNullTest { get; set; }
        public Nullable<short> shortNullTest { get; set; }
        public Nullable<int> intNullTest { get; set; }
        public Nullable<long> longNullTest { get; set; }
        public Nullable<float> floatNullTest { get; set; }
        public Nullable<double> doubleNullTest { get; set; }
        public Nullable<decimal> decimalNullTest { get; set; }
        public Nullable<bool> boolNullTest { get; set; }
        public Nullable<DateTime> dateTimeNullTest { get; set; }

        public List<ComplexProduntDM> complexProductDMTest { get; set; }
        public List<ComplexProduntDM> complexProductDMNullTest { get; set; }
    }
}
