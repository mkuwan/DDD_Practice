using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Postal;
using Domain.Repository;
using Repository.Models.Postal;

namespace Repository.Repositories.Postal
{
    public class ImportAddressRepository : IImportAddressRepository
    {
        private readonly PostaldbContext _postaldbContext;


        public ImportAddressRepository(PostaldbContext postaldbContext)
        {
            _postaldbContext = postaldbContext;
        }

        public async Task ImportAddressFromCsv(List<KenAllModel> kenAlls)
        {
            var temp = _postaldbContext.KenAlls.ToList();

            foreach (var x in kenAlls)
            {
                var ken = new KenAll
                {
                    PostalCode = x.PostalCode,
                    PrefectureKana = x.PrefectureKana,
                    MunicipalityKana = x.MunicipalityKana,
                    TownAreaKana = x.TownAreaKana,
                    Prefecture = x.Prefecture,
                    Municipality = x.Municipality,
                    TownArea = x.TownArea,
                    TownAreaContainAnyPostalCode = x.TownAreaContainAnyPostalCode,
                    PostalCodeContainAnyTownArea = x.PostalCodeContainAnyTownArea
                };

                _postaldbContext.Add(ken);
                await _postaldbContext.SaveChangesAsync();
            }
        }
    }
}

