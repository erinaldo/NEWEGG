using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Answer
{
    public class AnswerViewModel
    {  
        public string Code { get; set; }
        public string RecvName { get; set; }
        public string RecvMobile { get; set; }
        public string Email { get; set; }
        public int ItemID { get; set; }
        public bool rtgood { get; set; }
    }
}
