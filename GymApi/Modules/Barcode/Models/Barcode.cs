using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.Modules.Barcode.Models
{
    public class Barcode
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Unique barcode string generated per access rules
        public string Code { get; set; } = string.Empty;

        // Access/Barcode type (SalonGiris, PTOzelDers, SpaSauna, GrupDersi)
        public BarcodeTypes Types { get; set; }

        // Snapshot of validity date derived from the Member's active membership
        public DateTime ExpirationDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key referencing the single active member
        public Guid MemberId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}