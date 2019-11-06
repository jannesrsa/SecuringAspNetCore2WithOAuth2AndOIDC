using ImageGallery.Model;
using System.Collections.Generic;
using System.Security.Claims;

namespace ImageGallery.Client.ViewModels
{
    public class GalleryIndexViewModel
    {
        public IEnumerable<Image> Images { get; private set; }
            = new List<Image>();

        public string IdentityToken { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        public GalleryIndexViewModel(List<Image> images)
        {
           Images = images;
        }
    }
}
