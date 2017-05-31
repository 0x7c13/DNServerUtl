
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DNServerUtl
{
    using System;
    using System.Xml.Linq;


    class Program
    {
        // callback used to validate the certificate in an SSL conversation
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        static void Main(string[] args)
        {
            string url = "https://dorado.sdo.com/dn/sndalist/sndalist.xml";  // DN服务器配置文件地址

            //Trust all certificates
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);


            // validate cert by calling a function
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            XDocument doc = null;

            try
            {
                doc = XDocument.Load(url);
            }
            catch (Exception e)
            {
                Console.WriteLine($"服务器地址数据获取失败! 原因: 加载服务器配置文件失败: {url} 也许地址变了?");
                Console.ReadLine();
                throw;
            }

            if (doc == null || doc.Document == null)
            {
                Console.WriteLine($"服务器地址数据获取失败! 原因: 服务器地址配置文件为空: {url}");
                Console.ReadLine();
                return;
            }

            foreach (XElement channelList in doc?.Document?.Descendants("ChannelList"))
            {
                foreach (XElement local in channelList.Descendants("Local"))
                {
                    Console.WriteLine($"{local.Attribute("local_name")?.Value}");

                    foreach (XElement ip in local.Descendants("login"))
                    {
                        Console.WriteLine($"{ip.Attribute("addr")?.Value}:{ip.Attribute("port")?.Value}");
                    }
                    Console.WriteLine();
                }
            }

            Console.ReadLine();
        }
    }
}
