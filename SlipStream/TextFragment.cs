using PdfSharp.Pdf.Content.Objects;

using System;
using System.Linq.Expressions;

namespace SlipStream
{
    class TextFragment
    {
        public double X;
        public double Y;
        public String Text;

        public TextFragment()
        {

        }
        public void AddPos(COperator element)
        {
            bool firstOne = true;
            foreach (var op in element.Operands)
            {
                if (firstOne)
                {
                    firstOne = false;
                    X = Double.Parse(op.ToString());
                }
                else
                    Y = Double.Parse(op.ToString());
            }
        }
        public void AddText(COperator element)
        {
            Text = element.Operands[0].ToString();
            Text = Text.Substring( 1, Text.Length - 2).Replace("\\","");
        }

        public override string ToString()
        {
   //         return $"{X,6} {Y,6} {Text.Length,4} {Text}";
            return Text;
        }
    }
}
