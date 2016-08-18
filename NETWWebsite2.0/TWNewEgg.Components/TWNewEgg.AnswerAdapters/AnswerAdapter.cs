using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.AnswerAdapters.Interface;

namespace TWNewEgg.AnswerAdapters
{
    public class AnswerAdapter : IAnswerAdapter
    {
   
        IRepository<Answer> _Answer;
        IRepository<Problem> _Problem; 

        public AnswerAdapter(IRepository<Answer> _Answer,  IRepository<Problem> _Problem)
        {
        
            this._Answer = _Answer;
            this._Problem = _Problem;
        }
        public IQueryable<Answer> GetAnswerForProbelm(string PrblmCode)
        {
          
           var AnswerIQeury=this._Answer.GetAll().Where(x => x.PrblmCode == PrblmCode).AsQueryable();
           return AnswerIQeury;
        }
        public IQueryable<Problem> GetAccountProbelmInfo(string Email)
        {

            var ProbelmIQeury = this._Problem.GetAll().Where(x => x.Email == Email).AsQueryable();
            return ProbelmIQeury;
        }
        public Problem AddProblem(Problem Problem)
        {
            try
            {
                if (Problem != null)
                {


                    _Problem.Create(Problem);
                   
                }
                return Problem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }
        public IQueryable<Answer> GetALLAnswer() 
        {
            var AnswerIQeury = this._Answer.GetAll().AsQueryable();
            return AnswerIQeury;
        
        }
        public IQueryable<Problem> GetALLProbelm()
        {
            var ProblemIQeury = this._Problem.GetAll().AsQueryable();
            return ProblemIQeury;

        }
        public IQueryable<Problem> GetAccountProbelmInfoByAccID(int AccID)
        {

            var ProbelmIQeury = this._Problem.GetAll().Where(x => x.AccountID == AccID).AsQueryable();
            return ProbelmIQeury;
        }
    }
}
