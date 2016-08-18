using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 投票記錄
    /// </summary>
    [Table("VotingActivityRec")]
    public class VotingActivityRec
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 投票活動的GroupId, refer RateActivityGroup.Id
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public int GroupId { get; set; }

        /// <summary>
        /// 帳號Id, 因各家帳號儲存方式不同, 故以String儲存
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public string AccountId { get; set; }

        /// <summary>
        /// 帳號來源, Newegg, FB, Yahoo, Google+, etc.
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public string AccountSource { get; set; }

        /// <summary>
        /// 投票日期, yyyy/MM/dd
        /// </summary>
        [Key]
        [Column(Order = 3)]
        public string VoteDate { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 以JSON格式儲存的投票記錄
        /// </summary>
        public string Rec { get; set; }
    }
}
