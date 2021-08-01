using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging; // for ImageFormat
using System.Runtime.InteropServices;

namespace test_image
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Bitmap newbmp;
        Image image;
        ImageForm CurrentImage=null; 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        { 

        }
      
        // ==================================   使用者介面相關   =====================================
        // Load 按鈕事件處理函式
         private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageForm MyImage = new ImageForm(openFileDialog1.FileName); // 建立秀圖物件
                MyImage.Show();// 顯示秀圖照片
                CurrentImage = MyImage;
            }
        }

        // 高效率影像處理 --  反轉顏色範例
        private void button3_Click_1(object sender, EventArgs e)
        {
            if (CurrentImage != null)
            {
                CurrentImage.Invert();
            }
            else
            {
                MessageBox.Show("請先載入圖形");
            }
        }  

               private void button9_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image.Save(@"c:\Result.bmp", ImageFormat.Bmp);
                image.Save(@"c:\Result.exif", ImageFormat.Exif);
            }
        }
            

        // ==================================   影像處理公用程式相關   =====================================
       
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Timer, 每10ms讀取滑鼠座標值及像素顏色並顯示於 PictureBox背景
            Color color = GetPixelColor(Cursor.Position);
            labelx.Text = "X = " + Cursor.Position.X.ToString();
            labely.Text = "Y = " + Cursor.Position.Y.ToString();
            labelr.Text = " R = " + color.R;
            labelg.Text = " G = " + color.G;
            labelb.Text = " B = " + color.B;
            //Form.BackColor = color;
        }

        private object ImageForm()
        {
            throw new NotImplementedException();
        }

        static Color GetPixelColor(Point position)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(position, new Point(0, 0), new Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }
    }

    // 建立一個專門秀圖的 Form 類別
    class ImageForm : Form {
        Image image;

        // 預設建構子建構子
        public ImageForm() {
            // Step 1: 載入影像   
            LoadImage(@"c:\淡蘭古道_山貂嶺段q.bmp");
        }

        // 建構子
        public ImageForm(String Filename) {
            LoadImage(Filename);
        }

        public void LoadImage(String Filename) {
            // Step 1: 載入影像  
            image = Image.FromFile(Filename);
            this.Text = Filename;

            // Step 2: 調整視窗的大小
            this.Height = image.Height;
            this.Width = image.Width;
        }

        protected override void OnPaint(PaintEventArgs e) {
           // base.OnPaint(e);

          
            
            // Step 3: 顯示出影像
            e.Graphics.DrawImage(image, 0, 0,image.Width,image.Height);
            

            //int cx = ClientSize.Width;
            //int cy = ClientSize.Height;

            
            // 放大 2 倍
            /*
            this.Height = image.Height * 2;
            this.Width = image.Width * 2;
            
            e.Graphics.DrawImage(image, new Point[] { new Point(0, 0),
                                        new Point(2*image.Width, 0), 
                                        new Point(0, 2*image.Height) });
            */

            /*
            // 縮小 1/2 倍
            e.Graphics.DrawImage(image, new Point[] { new Point(0, 0),
                                        new Point(image.Width/2, 0), 
                                        new Point(0, image.Height/2) });
            */

            // 形變旋轉
            // e.Graphics.DrawImage(image, new Point[] { new Point(cx / 2, 0), new Point(cx, cy / 2), new Point(0, cy / 2) });

            
            /*
            image.Save(@"c:\Result.bmp", ImageFormat.Bmp);
            image.Save(@"c:\Result.gif", ImageFormat.Gif);
            image.Save(@"c:\Result.png", ImageFormat.Png);
            image.Save(@"c:\Result.jpg", ImageFormat.Jpeg);
            image.Save(@"c:\Result.exif", ImageFormat.Exif);
            image.Save(@"c:\Result.exif", ImageFormat.Emf);
             */
             

           //  image.Save(@"c:\kk.bmp", ImageFormat.Bmp);
        }

        // 傳回 RGB 陣列資訊
        public int[,,] getRGBData() {
            MessageBox.Show("開始 RGB 彩色資訊讀取");

            // Step 1: 利用 Bitmap 將 image 包起來
            Bitmap bimage = new Bitmap(image);
            int Height = bimage.Height;
            int Width = bimage.Width;
            int[,,]rgbData=new int[Width,Height,3];

            

            // Step 2: 取得像點顏色資訊
            int y;
            int x;
            
            for(y=0;y<Height;y++){
                for(x=0;x<Width;x++){
                    Color color = bimage.GetPixel(x, y);
                    rgbData[x, y, 0] = color.R;
                    rgbData[x, y, 1] = color.G;
                    rgbData[x, y, 2] = color.B;
                }
            }

            MessageBox.Show("RGB 彩色資訊讀取完成");

            return rgbData;
        }

        public void Invert() {
            Bitmap bimage = new Bitmap(image);
            Invert(bimage);
            image = bimage;
            this.Refresh();
        }

       
        // 高效率反轉圖片
        // 參考 Christian Graus 先生的文章 
        // 詳見 http://www.codeproject.com/cs/media/csharpgraphicfilters11.asp
        public static bool Invert(Bitmap bimage) {
            // Step 1: 先鎖住存放圖片的記憶體
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height),
                                           ImageLockMode.ReadWrite, 
                                           PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;

            // Step 2: 取得像點資料的起始位址
            System.IntPtr Scan0 = bmData.Scan0;

            // 計算每行的像點所佔據的byte 總數
            int ByteNumber_Width = bimage.Width * 3;
 
            // 計算每一行後面幾個 Padding bytes
            int ByteOfSkip = stride - ByteNumber_Width;

            
          
            // Step 3: 直接利用指標, 更改圖檔的內容
            int Height = bimage.Height;
            unsafe {
                byte* p = (byte*)(void*)Scan0;
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < ByteNumber_Width; x++) {
                        p[0] = (byte)(255 - p[0]); // 彩色資料反轉
                        ++p;
                    }
                    p += ByteOfSkip; // 跳過剩下的 Padding bytes
                }
            }

            bimage.UnlockBits(bmData);

            return true;
        }
        
        // 高效率圖形轉換工具 -- 讀取影像資料
        public int[, ,] getRGBData_unsafe() {
            Bitmap bimage = new Bitmap(image);
            return getRGBData(bimage);
        }
        public static int[, ,] getRGBData(Bitmap bimage) {
            // Step 1: 先鎖住存放圖片的記憶體
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height),
                                           ImageLockMode.ReadOnly,
                                           PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;

            // Step 2: 取得像點資料的起始位址
            System.IntPtr Scan0 = bmData.Scan0;

            // 計算每行的像點所佔據的byte 總數
            int ByteNumber_Width = bimage.Width * 3;

            // 計算每一行後面幾個 Padding bytes
            int ByteOfSkip = stride - ByteNumber_Width;
            int Height = bimage.Height;
            int Width = bimage.Width;
            int[, ,] rgbData = new int[Width, Height, 3];
            

            // Step 3:直接利用指標, 把影像資料取出來
            unsafe {
                byte* p = (byte*)(void*)Scan0;
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        rgbData[x, y, 2] = p[0];    // B
                        ++p;
                        rgbData[x,y,1]=p[0];        // G
                        ++p;
                        rgbData[x,y,0]=p[0];        // R
                        ++p;
                    }
                    p += ByteOfSkip; // 跳過剩下的 Padding bytes
                }
            }

            bimage.UnlockBits(bmData);
            return rgbData;
        }

        // 高效率圖形轉換工具 -- 由陣列建立新的 Bitmap
        public void setRGBData_unsafe(int[, ,] rgbData) {
            Bitmap bimage = CreateBitmap(rgbData);
            image = bimage;
            this.Refresh();
        }
        public static Bitmap CreateBitmap(int[, ,] rgbData) {
            int Width=rgbData.GetLength(0);
            int Height=rgbData.GetLength(1);

            Bitmap bimage = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            // Step 1: 先鎖住存放圖片的記憶體
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, Width, Height),
                                           ImageLockMode.WriteOnly,
                                           PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;

            // Step 2: 取得像點資料的起始位址
            System.IntPtr Scan0 = bmData.Scan0;

            // 計算每行的像點所佔據的byte 總數
            int ByteNumber_Width = bimage.Width * 3;

            // 計算每一行後面幾個 Padding bytes
            int ByteOfSkip = stride - ByteNumber_Width;
           

            // Step 3:直接利用指標, 把影像資料取出來
            unsafe {
                byte* p = (byte*)(void*)Scan0;
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        p[0] = (byte) rgbData[x, y, 2] ; // 先放 B
                        ++p;
                        p[0] = (byte) rgbData[x, y, 1];  // 再放 G 
                        ++p;
                        p[0] = (byte) rgbData[x, y, 0];  // 最後放 R 
                        ++p;
                    }
                    p += ByteOfSkip; // 跳過剩下的 Padding bytes
                }
            }

            bimage.UnlockBits(bmData);
            return bimage;
        }

        

    }
}