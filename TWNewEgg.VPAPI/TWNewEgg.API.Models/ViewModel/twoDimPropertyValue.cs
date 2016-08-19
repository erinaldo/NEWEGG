using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models.ViewModel
{
    public class twoDimPropertyValue
    {
        public class StandPropertyValue
        {
            public int ValueID { get; set; }

            public string Value { get; set; }
        }


        public twoDimPropertyValue()
        {
            PropertyValues = new List<StandPropertyValue>();
        }

        public string PropertyName { get; set; }

        public List<StandPropertyValue> PropertyValues { get; set; }


    }
}
