using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using GymApi.Models;
using GymApi.Modules.Barcode.DTOs;
using GymApi.Modules.Barcode.Models;

namespace GymApi.Modules.Barcode.Mappers
{
    public static class BarcodeMapper
    {
        public static BarcodeResponse ToBarcodeResponse(this BarcodeEntity entity ,MemberResponse user)
        {
            return new BarcodeResponse{
                Id = entity.Id,
                MemberName = user.AppUserName,
                MemberCode = user.MemberCode,
                Email = user.Email,
                Code = entity.Code,
                BarcodeType = entity.Types,
                IsActive = entity.IsActive,
                StartDate = user.StartDate,
                ExpirationDate = user.EndDate   
            };
        }
        public static IEnumerable<BarcodeResponse> ToResponseList(this IEnumerable<BarcodeEntity> entities,MemberResponse user)
        {
            return entities.Select(e => e.ToBarcodeResponse(user));
        }
    }
}