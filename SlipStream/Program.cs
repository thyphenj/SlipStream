using System;
using System.Collections.Generic;

namespace SlipStream
{
    class Program
    {
        static void Main()
        {
            string filename = "c:/users/davieste/downloads/HTML000002.pdf";

            var slipData = new SlipData(PDFWrapper.ReadFile(filename));





            // that old pauser!
            Console.ReadLine();
        }
    }
}
