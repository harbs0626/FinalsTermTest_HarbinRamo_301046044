using System;
using System.Collections.Generic;

namespace Finals_WPF.Models
{
    public partial class Covid19deaths
    {
        public string CountryRegion { get; set; }
        public string ProvinceState { get; set; }
        public DateTime RecordDate { get; set; }
        public int? DeathNumber { get; set; }
    }
}
