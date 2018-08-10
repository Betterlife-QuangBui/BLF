using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Web.Helpers.Buybid
{
    public class Common
    {
        public static string MakeRequest(string requestUrl)
        {
            try
            {
                System.Net.HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        public static Dictionary<string, string> MakeMultileRequest(Dictionary<string, string> requestUrl)
        {
            try
            {
                Dictionary<string, string> lst = new Dictionary<string, string>();
                Dictionary<string, Uri> uris = new Dictionary<string, Uri>();
                foreach (var item in requestUrl)
                {
                    uris.Add(item.Key, new Uri(item.Value));
                }
                Parallel.ForEach(uris, u =>
                {
                    try
                    {
                        WebRequest webR = HttpWebRequest.Create(u.Value);
                        using (HttpWebResponse webResponse = webR.GetResponse() as HttpWebResponse)
                        {
                            using (Stream responseStream = webResponse.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                                lst.Add(u.Key, reader.ReadToEnd());
                            }
                        }
                    }
                    catch { }
                });

                return lst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
