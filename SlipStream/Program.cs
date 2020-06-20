using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlipStream
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TextFragment> fragments = new List<TextFragment>();

            using (var pdfFile = PdfReader.Open("c:/users/t-j/downloads/HTML000002.pdf", PdfDocumentOpenMode.ReadOnly))
            {
                foreach (var page in pdfFile.Pages)
                {
                    TextFragment frag = new TextFragment();

                    var zz = ContentReader.ReadContent(page);

                    foreach (var element in zz)
                    {

                        if (element is COperator)
                        {
                            COperator op = (COperator)element;
                            if (op.Name == "Td")
                            {
                                 frag = new TextFragment();
                                frag.AddPos(op);
                            }
                            else if (op.Name == "Tj")
                            {
                                frag.AddText(op);
                                fragments.Add(frag);
                            }
                        }
                    }
                }
                var sorted =
           from frag in fragments
           orderby frag.Y descending, frag.X ascending
           select frag;

                double prevY = 0.0;
                foreach (var frag in sorted)
                {
                    if ( prevY != frag.Y)
                        Console.WriteLine();
                    Console.Write(frag.ToString());
                    prevY = frag.Y;
                }
                Console.WriteLine();
                Console.ReadLine();
            }
        }
        private static void Write(CObject obj)
        {
            if (obj is CNumber)
                Write((CNumber)obj);

            else if (obj is COperator)
                Write((COperator)obj);

            else if (obj is CString)
                Write((CString)obj);
        }

        private static void Write(COperator obj)
        {
            Console.Write("op:{0}(", obj.Name);
            foreach (var op in obj.Operands)
            {
                Write(op);
                Console.Write(", ");
            }
            Console.WriteLine(")");
        }

        private static void Write(CNumber obj)
        {
            Console.Write("num:{0}", obj.ToString());
        }

        private static void Write(CString obj)
        {
            Console.Write($"str:{obj.Value}");
        }

        //private static void Write(CComment obj)
        //{
        //    Console.Write("/* {0} */", obj.Text);
        //}

        //private static void Write(CInteger obj)
        //{
        //    Console.Write("int:{0}", obj.Value);
        //}

        //private static void Write(CName obj)
        //{
        //    Console.Write("name:{0}", obj.Name);
        //}
        private static void Write(CReal obj)
        {
            Console.Write("real:{0}", obj.Value);
        }

        private static void Write(CSequence obj)
        {
            foreach (var element in obj)
            {
                Write(element);
                Console.WriteLine();
            }
        }

    }
}
