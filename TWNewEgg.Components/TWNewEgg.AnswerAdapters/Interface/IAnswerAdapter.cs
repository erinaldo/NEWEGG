using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Answer;

namespace TWNewEgg.AnswerAdapters.Interface
{
     public interface IAnswerAdapter
     {
         IQueryable<Answer> GetAnswerForProbelm(string PrblmCode);
         IQueryable<Problem> GetAccountProbelmInfo(string Email);
         Problem AddProblem(Problem Problem);
         IQueryable<Answer> GetALLAnswer();
         IQueryable<Problem> GetALLProbelm();
         IQueryable<Problem> GetAccountProbelmInfoByAccID(int AccID);
     }
}
