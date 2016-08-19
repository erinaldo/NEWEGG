using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchServices.Model
{
    public class SolrRequestModel
    {
        public SettingParameters @params { get; set; }
    }
    public class SettingParameters
    {
        //query
        public string q { get; set; }
        //sort
        public string sort { get; set; }
        //offset
        public int start { get; set; }
        //limit
        public int rows { get; set; }
        //
        public bool indent { get; set; }
        //
        public string wt { get; set; }
        //
        public string fq { get; set; }
        //fields
        public string fl { get; set; }
        //bf
        public string bf { get; set; }
        //defType
        public string defType { get; set; }
        /*
         * Parameter    Description
            defType         Selects the query parser to be used to process the query.
            sort            Sorts the response to a query in either ascending or descending order based on the response's score or another specified characteristic.
            start           Specifies an offset (by default, 0) into the responses at which Solr should begin displaying content.
            rows            Controls how many rows of responses are displayed at a time (default value: 10)
            fq              Applies a filter query to the search results.
            fl              Limits the information included in a query response to a specified list of fields. The fields need to have been indexed as stored for this parameter to work correctly.
            debug          Request additional debugging information in the response. Specifying the debug=timing parameter returns just the timing information; specifying the debug=results parameter returns "explain" information for each of the documents returned; specifying the debug=query parameter returns all of the debug information.
            explainOther    Allows clients to specify a Lucene query to identify a set of documents. If non-blank, the explain info of each document which matches this query, relative to the main query (specified by the q parameter) will be returned along with the rest of the debugging information.
            timeAllowed     Defines the time allowed for the query to be processed. If the time elapses before the query response is complete, partial information may be returned.
            omitHeader      Excludes the header from the returned results, if set to true. The header contains information about the request, such as the time the request took to complete. The default is false.
            wt              Specifies the Response Writer to be used to format the query response.
            logParamsList	By default, Solr logs all parameters. Set this parameter to restrict which parameters are logged. Valid entries are the parameters to be logged, separated by commas (i.e., logParamsList=param1,param2). An empty list will log no parameters, so if logging all parameters is desired, do not define this additional parameter at all.
         */
    }
}
