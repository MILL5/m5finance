using System;
using System.Collections.Generic;
using System.Text;

namespace M5Finance.Models
{
    public class EdgarFilingInfo
    {
        public string CIK { get; set; }
        public string CompanyName { get; set; }
        public string AlternativeCompanyName { get; set; }
        public string SICName { get; set; }
        public int SICCode { get; set; }
        public int IrsNumber { get; set; }
        public string StateOfIncorporation { get; set; }
        public IEnumerable<string> Cusip { get; set; }
    }
}
