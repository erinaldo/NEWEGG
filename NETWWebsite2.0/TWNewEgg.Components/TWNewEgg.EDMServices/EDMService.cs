using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.EDMRepoAdapters.Interface;
using TWNewEgg.EDMServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.EDM;

namespace TWNewEgg.EDMServices
{
    public class EDMService : IEDMService
    {
        IEDMRepoAdapter _edmRepoAdapter;
        private string bodyStartString = "<body>";
        private string bodyEndString = "</body>";

        public EDMService(IEDMRepoAdapter edmRepoAdapter)
        {
            this._edmRepoAdapter = edmRepoAdapter;
        }

        public EDMBookDM GetLatestEDM()
        {
            return ConvertEDMToDM(this._edmRepoAdapter.GetLatestEDMBook());
        }

        private EDMBookDM ConvertEDMToDM(EDMBook dbEDM)
        {
            EDMBookDM domainModel = new EDMBookDM();
            if (dbEDM == null)
            {
                return domainModel;
            }

            domainModel.ID = dbEDM.ID;
            domainModel.EDMName = dbEDM.EDMName;
            domainModel.HtmlContext = dbEDM.HtmlContext;

            return domainModel;
        }

        private string ConvertContext(string context)
        {
            string replaceHtmlContext = string.Empty;
            if (string.IsNullOrEmpty(context))
            {
                context = "";
            }
            int bodyStart = context.IndexOf(bodyStartString);
            int bodyEnd = context.IndexOf(bodyEndString);
            if (bodyStart >= 0 && bodyEnd > 0)
            {
                int cutStart = bodyStart + bodyStartString.Length;
                replaceHtmlContext = context.Substring(cutStart, bodyEnd);
            }
            return replaceHtmlContext;
        }
    }
}
