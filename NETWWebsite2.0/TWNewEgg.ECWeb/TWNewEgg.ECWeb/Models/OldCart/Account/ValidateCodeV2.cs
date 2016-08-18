using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;
using System.Security.Cryptography;
//using Newegg.Framework.Web;
//using Newegg.Framework.Web.Cookie;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ValidateCodeV2
    {
        //驗証碼使用到的字元
        const string strCharList = "0123456789";

        //排除的字型
        static readonly string[] arExcludeFonts = {
        "Wingdings","Wingdings 2", "Wingdings 3", "Marlett", "Specialty", "Outlook", "Extra", "Math", "Symbol", "Webdings" };

        //亂數產生器
        RNGCryptoServiceProvider MyRandom = new RNGCryptoServiceProvider();


        /// <summary>
        /// 創建驗證碼的圖片
        /// </summary>
        /// <param name="containsPage">找到要輸出到的page對象</param>
        /// <param name="validateNum">驗證碼</param>
        public byte[] CreateValidateGraphic(HttpRequestBase Request, HttpResponseBase Response, int length)
        {
            //ContentType 為 Gif
            //context.Response.ContentType = "image/gif";

            //驗証碼      
            string strCAPTCHA = string.Empty;

            //建立驗証碼圖片 寬40x字元長度 高40
            Bitmap MyImage = new Bitmap(40 * length, 60);
            Graphics MyGraphic = Graphics.FromImage(MyImage);
            Matrix MyMatrix = new Matrix();
            FontFamily MyFamily;

            //填背景色
            MyGraphic.FillRectangle(Brushes.WhiteSmoke, 0, 0, MyImage.Width, MyImage.Height);

            //畫出驗証碼
            for (int i = 0; i < length; i++)
            {
                strCAPTCHA = string.Format("{0}{1}", strCAPTCHA, strCharList[MyNext(strCharList.Length)]);

                //亂數旋轉-20~20度
                MyMatrix.Reset();
                MyMatrix.RotateAt(MyNext(-20, 20), new PointF((40 * i) + 20, 20));
                MyGraphic.Transform = MyMatrix;

                //亂數字型
                do
                {
                    MyFamily = FontFamily.Families[MyNext(FontFamily.Families.Length)];
                } while (!MyFamily.IsStyleAvailable(FontStyle.Regular) || InExcludeFonts(MyFamily));

                //亂數顏色
                int intColor = MyNext(120, 180);

                //畫驗証碼
                
                MyGraphic.DrawString(
                    strCAPTCHA[i].ToString(),
                    new Font(MyFamily, MyNext(30, 36), (FontStyle.Bold | FontStyle.Italic)),
                    new SolidBrush(Color.Black),
                    new PointF((40 * i), 0));
                
                /*
                MyGraphic.DrawString(
                    strCAPTCHA[i].ToString(),
                    new Font(MyFamily, MyNext(30, 36), FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(intColor, intColor, intColor)),
                    new PointF((40 * i), 0));
                */
                /*
                MyGraphic.DrawString(
                    strCAPTCHA[i].ToString(), 
                    new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic)),
                    new LinearGradientBrush(new Rectangle(0, 0, MyImage.Width, MyImage.Height), Color.Blue, Color.DarkRed, 1.2f, true)
                    , 3, 2);
                */
                MyGraphic.ResetTransform();
            }

            //加雜點
            for (int i = 0; i < length * 60; i++)
                MyGraphic.FillEllipse(Brushes.Gray, MyNext(MyImage.Width), MyNext(MyImage.Height), 2, 2);

            //畫二條直線 二條貝茲曲線
            for (int i = 0; i < 2; i++)
            {
                MyGraphic.DrawLine(Pens.Gray,
                  MyNext(MyImage.Width), MyNext(MyImage.Height),
                  MyNext(MyImage.Width), MyNext(MyImage.Height));
                MyGraphic.DrawBezier(Pens.Gray,
                  MyNext(MyImage.Width), MyNext(MyImage.Height),
                  MyNext(MyImage.Width), MyNext(MyImage.Height),
                  MyNext(MyImage.Width), MyNext(MyImage.Height),
                  MyNext(MyImage.Width), MyNext(MyImage.Height));
            }

            //將驗証碼存入Cookie
            Response.Cookies["ValidateCode"].Value = strCAPTCHA;

            MemoryStream stream = new MemoryStream();
            MyImage.Save(stream, ImageFormat.Jpeg);
            return stream.ToArray();
            //將驗証碼圖片存到OutputStream
            //MyImage.Save(context.Response.OutputStream, ImageFormat.Gif);
        }

        //是否在排除的字型中
        bool InExcludeFonts(FontFamily MyFamily)
        {
            foreach (string strName in arExcludeFonts)
                if (MyFamily.Name.IndexOf(strName) > -1)
                    return true;
            return false;
        }

        //取一亂數不超過intMax
        int MyNext(int intMax)
        {
            byte[] MyByteArray = new byte[6];
            MyRandom.GetBytes(MyByteArray);
            return Math.Abs(BitConverter.ToInt32(MyByteArray, 0) % intMax);
        }

        //取一亂數在intMin與intMax之間!
        int MyNext(int intMin, int intMax)
        {
            return MyNext(intMax - intMin + 1) + intMin; ;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
