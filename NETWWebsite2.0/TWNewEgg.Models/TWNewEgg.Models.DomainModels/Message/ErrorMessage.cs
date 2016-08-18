using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.Models.DomainModels.Message
{
    public class ErrorMessage
    {
        public int Code { get; set; }
        public string Detail { get; set; }
    }
}
