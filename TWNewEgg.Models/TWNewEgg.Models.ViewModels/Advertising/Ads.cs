using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Advertising

{
    /*
     * [System.Web.Mvc.Bind(Exclude = "as_adsetcode, ads_sn")]
     * [AllowHtml]
     * �]�ݩ�System.Web.Mvc�R�W�Ŷ��������O
     * WMS �� TWBackend_WCF �b�ѷ� System.Web.Mvc �R�W�Ŷ���
     * �|����v�T�A�ɭPWCF���^������ܦ� text/html
     * �ӫDSOAP�w���� xml
     * �G�Ȯɥ��N�U���o�ӳ�������
     */
    //[System.Web.Mvc.Bind(Exclude = "as_adsetcode, ads_sn")]
    public class Ads
    {
        public enum OnlineStatusOption
        {
            Offline = 0,
            Online = 1
        }

        private string m_str_ads_html = "";
        private string m_str_ads_content = "";
        private string m_str_ads_updateuser = "";

        public int Id { get; set; }

        public string AdsetCode { get; set; }
        public string ImageTitle { get; set; }
        public string Link { get; set; }
        public string ImageAlt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrackId { get; set; }
        public int OnlineStatus { get; set; }
        public string STitle { get; set; }
        public string ImageUrl { get; set; }
        
        public string AdsContent
        {
            get
            {
                return HttpUtility.HtmlDecode(this.m_str_ads_content);
            }
            set
            {
                this.m_str_ads_content = HttpUtility.HtmlEncode(value);
            }
        }//end ads_content

        public String CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
