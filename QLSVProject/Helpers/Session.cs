using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QLSVProject.Helpers
{
    internal class Session
    {
        
        public static RSA RsaKey { get; set; } 

        public static string MaNV { get; set; }

        public static string HoTen { get; set; }

        public static string Email { get; set; }

        public static int Luong { get; set; }
    }
}
