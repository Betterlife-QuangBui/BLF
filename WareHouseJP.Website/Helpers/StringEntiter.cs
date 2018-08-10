using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class StringEntiter
{
    //var test = db.StorageJPs.ToList();
    //test.Iterate(c => { c.TrackingCode = "test"; });
    //var array = db.StorageJPs.ToList().Select(n => n.TrackingCode).ToArray();
    //var test = db.StorageJPs.ToList();
    //test.Select((n, i) => n.TrackingCode= array[i]);
    public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T> callback)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException("enumerable");
        }

        IterateHelper(enumerable, (x, i) => callback(x));
    }

    public static void Iterate<T>(this IEnumerable<T> enumerable, Action<T, int> callback)
    {
        if (enumerable == null)
        {
            throw new ArgumentNullException("enumerable");
        }

        IterateHelper(enumerable, callback);
    }

    private static void IterateHelper<T>(this IEnumerable<T> enumerable, Action<T, int> callback)
    {
        int count = 0;
        foreach (var cur in enumerable)
        {
            callback(cur, count);
            count++;
        }
    }
    static public String ReadToEnd(String path)
    {
        StreamReader read = new StreamReader(path);
        String noidung = read.ReadToEnd();
        read.Close();
        return noidung;
    }

    static public String SplitMaritalRakuten(this string s)
    {
        try
        {
            int index = s.IndexOf("原材料名");
            int indexEnd = s.IndexOf("特定原材料");
            string content = s.Substring(index + "原材料名".Length, indexEnd - index);
            if (index <= 0)
            {
                index = s.IndexOf("セット内容");
                indexEnd = s.IndexOf("箱サイズ");
                content = s.Substring(index + "セット内容".Length, indexEnd - index);
            }
            if (index <= 0)
            {
                index = s.IndexOf("商品説明");
                indexEnd = s.IndexOf("内容量");
                content = s.Substring(index + "商品説明".Length, indexEnd - index);
            }
            if (index <= 0){ return s; }
            return content;
        }
        catch { }
        return s;
    }

    public static string getString(string url)
    {
        string s = "";
        WebClient v = new WebClient();
        v.Encoding = Encoding.UTF8;
        s = v.DownloadString(url);
        return s;

    }
    static public String RemoveCData(this string s)
    {
        s = Regex.Replace(s.Replace(Environment.NewLine, String.Empty).Trim(), @"(&lt;\w+&gt;)", "").RemoveHtmlTags();
        s = Regex.Replace(s.Replace(Environment.NewLine, String.Empty).Trim(), @"(&lt;/\w+&gt;)", "").RemoveHtmlTags();
        return s;
    }

    public static string CreateString(this string s, int PasswordLength)
    {
        string _allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
        Random randNum = new Random();
        char[] chars = new char[PasswordLength];
        int allowedCharCount = _allowedChars.Length;

        for (int i = 0; i < PasswordLength; i++)
        {
            chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
        }
        return new string(chars);
    }

    public static string ChangeImage(this string s)
    {
        string _allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
        string duoihinh = s.Substring(s.IndexOf('.'), s.Length - s.IndexOf('.'));
        Random randNum = new Random();
        int PasswordLength = 10;
        char[] chars = new char[PasswordLength];
        int allowedCharCount = _allowedChars.Length;

        for (int i = 0; i < PasswordLength; i++)
        {
            chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
        }
        return new string(chars) + duoihinh;
    }
    public static string ChangeImage(this string s, string fileHinh)
    {
        string _allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
        string duoihinh = fileHinh.Substring(fileHinh.IndexOf('.'), fileHinh.Length - fileHinh.IndexOf('.'));
        Random randNum = new Random();
        int PasswordLength = 10;
        char[] chars = new char[PasswordLength];
        int allowedCharCount = _allowedChars.Length;

        for (int i = 0; i < PasswordLength; i++)
        {
            chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
        }
        return new string(chars) + duoihinh;
    }
    public static string RemoveBG(this string s)
    {
        string temp = s.Replace("background-color:", "");
        temp = Regex.Replace(temp, "width:.*;", "width:100%;");
        return temp;
    }
    public static string RemoveBGTT(this string s)
    {
        string temp = s.Replace("background-color:", "");
        temp = temp.Replace("background-image:", "");

        return temp;
    }
    public static string SubstringByLength(this string s, int abc)
    {
        try
        {
            string temp = s.Substring(0, abc);
            int vitri = temp.LastIndexOf(' ');
            temp = temp.Substring(0, vitri);
            return temp;
        }
        catch { return s; }
    }
    public static string SplitString(this string s)
    {
        try
        {
            string temp = s.Substring(0, 64);
            int vitri = temp.LastIndexOf(' ');
            temp = temp.Substring(0, vitri);
            return temp;
        }
        catch { return s; }
    }
    public static string ChangeUnsigned(this string s)
    {
        if (s == null) { s = ""; }
        for (int i = 33; i < 48; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        for (int i = 58; i < 65; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        for (int i = 91; i < 97; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        for (int i = 123; i < 127; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        s = s.Replace(" ", "-").Replace("--", "-");
        s = s.ToLower();
        Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
        string strFormD = s.Normalize(System.Text.NormalizationForm.FormD);
        return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace("--", "-");

    }
    public static string ChangeUnsigned2(this string s)
    {
        for (int i = 33; i < 48; i++)
        {
            if (i != 44)
            {
                s = s.Replace(((char)i).ToString(), "");
            }
        }
        for (int i = 58; i < 65; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        for (int i = 91; i < 97; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        for (int i = 123; i < 127; i++)
        {
            s = s.Replace(((char)i).ToString(), "");
        }
        s = s.Replace(" ", "-").Replace("--", "-");
        s = s.ToLower();
        Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
        string strFormD = s.Normalize(System.Text.NormalizationForm.FormD);
        return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace("--", "-");

    }
}