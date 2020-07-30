using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlipStream
{
    class SlipData
    {
        // ---- This is the PK ????
        public DateTime PayDate;

        // ---- Who am I?
        public string Name;
        public int PayrollNumber;

        // ---- What's my "situation"?
        public string TaxCode;
        public string TaxPeriodNumber;
        public string Message;

        // ---- The "detail" lines
        public Dictionary<string, float> Allowances = new Dictionary<string, float>();
        public Dictionary<string, float> Deductions = new Dictionary<string, float>();
        public Dictionary<string, float> YearToDate = new Dictionary<string, float>();

        // ---- The "bottom line"
        public float TotalPayments;
        public float TotalDeductions;
        public float NetPay;

        public SlipData(List<string> content)
        {
            // ---- Get the "Address"
            int lineNo = 0;
            var address = new List<string>();
            while (content[lineNo][0] != '-')
                address.Add(content[lineNo++].Trim());

            // ---- For each subsequent line
            while (lineNo < content.Count)
            {
                // ---- Depending on the number of fields we can "guess" what the line contains
                string[] line = content[lineNo].Split(new char[] { '|' });
                switch (line.Length)
                {
                    case 6:
                        switch (line[1].Trim())
                        {
                            case "Name":
                                Name = line[2].Trim();
                                break;
                            case "Payroll Number":
                                PayrollNumber = int.Parse(line[2].Trim());
                                break;
                            default:
                                break;
                        }
                        switch (line[3].Trim())
                        {
                            case "Pay Date":
                                var dat = line[4].Trim().Split(new char[] { '.' });
                                PayDate = new DateTime(int.Parse(dat[2]), int.Parse(dat[1]), int.Parse(dat[0]));
                                break;
                            case "Tax Period Number":
                                TaxPeriodNumber = line[4].Trim();
                                break;
                            case "Tax Code":
                                TaxCode = line[4].Trim();
                                break;
                            default:
                                break;
                        }
                        //Console.WriteLine($"{line.Length,2} {content[lineNo]} ****************DONE");
                        break;
                    case 5:
                        if (line[1].Contains("Payments") && line[2].Contains("Deductions") && line[3].Contains("Pay"))
                        {
                            TotalPayments = float.Parse(line[1].Remove(0, 12).Trim().Replace(",", ""));
                            TotalDeductions = float.Parse(line[2].Remove(0, 12).Trim().Replace(",", ""));
                            NetPay = float.Parse(line[3].Remove(0, 12).Trim().Replace(",", ""));
                        }
                        //Console.WriteLine($"{line.Length,2} {content[lineNo]} ****************DONE");
                        break;
                    case 3:
                        if (line[1].Contains("Message"))
                        {
                            Message = line[1].Trim().Substring(8);
                            while (content[++lineNo][0] == '|')
                                Message += $"\n{content[lineNo].Substring(1, content[lineNo].Length - 2).Trim()}";
                        }
                        //Console.WriteLine($"{line.Length,2} {content[lineNo]} ****************DONE");
                        break;
                    case 10:
                        float xx;
                        if (float.TryParse(line[4].Trim().Replace(",", ""), out xx))
                        {
                            Allowances.Add(line[1].Trim(), xx);
                        }
                        if (float.TryParse(line[6].Trim().Replace(",", ""), out xx))
                        {
                            Deductions.Add(line[5].Trim(), xx);
                        }
                        if (float.TryParse(line[8].Trim().Replace(",", ""), out xx))
                        {
                            YearToDate.Add(line[7].Trim(), xx);
                        }
                        break;
                    //case 1:
                    //case 7: // This is the "Holidays" section - absolute shite from beginning to end!
                    //case 4:
                    //case 8:
                    //    break;
                    default:
//                        Console.WriteLine($"{line.Length,2} {content[lineNo]}");
                        break;

                }
                lineNo++;
            }

        }
        public override string ToString()
        {
            return $"{TaxPeriodNumber} {TotalPayments}";
        }
    }
}
