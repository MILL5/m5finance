using System;

namespace M5Finance
{
    public class EdgarFiling
    {
        public int CIK { get; set; }
        public string CompanyName { get; set; }
        public string FormType { get; set; }
        public string Filing { get; set; }
        public string DateFiled { get; set; }

        public string FilingUrl
        {
            get
            {
                return $@"https://www.sec.gov/Archives/{Filing}";
            }
        }
    }
}
