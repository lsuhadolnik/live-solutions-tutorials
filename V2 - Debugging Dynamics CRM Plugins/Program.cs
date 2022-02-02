using LiveSolutions;
using LS.BrokenPlugin;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.LocalTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var plugin = new CheckEmailAddressForDotCom();

            LocalServiceProvider provider = new LocalServiceProvider(
                "https://org3216c765.crm4.dynamics.com/",
                new Account()
                {
                    Id = Guid.Parse("e35136c1-c652-ec11-8c62-000d3ade8a10"),
                    EMailAddress1 = "blabla@dynamics.de"
                }
            );

            plugin.Execute(provider);


            Console.ReadLine();
        }
    }
}
