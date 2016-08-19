using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Logistics.HiLife.Model
{
    public class HiLifeBasicFieldStruct
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public HiLifeBasic.Align Align { get; set; }
        public char Symbol { get; set; }
        public string StaticValue { get; set; }
        public bool HasStaticValue { get; set; }
    }
}
