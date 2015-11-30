using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Drawing.Printing;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using NAudio.Wave;


namespace Speech_Typing
{
  
    public partial class SpeechTyping : Form
    {
        Font myanmar3 = new Font("Myanmar3", 10, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        Font zawgyi = new Font("Zawgyi-One", 11, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        DataSet ds1 = new DataSet();
        DataSet ds2 = new DataSet();
        DataSet ds3 = new DataSet();
        DataSet ds4 = new DataSet();
        DataSet ds5 = new DataSet();
        DataSet dsPatSint = new DataSet();
        DataSet ds1000 = new DataSet();
        string xmlpath1 = Properties.Resources.ShortForm1;       
        string xmlpath2 = Properties.Resources.ShortForm2;
        string xmlpath3 = Properties.Resources.ShortForm3;
        string xmlpath4=Properties.Resources.SelectedAll;
        string xmlpath5=Properties.Resources.ShortForm4;
        string xmlpathPatSint = Properties.Resources.PatSint;
        string xmlpath1000 =Properties.Resources.Syallable1000;
        string dir = Directory.GetCurrentDirectory() + "\\Voice\\";
        string dirselected = Directory.GetCurrentDirectory() + "\\SelectedAll\\";
        string dirPatSint = Directory.GetCurrentDirectory() + "\\PatSint\\";
        string dir1000 = Directory.GetCurrentDirectory() + "\\Syallable1000\\";
        Hashtable ht1000=new Hashtable();
        Dictionary<string, string>dic1000 = new Dictionary<string, string>();
        Dictionary<string, string> dicPatSint = new Dictionary<string, string>();
        const string fileformat = ".wav";
        DataRowCollection drc1, drc2, drc3, drc1000, drc4,drc5,drcPatSint;
        string tempStr = string.Empty;
        bool isCollect = false;
        bool isForAnnuvsara = false;
        bool isForDotBelow = false;
        bool isForShaeHtoe = false;
        bool isForSpelling = false;
        string spellingCollect = string.Empty;
        string tempSpellingCollect = string.Empty;
        string tempTemp2 = string.Empty;
        string tempTemp3 = string.Empty;
        int count = 0;
        int time = 475;
        const int pressTime = 200;
        string[] consonants = { "က", "ခ", "ဂ", "ဃ", "င", "စ", "ဆ", "ဇ", "ဈ", "ဉ", "ည", "ဋ", "ဌ", "ဍ", "ဎ", "ဏ", "တ", "ထ", "ဒ", "ဓ", "န", "ပ", "ဖ", "ဗ", "ဘ", "မ", "ယ", "ရ", "လ", "ဝ", "သ", "ဟ", "ဠ", "အ" };
        string[] vowels = { "ါ", "ာ", "ိ", "ီ", "ု", "ူ", "ေ", "ဲ", "ံ", "့", "း", "္", "်", "ျ", "ြ", "ွ", "ှ", "ဿ", "ဣ", "ဤ", "ဥ", "ဦ", "ဧ", "ဩ", "ဪ", "၌", "၍", "၏", "၎င်း" };
        string[] vowelCheck = { "ါ", "ာ", "ိ", "ီ", "ု", "ူ", "ေ", "ဲ", "ံ", "့", "း", "္", "်", "ျ", "ြ", "ွ", "ှ" };
        string[] digit = { "၀", "၁", "၂", "၃", "၄", "၅", "၆", "၇", "၈", "၉", "၊", "။", "+", "−", "×", "÷", "%", "/", "=" };
        string[] punq = { "၊", "။", "(", ")" };
        string[] independentvowels = {"ဿ", "ဣ", "ဤ", "ဥ", "ဦ", "ဧ", "ဩ", "ဪ", "၌", "၍", "၏", "၎င်း" };

        string soundFileCollectorStr = string.Empty;
        public SpeechTyping()
        {
            InitializeComponent();
            txtBox1.ScrollBars = ScrollBars.Vertical;
            using (StringReader readerxmlpath1 = new StringReader(xmlpath1))
            { ds1.ReadXml(readerxmlpath1); }
            using (StringReader readerxmlpath2 = new StringReader(xmlpath2))
            { ds2.ReadXml(readerxmlpath2); }
            using (StringReader readerxmlpath3 = new StringReader(xmlpath3))
            { ds3.ReadXml(readerxmlpath3); }
            using (StringReader readerxmlpath4 = new StringReader(xmlpath4))
            {ds4.ReadXml(readerxmlpath4);}
            using (StringReader readerxmlpath5 = new StringReader(xmlpath5))
            { ds5.ReadXml(readerxmlpath5);}
            using (StringReader readerxmlpathPatSint = new StringReader(xmlpathPatSint))
            { dsPatSint.ReadXml(readerxmlpathPatSint);}
            using (StringReader readerxmlpath1000 = new StringReader(xmlpath1000))
            {  ds1000.ReadXml(readerxmlpath1000);}
            drc1 = ds1.Tables["short"].Rows;
            drc2 = ds2.Tables["short"].Rows;
            drc3 = ds3.Tables["short"].Rows;
            drc4=ds4.Tables["short"].Rows;
            drc5 = ds5.Tables["short"].Rows;
            drcPatSint = dsPatSint.Tables["short"].Rows;
            drc1000 = ds1000.Tables["short"].Rows;
           ht1000 = loadDataForHashTable(drc1000);
            dic1000 = GetDict(ds1000.Tables["short"]);
            dicPatSint=GetDict(dsPatSint.Tables["short"]);
            SoundPlayer start = new SoundPlayer(String.Format("{0}START{1}", @dir, fileformat));
            start.Play();
        }

        internal Dictionary<string, string> GetDict(DataTable dt)
        {
            return dt.AsEnumerable()
              .ToDictionary<DataRow, string, string>(row => row.Field<string>(0),
                                        row => row.Field<string>(1));
        }

     private Hashtable loadDataForHashTable(DataRowCollection drc)
        {
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in drc)
            {
                ht.Add(dr[0].ToString(), dr[1].ToString());
            }
            return ht;
        }
     #region Menu Item
     private void aboutSpeechTypingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                sw.Write(txtBox1.Text);
                sw.Close();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtBox1.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                txtBox1.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtBox1.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtBox1.Copy();
            SoundPlayer Sound = new SoundPlayer(String.Format("{0}Ctrl+C{1}", @dir, fileformat));
            Sound.Play();
        }
        
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtBox1.SelectAll();
            soundFileCollectorStr = string.Empty;
           SoundPlayer soundplay = new SoundPlayer(String.Format("{0}Ctrl+A{1}", @dir, fileformat));
            soundplay.Play();
           Thread.Sleep(GetWavFileDuration(String.Format("{0}Ctrl+A{1}", @dir, fileformat)));
            ArrayList outarrlist = new ArrayList();
            #region
            /*    string paragraph = txtBox1.Text;
            int count = 0;
            int sentence = 0;
           string lineNo = string.Empty;

            for (int j = 0; j < paragraph.Length;j++ )
            {
                if (paragraph[j] == '။')
                {
                   count++;
               }
                sentence = count;
                lineNo = LocalFunction.ChangeBurmeseNo(sentence.ToString());
                lineNo = lineNo.Replace(" ", "");   
           }
            FrmShowDialog f3 = new FrmShowDialog();
            f3.SetLabelText(lineNo);
            f3.ShowDialog();
            */
            #endregion
          
