using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using CsQuery;

namespace Ohayoo.Lib
{
    public class TranslateUtils
    {   
        /// <summary>
        /// Translate Text using Google Translate API’s
        /// Google URL – http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="languagePair">2 letter Language Pair, delimited by "|".
        /// E.g. "ar|en" language pair means to translate from Arabic to English</param>
        /// <returns>Translated to String</returns>
        public static string TranslateText(string input, string languagePair = "ja|vi")
        {
            languagePair = "ja|vi";
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            var tomtat_dom = CQ.CreateFromUrl(url).Select("#result_box");
            String result = tomtat_dom.ToList()[0].InnerHTML;
            return result;
        }
        public static string TranslateGoogleTextEN(string input, string languagePair = "ja|en")
        {
            languagePair = "ja|en";
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            var tomtat_dom = CQ.CreateFromUrl(url).Select("#result_box");
            String result = tomtat_dom.ToList()[0].InnerHTML.RemoveHtmlTags();
            return result;
        }
        public static string TranslateEN(string textvalue)
        {
            string appId = "4BC3CCDB5C3B2AA577A302890ABC414F4E19C136";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + appId + "&text=" + textvalue + "&from=ja&to=en";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    return translation + "";
                }
            }
            catch (WebException e)
            {
                return e.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        public static string Translate(string textvalue)
        {
            string appId = "4BC3CCDB5C3B2AA577A302890ABC414F4E19C136";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + appId + "&text=" + textvalue + "&from=ja&to=vi";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    return translation + "";
                }
            }
            catch (WebException e)
            {
                return e.Message;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
    }
}