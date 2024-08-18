using System;
using System.ComponentModel.DataAnnotations;

namespace CRUD.Models
{
    public class SaranModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Nama { get; set; }

        [Required]
        [StringLength(200)]
        public string ?Alamat { get; set; }

        [Required]
        [StringLength(500)]
        public string? Pesan { get; set; }

        public DateTime TanggalDikirim { get; set; }
    }
}