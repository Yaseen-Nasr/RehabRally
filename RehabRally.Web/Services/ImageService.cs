using RehabRally.Web.Core.Consts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RehabRally.Web.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(bool isUploaded, string? errorMessage)> UploadFiles(IFormFile image, string imageName, string folderPath, bool hasThumbnail)
        {

            var extension = Path.GetExtension(image.FileName);

            if (!_allowedExtensions.Contains(extension))
                return (isUploaded: false, Errors.NotAllowedExtension);


            if (image.Length > _maxAllowedSize)
                return (isUploaded: false, Errors.MaxSize);

            var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}", imageName);
            //save data on server 
            using var stream = File.Create(path);
            await image.CopyToAsync(stream);
            stream.Dispose();

            if (hasThumbnail)
            {
                var pathThumb = Path.Combine($"{_webHostEnvironment.WebRootPath}{folderPath}/thumb", imageName);
                using var lodaedImage = Image.Load(image.OpenReadStream());
                float ratioe = lodaedImage.Width / 200;
                var height = lodaedImage.Height / ratioe;
                lodaedImage.Mutate(i => i.Resize(width: 200, height: (int)height));
                lodaedImage.Save(pathThumb);

            }
            return (isUploaded: true, errorMessage: null);

        }
        public void Delete(string imagePath, string? imageThumbnailPath = null)
        {
            var oldImagePath = $"{_webHostEnvironment.WebRootPath}{imagePath}";

            if (File.Exists(oldImagePath))
                File.Delete(oldImagePath);
            if (!string.IsNullOrEmpty(imageThumbnailPath))
            {
                var oldImageThembPath = $"{_webHostEnvironment.WebRootPath}{imageThumbnailPath}";
                if (File.Exists(oldImageThembPath))
                    File.Delete(oldImageThembPath);
            }
        }

    }
}
