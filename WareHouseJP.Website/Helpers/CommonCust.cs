using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WareHouseJP.Website.Helpers
{
    public static class CommonCust
    {
        public static bool CheckNumber(string input)
        {
            if (input.IsNumeric())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNumeric(this string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }
    }
}