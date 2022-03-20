using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Transactions;
using Domain.Models.Postal;
using Domain.Repository;
using Microsoft.VisualBasic;
using Repository.Models.Postal;
using Shared.Converters;

namespace AngularBackend.Services.Csv
{
    public class ImportAddressCsv
    {
        private readonly IImportAddressRepository _importAddressRepository;

        public ImportAddressCsv(IImportAddressRepository importAddressRepository)
        {
            _importAddressRepository = importAddressRepository;
        }

        public async Task<List<KenAllModel>> ReadCsvAsync(string path)
        {
            var tempDatas = new List<KenAllModel>();

            if (!string.IsNullOrWhiteSpace(path))
            {
                await Task.Run(() =>
                {
                    CultureInfo cultureInfo = new CultureInfo("ja-JP");
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // shift_jisを使えるようにする
                    using (var reader = new StreamReader(path, System.Text.Encoding.GetEncoding("Shift_JIS")))
                    using (var csv = new CsvHelper.CsvReader(reader, cultureInfo))
                    {
                        //読み込み を先に行う
                        csv.Read();
                        // ヘッダー　を次に行う
                        csv.ReadHeader();
                        // データを読み出し
                        var records = csv.GetRecords<KenAllModel>();

                        tempDatas = new List<KenAllModel>();
                        foreach (var record in records)
                        {
                            tempDatas.Add(new KenAllModel()
                            {
                                PostalCode = record.PostalCode,
                                PrefectureKana = record.PrefectureKana?.ToWide(),
                                MunicipalityKana = record.MunicipalityKana?.ToWide(),
                                TownAreaKana = record.TownAreaKana?.ToWide(),
                                Prefecture = record.Prefecture,
                                Municipality = record.Municipality,
                                TownArea = record.TownArea,
                                TownAreaContainAnyPostalCode = record.TownAreaContainAnyPostalCode,
                                PostalCodeContainAnyTownArea = record.PostalCodeContainAnyTownArea
                            });
                            //進捗状況
                            Console.WriteLine(record.Prefecture + record.Municipality + record.TownArea);
                        }
                    }
                });

            }

            return tempDatas;
        }

        public async Task SaveToDb(List<KenAllModel> AddressDatas)
        {
            await _importAddressRepository.ImportAddressFromCsv(AddressDatas);
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{

            //    scope.Complete();
            //}
        }
    }
}
