using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;



namespace Speech_Typing
{
    public class LocalFunction
    {
         public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "သုည";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"တစ် ", "နှစ် ", "သုံး ", "လေး ",
            "ငါး " ,"ခြောက် ", "ခုနစ် ", "ရှစ် ", "ကိုး "};
            string[] words1 = {"တဆယ် ", "ဆယ့်တစ် ", "ဆယ့်နှစ် ", "ဆယ့်သုံး ", "ဆယ့်လေး ",
            "ဆယ့်ငါး ","ဆယ့်ခြောက် ","ဆယ့်ခွန် ","ဆယ့်ရှစ် ", "ဆယ့်ကိုး "};
            string[] words2 = {"နှစ်ဆယ် ", "သုံးဆယ် ", "လေးဆယ် ", "ငါးဆယ် ", "ခြောက်ဆယ် ",
            "ခုနစ်ဆယ် ","ရှစ်ဆယ် ", "ကိုးဆယ် "};
            string[] words3 = { "ထောင် ", "သိန်း ", "ရာ " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "ရာ ");
                if (u > 0 || t > 0)
                {
                    //if (h > 0 || i == 0) sb.Append("့ ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                    {
                        if (i == 1)
                        {
                            sb.Append(words0[t] + "သောင်း" + words0[u]);
                        }
                        else
                            sb.Append(words1[u]);
                        
                    }
                    else
                    {
                        if (i == 1)
                        {
                            sb.Append(words0[t] + "သောင်း" + words0[u]);
                        }
                        else
                            sb.Append(words2[t - 2] + words0[u]);
                    }
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public static string ChangeEnglishNo(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "0";
            return str.Replace('၁','1')
                        .Replace('၂','2')
                        .Replace('၃','3')
                        .Replace('၄','4')
                        .Replace('၅','5')
                        .Replace('၆','6')
                        .Replace('၇','7')
                        .Replace('၈','8')
                        .Replace('၉','9')
                        .Replace('၀','0');
        }
        public static string ChangeBurmeseNo(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "0";     
            return str.Replace('1', '၁')
                        .Replace('2', '၂')
                         .Replace('3', '၃')
                        .Replace('4', '၄')
                        .Replace('5', '၅')
                        .Replace('6', '၆')
                        .Replace('7', '၇')
                        .Replace('8', '၈')
                        .Replace('9', '၉')
                         .Replace('0', '၀');
              
        }

        public static object Parse(string from, Type to, object defaultValue)
        {
            try
            {
                if (to == typeof(int))
                {
                    int result = 0;
                    int.TryParse(from, out result);
                    return result;
                }
                else if (to == typeof(float))
                {
                    float result = 0;
                    float.TryParse(from, out result);
                    return result;
                }
                else if (to == typeof(double))
                {
                    double result = 0;
                    double.TryParse(from, out result);
                    return result;
                }
                // else if (to == typeof(bool))
                // else if....
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
            return defaultValue;
        }


        public static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    using (WaveFileReader reader = new WaveFileReader(sourceFile))
                    {                        
                        if (waveFileWriter == null)
                        {
                            // first time in create new Writer
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                //throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");
                            }
                        }

                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteData(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose();
                }
            }

        }
    }
}
