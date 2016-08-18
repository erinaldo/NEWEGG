using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class ItemXMLProperties
    {

        public int PropertyCode { get; set; }
        public string PropertyName { get; set; }
        public string ValueCode { get; set; }

        public string UserInputed { get; set; }



    
    }
}
