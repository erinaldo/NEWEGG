using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("answr")]
    public class Answer
    {
        public Answer()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            Date = defaultDate;
            CreateDate = defaultDate;
            UpdateDate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public string PrblmCode { get; set; }
        public string Cont { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string AnswrCode { get; set; }
        public string ProcessUser { get; set; }
        public string ToUser { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}