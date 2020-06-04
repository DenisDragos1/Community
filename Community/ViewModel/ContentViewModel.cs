using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Community.ViewModel
{
    public class ContentViewModel
    {
        public int ID { get; set; }
        /// <summary>
        /// Get and set title of content 
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// Get and set Description for content
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Get and set Content for content
        /// </summary>
        [AllowHtml]
        [Required]
        public string Contents { get; set; }
        /// <summary>
        /// Images
        /// </summary>
        [Required]
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
