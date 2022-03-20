using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Repository.Models.Postal
{
    public partial class KenAll
    {
        public int Id { get; set; }

        [Description("郵便番号")]
        public string? PostalCode { get; set; }

        [Description("都道府県名カナ")]
        public string? PrefectureKana { get; set; }

        [Description("市区町村名カナ")]
        public string? MunicipalityKana { get; set; }

        [Description("町域名カナ")]
        public string? TownAreaKana { get; set; }

        [Description("都道府県名")]
        public string? Prefecture { get; set; }

        [Description("市区町村名")]
        public string? Municipality { get; set; }

        [Description("町域名")]
        public string? TownArea { get; set; }

        [Description("一町域が二以上の郵便番号で表される場合の表示")]
        public int? TownAreaContainAnyPostalCode { get; set; }

        [Description("一つの郵便番号で二以上の町域を表す場合の表示")]
        public int? PostalCodeContainAnyTownArea { get; set; }
    }
}
