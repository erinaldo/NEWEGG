using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Drawing.Drawing2D;
using System.IO;
//using Newegg.Framework.Web;
//using Newegg.Framework.Web.Cookie;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ValidateCode
    {
        public ValidateCode()
        {
        }
        /// <summary>
        /// 驗證碼的最大長度
        /// </summary>
        public int MaxLength
        {
            get { return 10; }
        }
        /// <summary>
        /// 驗證碼的最小長度
        /// </summary>
        public int MinLength
        {
            get { return 1; }
        }
        /// <summary>
        /// 生成驗證碼
        /// </summary>
        /// <param name="length">指定驗證碼的長度</param>
        /// <returns></returns>
        public string CreateValidateCode(int length)
        {
            int rand;
            char code;
            string randomcode = String.Empty;
            //生成一定長度的驗證碼  
            System.Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rand = random.Next();
                /*
                if (rand % 3 == 0)
                {
                    code = (char)('A' + (char)(rand % 26));
                }
                else if (rand % 3 == 1)
                {
                    code = (char)('a' + (char)(rand % 26));
                }
                else
                {*/
                    code = (char)('0' + (char)(rand % 10));
                //}
                randomcode += code.ToString();
            }

            return randomcode;

            /*
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成隨機數字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取隨機數字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成驗證碼
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
            */
        }
        /// <summary>
        /// 創建驗證碼的圖片
        /// </summary>
        /// <param name="containsPage">找到要輸出到的page對象</param>
        /// <param name="validateNum">驗證碼</param>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成隨機生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //畫圖片的干擾線

                /*
                int randAngle = 45; //隨機轉動角度
                char[] chars = validateCode.ToCharArray();//拆散字符串成單字數組
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                */

                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                    //g.DrawEllipse(new Pen(Color.DarkViolet), new System.Drawing.Rectangle(x1, y1, x2, y2));  //讓畫面更亂...

                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);

                /*
                Point dot = new Point(16, 16);
                float angle = random.Next(-randAngle, randAngle);//轉動的度數
                g.TranslateTransform(dot.X, dot.Y);//移動游標到指定位置   
                g.RotateTransform(angle);
                //g.DrawString(chars.ToString(), font, brush, 1, 1, format);
                //graph.DrawString(chars.ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);   
                g.RotateTransform(-angle);//轉回去   
                g.TranslateTransform(2, -dot.Y);//移動游標到指定位置
                */

                //畫圖片的前景干擾點
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //化圖片的邊框線
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存圖片數據
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //輸出圖片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        /// <summary>
        /// 得到驗證碼的長度
        /// </summary>
        /// <param name="validateNumLength">驗證碼的長度</param>
        /// <returns></returns>
        public static int GetImageWidth(int validateNumLength)
        {
            return (int)(validateNumLength * 12.0);
        }
        /// <summary>
        /// 得到驗證碼的高度
        /// </summary>
        /// <returns></returns>
        public static double GetImageHeight()
        {
            return 22.5;
        }
    }
}

/*using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
//using System.IO;

namespace MvcShopping.Models
{
    ///  <summary>   
    ///  完美随机验证码  0.10   
    ///  Verion:0.10   
    ///  Description:随机生成设定验证码，并随机旋转一定角度，字体颜色不同   
    ///  </summary>   
    public class ValidateCode
    {

        ///  <summary>   
        ///  生成随机码   
        ///  </summary>   
        ///  <param  name="length">随机码个数www.52mvc.com</param>   
        ///  <returns></returns>   
        public static string CreateRandomCode(int length)
        {
            int rand;
            char code;
            string randomcode = String.Empty;
            //生成一定长度的验证码   
            System.Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    code = (char)('A' + (char)(rand % 26));
                }
                else
                {
                    code = (char)('0' + (char)(rand % 10));
                }
                randomcode += code.ToString();
            }
            return randomcode;
        }
        ///  <summary>   
        ///  创建随机码图片   
        ///  </summary>   
        ///  <param  name="randomcode">随机码</param>   
        public static void CreateImage(string randomcode)
        {
            //randomcode = "1";
            int randAngle = 45; //随机转动角度   
            int mapwidth = (int)(randomcode.Length * 23);
            Bitmap map = new Bitmap(mapwidth, 28);//创建图片背景   
            Graphics graph = Graphics.FromImage(map);
            graph.Clear(Color.AliceBlue);//清除画面，填充背景   
            graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框   
            //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//模式   
            Random rand = new Random();
            //背景噪点生成   
            Pen blackPen = new Pen(Color.LightGray, 0);
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }
            //验证码旋转，防止机器识别    更多http://www.52mvc.com/topictag-106.aspx
            char[] chars = randomcode.ToCharArray();//拆散字符串成单字符数组   
            //文字距中   
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            //定义颜色   
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //定义字体   
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "" };
            for (int i = 0; i < chars.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(5);
                Font f = new System.Drawing.Font(font[findex], 13, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)   
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                Point dot = new Point(16, 16);
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的   
                float angle = rand.Next(-randAngle, randAngle);//转动的度数   
                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置   
                graph.RotateTransform(angle);
                graph.DrawString(chars.ToString(), f, b, 1, 1, format);
                //graph.DrawString(chars.ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);   
                graph.RotateTransform(-angle);//转回去   
                graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置   
            }
            //graph.DrawString(randomcode,fontstyle,new SolidBrush(Color.Blue),2,2); //标准随机码   
            //生成图片   
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            graph.Dispose();
            map.Dispose();
            
        }

    }
}*/