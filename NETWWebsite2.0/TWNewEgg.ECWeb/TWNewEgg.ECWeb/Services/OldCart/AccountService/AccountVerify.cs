using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TWNewEgg.DB.TWSQLDB;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class AccountVerify
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        /// <summary>
        /// 建構函式
        /// </summary>
        public AccountVerify()
        {
        }

        /// <summary>
        /// 增加註冊的帳號
        /// </summary>
        /// <param name="argObjAccount">Account物件</param>
        /// <returns>註冊成功回傳AccountId, 註冊失敗回傳0</returns>
        public int CreateAccount(Account argObjAccount)
        {
            int numAccountId = 0;
            TWNewEgg.DB.TWSqlDBContext objDb = null;//DbContext
            AesCookies objAesEnc = null;//密碼加解密用的物件

            /* ------ 撰寫建立的程式 ------ */
            objDb = new DB.TWSqlDBContext();
            objAesEnc = new AesCookies();
            argObjAccount.PWD = objAesEnc.AESenprypt(argObjAccount.PWD);
            argObjAccount.PWDtxt = argObjAccount.PWD;
            objDb.Account.Add(argObjAccount);

            try
            {
                objDb.SaveChanges();
                numAccountId = argObjAccount.ID;
            }
            catch (Exception e)
            {
               
                logger.Info(e.ToString());
                numAccountId = 0;
            }
            finally
            {
                if (objDb != null)
                {
                    objDb.Dispose();
                    objDb = null;
                }
            }

            return numAccountId;

        }

        /// <summary>
        /// 驗證帳號登入
        /// </summary>
        /// <param name="argStrAccount">帳號</param>
        /// <param name="argStrPassword">密碼</param>
        /// <returns>驗證成功回傳true, 驗證失敗回傳false</returns>
        public bool VerifyAccountLogin(string argStrAccount, string argStrPassword)
        {

            bool boolExec = false; //執行結果
            TWNewEgg.DB.TWSqlDBContext objDb = null;//DbContext
            AesCookies objAesEnc = null;//密碼加解密用的物件
            Account Verify = null;
            string strVerifyPw = "";
            objDb = new DB.TWSqlDBContext();
            objAesEnc = new AesCookies();
            strVerifyPw = objAesEnc.AESenprypt(argStrPassword); // verifyPw存經過加密後的argStrPassword

            /* ------ 撰寫驗證流程 ------ */   

            // 抓出在資料庫裡的資料
            Verify = objDb.Account.Where(x => x.Email == argStrAccount && x.PWD == strVerifyPw).FirstOrDefault();

            // 驗證密碼是否於存在資料庫之密碼相同
            if (Verify != null) 
            {
                // 驗證成功回傳true
                boolExec = true;  
            }

            return boolExec;
        }

        /// <summary>
        /// 驗證帳號登入,取得該帳號物件(不含密碼)
        /// </summary>
        /// <param name="argStrAccount">帳號</param>
        /// <param name="argStrPassword">密碼</param>
        /// <param name="argGetObject">是否取得物件</param>
        /// <returns></returns>
        public Account VerifyAccountLogin(string argStrAccount, string argStrPassword, bool argGetObject)
        {
            TWNewEgg.DB.TWSqlDBContext objDb = null;//DbContext
            AesCookies objAesEnc = null;//密碼加解密用的物件
            Account objVerifyAccount = null;
            objAesEnc = new AesCookies();
            var verifyPw = objAesEnc.AESenprypt(argStrPassword); // verifyPw存經過加密後的argStrPassword

            /* ------ 撰寫驗證流程 ------ */

            objDb = new DB.TWSqlDBContext();
            objAesEnc = new AesCookies();
            objVerifyAccount = new Account();

            // 抓出在資料庫裡的資料
            objVerifyAccount = objDb.Account.Where(x => x.Email == argStrAccount && x.PWD == verifyPw).FirstOrDefault();
            //清掉密碼欄位
            if (objVerifyAccount != null)
            {
                objVerifyAccount.PWD = "";
                objVerifyAccount.PWDtxt = "";
            }
            objDb = null;
            objAesEnc = null;

            return objVerifyAccount;
        }

        /// <summary>
        /// 驗證密碼安全性
        /// </summary>
        /// <param name="argStrPassword">密碼</param>
        /// <returns>密碼強度結果的字串: "Pass(or NoPass);強度百分比;其他訊息</returns>
        public string VerifyAccountRule(string argStrPassword, string argStrAccount)
        {
            /* ****** 驗證密碼強度 ******
             * 1. 至少8至30個字元
             * 2. 至少使用1個大寫字母
             * 3. 至少使用1個小寫字母
             * 4. 至少使用1個數字
             * 5. 至少使用1個特殊符號
             * 6. 密碼不可與帳號相同
             * 根據完成度, 分別累計強度0, 強度百分比, 100
             * 回傳強度之外, 另外回傳缺少的強度, 如 return "66;至少使用1個特殊符號";
             * 如完成60%, 則回傳 return "66; 至少8至30個字元;至少使用1個特殊符號";
             * 以分號";"做為字串訊息的切割
             * 若完成度為100, 則回傳100即可, 如 return "100";
             * 強度百分比計算 = (完成度 / 全部條件 * 100)
             * 第1項及第6項為必要條件, 2-5只需要符合3項即可PASS
             * 密碼強度與PASS規格分開計算
             * 回傳值為 "Pass(或NoPass);強度百分比;其他訊息"
             * 如 "NoPass;80;密碼不可與帳號相同", 以分號分隔
             */

            string strVerifyResult = "";//最終結果
            float numMaxCondition = 0;//全部條件
            float numConformCondition = 0;//選擇符合條件
            float numMustConform = 0;//必要符合條件
            int dRange = 0;
            int dstringmax = Convert.ToInt32("9fff", 16);//中文字Unicode範圍
            int dstringmin = Convert.ToInt32("4e00", 16);
            int dstringmax2 = Convert.ToInt32("ff5e", 16);//全形符號Unicode範圍
            int dstringmin2 = Convert.ToInt32("ff01", 16);
            int dstringmax3 = Convert.ToInt32("3105", 16);//注音符號Unicode範圍
            int dstringmin3 = Convert.ToInt32("312C", 16);

            /* ------ 撰寫AccountRule ------ */
            if (argStrPassword == null || argStrAccount == null || argStrPassword.Length <= 0 || argStrAccount.Length <= 0)
            {
                return "NoPass;0;請輸入帳號密碼";
            }
            
            //密碼不能打中文字:必要條件
            for (int i = 0; i < argStrPassword.Length; i++)
            {
                dRange = Convert.ToInt32(Convert.ToChar(argStrPassword.Substring(i, 1)));
                if ((dRange >= dstringmin && dRange < dstringmax) || (dRange >= dstringmin2 && dRange < dstringmax2) || (dRange >= dstringmin3 && dRange < dstringmax3))
                {
                    return "NoPass;0;不可輸入非指定文字";
                    break;
                }
            }

            //帳密不可相同:必要條件
            numMaxCondition++;
            if (argStrPassword.Equals(argStrAccount))
            {
                strVerifyResult += ";密碼不可與帳號相同";
            }
            else
            {
                numMustConform++;
            }

            //判斷密碼是否符合8至30個字元:必要條件
            numMaxCondition++;
            if (argStrPassword.Length < 8 || argStrPassword.Length > 30)
            {
                strVerifyResult += ";至少8至30個字元";
            }
            else
            {
                numMustConform++;
            }

            //判斷密碼是否至少使用1個大寫字母:選擇條件
            numMaxCondition++;
            if (Regex.Matches(argStrPassword, "[A-Z]").Count > 0 == false)
            {
                strVerifyResult += ";至少使用1個大寫字母";
            }
            else
            {
                numConformCondition++;
            }

            //判斷密碼是否至少使用1個小寫字母:選擇條件
            numMaxCondition++;
            if (Regex.Matches(argStrPassword, "[a-z]").Count > 0 == false)
            {
                strVerifyResult += ";至少使用1個小寫字母";
            }
            else
            {
                numConformCondition++;
            }
            //判斷密碼是否至少使用1個數字:選擇條件
            numMaxCondition++;
            if (Regex.Matches(argStrPassword, "[0-9]").Count > 0 == false)
            {
                strVerifyResult += ";至少使用1個數字";
            }
            else
            {
                numConformCondition++;
            }
            //判斷密碼是否至少使用一個特殊符號:選擇條件
            numMaxCondition++;
            if (Regex.Matches(argStrPassword, "[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]").Count > 0 == false)
            {
                strVerifyResult += ";至少使用1個特殊符號";
            }
            else
            {
                numConformCondition++;
            }

            strVerifyResult = Convert.ToString(Convert.ToInt32((numConformCondition + numMustConform) / numMaxCondition * 100)) + strVerifyResult;

            //判斷必須條件與選擇條件是否可通過:必要符合條件全通過 + 選擇符合條件至少2項
            if (numMustConform == 2 && numConformCondition >= 3)
            {
                strVerifyResult = "Pass;" + strVerifyResult;
            }
            else
            {
                strVerifyResult = "NoPass;" + strVerifyResult;
            }

            return strVerifyResult;
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="argStrAccount">帳號</param>
        /// <param name="argStrOldPassword">舊密碼</param>
        /// <param name="argStrNewPassword">新密碼</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        public bool UpdateAccountPassword(string argStrAccount, string argStrOldPassword, string argStrNewPassword, bool turnOn = true)
        {

            //驗證登入, 若登入不成功就回傳false
            if (turnOn && !this.VerifyAccountLogin(argStrAccount, argStrOldPassword))
            {
                return false;
            }

            //驗證密碼規則, 不符合規則就回傳false
            if (this.VerifyAccountRule(argStrNewPassword, argStrAccount).IndexOf("NoPass") >= 0)
            {
                return false;
            }

            /* ------ 撰寫修改密碼的程式 ------ */
            // 修改執行結果
            bool boolExec = false;
            string newPassword = "";
            TWNewEgg.DB.TWSqlDBContext objDb = null;//DbContext
            objDb = new DB.TWSqlDBContext();
            Account AddnewPassword = null;
            // 密碼加解密用的物件
            AesCookies objAesEnc = null;
            AddnewPassword = new Account();
            objAesEnc = new AesCookies();
            // newPassword存加密過後的新密碼
            newPassword = objAesEnc.AESenprypt(argStrNewPassword);
            AddnewPassword = objDb.Account.Where(x => x.Email == argStrAccount).FirstOrDefault();
            // 把加密後的新密碼存到DB
            AddnewPassword.PWD = newPassword;
            // 把加密後的新密碼存到DB
            AddnewPassword.PWDtxt = newPassword;
            if (AddnewPassword.Updated == null)
            {
                AddnewPassword.Updated = 1;
            }
            else
            {
                AddnewPassword.Updated += 1;    
            }
            AddnewPassword.UpdateDate = DateTime.Now;
            AddnewPassword.UpdateUser = argStrAccount;
            objDb.SaveChanges();

            boolExec = true;

            return boolExec;
        }

        /// <summary>
        /// 檢查是否已存在此Email的註冊帳號
        /// </summary>
        /// <param name="argStrEmail">Email Address</param>
        /// <returns>if yes return true, else return false</returns>
        public bool HasAccountEmail(string argStrEmail)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            Account objAccount = null;

            oDb = new DB.TWSqlDBContext();
            objAccount = oDb.Account.Where(x => x.Email == argStrEmail).FirstOrDefault();
            oDb.Dispose();
            oDb = null;

            if (objAccount != null)
            {
                boolExec = true;
                objAccount = null;
            }
            
            return boolExec;
        }

        /// <summary>
        /// 檢查此Email是否為會員，如是會員則回傳Account資料
        /// </summary>
        /// <param name="argStrEmail">Email Address</param>
        /// <param name="argObjAccount">Account Data</param>
        /// <returns>if yes return true, else return false</returns>
        public bool HasAccountEmail(string argStrEmail, out Account argObjAccount)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            Account objAccount = null;

            oDb = new DB.TWSqlDBContext();
            objAccount = oDb.Account.Where(x => x.Email == argStrEmail).FirstOrDefault();
            oDb.Dispose();
            oDb = null;

            // 先判斷是否有資料
            if (objAccount != null)
            {
                // 如有資料，再判斷是否為非會員購物
                if (objAccount.GuestLogin == 0)
                {
                    boolExec = true;
                    argObjAccount = null;
                    objAccount = null;
                }
                else
                {
                    boolExec = false;
                    argObjAccount = objAccount;
                    objAccount = null;
                }
            }
            else
            {
                boolExec = false;
                argObjAccount = null;
                objAccount = null;
            }

            return boolExec;
        }

        /// <summary>
        /// 根據Email取得Account物件
        /// </summary>
        /// <param name="argStrEmail"></param>
        /// <param name="argBoolGetPwd">是否包含密碼, true:包含, false:不包含</param>
        /// <returns>有資料回傳Account物件, 無資料回傳null</returns>
        public Account GetAccountByEmail(string argStrEmail, bool argBoolGetPwd)
        {
            Account objAccount = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            if (argStrEmail == null || argStrEmail.Trim().Length <= 0)
                return null;

            objDb = new DB.TWSqlDBContext();
            objAccount = objDb.Account.Where(x => x.Email == argStrEmail).FirstOrDefault();

            //清除密碼欄位的資料
            if (objAccount != null && !argBoolGetPwd)
            {
                objAccount.PWD = "";
                objAccount.PWDtxt = "";
            }

            return objAccount;
        }

        /// <summary>
        /// 更新Account,但不更新帳號、密碼、Create的資料
        /// </summary>
        /// <param name="argObjAccount">Account Object</param>
        /// <returns>return true when update success, else return false</returns>
        public bool UpdateAccount(Account argObjAccount, bool IsUpdatePassword = false)
        {
            bool boolExec = false;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            Account objNewAccount = null;

            //檢查必要參數
            if (argObjAccount == null || argObjAccount.Email == null || argObjAccount.Email.Length <= 0)
                return false;

            objDb = new DB.TWSqlDBContext();
            objNewAccount = objDb.Account.Where(x => x.Email == argObjAccount.Email).FirstOrDefault();
            if (objNewAccount == null)
            {
                objDb = null;
            return false;
            }

            //置換資料
            objNewAccount.Name = argObjAccount.Name;
            objNewAccount.Nickname = argObjAccount.Nickname;
            objNewAccount.NO = argObjAccount.NO;
            objNewAccount.Sex = argObjAccount.Sex;
            objNewAccount.Type = argObjAccount.Type;
            objNewAccount.Birthday = argObjAccount.Birthday;
            objNewAccount.Email2 = argObjAccount.Email2;
            objNewAccount.Loc = argObjAccount.Loc;
            objNewAccount.Zip = argObjAccount.Zip;
            objNewAccount.Address = argObjAccount.Address;
            objNewAccount.TelDay = argObjAccount.TelDay;
            objNewAccount.TelNight = argObjAccount.TelNight;
            objNewAccount.Mobile = argObjAccount.Mobile;
            objNewAccount.ConfirmDate = argObjAccount.ConfirmDate;
            objNewAccount.ConfirmCode = argObjAccount.ConfirmCode;
            objNewAccount.Subscribe = argObjAccount.Subscribe;
            objNewAccount.ACTName = argObjAccount.ACTName;
            objNewAccount.Degree = argObjAccount.Degree;
            objNewAccount.Income = argObjAccount.Income;
            objNewAccount.Job = argObjAccount.Job;
            objNewAccount.Marrige = argObjAccount.Marrige;
            objNewAccount.ServerName = argObjAccount.ServerName;
            objNewAccount.Chkfailcnt = argObjAccount.Chkfailcnt;
            objNewAccount.Status = argObjAccount.Status;
            objNewAccount.StatusDate = argObjAccount.StatusDate;
            objNewAccount.StatusNote = argObjAccount.StatusNote;
            objNewAccount.Note = argObjAccount.Note;
            objNewAccount.NewLinks = argObjAccount.NewLinks;
            objNewAccount.LoginStatus = argObjAccount.LoginStatus;
            objNewAccount.ValidateCode = argObjAccount.ValidateCode;
            objNewAccount.Registeron = argObjAccount.Registeron;
            objNewAccount.Loginon = argObjAccount.Loginon;
            objNewAccount.RememberMe = argObjAccount.RememberMe;
            objNewAccount.AgreePaper = argObjAccount.AgreePaper;
            objNewAccount.MessagePaper = argObjAccount.MessagePaper;
            objNewAccount.Updated = argObjAccount.Updated;
            objNewAccount.UpdateDate = argObjAccount.UpdateDate;
            objNewAccount.UpdateUser = argObjAccount.UpdateUser;
            objNewAccount.FacebookUID = argObjAccount.FacebookUID;
            objNewAccount.Istosap = argObjAccount.Istosap;
            objNewAccount.MemberAgreement = argObjAccount.MemberAgreement;
            objNewAccount.ActionCode = argObjAccount.ActionCode;
            objNewAccount.GuestLogin = argObjAccount.GuestLogin;
            objNewAccount.ReceiveEDM = argObjAccount.ReceiveEDM;

            // 判斷是否修改密碼
            if (IsUpdatePassword == true)
            {
                AesCookies objAesEnc = new AesCookies();
                argObjAccount.PWD = objAesEnc.AESenprypt(argObjAccount.PWD);
                argObjAccount.PWDtxt = argObjAccount.PWD;

                objNewAccount.PWD = argObjAccount.PWD;
                objNewAccount.PWDtxt = argObjAccount.PWDtxt;
            }

            try
            {
                objDb.SaveChanges();
            boolExec = true;
            }
            catch
            {
                boolExec = false;
            }
            finally
            {
                if (objDb != null)
                    objDb.Dispose();
            }



            return boolExec;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="time">Try login time, ex: 0, 1, 2, 3</param>
        /// <param name="date">Date format(yyyy/MM/dd) only date and don't have time info, ex: 2015/01/21 00:00:00.000</param>
        /// <param name="ipAddress">user ip address, ex: 172.16.131.0 or 8.8.8.8</param>
        /// <returns>Encryption string.</returns>
        public string LoginTimeEncryption(int time, DateTime date, string ipAddress)
        {
            string encryptionString = string.Empty;
            int tempCode = 0;
            string[] ipArr=new string[4];
            ipArr = ipAddress.Split('.');
            tempCode = date.Month + date.Day;
            //IP1(3碼) + year(4碼) + IP2(3碼) + time(1碼) + IP3(3碼) + month(2碼) + ((int)month+day)(2碼) + IP4(3碼)=======>共21碼
            encryptionString = ipArr[0].PadLeft(3, '0') + date.Year.ToString() + ipArr[1].PadLeft(3, '0') + time.ToString() + ipArr[2].PadLeft(3, '0') + date.Month.ToString().PadLeft(2,'0') + tempCode.ToString().PadLeft(2,'0') + ipArr[3].PadLeft(3, '0');
            //TODO(bw52) : Coding here.

            return encryptionString;
        } 

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="returnCode">Encryption string</param>
        /// <returns>Three Key/Pair value, Keys/Values must have "Time", "Date", "IPAddress". </returns>
        public Dictionary<string, string> LoginTimeDecryption(string encryptionString)
        {
            Dictionary<string, string> decryptionData = new Dictionary<string, string>();
            string[] ipArr1=new string[4];
            string[] ipArr2 = new string[3];
            string ipAddress="";
            int monDay = 0;
            int day = 0;
            monDay = Convert.ToInt32(encryptionString.Substring(16, 2));
            day = monDay-Convert.ToInt32(encryptionString.Substring(14, 2));
            ipArr1[0]=encryptionString.Substring(0,3);
            ipArr1[1] = encryptionString.Substring(7,3);
            ipArr1[2] = encryptionString.Substring(11, 3);
            ipArr1[3] = encryptionString.Substring(18,3);
            for (int i = 0; i <= 3; i++)
            {
                ipArr2[0] = ipArr1[i].Substring(0, 1);
                ipArr2[1] = ipArr1[i].Substring(1, 1);
                ipArr2[2] = ipArr1[i].Substring(2, 1);
                if (ipArr2[0] == "0")
                {
                    if (ipArr2[1] == "0")
                    {
                        if (ipAddress != "")
                        {
                            ipAddress = ipAddress + "." + ipArr2[2];
                        }
                        else ipAddress = ipArr2[2];
                    }
                    else
                    {
                        if (ipAddress != "")
                        {
                            ipAddress = ipAddress + "." + ipArr2[1] + ipArr2[2];
                        }
                        else ipAddress = ipArr2[1] + ipArr2[2];
                    }
                }
                else
                {
                    if (ipAddress != "")
                    {
                        ipAddress = ipAddress + "." + ipArr1[i];
                    }
                    else ipAddress = ipArr1[i];
                }
            }

            decryptionData.Add("Date", encryptionString.Substring(3, 4) + "/" + encryptionString.Substring(14, 2)+"/"+day.ToString());
            decryptionData.Add("Time", encryptionString.Substring(10, 1));
            decryptionData.Add("IPAddress",ipAddress);
            //string a = "001";
            //string b = a.Replace("00", "");
            //TODO(bw52) : Coding here.

            if (!decryptionData.ContainsKey("Time") && !decryptionData.ContainsKey("Date") && !decryptionData.ContainsKey("IPAddress"))
            {
                decryptionData.Add("Time", "");
                decryptionData.Add("Date", "");
                decryptionData.Add("IPAddress", "");
            }
            return decryptionData;
        }
    }
}