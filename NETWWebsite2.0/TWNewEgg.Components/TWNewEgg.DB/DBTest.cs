using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.Service;

namespace TWNewEgg.DB
{
    public class DBTest
    {
        public void testSelf()
        {
            ITestSelf testDB = new TestSelfRepository();
            //TWNewEgg.DB.Service.ITestSelf test = new DB.Service.TestSelfRepository();
            //TWNewEgg.DB.Service.ITestSelf test = new DB.Service.TestSelf();
            var zzz = testDB.quickTest("TWSQLDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var xxx = testDB.testDetail("TWSQLDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var ttt = testDB.quickTest("TWSQLDB");
            var yyy = testDB.testDetail("TWSQLDB");
            var aaa = testDB.quickTest("TWBACKENDDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var vvv = testDB.testDetail("TWBACKENDDB", "ST02QDBS01\\ST02GQCDBS01", "TWSQLDBo", "SQLDbo@TW");
            var ccc = testDB.quickTest("TWBACKENDDB");
            var ddd = testDB.testDetail("TWBACKENDDB");
            var eee = testDB.quickTest("TWSELLERPORTALDB");
            var fff = testDB.testDetail("TWSELLERPORTALDB");
        }
    }
}
