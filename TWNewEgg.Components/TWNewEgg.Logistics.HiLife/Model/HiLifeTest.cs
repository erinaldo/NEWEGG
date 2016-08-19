using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Xml;
using TWNewEgg.Logistics.HiLife.Service;

namespace TWNewEgg.Logistics.HiLife.Model
{
    public class HiLifeTest:HiLifeBasic
    {
        private Dictionary<string, List<HiLifeBasicFieldStruct>> m_objDict = null;

        public HiLifeTest()
        {
            this.m_objDict = this.MapHiLifeField(@"D:\Work\Web\IPP\Configurations\HiLifeConfiguration.xml", HiLifeFileType.F01);
        }

        public Dictionary<string, List<HiLifeBasicFieldStruct>>  GetDict
        {
            get
            {
                return this.m_objDict;
            }
        }

        

    }
}
