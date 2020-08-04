using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlipStream
{
    class SlipData
    {
        // ---- Is this the PK ????
        public string PayDate { get; set; }

        // ---- Who am I?
        public string Name { get; set; }
        public int PayrollNumber { get; set; }
        public List<string> Address { get; set; }

        // ---- What's my "situation"?
        public string TaxCode { get; set; }
        public string TaxPeriodNumber { get; set; }
        public string Message { get; set; }

        // ---- The "detail" lines
        public Dictionary<string, float> Allowances { get; set; } 
        public Dictionary<string, float> Deductions { get; set; } 
        public Dictionary<string, float> YearToDate { get; set; } 

        // ---- The "bottom line"
        public float TotalPayments { get; set; }
        public float TotalDeductions { get; set; }
        public float NetPay { get; set; }

        // ---- Any problems with validation?
        public List<string> Issues { get; set; } 

        public SlipData(List<string> content)
        {
            Address = new List<string>();

            Allowances = new Dictionary<string, float>();
            Deductions  = new Dictionary<string, float>();
            YearToDate = new Dictionary<string, float>();

            Issues = new List<string>();

            // ---- Get the "Address"
            int lineNo = 0;
            while (content[lineNo][0] != '-')
                Address.Add(content[lineNo++].Trim());

            // ---- For each subsequent line
            while (lineNo < content.Count)
            {
                // ---- Depending on the number of fields we can "guess" what the line contains
                string[] line = content[lineNo].Split(new char[] { '|' });
                //Console.WriteLine($"{line.Length,2} {content[lineNo]}");
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
                                PayDate = $"{int.Parse(dat[2]),4:D4}-{int.Parse(dat[1]),2:D2}-{int.Parse(dat[0]),2:D2}";
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
                        break;
                    case 5:
                        if (line[1].Contains("Payments") && line[2].Contains("Deductions") && line[3].Contains("Pay"))
                        {
                            TotalPayments = float.Parse(line[1].Remove(0, 12).Trim().Replace(",", ""));
                            TotalDeductions = float.Parse(line[2].Remove(0, 12).Trim().Replace(",", ""));
                            NetPay = float.Parse(line[3].Remove(0, 12).Trim().Replace(",", ""));
                        }
                        break;
                    case 3:
                        if (line[1].Contains("Message"))
                        {
                            Message = line[1].Trim().Substring(8);
                            while (content[++lineNo][0] == '|')
                                Message += $"\n{content[lineNo].Substring(1, content[lineNo].Length - 2).Trim()}";
                        }
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
                        break;

                }
                lineNo++;
            }

            // ---- Does it all add up?

            // ---- Add Allowances - does it match TotalPayments?
            float sum = 0;
            foreach (var a in Allowances)
            {
                sum += a.Value;
            }
            if (NotEqual(sum, TotalPayments))
                Issues.Add($"TotalPayments = {TotalPayments} but sum of Allowances = {sum}");

            // ---- Do deductions add up?
            sum = 0;
            foreach (var a in Deductions)
            {
                sum += a.Value;
            }
            if (NotEqual(sum, TotalDeductions))
                Issues.Add($"TotalDeductions = {TotalDeductions} but sum of Deductions = {sum}");

            // ---- Can they subtract x from y to give z?
            if (NotEqual(TotalPayments - TotalDeductions, NetPay))
                Issues.Add($"TotalPayments ({TotalDeductions}) minus TotalDeductions ({TotalDeductions}) not equal to NetPay ({NetPay})");
        }

        public bool IssuesPresent()
        {
            return (Issues.Count > 0);
        }

        private static bool Equal(float x, float y)
        {
            return (Math.Abs(x - y) < 0.005);
        }

        private static bool NotEqual(float x, float y)
        {
            return (Math.Abs(x - y) > 0.009);
        }

        public override string ToString()
        {
            return $"{TaxPeriodNumber} {TotalPayments}";
        }

        public int TaxYear ()
        {
            return int.Parse(TaxPeriodNumber.Substring(3));
        }

        public int Year()
        {
            return int.Parse(PayDate.Substring(0,4));
        }
    }
}
