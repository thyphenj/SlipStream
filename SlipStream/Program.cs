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

            foreach (var filename in Directory.GetFiles(folder, "*.pdf"))
            {
                try
                {
                    List<string> content = PDFWrapper.ReadFile(filename);

                    if (content.Count > 40)
                    {
                        SlipData slipData = new SlipData(content);

                        if (slipData.IssuesPresent() )
                        {
                            Console.WriteLine($"{slipData.PayDate} : {slipData.Issues[0]}");
                        }
                        results.Add(JsonConvert.SerializeObject(slipData));
                    }
                }
                catch (Exception)
                {
                    // ---- ignore throw;
                }
            }


            // that old pauser!
            _ = Console.ReadLine();
        }
    }
}
