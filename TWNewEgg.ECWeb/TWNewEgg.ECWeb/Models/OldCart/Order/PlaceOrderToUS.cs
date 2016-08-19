using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class Publish
    {
        //space
        [XmlAttribute("schemaLocation", Namespace = "http://soa.newegg.com/SOA/USA/InfrastructureService/V10/EcommercePubSubService")]
        public string schemaLocation;

        /// <summary>
        /// Service Name
        /// </summary>
        public string Subject;

        /// <summary>
        /// FromService http://soa.newegg.com/SOA/USA/InfrastructureService/V10/Ecommerce/PubSubService
        /// </summary>
        public string FromService;


        /// <summary>
        /// ToService http://soa.newegg.com/SOA/USA/OrderManagement/V10/SSL31/CreateSOForEDI
        /// </summary>
        public string ToService;



        /// <summary>
        /// Node
        /// </summary>
        public NodeList Node;
    }
}