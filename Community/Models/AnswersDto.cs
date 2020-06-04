using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Community.Models
{
    public class AnswersDto
    {
        public int RequestId { get; set; }
        public string Titlu { get; set; }
        public string Descriere { get; set; }
        public string Pret { get; set; }
        public string Telefon { get; set; }
        public bool Gasit { get; set; }
        public int? ServiceId { get; set; }
        public string Message { get; set; }
    }
}