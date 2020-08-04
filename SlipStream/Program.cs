using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace SlipStream
{
    class Program
    {
        static void Main()
        {
            string folder = @"c:\temp\slips";
            List<string> results = new List<string>();
            List<SlipData> allSlips = new List<SlipData>();

            foreach (var filename in Directory.GetFiles(folder, "*.pdf"))
            {
                try
                {
                    List<string> content = PDFWrapper.ReadFile(filename);

                    if (content.Count > 40)
                    {
                        SlipData slipData = new SlipData(content);

                        if (slipData.IssuesPresent())
                        {
                            Console.WriteLine($"{slipData.PayDate} : {slipData.Issues[0]}");
                        }
                        allSlips.Add(slipData);
                        results.Add(JsonConvert.SerializeObject(slipData));
                    }
                }
                catch (Exception)
                {
                    // ---- ignore throw;
                }
            }

            foreach (var xx in allSlips)
            {
                if (xx.Year() == 2018)
                    Console.WriteLine($"{xx.PayDate} {xx.TaxPeriodNumber} {xx.NetPay}");
            }


            // that old pauser!
            _ = Console.ReadLine();
        }
    }
}
