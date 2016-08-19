using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Answer;

namespace TWNewEgg.AnswerServices.Interface
{
    public interface IAnswerService
    {
        List<AnswerInfo> GetPrblmRecode(int accID, int Mouth, string Email, string Salceorder);
        AnswerInfo GetPrblmRecodeSelect(int accID, int Mouth, string Email, string Salceorder, string ProblemId);
        AnswerInfo GetSalceOrderInfo(string SalesOrderCode, int accID, string Name);
        Models.DomainModels.Redeem.ActionResponse<AnswerInfo> AddSalseOrderForAnswerInfo(SalesOrderInfo SalesOrderInfo, int? ItemID, short? faqtypeval, string maintext, int accID);
    }
}
