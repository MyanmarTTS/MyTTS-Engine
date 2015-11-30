using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Speech_Typing
{
    class ZawGyiToUnicode
    {
        public static string normalize(string nol)
        {
            // nol = nol.Replace("\r\n", "");
            //   nol=nol.Replace("\r\n", "");
            nol = nol.Replace("။", "။\r\n");
            //   nol=nol.Replace("\r\n\r\n", "\r\n");
            nol = nol.Replace("\r\n\r\n ", "");
            nol = nol.Replace("ဥ်", "ဉ်");
            nol = nol.Replace("ဥ့်", "ဉ့်");
            nol = nol.Replace("ွွ", "ွ");
            nol = nol.Replace("ူူ", "ူ");
            nol = nol.Replace("ိိ", "ိ");
            nol = nol.Replace("~", "");
            nol = nol.Replace("\t", "");
            nol = nol.Replace("\r\n ", "\r\n");
            nol = Regex.Replace(nol, " ်", "်");
            nol = Regex.Replace(nol, "(?<space>[ ]+)(?<vm>[ါ-ှ])", "${vm}");
            nol = Regex.Replace(nol, " (?<consonent>[က-အ])(?<killer>[္်])", "${consonent}${killer}");
            nol = Regex.Replace(nol, "(?<pun>[၊။‘’“”!-+:-@])", " ${pun} ");
            //EngText Remove 
            //nol=Regex.Replace(nol, "([A-Za-z!-@၀-။›©»“”_|]+)", "", RegexOptions.IgnoreCase);
            nol = Regex.Replace(nol, "(?<consonant1>[က-အ])(?<consonant2>[ျြေ]?)င်္", "င်္${consonant1}${consonant2}");


            // Typing Error-Nov052015 (1088)
            nol = Regex.Replace(nol, "(?<vowel>[ိ-ု]*)(?<medial>[ျြ])", "${medial}${vowel}");

            // vowelmedia + one or more
            //nol = Regex.Replace(nol, "(?<zero>[၀])(?<vowelmedia>[\u102B-\u103E]+)", "ဝ${vowelmedia}");
            nol = Regex.Replace(nol, "(?<zero>[ဝ])(?<num>[၁-၉])", "၀${num}");
            nol = Regex.Replace(nol, "(?<num>[၁-၉])(?<zero>[ဝ])", "${num}၀");


            return nol;

        }
        public static string Zawgyi2Uni(string input)
        {
            // copy inputted string to unistr
            String unistr = "";
            unistr = input.Substring(0);

            #region call

            System.Data.DataSet ds = new System.Data.DataSet();
            string xmlPath = Speech_Typing.Properties.Resources.Zawgyi;
            using (StringReader readerxmlpath1 = new StringReader(xmlPath))
            { ds.ReadXml(readerxmlpath1); }
            System.Data.DataRowCollection drc = ds.Tables["fontTable"].Rows;

            foreach (System.Data.DataRow dr in drc)
            {
                unistr = unistr.Replace(dr[0].ToString(), dr[1].ToString());
            }

            #endregion call



            unistr = Regex.Replace(unistr, "(?<E>\u1031)?(?<R>\u103C)?(?<con>[က-အ])\u1064", "\u1064${E}${R}${con}"); //reordering kinzi
            unistr = Regex.Replace(unistr, "(?<E>\u1031)?(?<R>\u103C)?(?<con>[က-အ])\u108B", "\u1064${E}${R}${con}\u102D"); //reordering kinzi lgt
            unistr = Regex.Replace(unistr, "(?<E>\u1031)?(?<R>\u103C)?(?<con>[က-အ])\u108C", "\u1064${E}${R}${con}\u102E"); //reordering kinzi lgtsk
            unistr = Regex.Replace(unistr, "(?<E>\u1031)?(?<R>\u103C)?(?<con>[က-အ])\u108D", "\u1064${E}${R}${con}\u1036"); //reordering kinzi ttt   

            # region Reordering

            unistr = Regex.Replace(unistr, "(?<R>\u103C)(?<con>[က-အ])(?<scon>\u1039[က-အ])?", "${con}${scon}${R}"); //reordering ra       
            //unistr = Regex.Replace(unistr, "(?<=(?<Mm>[\u1000-\u101C\u101E-\u102A\u102C\u102E-\u103F\u104C-\u109F\u0020]))(?<z>\u1040)|(?<z>\u1040)(?=(?<Mm>[\u1000-\u101C\u101E-\u102A\u102C\u102E-\u103F\u104C-\u109F\u0020]))", "\u101D");//zero and wa
            //unistr = Regex.Replace(unistr, "(?<=(?<Mm>[\u1000-\u101C\u101E-\u102A\u102C\u102E-\u103F\u104C-\u109F\u0020]))(?<z>\u1040)|(?<z>\u1047)(?=(?<Mm>[\u1000-\u101C\u101E-\u102A\u102C\u102E-\u103F\u104C-\u109F\u0020]))", "\u101B");//seven and ra
            unistr = Regex.Replace(unistr, "(?<E>\u1031)?(?<con>[က-အ])(?<scon>\u1039[က-အ])?(?<upper>[\u102D\u102E\u1032])?(?<DVs>[\u1036\u1037\u1038]{0,2})(?<M>[\u103B-\u103E]*)(?<lower>[\u102F\u1030])?(?<upper>[\u102D\u102E\u1032])?", "${con}${scon}${M}${E}${upper}${lower}${DVs}"); //reordering storage order
            #endregion

            unistr =Correction1(unistr);
            unistr = normalize(unistr);
            return unistr;


        }

        public static string Correction1(string input)
        {

            #region declaration
            string[] C = { "က", "ခ", "ဂ", "ဃ", "င", "စ", "ဆ", "ဇ", "ဈ", "ဉ", "ည", "ဋ", "ဌ", "ဍ", "ဎ", "ဏ", "တ", "ထ", "ဒ", "ဓ", "န", "ပ", "ဖ", "ဗ", "ဘ", "မ", "ယ", "ရ", "လ", "ဝ", "သ", "ဟ", "ဠ", "အ" };
            string[] M = { "\u103B", "\u103C", "\u103D", "\u103E" };
            string[] V = { "\u102B", "\u102C", "\u102D", "\u102E", "\u102F", "\u1030", "\u1031", "\u1032", "\u1036" };
            string[] IV = { "ဣ", "ဤ", "ဥ", "ဦ", "ဧ", "ဩ", "ဪ", "၎" };
            string[] T = { "\u1037", "\u1038", "\u103A", "\u1039" };
            string[] D = { "၀", "၁", "၂", "၃", "၄", "၅", "၆", "၇", "၈", "၉" };
            string unistr = null;
            #endregion

            unistr = input;

            //string pattern = "\u102D+|\u102E+|\u103D+|\u103E+|\u1032+|\u1037+|\u1036+|\u103A+";
            //unistr = Regex.Replace(unistr, pattern, new MatchEvaluator(childdeldul));

            unistr = Regex.Replace(unistr, C[5] + M[0], C[8]);
            unistr = Regex.Replace(unistr, C[30] + M[1], IV[5]);
            unistr = Regex.Replace(unistr, C[30] + M[1] + V[6] + V[1] + T[2], IV[6]);
            unistr = Regex.Replace(unistr, IV[5] + V[6] + V[1] + T[2], IV[6]);
            unistr = Regex.Replace(unistr, IV[2] + V[3], IV[3]);
            unistr = Regex.Replace(unistr, IV[2] + T[3], C[9] + T[3]);
            unistr = Regex.Replace(unistr, IV[2] + T[2], C[9] + T[2]);
            unistr = Regex.Replace(unistr, IV[2] + V[1], C[9] + V[1]);
            unistr = Regex.Replace(unistr, D[4] + C[4] + T[2] + T[1], IV[7] + C[4] + T[2] + T[1]);
            unistr = Regex.Replace(unistr, T[0] + T[2], T[2] + T[0]);
            unistr = Regex.Replace(unistr, T[1] + T[2], T[2] + T[1]);

            #region Medial reordering

            unistr = Regex.Replace(unistr, M[3] + M[0], M[0] + M[3]);
            unistr = Regex.Replace(unistr, M[3] + M[1], M[1] + M[3]);
            unistr = Regex.Replace(unistr, M[3] + M[2], M[2] + M[3]);
            unistr = Regex.Replace(unistr, M[2] + M[0], M[0] + M[2]);
            unistr = Regex.Replace(unistr, M[2] + M[1], M[1] + M[2]);
            unistr = Regex.Replace(unistr, M[3] + M[2] + M[1] + "|" + M[3] + M[1] + M[2] + "|" + M[2] + M[3] + M[1] + "|" + M[2] + M[1] + M[3] + "|" + M[1] + M[3] + M[2], M[1] + M[2] + M[3]);
            unistr = Regex.Replace(unistr, M[3] + M[2] + M[0] + "|" + M[3] + M[0] + M[2] + "|" + M[2] + M[3] + M[0] + "|" + M[2] + M[0] + M[3] + "|" + M[0] + M[3] + M[2], M[0] + M[2] + M[3]);

            #endregion

            # region Vowel reordering

            unistr = Regex.Replace(unistr, V[8] + V[4], V[4] + V[8]);
            unistr = Regex.Replace(unistr, V[4] + V[2], V[2] + V[4]);
            unistr = Regex.Replace(unistr, V[8] + V[2], V[2] + V[8]);
            unistr = Regex.Replace(unistr, T[0] + V[4], V[4] + T[0]);
            unistr = Regex.Replace(unistr, T[0] + V[7], V[7] + T[0]);
            unistr = Regex.Replace(unistr, T[0] + V[8], V[8] + T[0]);

            #endregion

            # region Contracted words
            unistr = Regex.Replace(unistr, C[26] + V[6] + V[1] + C[0] + M[0] + T[2] + V[1], C[26] + V[6] + V[1] + C[0] + T[2] + M[0] + V[1]);
            unistr = Regex.Replace(unistr, C[20] + V[4] + T[2], C[20] + T[2] + V[4]);
            #endregion

            # region two times typing
            unistr = Regex.Replace(unistr, "\u103A\u103A", "\u103A");
            #endregion

            #region Recognition of digit as consonant
            unistr = Regex.Replace(unistr, D[0] + T[2], C[29] + T[2]);
            unistr = Regex.Replace(unistr, D[7] + T[2], C[27] + T[2]);
            unistr = Regex.Replace(unistr, D[8] + T[2], C[2] + T[2]);

            unistr = Regex.Replace(unistr, D[0] + T[3], C[29] + T[3]);
            unistr = Regex.Replace(unistr, D[7] + T[3], C[27] + T[3]);
            unistr = Regex.Replace(unistr, D[8] + T[3], C[2] + T[3]);

            unistr = Regex.Replace(unistr, D[0] + "(?<vowel>[" + V[0] + "-" + V[8] + "])", C[29] + "${vowel}");
            unistr = Regex.Replace(unistr, D[7] + "(?<vowel>[" + V[0] + "-" + V[8] + "])", C[27] + "${vowel}");
            unistr = Regex.Replace(unistr, D[8] + "(?<vowel>[" + V[0] + "-" + V[8] + "])", C[2] + "${vowel}");

            unistr = Regex.Replace(unistr, D[0] + "(?<medial>[" + M[0] + "-" + M[3] + "])", C[29] + "${medial}");
            unistr = Regex.Replace(unistr, D[7] + "(?<medial>[" + M[0] + "-" + M[3] + "])", C[27] + "${medial}");
            unistr = Regex.Replace(unistr, D[8] + "(?<medial>[" + M[0] + "-" + M[3] + "])", C[2] + "${medial}");

            unistr = Regex.Replace(unistr, D[0] + "(?<finale>([\u1000-\u1031][\u1039-\u103A]))", C[29] + "${finale}");
            unistr = Regex.Replace(unistr, D[7] + "(?<finale>([\u1000-\u1031][\u1039-\u103A]))", C[27] + "${finale}");
            unistr = Regex.Replace(unistr, D[8] + "(?<finale>([\u1000-\u1031][\u1039-\u103A]))", C[2] + "${finale}");
            #endregion

            #region reordering
            unistr = Regex.Replace(unistr, "(?<upper>[\u102D\u102E\u1036\u1032])(?<M>[\u103B-\u103E]+)", "${M}${upper}"); //reordering storage order
            unistr = Regex.Replace(unistr, "(?<DVs>[\u1036\u1037\u1038]+)(?<lower>[\u102F\u1030])", "${lower}${DVs}"); //reordering storage order
            unistr = unistr.Replace("့်", "့်");
            #endregion

            return unistr;
        }
    }
}
