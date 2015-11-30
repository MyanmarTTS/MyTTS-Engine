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

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pasteTxt=Clipboard.GetText();
            string result = string.Empty;
            if (checkFontIsZawgyi(pasteTxt))
            {
                result = ZawGyiToUnicode.Zawgyi2Uni(pasteTxt);
            }
            else result = pasteTxt;

            txtBox1.Paste(result);
            //SoundPlayer player = new SoundPlayer(String.Format("{0}Ctrl+V{1}", @dir, fileformat));
            //player.Play();
           
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

                     Match matchcase = Regex.Match(str.Trim(), @"([၀-၉])", RegexOptions.IgnoreCase);
                      if (matchcase.Success)
                     {
                         string strNumber = string.Empty;
                         string strPurt = Regex.Replace(str, "(?<pun>[(),။၊])", "Ò");
                         string[] strArrForNumber = strPurt.Split(new string[] { "Ò" }, StringSplitOptions.RemoveEmptyEntries);
                         foreach (string strForNumber in strArrForNumber)
                         {
                             int number = (int)LocalFunction.Parse(LocalFunction.ChangeEnglishNo(strForNumber), typeof(int), 0);
                             strNumber += LocalFunction.NumbersToWords(number);
                         }
                         strNumber = strNumber.Replace(" ", "");
                         strNumber = strNumber.Replace("့်", "့်");
                         strNumber = syllableOrtho(strNumber);
                         string[] nosyllableArr = strNumber.Split(new string[] { "Ò" }, StringSplitOptions.RemoveEmptyEntries);
                         foreach (string strNo in nosyllableArr)
                         {
                             if (dic1000.ContainsKey(strNo.Trim()))
                             {
                                 string strTemp = dic1000[strNo.Trim()];
                                 SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                                 soundPlayer1.Play();
                                 soundFileCollectorStr += dir1000 + strTemp + fileformat+"\n";
                                 //tempSpellingCollect = string.Empty;
                                 //Thread.Sleep(time);
                                 Thread.Sleep(GetWavFileDuration(dir1000 + strTemp + fileformat));
                             }
                             else if (consonants.Contains(strNo.Trim()))
                             {
                                 foreach (DataRow dr in drc4)
                                 {
                                     if (strNo.Trim() == dr[0].ToString())
                                     {
                                         SoundPlayer selectconsonant = new SoundPlayer(dirselected + dr[1] + fileformat);
                                         selectconsonant.Play();
                                         soundFileCollectorStr += dirselected + dr[1] + fileformat + "\n";
                                         //Thread.Sleep(time);
                                         Thread.Sleep(GetWavFileDuration(dirselected + dr[1] + fileformat));
                                     }
                                 }
                             }
                         }
                         continue;
                     }
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
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            char ch = e.KeyChar;
            string chstr = ch.ToString();
            if (chstr != "\b")/////////////For ThaWaiHtoe***********************
            {
                if (chstr == "\u200B")
                {
                    isCollect = true;
                }
                else if (isCollect)
                {
                    if (chstr == "ေ")
                    {
                        isCollect = false;
                        SoundPlayer soundPlayer = new SoundPlayer(String.Format("{0}1031{1}", @dir, fileformat));
                        soundPlayer.Play();
                        return;
                    }
                }

                if (chstr == "၎")
                {
                    tempStr += chstr;
                    isCollect = true;
                    return;
                }
                else if (isCollect)
                {
                    if (chstr == "း")
                    {
                        tempStr += chstr;
                        isCollect = false;
                        chstr = tempStr;
                        tempStr = string.Empty;
                    }
                    else
                    {
                        tempStr += chstr;
                        return;
                    }
                }

                if (chstr == "ု") isForAnnuvsara = true;
                else if (isForAnnuvsara)
                {
                    if (chstr == "ံ")
                    {
                        isForAnnuvsara = false;
                        return;
                    }
                }
                if (chstr == "့") isForDotBelow = true;
                else if (isForDotBelow)
                {
                    if (chstr == "်")
                    {
                        isForDotBelow = false;
                        return;
                    }
                }
                if (chstr == "ာ") isForShaeHtoe = true;
                else if (chstr == "ါ") isForShaeHtoe = true;
                else if (consonants.Contains(chstr)) isForShaeHtoe = false;
                /*if (isForShaeHtoe)
                //{
                //    if (chstr == "်")
                //    {
                //        isForShaeHtoe = false;
                //        SoundPlayer soundPlayer = new SoundPlayer(String.Format("{0}103A_Htoe{1}", @dir, fileformat));
                //        soundPlayer.Play();
                //        //return;
                //    }
                //}*/

                //if (consonants.Contains(chstr)) { spellingCollect = chstr; isForSpelling = false;}
                //else if (vowelCheck.Contains(chstr)) { spellingCollect += chstr; isForSpelling = true; }
                //else if (Convert.ToChar(chstr) == (char)Keys.Space) { spellingCollect = string.Empty; tempSpellingCollect = string.Empty; isForSpelling = true; }
                if (chstr != "\b")
                {
                    spellingCollect += chstr;
                }
                // if (ch == 'ေ') return;
                DataRowCollection drcollection = null;
                if (consonants.Contains(chstr))
                {
                    drcollection = drc1;
                }
                else if (vowels.Contains(chstr))
                {
                    drcollection = drc2;
                }
                else if (digit.Contains(chstr))
                {
                    drcollection = drc3;
                }
                if (drcollection != null)
                {
                    foreach (DataRow dr in drcollection)
                    {
                        if (chstr == dr[0].ToString())
                        {
                            if (isForShaeHtoe)
                            {
                                if (chstr == "်")
                                {
                                    isForShaeHtoe = false;
                                    SoundPlayer soundPlayer = new SoundPlayer(String.Format("{0}103A_Htoe{1}", @dir, fileformat));
                                    soundPlayer.Play();
                                    Thread.Sleep(pressTime);
                                    // return;
                                }
                                else
                                {
                                    SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                                    soundPlayer.Play();
                                    //Thread.Sleep(pressTime);
                                    Thread.Sleep(GetWavFileDuration(@dir + dr[1] + fileformat));
                                }
                            }
                            else
                            {
                                if (chstr == "ေ" || chstr == "ံ")
                                {
                                    count++;
                                    if (count == 1)
                                    {
                                        SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                                        soundPlayer.Play();
                                        //Thread.Sleep(pressTime);
                                        Thread.Sleep(GetWavFileDuration(@dir + dr[1] + fileformat));
                                    }
                                    else { count = 0; }
                                }
                                else
                                {
                                    SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                                    soundPlayer.Play();
                                    //Thread.Sleep(pressTime);
                                    Thread.Sleep(GetWavFileDuration(@dir + dr[1] + fileformat));
                                }
                            }
                            break;
                            //Thread.Sleep(400);
                        }
                    }
                }

                #region SpellingCharstr
                //if (isForSpelling)
                //{
                //    spellingCollect = spellingCollect.Replace("ံု", "ုံ");
                //    if (dic1000.ContainsKey(spellingCollect))
                //    {
                //        tempSpellingCollect = spellingCollect;
                //        string strTemp = dic1000[spellingCollect];
                //        SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                //        soundPlayer1.Play();
                //        spellingCollect = string.Empty;
                //        Thread.Sleep(400);
                //    }
                //    else
                //    {
                //        string playStr = tempSpellingCollect + spellingCollect;
                //        tempSpellingCollect += spellingCollect;
                //        if (dic1000.ContainsKey(tempSpellingCollect))
                //        {
                //            string strTemp = dic1000[tempSpellingCollect];
                //            SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                //            soundPlayer1.Play();
                //            tempSpellingCollect = string.Empty;
                //            Thread.Sleep(600);
                //        }
                //        else
                //        {
                //            if (chstr == "်")
                //            {
                //                tempTemp2 += chstr;
                //                string playStr = tempTemp2 + chstr;
                //                if (dic1000.ContainsKey(tempTemp2))
                //                {
                //                    string strTemp = dic1000[tempTemp2];
                //                    SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                //                    soundPlayer1.Play();
                //                    tempTemp3 = tempTemp2;
                //                    tempTemp2 = string.Empty;
                //                    Thread.Sleep(600);
                //                }

                //            }
                //            else if (chstr == "း")
                //            {
                //                tempTemp3 += chstr;
                //                if (dic1000.ContainsKey(tempTemp3))
                //                {
                //                    string strTemp = dic1000[tempTemp3];
                //                    SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                //                    soundPlayer1.Play();
                //                    tempTemp3 = string.Empty;
                //                    Thread.Sleep(600);
                //                }
                //            }
                //            else if (chstr == "့")
                //            {
                //                tempTemp3 += chstr;
                //                if (dic1000.ContainsKey(tempTemp3))
                //                {
                //                    string strTemp = dic1000[tempTemp3];
                //                    SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                //                    soundPlayer1.Play();
                //                    tempTemp3 = string.Empty;
                //                    Thread.Sleep(600);
                //                }

                //            }
                //        }
                //    }
                //    #region SpellingCollect>ToCharArray

                //    /* bool isForKiller = false;
                //    foreach (char getcharr in spellingCollect.ToCharArray())
                //    {
                //        drcollection = null;
                //        if (consonants.Contains(getcharr.ToString()))
                //        {
                //            drcollection = drc1;
                //        }
                //        else if (vowels.Contains(getcharr.ToString()))
                //        {
                //            drcollection = drc2;
                //        }
                //        else if (digit.Contains(getcharr.ToString()))
                //        {
                //            drcollection = drc3;
                //        }
                //        if (drcollection != null)
                //        {
                //            foreach (DataRow dr in drcollection)
                //            {
                //                if (getcharr.ToString() == dr[0].ToString())
                //                {
                //                    if (isForKiller)
                //                    {
                //                        if (getcharr == '်')
                //                        {
                //                            SoundPlayer soundPlayer = new SoundPlayer(@dir + "103A_Htoe" + fileformat);
                //                            soundPlayer.Play();
                //                        }
                //                        else
                //                        {
                //                            SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                //                            soundPlayer.Play();
                //                        }
                //                    }
                //                    else
                //                    {
                //                        if (getcharr == 'ာ' || getcharr == 'ါ') isForKiller = true;
                //                        else isForKiller = false;
                //                        SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                //                        soundPlayer.Play();
                //                    }
                //                    Thread.Sleep(500);
                //                    break;
                //                }
                //            }
                //        }
                //    }*/
                //    #endregion
                //}
                //else
                //{
                //    if (tempTemp2.ToCharArray().Length < 2)
                //        tempTemp2 += chstr;
                //    else
                //    {
                //        tempTemp2 = tempTemp2.Remove(0);
                //        tempTemp2 += chstr;
                //    }
                //}
                #endregion


                #region SpellTyping

                //spellingCollect = spellingCollect.Replace("\b", "");
                //spellingCollect = spellingCollect.Replace("\u1031\u103B\u1031", "\u103B\u1031");
                spellingCollect = spellingCollect.Replace('္', '်');
                spellingCollect = Regex.Replace(spellingCollect, "(?<con>[ဂခငဒပဝ])\u102C", "${con}\u102B");
                spellingCollect = Regex.Replace(spellingCollect, "\u1031(?<med>[ျ-ှ])\u1031", "${med}\u1031");
                //spellingCollect = Regex.Replace(spellingCollect, "(?<con>[က-အ])(?<thawai>[ေ])(?<med>[ျ-ှ])(?<thawai2>[ေ])", "${con}${med}${thawai2}");
                //spellingCollect = Regex.Replace(spellingCollect, "(?<con>[က-အ])(?<thawai>[ေ])(?<med>[ျ-ှ])", "${con}${med}${thawai}");
                spellingCollect = syllableOrtho(spellingCollect);
                string[] spellArr = spellingCollect.Split(new string[] { "Ò" }, StringSplitOptions.RemoveEmptyEntries);
                if (spellArr.Length != 0)
                {
                    string lastSyllable = spellArr[spellArr.Length - 1];
                    if (dic1000.ContainsKey(lastSyllable))
                    {
                        string strTemp = dic1000[lastSyllable];
                        SoundPlayer soundPlayer1 = new SoundPlayer(dir1000 + strTemp + fileformat);
                        soundPlayer1.Play();
                        spellingCollect = lastSyllable;
                        Thread.Sleep(GetWavFileDuration(dir1000 + strTemp + fileformat));
                        //Thread.Sleep(pressTime);
                    }
                    else spellingCollect = spellingCollect.Replace("Ò", "");
                }
                else spellingCollect = spellingCollect.Replace("Ò", "");
                #endregion


                #region Controls Keys
                if (ch == (char)Keys.Enter)
                {
                    SoundPlayer soundplayer = new SoundPlayer(String.Format("{0}Enter{1}", @dir, fileformat));
                    soundplayer.Play();
                }
                else if (ch == (char)Keys.Space)
                {
                    SoundPlayer sound = new SoundPlayer(String.Format("{0}Space{1}", @dir, fileformat));
                    sound.Play();
                }
                #endregion

                if (checkFontIsZawgyi(chstr))
                {
                    
                }
              
                     
                 

            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           
            string deleteChar = string.Empty;
            TextBox t = sender as TextBox;
            bool isForKiller = false;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                case Keys.Left:
                case Keys.Right:
                case Keys.Back:
                case Keys.End:
                case Keys.Home:
                    int start = e.KeyCode == Keys.Back && t.SelectionLength == 0 ? t.SelectionStart - 1 : t.SelectionStart;
                    if (e.KeyCode == Keys.Left) start = start - 1;
                    else if (e.KeyCode == Keys.Home) start = 0;
                    else if (e.KeyCode == Keys.End) start = t.Text.Length - 1;
                    Point p = Cursor.Position;
                    int indexPosition = t.GetCharIndexFromPosition(p);
                    int length = t.SelectionLength == 0 ? 1 : t.SelectionLength;
                    int textLength = t.Text.Length;
                    if (start != -1)
                    {
                        if (start != textLength)
                            deleteChar = t.Text.Substring(start, length);
                    }
                    if (deleteChar != " ")
                    {
                        #region Keys.Delete
                        if (e.KeyCode == Keys.Delete)
                        {
                            if (start != t.Text.Length)
                            {
                                string str = t.Text.Substring(start + 1);
                                string lastChar = string.Empty;
                                bool isBreak = true;
                                foreach (char ch in str.ToCharArray())
                                {
                                    string compStr = ch.ToString();
                                    if (ch == '္')
                                    {
                                        isBreak = false;
                                    }
                                    if (consonants.Contains(compStr))
                                    {
                                        if (isBreak)
                                        {
                                            lastChar = compStr; break;
                                        }
                                        else isBreak = true;
                                    }
                                }
                                if (lastChar != "")
                                {
                                    int lastIndex = str.IndexOf(Convert.ToChar(lastChar)) + start + 1;
                                    length = lastIndex - start;
                                    start = lastIndex;
                                }
                                else
                                {
                                    if (str.ToCharArray().Length != 0)
                                    {
                                        length = str.ToCharArray().Length + 1;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Keys.Left or Keys.End
                        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.End)
                        {
                            string str = t.Text.Substring(0, start + 1);
                            string lastChar = string.Empty;
                            foreach (char ch in str.ToCharArray())
                            {
                                string compStr = ch.ToString();
                                if (consonants.Contains(compStr))
                                {
                                    lastChar = compStr;
                                }
                            }

                            if (str.Contains('္'))
                            {
                                if (str.LastIndexOf('္') > str.LastIndexOf(Convert.ToChar(lastChar)))
                                {
                                    int lastIndex = str.LastIndexOf('္') - 1;
                                    length = start + 1 - lastIndex;
                                    start = lastIndex;
                                }
                                else
                                {
                                    int lastIndex = str.LastIndexOf(Convert.ToChar(lastChar));
                                    length = start + 1 - lastIndex;
                                    start = lastIndex;
                                }
                            }
                            else if (lastChar != "")
                            {
                                int lastIndex = str.LastIndexOf(Convert.ToChar(lastChar));
                                length = start + 1 - lastIndex;
                                start = lastIndex;
                            }

                        }
                        #endregion

                        #region Keys.Right or Keys.Home
                        else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Home)
                        {
                            if (start != t.Text.Length)
                            {
                                string str = t.Text.Substring(start + 1);
                                string lastChar = string.Empty;
                                bool isBreak = true;
                                foreach (char ch in str.ToCharArray())
                                {
                                    string compStr = ch.ToString();
                                    if (ch == '္')
                                    {
                                        isBreak = false;
                                    }
                                    if (consonants.Contains(compStr))
                                    {
                                        if (isBreak)
                                        {
                                            lastChar = compStr; break;
                                        }
                                        else isBreak = true;
                                    }
                                }
                                if (lastChar != "")
                                {
                                    int lastIndex = str.IndexOf(Convert.ToChar(lastChar)) + start + 1;
                                    length = lastIndex - start;
                                    //start = lastIndex;
                                }
                                else
                                {
                                    if (str.ToCharArray().Length != 0)
                                    {
                                        length = str.ToCharArray().Length + 1;
                                    }
                                }
                            }
                        }
                        #endregion

                        textLength = t.Text.Length;
                        if (start != -1)
                        {
                            if (start != textLength)
                            {
                                if(start!=length)
                                deleteChar = t.Text.Substring(start, length);
                                else deleteChar = t.Text;
                            }
                        }
                    }
                    else
                    {
                        if (deleteChar == " ")
                        {
                            SoundPlayer sound = new SoundPlayer(String.Format("{0}Space{1}", @dir, fileformat));
                            sound.Play();
                            return;
                        }
                    }

                    if (deleteChar != "")
                    {
                       
                        foreach (char getcharr in deleteChar.ToCharArray())
                        {
                            DataRowCollection drcollection = null;
                            if (consonants.Contains(getcharr.ToString()))
                            {
                                drcollection = drc1;
                            }
                            else if (vowels.Contains(getcharr.ToString()))
                            {
                                drcollection = drc2;
                            }
                            else if (digit.Contains(getcharr.ToString()))
                            {
                                drcollection = drc3;
                            }
                            if (drcollection != null)
                            {
                                foreach (DataRow dr in drcollection)
                                {
                                    if (getcharr.ToString() == dr[0].ToString())
                                    {
                                        if (isForKiller)
                                        {
                                            if (getcharr == '်')
                                            {
                                                SoundPlayer soundPlayer = new SoundPlayer(String.Format("{0}103A_Htoe{1}", @dir, fileformat));
                                                soundPlayer.Play();
                                            }
                                            else
                                            {
                                                SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                                                soundPlayer.Play();
                                                isForKiller = false;
                                            }
                                        }
                                        else
                                        {
                                            if (getcharr == 'ာ' || getcharr == 'ါ') isForKiller = true;
                                            else isForKiller = false;
                                            SoundPlayer soundPlayer = new SoundPlayer(@dir + dr[1] + fileformat);
                                            soundPlayer.Play();
                                        }
                                        Thread.Sleep(pressTime);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            #region Controls + Keys
            if (e.Control && (e.KeyCode == Keys.A))
            {
                txtBox1.SelectAll();
                SoundPlayer soundplay = new SoundPlayer(String.Format("{0}Ctrl+A{1}", @dir, fileformat));
                soundplay.Play();
            }
            else if (e.Control && (e.KeyCode == Keys.C))
            {
                txtBox1.Copy();
                SoundPlayer Sound = new SoundPlayer(String.Format("{0}Ctrl+C{1}", @dir, fileformat));
                Sound.Play();
            }
            else if (e.Control && (e.KeyCode == Keys.V))
            {
                txtBox1.Paste();
                SoundPlayer player = new SoundPlayer(String.Format("{0}Ctrl+V{1}", @dir, fileformat));
                player.Play();
            }
            else if (e.KeyCode == Keys.Home)
            {
                SoundPlayer soundplayer = new SoundPlayer(String.Format("{0}Home{1}", dir, fileformat));
                soundplayer.Play();
            }
            else if (e.KeyCode == Keys.End)
            {
                SoundPlayer soundplayer = new SoundPlayer(String.Format("{0}End{1}", @dir, fileformat));
                soundplayer.Play();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                SoundPlayer soundplayer = new SoundPlayer(String.Format("{0}PageUp{1}", @dir, fileformat));
                soundplayer.Play();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                SoundPlayer soundplayer = new SoundPlayer(String.Format("{0}PageDown{1}", @dir, fileformat));
                soundplayer.Play();
            }
        }
            #endregion
        public string detect(string inputStr)
        {

            string result = string.Empty;
            bool isZawgyi = false;
            bool isUnicode = false;
            isZawgyi = checkFontIsZawgyi(inputStr);
            isUnicode = checkFontIsUnicode(inputStr);
            if (isUnicode == isZawgyi) result = string.Empty;
            else if (isUnicode) result = myanmar3.ToString();
            else if (isZawgyi) result = zawgyi.ToString();
            return result;
        }
        public string detect(char[] text)
        {
            string result = string.Empty;
            bool isZawgyi = false;
            bool isUnicode = false;
            isZawgyi = checkFontIsZawgyi(new string(text));
            isUnicode = checkFontIsUnicode(new string(text));
            if (isUnicode == isZawgyi) result = string.Empty;
            else if (isUnicode) result = myanmar3.ToString();
            else if (isZawgyi) result = zawgyi.ToString();
            return result;
        }
        public string detectUnicode(string inputStr)
        {
            string result = zawgyi.ToString();
            return result;
        }
        public static bool checkFontIsZawgyi(string inputStr)
        {
            bool isZawgyi = false;
            bool isMyanmarFont = true;
            //   Regex rx = new Regex("\\p{InMyanmar}");
            isMyanmarFont = Regex.IsMatch(inputStr, @"\p{IsMyanmar}");
            //MatchCollection matches = rx.Matches(inputStr);
            if (isMyanmarFont)
            {
                isZawgyi = Regex.IsMatch(inputStr, "[ၚ-႗]");
                isZawgyi = Regex.IsMatch(inputStr, "[ဳဴ]");
                isZawgyi = Regex.IsMatch(inputStr, "[ေ][ျၾၿႀႁႂႃႄ][က-အႏ႐]");
                isZawgyi = Regex.IsMatch(inputStr, "[^က-အႏ႐][ေ][က-အႏ႐]");
                isZawgyi = Regex.IsMatch(inputStr, "[^က-အႏ႐][ျၾၿႀႁႂႃႄ][က-အႏ႐]");
                isZawgyi = Regex.IsMatch(inputStr, "[က-အႏ႐][်][ား]");
                if (inputStr.Contains("္"))
                {
                    isZawgyi = Regex.IsMatch(inputStr, "[္][့းငဉညယရလဝသဟဠအ]");
                }
                if (isZawgyi)
                {
                    isZawgyi = Regex.IsMatch(inputStr, "[္][^က-အ]");
                    isZawgyi = Regex.IsMatch(inputStr, "[္$]");
                }
            }
            return isZawgyi;

        }
        private bool checkFontIsUnicode(string inputStr)
        {
            bool isMyanmarFont = false;
            bool isUnicode = false;
            Regex rx = new Regex("\\p{IsMyanmar}");
            MatchCollection matches = rx.Matches(inputStr);
            if (isMyanmarFont)
            {
                isUnicode = Regex.IsMatch(inputStr, "[က-အ][ျ-ှ]+[ါ-ံ]+[့း]?");
                isUnicode = Regex.IsMatch(inputStr, "[က-အ][ျ-ှ]+[ါ-ံ]+[့း]?");
                isUnicode = Regex.IsMatch(inputStr, "[က-အ][ျ-ှ]+");
                isUnicode = Regex.IsMatch(inputStr, "[က-အ][ါ-ံ]+[့း]?");
                if (inputStr.Contains("်"))
                {
                    isUnicode = Regex.IsMatch(inputStr, "[က-အ][်]");
                }
                if (inputStr.Contains("္"))
                {
                    isUnicode = Regex.IsMatch(inputStr, "[္][က-အ]");
                }
            }
            return isUnicode;
        }
        private void convertToUnicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Outputstr = UniConversion.Converter.ZawGyiOne2Uni(txtBox1.Text);
            txtBox1.Text = null;
            txtBox1.Font = myanmar3;
            txtBox1.Text = Outputstr;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveAudioFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] soundFileCollectorArr = soundFileCollectorStr.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                LocalFunction.Concatenate(saveAudioFileDialog.FileName, soundFileCollectorArr);
            }
        }

    }
}
