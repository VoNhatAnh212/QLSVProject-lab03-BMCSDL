using QLSVProject.Helpers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace QLSVNhomApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}

//RSA rsa = RsaHelper.CreateRSA("nva|abcd12|nv01");

////byte[] plaintext = Encoding.UTF8.GetBytes("vonhatanh");
////byte[] ciphertext = RsaHelper.Encrypt(rsa, plaintext);

////Console.WriteLine(Convert.ToBase64String(ciphertext));

//string ciphertext = "AvDL+VnJ3J5d4C8Z7zs0ZgzRiHcXNsHbqAjLYTFUT7x/4FxXEIwql2ian2IM/Uxhzy8gmSRfUmxHNtR5aK3u4wRjg42hXDTr1AnQBs4fGrNPcGrDQ43/mV/RTwjL0huol44qT+bliB86zHadwasirhm2232ZNXWYWoc96b7zsAOQptRawd3ePhzQWdKWeVWrRZ3pOMkgbEU358ZipXRmF5Nduqsmzn9/r8JVE8+loHT/IoCHY28qo2DHZuDmVFqK076HkD9qFdjXX1PZqHuEI3h7s5TzAbU2RYRxUL/u8RmgCxApf99PbAa4mGy5pMHWGWa5J8NRMu2Yxh2iypDOPg==";

//// ❌ Sai: UTF8 không decode được Base64
//// byte[] bytes = Encoding.UTF8.GetBytes(ciphertext);

//// ✅ Đúng: ciphertext là Base64 → dùng FromBase64String
//byte[] decryptedBytes = RsaHelper.Decrypt(rsa, Convert.FromBase64String(ciphertext));
//string plaintext = Encoding.UTF8.GetString(decryptedBytes);

//Console.WriteLine(plaintext);
