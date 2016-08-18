using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.Models.WebsiteMappingRules
{
    //從1.0的IPP移過來的
    //將圖片的路徑增加時間戳記------------add by bruce 20160308    
    public class ImagesUrlChangerules
    {
        public static string Environmentt = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        public static string ImageServer_Path = System.Configuration.ConfigurationManager.AppSettings[Environmentt + "_ImageServerUrl"];
        public static string IPPServer_Path = System.Configuration.ConfigurationManager.AppSettings[Environmentt + "_IPP_URL"];

        /// <summary>
        /// 將圖片的路徑轉小寫
        /// </summary>
        /// <param name="input">/Pic/SubCategoryStore/StoreImage_0_美妝保養.jpg?160322042603637405769</param>
        /// <returns></returns>
        public static string ImagesUrlPathToLower(string input)
        {
            //將圖片的路徑轉小寫----------------add by bruce 20160323

            string img_url = string.Empty;

            if (!string.IsNullOrEmpty(input))
            {
                string[] img_paths = input.Split('/');
                //path to lower
                int path_len = img_paths.Length - 1;
                StringBuilder lower_path = new StringBuilder();
                //處理成小寫
                for (int i = 0; i < path_len; i++)
                {
                    if (img_paths[i].ToString() == string.Empty) continue;
                    lower_path.Append("/" + img_paths[i].ToString().ToLower());
                }
                //加入檔案名稱
                lower_path.Append("/" + img_paths[path_len]);

                img_url = lower_path.ToString();
            }

            return img_url;
        }

        //將圖片的檔名加入時間戳記
        public static string ImagesUrladdrandomResolver(string input)
        {
            string imgurl = "";
            DateTime time = DateTime.Now;

            //if (input != "" && input != null)
            //{
            //    imgurl = input + "?" + time.ToString("yyMMddhhmmssFFF") + (time.Millisecond * time.Millisecond).ToString();
            //}

            //檢查問號----------------changed by bruce 20160308
            //http://jira/browse/WMTWNOR-3104
            if (!string.IsNullOrEmpty(input))
            {
                string[] question_mark_len = input.Split('?');
                //已有問號不再時間戳記
                if (question_mark_len.Length > 1) 
                    imgurl = input;
                else
                    imgurl = input + "?" + time.ToString("yyMMddhhmmssFFF") + (time.Millisecond * time.Millisecond).ToString();
            }

            return imgurl;
        }

        public static string ImagesUrlremoverandomResolver(string input)
        {
            string imgurl = "";
            if (input != "" && input != null)
            {
                imgurl = input.Split('?').FirstOrDefault();
            }

            return imgurl;
        }

        public static string ImagesUrlRemoveDomainName(string input)
        {
            char firstWord;
            string imgurl = "";
            if (input != "" && input != null)
            {
                //imgurl = input.Split('?').FirstOrDefault();
                string domainname1 = "http://";
                string domainname2 = "https://";
                if ((input.Contains(domainname1)) || (input.Contains(domainname2)))
                {
                    if (input.IndexOf(ImageServer_Path) != -1)
                    {
                        input = input.Replace(ImageServer_Path, "");
                    }
                    if (input.IndexOf(IPPServer_Path) != -1)
                    {
                        input = input.Replace(IPPServer_Path, "");
                    }

                    firstWord = input[0];

                    if (firstWord != '/' && firstWord == 'h')
                    {
                        input = '/' + input;
                    }
                }
                imgurl = input;
            }

            return imgurl;
        }


        public static string ImagesUrladddomainname(string input)
        {
            string imgurl = "";
            if (input != null)
            {
                input = input.Trim();
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                char firstWord;
                firstWord = input[0];
                string domainname1 = "http://";
                string domainname2 = "https://";
                if ((input.Contains(domainname1)) || (input.Contains(domainname2)))
                    imgurl = input;
                else
                {
                    if (firstWord != '/')
                    {
                        imgurl = ImageServer_Path + '/' + input;
                    }
                    else
                    {
                        imgurl = ImageServer_Path + input;
                    }
                }
            }

            return imgurl;
        }

        public static string IPPUrladddomainname(string input)
        {
            char firstWord;
            firstWord = input[0];
            string imgurl = "";
            if (input != "" && input != null)
            {
                string domainname1 = "http://";
                string domainname2 = "https://";
                if ((input.Contains(domainname1)) || (input.Contains(domainname2)))
                    imgurl = input;
                else
                {
                    if (firstWord != '/')
                    {
                        imgurl = IPPServer_Path + '/' + input;
                    }
                    else
                    {
                        imgurl = IPPServer_Path + input;
                    }
                }
            }

            return imgurl;
        }
    }
}
