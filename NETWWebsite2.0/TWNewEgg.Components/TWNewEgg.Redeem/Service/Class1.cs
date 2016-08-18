using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
        /*
namespace TWNewEgg.Redeem.Service
{
    public class Class1 : IDisposable
    {

        private TWSqlDBContext db = new TWSqlDBContext();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db = null;
            }
        }

        public List<int> aaa()
        {
            TWNewEgg.DB.Service.ITestSelf test = new DB.Service.TestSelfRepository();
            //TWNewEgg.DB.Service.ITestSelf test = new DB.Service.TestSelf();
            var zzz = test.quickTest("TWSQLDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var xxx = test.testDetail("TWSQLDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var ttt = test.quickTest("TWSQLDB");
            var yyy = test.testDetail("TWSQLDB");
            var aaa = test.quickTest("TWBACKENDDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var vvv = test.testDetail("TWBACKENDDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var ccc = test.quickTest("TWBACKENDDB");
            var ddd = test.testDetail("TWBACKENDDB");
            var eee = test.quickTest("TWSELLERPORTALDB");
            var fff = test.testDetail("TWSELLERPORTALDB");
            List<int> bbb = new List<int>();
            bbb = db.Item.Select(x => x.ID).ToList();
            return bbb;
        }

    }
}
         */