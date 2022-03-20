using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CustomerModel.ValueObjects
{
    public record CustomerAddress(string? PostalCode, string? Prefecture, string? Municipality, string? TownArea)
    {
        //public CustomerAddress(string? postalCode, string? prefecture, string? municipality, string? townArea)
        //{
        //    PostalCode = postalCode;
        //    Prefecture = prefecture;
        //    Municipality = municipality;
        //    TownArea = townArea;
        //}

        //public string? PostalCode { get; init; }

        //public string? Prefecture { get; init; }

        //public string? Municipality { get; init; }

        //public string? TownArea { get; init; }
    }
}
