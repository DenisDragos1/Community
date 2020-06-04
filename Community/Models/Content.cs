using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Community.Models
{
    public class Content
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public byte[] Image { get; set; }

        public Nullable<int> OwnerId { get; set; }
        public Nullable<System.DateTime> Data { get; set; }
        public Nullable<bool> Solved { get; set; }
        public string Greutate { get; set; }
        public string AdresaExpeditor { get; set; }
        public string AdresaDestinatar { get; set; }
        public string CategorieProdus { get; set; }
        public string Judet { get; set; }
        public string Oras { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}