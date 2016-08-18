using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CartPayment
{
    public class VM_Installment
    {
        public int ID { get; set; }

        public int Value { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }
    }
}
