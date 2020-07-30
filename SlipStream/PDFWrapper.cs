using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SlipStream
{
    class PDFWrapper
    {
        public double X;
        public double Y;
        public String Text;

        public PDFWrapper()
        {
        }

        public void AddPos(COperator element)
        {
            X = Double.Parse(element.Operands[0].ToString());
            Y = Double.Parse(element.Operands[1].ToString());
        }

        public void AddText(COperator element)
        {
            Text = element.Operands[0].ToString();
            Text = Text.Substring( 1, Text.Length - 2).Replace("\\","");
        }

        public override string ToString()
        {
            return Text;
        }

        public static List<string> ReadFile ( string filename)
        {
            List<PDFWrapper> fragments = new List<PDFWrapper>();

            //get text fragments out of file into <fragments>
            using (var pdfFile = PdfReader.Open(filename, PdfDocumentOpenMode.ReadOnly))
            {
                foreach (var page in pdfFile.Pages)
                {
                    PDFWrapper fragment = new PDFWrapper();

                    var zz = ContentReader.ReadContent(page);

                    foreach (var element in zz)
                    {
                        if (element is COperator op)
                        {
                            if (op.Name == "Td")
                            {
                                fragment = new PDFWrapper();
                                fragment.AddPos(op);
                            }
                            else if (op.Name == "Tj")
                            {
                                fragment.AddText(op);
                                fragments.Add(fragment);
                            }
                        }
                    }
                }
            }


            //write the fragments into the retval
            var retval = new List<string>();

            double prevY = 0.0;
            string str = "";
            foreach (var f in from frag in fragments
                              orderby frag.Y descending, frag.X ascending
                              select frag)
            {
                if (prevY != f.Y && str != "")
                {
                    retval.Add(str);
                    str = "";
                }
                str += f.ToString();
                prevY = f.Y;
            }

            if (str != "")
                retval.Add(str);

            return retval;
        }
    }
}
