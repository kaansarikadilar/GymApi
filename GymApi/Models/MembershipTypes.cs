using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.Models
{
    public class MembershipTypes
    {
        public int Id { get; set; }
        //VIP,Standart,ogrenci
        public string Name { get; set; } = string.Empty;
        //ST,VP,OG
        public string Code { get; set; } = string.Empty;
    }
}