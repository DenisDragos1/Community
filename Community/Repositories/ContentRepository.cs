using Community.Models;
using Community.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Community.Repositories
{
    public class ContentRepository
    {
        private readonly DBContext db = new DBContext();
        public int UploadImageInDataBase(HttpPostedFileBase file, ContentViewModel contentViewModel)
        {
            contentViewModel.Image = ConvertToBytes(file);
            var Content = new Content
            {
                Title = contentViewModel.Title,
                Description = contentViewModel.Description,
                Contents = contentViewModel.Contents,
                Image = contentViewModel.Image,
                OwnerId=contentViewModel.OwnerId,
                Solved=contentViewModel.Solved,
                Greutate=contentViewModel.Greutate,
                AdresaDestinatar=contentViewModel.AdresaDestinatar,
                AdresaExpeditor=contentViewModel.AdresaExpeditor,
                CategorieProdus=contentViewModel.CategorieProdus,
                Judet=contentViewModel.Judet,
                Oras=contentViewModel.Oras,
                Email=contentViewModel.Oras,
                Phone=contentViewModel.Phone
            };
           
            int i = db.SaveChanges();
            if (i == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}