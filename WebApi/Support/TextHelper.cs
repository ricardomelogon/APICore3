using System;
using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Support
{
    public static class TextHelper
    {
        public static bool IsHexColor(string color)
        {
            string regex = "^#[0-9A-F]{6}$";
            Match match = Regex.Match(color, regex, RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        ///
        /// Utility method used to convert special accented characters to their simple counterpart
        /// <param name="text">The text to be converted</param>
        ///
        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        ///
        /// Utility method used to remove special characters, leaving only letters, numbers, dots and underscore
        /// <param name="str">The text to be processed</param>
        ///
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string GetIcon(string filetype)
        {
            return filetype switch
            {
                ".rar" => "mdi mdi-folder-zip-outline",
                ".zip" => "mdi mdi-folder-zip-outline",
                ".csv" => "mdi mdi-file-delimited-outline",
                ".jpeg" => "mdi mdi-file-image-outline",
                ".jpg" => "mdi mdi-file-image-outline",
                ".png" => "mdi mdi-file-image-outline",
                ".psd" => "mdi mdi-file-image-outline",
                ".svg" => "mdi mdi-file-image-outline",
                ".ico" => "mdi mdi-file-image-outline",
                ".odp" => "mdi mdi-file-word-outline",
                ".pps" => "mdi mdi-file-powerpoint-outline",
                ".ppt" => "mdi mdi-file-powerpoint-outline",
                ".pptx" => "mdi mdi-file-powerpoint-outline",
                ".ods" => "mdi mdi-file-excel-outline",
                ".xls" => "mdi mdi-file-excel-outline",
                ".xlsx" => "mdi mdi-file-excel-outline",
                ".txt" => "mdi mdi-file-document-outline",
                ".pdf" => "mdi mdi-file-pdf-outline",
                ".doc" => "mdi mdi-file-word-outline",
                ".docx" => "mdi mdi-file-word-outline",
                ".odt" => "mdi mdi-file-word-outline",
                _ => "mdi mdi-file-outline",
            };
        }

        public static string GeneratePassword()
        {
            int Size = ClosestNumber(12, 4) / 4;
            string a = Random(Size, "ABCDEFGHIJKMNPQRSTUVWXYZ");
            string b = Random(Size, "23456789");
            string c = Random(Size, "abcdefghjkmnpqrstuvwxyz");
            string d = Random(Size, "!@#$%^&*_+-?");
            string e = a + b + c + d;
            return Shuffle(e);
        }

        private static string Random(int length, string valid)
        {
            //const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        /// <summary>
        /// Returns a truncated version of the string with added ellipsis.
        /// </summary>
        /// <param name="text">original text</param>
        /// <param name="size">maximum text size before truncating</param>
        /// <param name="Position">Posotion of the ellipsis, null is at the middle, false is at the beginning, true is at the end</param>
        /// <returns></returns>
        public static string Ellipsis(string text, int size = 10, bool? Position = null)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return "";
                if (text.Length > size)
                {
                    switch (Position)
                    {
                        case null:
                            return $"{text.Substring(0, size / 2)}...{text.Substring(text.Length - (size / 2))}";

                        case false:
                            return $"{text.Substring(0, size)}...";

                        case true:
                            return $"...{text.Substring(text.Length - size)}";
                    }
                }
                else return text;
            }
            catch
            {
                return "";
            }
        }

        public static bool IsVowel(char c)
        {
            try
            {
                return "aeiou".IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        private static string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        ///function to find the number closest to n
        /// and divisible by m
        private static int ClosestNumber(int n, int m)
        {
            // find the quotient
            int q = n / m;

            // 1st possible closest number
            int n1 = m * q;

            // 2nd possible closest number
            int n2 = (n * m) > 0 ? (m * (q + 1)) : (m * (q - 1));

            // if true, then n1 is the required closest number
            if (Math.Abs(n - n1) < Math.Abs(n - n2))
                return n1;

            // else n2 is the required closest number
            return n2;
        }
    }
}