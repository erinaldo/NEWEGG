using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.AccountEnprypt.Interface;

namespace TWNewEgg.ECWeb_Mobile.Services.Account
{
    public class ECAccountAuth : IAccountAuth
    {
        private IAesService _aesCode;
        public ECAccountAuth()
        {
            _aesCode = AutofacConfig.Container.Resolve<IAesService>();
        }

        public AccountVM CheckAuth(Login model)
        {
            var result = Processor.Request<AccountInfoVM, AccountInfoDM>("AccountService", "Login", model.user, _aesCode.Enprypt(model.pass));
            if (!string.IsNullOrEmpty(result.error))
            {
                return null;
            }
            if (result.results == null || result.results.AVM == null)
            {
                return null;
            }
            return result.results.AVM;
            //throw new NotImplementedException();
        }

        public PasswordError CheckPassword(RegisterVM RegisterVM)
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
            PasswordError PasswordError = new PasswordError();
            int dRange = 0;
            int dstringmax = Convert.ToInt32("9fff", 16);//中文字Unicode範圍
            int dstringmin = Convert.ToInt32("4e00", 16);
            int dstringmax2 = Convert.ToInt32("ff5e", 16);//全形符號Unicode範圍
            int dstringmin2 = Convert.ToInt32("ff01", 16);
            int dstringmax3 = Convert.ToInt32("3105", 16);//注音符號Unicode範圍
            int dstringmin3 = Convert.ToInt32("312C", 16);

            /* ------ 撰寫AccountRule ------ */
            if (RegisterVM.PWD == null || RegisterVM.PWD.Length <= 0)
            {
                PasswordError.error = true;
                PasswordError.errormessage = PasswordError.errormessage + "請輸入帳號密碼\n";
            }

            //帳號密碼不得相同
            if ((RegisterVM.PWD != null && RegisterVM.PWD.Length > 0) && (RegisterVM.PWD == RegisterVM.Email))
            {
                PasswordError.error = true;
                PasswordError.sameAccountPassword = true;
                PasswordError.errormessage = PasswordError.errormessage + "帳號密碼相同\n";
            }

            //密碼不能打中文字:必要條件
            for (int i = 0; i < RegisterVM.PWD.Length; i++)
            {
                dRange = Convert.ToInt32(Convert.ToChar(RegisterVM.PWD.Substring(i, 1)));
                if ((dRange >= dstringmin && dRange < dstringmax) || (dRange >= dstringmin2 && dRange < dstringmax2) || (dRange >= dstringmin3 && dRange < dstringmax3))
                {
                    PasswordError.error = true;
                    PasswordError.includingIntEnglish = true;
                    PasswordError.errormessage = PasswordError.errormessage + "帳號密碼含有中文\n";
                }
            }

            //判斷密碼是否符合8至30個字元:必要條件
            if (RegisterVM.PWD.Length < 6 || RegisterVM.PWD.Length > 16)
            {
                PasswordError.error = true;
                PasswordError.passwordLength = true;
                PasswordError.errormessage = PasswordError.errormessage + "密碼長度6至16個字元\n";
            }

            //判斷密碼是否至少使用1個英文字母:必要條件
            //if ((Regex.Matches(RegisterVM.PWD, "[A-Za-z]").Count > 0) == false)
            //{
            //    PasswordError.error = true;
            //    PasswordError.includingIntEnglish = true;
            //    PasswordError.errormessage = PasswordError.errormessage + "至少使用1個英文字母\n";
            //}

            //判斷密碼是否至少使用1個數字:必要條件
            //if ((Regex.Matches(RegisterVM.PWD, "[0-9]").Count > 0) == false)
            //{
            //    PasswordError.error = true;
            //    PasswordError.includingIntEnglish = true;
            //    PasswordError.errormessage = PasswordError.errormessage + "至少使用1個數字\n";
            //}

            //判斷密碼是否至少使用一個特殊符號:選擇條件
            //if (Regex.Matches(RegisterVM.PWD, "[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]").Count > 0 == false)
            //{
            //    PasswordError.error = true;
            //    PasswordError.errormessage = PasswordError.errormessage + "至少使用1個特殊符號\n";
            //}
            RegisterVM.PWDtxt = _aesCode.Enprypt(RegisterVM.PWD);
            RegisterVM.PWD = _aesCode.Enprypt(RegisterVM.PWD);
            RegisterVM.confirmPWD = _aesCode.Enprypt(RegisterVM.confirmPWD);
            return PasswordError;
        }

        public bool CheckAgreePaper(RegisterVM RegisterVM)
        {
            if (RegisterVM.AgreePaper == null || RegisterVM.AgreePaper == 0)
            {
                return false;
            }
            return true;
        }
    }
}