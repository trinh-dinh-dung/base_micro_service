using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public static class CommonFunction
    {
		public static string ToUnSign(string input, bool isCLean = true)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return "";
			}
			input = input.Trim();

			input = input.Replace("'", "").Replace("\"", "");
			Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
			string str = input.Normalize(NormalizationForm.FormD);
			string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
			while (str2.IndexOf("?") >= 0)
			{
				str2 = str2.Remove(str2.IndexOf("?"), 1);
			}

			return str2.ToLower().Trim();
		}

        public static string ConvertTitle(string Name)
        {
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            var result = string.Empty;
            var currentchar = string.Empty;
            Name = Name.Replace(" ", "-")
                    .Replace(")", "")
                    .Replace("(", "")
                    .Replace("*", "")
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("{", "")
                    .Replace("}", "")
                    .Replace(">", "")
                    .Replace("<", "")
                    .Replace("=", "")
                    .Replace(":", "")
                    .Replace(",", "")
                    .Replace("'", "")
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace("&", "")
                    .Replace("?", "")
                    .Replace(";", "")
                    .Replace("'", "")
                    .Replace(".", "")
                    .ToLower();
            int len = Name.Length;
            if (Name.Length > 0)
            {
                int i;
                for (i = 0; i < len; i++)
                {
                    currentchar = Name.Substring(i, 1);
                    result = result + ChangeChar(currentchar);
                }
            }
            return result;
        }

        private static string[] a = new string[] { "à", "á", "ạ", "ã", "â", "ầ", "ẫ", "ậ", "ẩ", "ă", "ẳ", "ẵ", "ặ", "ắ", "ằ", "a", "ả" };
        private static string[] d = new string[] { "đ", "d" };
        private static string[] e = new string[] { "e", "é", "è", "ẹ", "ẽ", "ẻ", "ê", "ể", "ễ", "ệ", "ế", "ề" };
        private static string[] i = new string[] { "ì", "í", "ị", "ỉ", "ĩ", "i" };
        private static string[] y = new string[] { "y", "ý", "ỳ", "ỵ", "ỹ", "ỷ" };
        private static string[] o = new string[] { "o", "ò", "ó", "ọ", "õ", "ỏ", "ô", "ố", "ồ", "ỗ", "ổ", "ộ", "ơ", "ớ", "ờ", "ợ", "ỡ", "ở" };
        private static string[] u = new string[] { "ù", "ú", "ụ", "ủ", "ũ", "ư", "ứ", "ừ", "ự", "ử", "ữ", "u" };
        private static string ChangeChar(string charinput)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Equals(charinput))
                    return "a";
            }

            for (int i = 0; i < d.Length; i++)
            {
                if (d[i].Equals(charinput))
                    return "d";
            }

            for (int i = 0; i < e.Length; i++)
            {
                if (e[i].Equals(charinput))
                    return "e";
            }

            for (int c = 0; c < i.Length; c++)
            {
                if (i[c].Equals(charinput))
                    return "i";
            }

            for (int i = 0; i < y.Length; i++)
            {
                if (y[i].Equals(charinput))
                    return "y";
            }

            for (int i = 0; i < o.Length; i++)
            {
                if (o[i].Equals(charinput))
                    return "o";
            }

            for (int i = 0; i < u.Length; i++)
            {
                if (u[i].Equals(charinput))
                    return "u";
            }

            return charinput;
        }

    }
}
