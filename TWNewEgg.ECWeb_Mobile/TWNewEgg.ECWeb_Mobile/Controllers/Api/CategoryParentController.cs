using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class CategoryParentController : ApiController
    {
        /// <summary>
        /// 根據CategoryId取得所有的Parent
        /// </summary>
        /// <param name="CategoryId">Category Id</param>
        /// <returns></returns>
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        public List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> Get(string cIDs)
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> resutls = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();
            if (string.IsNullOrEmpty(cIDs))
            {
                return resutls;
            }
            List<string> categoryList = cIDs.Split(',').ToList();
            if (categoryList.Count < 1)
            {
                return resutls;
            }
            List<int> categoryIDs = new List<int>();
            for (int i = 0; i < categoryList.Count; i++)
            {
                int tryParse = new int();
                if (int.TryParse(categoryList[i], out tryParse))
                {
                    categoryIDs.Add(tryParse);
                }
            }

            if (categoryIDs == null || categoryIDs.Count == 0)
            {
                return resutls;
            }
            categoryIDs = categoryIDs.Distinct().Take(100).OrderBy(x => x).ToList();
            string cacheFileName = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                string input = TWNewEgg.Framework.Common.JSONSerialization.Serializer(categoryIDs);
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                cacheFileName = "c" + sBuilder.ToString();
            }

            var response = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("CategoryNewServices", "GetAllParentCategoriesByCIDs", categoryIDs, cacheFileName);
            if (string.IsNullOrEmpty(response.error))
            {
                resutls = response.results;
            }
            return resutls;
        }

        /// <summary>
        /// 根據itme Id取得所有的Parent
        /// </summary>
        /// <param name="itemIDs">item Ids</param>
        /// <returns></returns>
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpPost]
        public List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> Post([FromBody]List<int> itemIDs)
        {
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> resutls = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();
            if (itemIDs == null || itemIDs.Count == 0)
            {
                return resutls;
            }
            itemIDs = itemIDs.Distinct().Take(100).OrderBy(x => x).ToList();
            string cacheFileName = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                string input = TWNewEgg.Framework.Common.JSONSerialization.Serializer(itemIDs);
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                cacheFileName = "i" + sBuilder.ToString();
            }

            var response = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("CategoryNewServices", "GetAllParentCategoriesByItemIDs", itemIDs, cacheFileName);
            if (string.IsNullOrEmpty(response.error))
            {
                resutls = response.results;
            }
            return resutls;
        }

        // PUT api/category/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/category/5
        public void Delete(int id)
        {
        }
    }
}
