using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.Models.DomainModels.Finance
{
    [Serializable]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    [XmlRootAttribute(Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService", IsNullable = false, ElementName = "Publish")]
    public partial class PublishDM
    {

        private PublishPublish publish1Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Publish")]
        public PublishPublish Publish1
        {
            get
            {
                return this.publish1Field;
            }
            set
            {
                this.publish1Field = value;
            }
        }

        public PublishDM() { }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    public partial class PublishPublish
    {

        private string fromServiceField;

        private string toServiceField;

        private PublishPublishRouteTable routeTableField;

        private PublishPublishNode nodeField;

        /// <remarks/>
        public string FromService
        {
            get
            {
                return this.fromServiceField;
            }
            set
            {
                this.fromServiceField = value;
            }
        }

        /// <remarks/>
        public string ToService
        {
            get
            {
                return this.toServiceField;
            }
            set
            {
                this.toServiceField = value;
            }
        }

        /// <remarks/>
        public PublishPublishRouteTable RouteTable
        {
            get
            {
                return this.routeTableField;
            }
            set
            {
                this.routeTableField = value;
            }
        }

        /// <remarks/>
        public PublishPublishNode Node
        {
            get
            {
                return this.nodeField;
            }
            set
            {
                this.nodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    public partial class PublishPublishRouteTable
    {

        private PublishPublishRouteTableArticle articleField;

        /// <remarks/>
        public PublishPublishRouteTableArticle Article
        {
            get
            {
                return this.articleField;
            }
            set
            {
                this.articleField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    public partial class PublishPublishRouteTableArticle
    {

        private string articleCategoryField;

        private string articleType1Field;

        private string articleType2Field;

        /// <remarks/>
        public string ArticleCategory
        {
            get
            {
                return this.articleCategoryField;
            }
            set
            {
                this.articleCategoryField = value;
            }
        }

        /// <remarks/>
        public string ArticleType1
        {
            get
            {
                return this.articleType1Field;
            }
            set
            {
                this.articleType1Field = value;
            }
        }

        /// <remarks/>
        public string ArticleType2
        {
            get
            {
                return this.articleType2Field;
            }
            set
            {
                this.articleType2Field = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    public partial class PublishPublishNode
    {

        private PublishPublishNodeBAPIResponse bAPIResponseField;

        /// <remarks/>
        public PublishPublishNodeBAPIResponse BAPIResponse
        {
            get
            {
                return this.bAPIResponseField;
            }
            set
            {
                this.bAPIResponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService")]
    public partial class PublishPublishNodeBAPIResponse
    {

        private string transactionNumberField;

        private string fIDocField;

        private string statusField;

        private string messageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TransactionNumber
        {
            get
            {
                return this.transactionNumberField;
            }
            set
            {
                this.transactionNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FIDoc
        {
            get
            {
                return this.fIDocField;
            }
            set
            {
                this.fIDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }
}