          string strSyllable = txtBox1.Text;         
            strSyllable = strSyllable.Replace(" ", "");
            int i = 0;
            foreach (DataRow p in drcPatSint)
            {
                if (Regex.IsMatch(strSyllable, p[0].ToString(), RegexOptions.IgnoreCase))
                {
                    strSyllable = strSyllable.Replace(p[0].ToString(), "Sh" + i + "ort");
                    outarrlist.Add(p[0].ToString());
                    i++;
                }
            }


            strSyllable = strSyllable.Replace(' ', ' ');
            strSyllable = strSyllable.Replace('္', '်');
            strSyllable = strSyllable.Replace("့်", "့်");
            
            strSyllable = strSyllable.Replace("\u200C", "");
            strSyllable = strSyllable.Replace("\r\n", "");
            strSyllable = Regex.Replace(strSyllable, "(?<con>[ဂခငဒပဝ])\u102C", "${con}\u102B");

            strSyllable = syllableOrtho(strSyllable);

            #region shortFormReplace
            for (int a = 0; a < outarrlist.Count; a++)
            {
                strSyllable = strSyllable.Replace("Sh" + a + "ort", outarrlist[a].ToString() + "Ò");
            }

            #endregion

            strSyllable = strSyllable.Replace("ÒÒ", "Ò");
            strSyllable = strSyllable.Replace("််","်");
            string[] syllableArr = strSyllable.Split(new string[] { "Ò" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in syllableArr)
            {
                if(punq.Contains(str))
                {
                    Thread.Sleep(time);
                    continue;
                }
                 try 
                 {

                     string strPur = Regex.Replace(str, "(?<pun>[၊။])", "");
                     #region dic1000.ContainsKey(strPur.Trim()
                     if (dic1000.ContainsKey(strPur.Trim()))
                    {
                        string strTemp = dic1000[strPur.Trim()];
                        SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                        soundPlayer1.Play();
                        soundFileCollectorStr += dir1000 + strTemp + fileformat + "\n";
                        tempSpellingCollect = string.Empty;
                        //Thread.Sleep(time);
                        Thread.Sleep(GetWavFileDuration(dir1000 + strTemp + fileformat));
                    }
                     #endregion
                     #region consonants.Contains(strPur.Trim()
                     else if (consonants.Contains(strPur.Trim()))
                    {
                        foreach (DataRow dr in drc4)
                        {
                            if (strPur.Trim() == dr[0].ToString())
                            {
                                SoundPlayer selectconsonant = new SoundPlayer(dirselected + dr[1] + fileformat);
                                selectconsonant.Play();
                                soundFileCollectorStr += dirselected + dr[1] + fileformat + "\n";
                                //Thread.Sleep(time);
                                Thread.Sleep(GetWavFileDuration(dirselected + dr[1] + fileformat));
                                continue;
                            }
                        }
                    }
                     #endregion
                     #region digit.Contains(strPur.Trim()
                     else if (digit.Contains(strPur.Trim()))
                    {
                        foreach (DataRow dr in drc3)
                        {
                            if (strPur.Trim() == dr[0].ToString())
                            {
                                SoundPlayer selectdigit = new SoundPlayer(@dir + dr[1] + fileformat);
                                selectdigit.Play();
                                soundFileCollectorStr += @dir + dr[1] + fileformat + "\n";
                                //Thread.Sleep(time);
                                Thread.Sleep(GetWavFileDuration(@dir + dr[1] + fileformat));
                                continue;
                            }
                        }
                    }
                     #endregion
                     #region dicPatSint.ContainsKey(strPur.Trim()
                     else if (dicPatSint.ContainsKey(strPur.Trim()))
                    {
                       
                        string strTemp = dicPatSint[strPur.Trim()];
                        SoundPlayer PatSint = new SoundPlayer(dirPatSint+ strTemp + fileformat);
                        PatSint.Play();
                        soundFileCollectorStr += dirPatSint + strTemp + fileformat + "\n";
                        tempSpellingCollect = string.Empty;
                        //Thread.Sleep(time+250);
                        Thread.Sleep(GetWavFileDuration(dirPatSint + strTemp + fileformat));

                    }
                     #endregion
                     #region independentvowels.Contains(strPur.Trim()
                     else if (independentvowels.Contains(strPur.Trim()))
                    {
                        foreach (DataRow dr in drc5)
                        {
                            if (strPur.Trim() == dr[0].ToString())
                            {
                                SoundPlayer independentvowel = new SoundPlayer(dirselected + dr[1] + fileformat);
                                independentvowel.Play();
                                
                                soundFileCollectorStr += dirselected + dr[1] + fileformat + "\n";
                                //Thread.Sleep(time);
                                Thread.Sleep(GetWavFileDuration(dirselected + dr[1] + fileformat));
                                continue;
                            }
                        }
                    }
                     #endregion
                 }
                  catch
                {
                    SoundPlayer exception = new SoundPlayer(String.Format("{0}exception{1}", @dir, fileformat));
                    exception.Play();
                    Thread.Sleep(time);
                    //Thread.Sleep(GetWavFileDuration(dirPatSint + strTemp + fileformat));
                }                                         
             }
        }

