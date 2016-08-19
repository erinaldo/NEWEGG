using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models.Models
{
    public class ItemCreationResult
    {
        public ResponseCode SaveDBResult { get; set; }

        public string ResultData { get; set; }
    }
}
