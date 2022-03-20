using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Postal
{
    public record KenAllModel
    {
        [Name("郵便番号")]
        public string? PostalCode { get; set; }

        [Name("都道府県名カナ")]
        public string? PrefectureKana { get; set; }

        [Name("市区町村名カナ")]
        public string? MunicipalityKana { get; set; }

        [Name("町域名カナ")]
        public string? TownAreaKana { get; set; }

        [Name("都道府県名")]
        public string? Prefecture { get; set; }

        [Name("市区町村名")]
        public string? Municipality { get; set; }

        [Name("町域名")]
        public string? TownArea { get; set; }

        [Name("一町域が二以上の郵便番号で表される場合の表示")]
        public int? TownAreaContainAnyPostalCode { get; set; }

        [Name("一つの郵便番号で二以上の町域を表す場合の表示")]
        public int? PostalCodeContainAnyTownArea { get; set; }


    }
}
