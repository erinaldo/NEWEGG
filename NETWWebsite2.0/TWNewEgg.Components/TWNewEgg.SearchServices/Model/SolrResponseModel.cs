using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchServices.Model
{
    public class SolrResponseFullModel
    {
        public ResponseHeader responseHeader { get; set; }
        public ResponseFull response { get; set; }
    }

    public class SolrResponseSimpleModel
    {
        public ResponseHeader responseHeader { get; set; }
        public ResponseSimple response { get; set; }
    }

    public class ResponseHeader
    {
        public int status { get; set; }
        public int QTime { get; set; }
        public Parameters @params { get; set; } 
    }

    public class Parameters
    {
        public string json { get; set; }
    }

    public class ResponseFull
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public List<ItemSearch> docs { get; set; }
    }
    public class ResponseSimple
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public List<ItemSearchSimpleModel> docs { get; set; }
    }

    public class ItemSearchSimpleModel
    {
        public int CategoryID { get; set; }
        public int PriceCash { get; set; }
    }
}
