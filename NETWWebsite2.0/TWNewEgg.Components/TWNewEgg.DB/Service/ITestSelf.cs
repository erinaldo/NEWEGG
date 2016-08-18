using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.Service
{
    public interface ITestSelf
    {
        /// <summary>
        /// Compare DB table with DB model, and output different numbers.
        /// </summary>
        /// <param name="dbName">DB Name</param>
        /// <param name="ipAddr">SQL server address, if ipAddr is empty, then it'll using the connect setting in web.config.</param>
        /// <param name="userName">Sql server username</param>
        /// <param name="password">Sql server password</param>
        /// <returns></returns>
        Dictionary<string, string> quickTest(string dbName, string ipAddr = "", string userName = "", string password = "");
        /// <summary>
        /// Compare DB table with DB model, and output detail message.
        /// </summary>
        /// <param name="dbName">DB Name</param>
        /// <param name="ipAddr">SQL server address, if ipAddr is empty, then it'll using the connect setting in web.config.</param>
        /// <param name="userName">Sql server username</param>
        /// <param name="password">Sql server password</param>
        /// <returns></returns>
        Dictionary<string, List<string>> testDetail(string dbName, string ipAddr = "", string userName = "", string password = "");
    }
}
