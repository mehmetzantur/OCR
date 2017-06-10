
using AForge;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using tesseract;

namespace LicensePlateRecognition
{
    public partial class OCR : Form
    {

        #region Initialize
        private List<string> lstimages = new List<string>(); //Image urlsi için liste oluşturulmaktadır.
        int position = 0;

        private TesseractProcessor m_tesseract = null;
        private string m_path = Application.StartupPath + @"\data\";
        private const string m_lang = "eng";
        #endregion

        #region InitializeComponent
        public OCR()
        {
            InitializeComponent();
        }
        #endregion

        #region Ocr
        private string Ocr(Bitmap image) //OCR Metotları
        {
            m_tesseract.Clear();
            m_tesseract.ClearAdaptiveClassifier();
            
            return m_tesseract.Apply(image);
        }
        #endregion

        #region OCR_Load
        private void OCR_Load(object sender, EventArgs e)
        {
            m_tesseract = new TesseractProcessor();
            bool succeed = m_tesseract.Init(m_path, m_lang, 3);
            if (!succeed)
            {
                MessageBox.Show("Tesseract initialization failed. The application will exit.");
                Application.Exit();
            }
            m_tesseract.SetVariable("tessedit_char_whitelist", "ABCDEFHKLMNPQRSTUVXYZ-.1234567890").ToString(); //Training Sets //abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVXYZ-.1234567890
            
            System.Environment.CurrentDirectory = System.IO.Path.GetFullPath(m_path);
        }
        #endregion

        #region btnLoad_Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            lstimages.Clear();
            timer1.Enabled = false;
            string folder = Application.StartupPath + "\\Images";

            foreach (string fileName in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
            {
                lstimages.Add(Path.GetFullPath(fileName)); //Image urlsi listeye eklenmektedir.
            }

            if (lstimages.Count == 0)
            {
                foreach (string fileName in Directory.GetFiles(folder, "*.jpg", SearchOption.TopDirectoryOnly))
                {
                    lstimages.Add(Path.GetFullPath(fileName)); //Image urlsi listeye eklenmektedir.
                }
            }
            if (lstimages.Count != 0)
                timer1.Enabled = true;
        }
        #endregion

        #region timer1_Tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (position < lstimages.Count)
            {
                ProcessImage(lstimages[position]); //Listede bulunan ilgili pozisyondaki görüntü ProcessImage metoduna gönderilmetekdir.
                position++;
            }
            else
                position = 0;
        }
        #endregion

        #region ProcessImage
        public void ProcessImage(string urlImage) //Görüntünün işlendiği metot.
        {
            Stopwatch watch = Stopwatch.StartNew(); //Görüntünün işlenmesi sırasında süre tutulması için süre başlatılmaktadır.

            Bitmap root = new Bitmap(urlImage); //Root Image (Default Image)
            Bitmap bmp = new Bitmap(root); //Resize Image
            Bitmap bmp1 = new Bitmap(root); //Smoothing Image
            Bitmap bmp2 = new Bitmap(root); //Color Filtering Image
            string outputstring = "";

            ///Root
            outputstring = this.Ocr(root);
            txtNumbePlate1.Text = outputstring;
            ptBNumberPlate1.Image = root;


            //Resize Image to : 1920x1080
            outputstring = "";
            ResizeBilinear filterr = new ResizeBilinear(1920, 1080);
            bmp = filterr.Apply(bmp);

            outputstring = this.Ocr(bmp);
            txtNumbePlate2.Text = outputstring;
            ptBNumberPlate2.Image = bmp;


            //Smoothing
            outputstring = "";
            if (BitmapFilter.GaussianBlur(bmp1, 4))
                if (BitmapFilter.GaussianBlur(bmp1, 4))
                    if (BitmapFilter.MeanRemoval(bmp1, 9))
                    { }
            outputstring = this.Ocr(bmp1);
            txtNumbePlate3.Text = outputstring;
            ptBNumberPlate3.Image = bmp1;


            //Color Filtering
            outputstring = "";
            ColorFiltering filter = new ColorFiltering();
            filter.Green = new IntRange(175, 255);
            bmp2 = filter.Apply(bmp2);

            outputstring = this.Ocr(bmp2);
            txtNumbePlate4.Text = outputstring;
            ptBNumberPlate4.Image = bmp2;

            watch.Stop(); //Stop The Timer
            lblTime.Text = String.Format("Total Time : {0} Seconds", watch.Elapsed.TotalSeconds);
            lblCount.Text = position.ToString() + "/" + lstimages.Count.ToString();
            System.Threading.Thread.Sleep(5000);
        }
        #endregion

        #region FilterLicensePlate
        /*private string FilterLicensePlate(string licensePlate)
        {
            licensePlate = licensePlate.Replace(".", "");
            licensePlate = licensePlate.Replace("-", "");

            licensePlate = licensePlate.Replace(" ", "");
            licensePlate = licensePlate.Replace("?", "");
            licensePlate = licensePlate.Replace("_", "");
            licensePlate = licensePlate.Replace(",", "");
            licensePlate = licensePlate.Replace("|", "");
            licensePlate = licensePlate.Replace(" ", "");
            licensePlate = licensePlate.Replace("l", "");
            licensePlate = licensePlate.Replace("~", "");
            licensePlate = licensePlate.Replace(",", "");
            licensePlate = licensePlate.Replace(":", "");
            licensePlate = licensePlate.Replace("'", "");
            licensePlate = licensePlate.Replace("!", "");

            licensePlate = licensePlate.ToUpper();
            return licensePlate;
        }*/
        #endregion
    }
}
