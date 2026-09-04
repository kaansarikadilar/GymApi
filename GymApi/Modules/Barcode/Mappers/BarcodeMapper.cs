using System.Collections.Generic;
using System.Linq;
using GymApi.Modules.Barcode.DTOs;
using GymApi.Modules.Barcode.Models;

namespace GymApi.Modules.Barcode.Mappers
{
    public static class BarcodeMapper
    {
        public static BarcodeResponse ToBarcodeResponse(this BarcodeEntity entity)
        {
            return new BarcodeResponse
            {
                Id = entity.Id,
                MemberName = entity.MemberName,
                MemberCode = entity.MemberCode,
                Email = entity.Email,
                Code = entity.Code,
                BarcodeType = entity.Types,
                IsActive = entity.IsActive,
                StartDate = entity.CreatedAt,
                ExpirationDate = entity.ExpirationDate
            };
        }

        public static IEnumerable<BarcodeResponse> ToResponseList(this IEnumerable<BarcodeEntity>? entities)
        {
            if (entities == null)
            {
                return Enumerable.Empty<BarcodeResponse>();
            }
            return entities.Select(e => e.ToBarcodeResponse());
        }

        public static List<BarcodeResponse> ToResponseListForId(this BarcodeEntity entity)
        {
            return new List<BarcodeResponse> 
            { 
                entity.ToBarcodeResponse() 
            };
        }
    }
}