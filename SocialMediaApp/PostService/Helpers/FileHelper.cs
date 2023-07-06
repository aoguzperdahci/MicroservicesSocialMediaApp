namespace PostService.Helpers
{
    public class FileHelper
    {
        public static string GetUniqueFileName(string fileName)
        {
            return string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(fileName));
        }
    }
}
