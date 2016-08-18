using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Drawing.Drawing2D;
using System.IO;
namespace TWNewEgg.Models.ViewModels.Account
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
                code = (char)('0' + (char)(rand % 10));
                randomcode += code.ToString();
            }
            return randomcode;            
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