        public static TimeSpan GetWavFileDuration(string fileName)
        {
            WaveFileReader wf = new WaveFileReader(fileName);
            return wf.TotalTime;
        }

        public string syllableOrtho(string instr)
        {
            //Pre process
            instr = instr.Replace("့်", "့်");
            instr = instr.Replace("ဿ", "သ်သ");
            instr = Regex.Replace(instr, "(?<con>[က-ဪဿ၌-၏])", "Ò${con}");
      //      InputStr=InputStr.replaceAll("([က-ဿ၌-၏])([!-×၀-။‘-”])", "$1Ò$2");//syllable English and Pun(For Phono)
            instr = Regex.Replace(instr, "Ò(?<con>[က-အ])(?<killer>[့်])", "${con}${killer}");
            instr = Regex.Replace(instr, "Ò(?<con>[က-အ])(?<killer>[္်])", "${con}${killer}");
            instr = Regex.Replace(instr, "(?<kill>[္])Ò(?<con>[က-အ])", "${kill}${con}");
            instr = Regex.Replace(instr, "(?<con>[က-ဿ၌-၏])(?<pun>[!-×၀-။‘-”])", "${con}Ò${pun}");//syllable English and Pun
            //instr = Regex.Replace(instr, "(?<no>[၀-၉])", "Ò${no}");
            instr = instr.Replace("ÒÒ", "Ò");
            instr = instr.Replace("ဿ", "သ်သ");
            //instr = Regex.Replace(instr, "(?<no1>[၀-၉])Ò(?<no2>[၀-၉])", "${no1}${no2}");
            instr = instr + "Ò";
            //End pre process
            return instr;
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string help = Directory.GetCurrentDirectory() + "\\User Manual for Speech Typing.pdf";
            System.Diagnostics.Process.Start(help);
        }

        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                txtBox1.Font = fontDialog1.Font;
                txtBox1.ForeColor = fontDialog1.Color;
            }
        }
     #endregion
        
     

    }
}
