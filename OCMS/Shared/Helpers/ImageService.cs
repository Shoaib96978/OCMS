namespace OCMS.Shared.Helpers
{
    public static class ImageService
    {
        private static readonly string _baseUploadPath = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        // ===================== SAVE =====================
        public static async Task<string?> SaveAsync(IFormFile? file, string folder = "complaints")
        {
            if (file == null || file.Length == 0)
                return null;

            var folderPath = Path.Combine(_baseUploadPath, folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        // ===================== DELETE =====================
        public static bool Delete(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            // "/uploads/complaints/file.jpg" → physical path
            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
            );

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }
    }
}
