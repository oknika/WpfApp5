using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5.Generator
{
    public class IDPurchaseGen
    {
        public static string GenerateNextPurchaseId(string prefix, string lastId)
        {
            int number = 1;

            if (!string.IsNullOrEmpty(lastId) && lastId.Length == 5)
            {
                string numericPart = lastId.Substring(2);
                if (int.TryParse(numericPart, out int parsed))
                    number = parsed + 1;
            }

            return $"{prefix}{number:D3}";
        }
    }

}
