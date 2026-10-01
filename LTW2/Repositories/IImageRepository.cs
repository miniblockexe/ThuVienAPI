using LTW2.Models.Domain;

namespace LTW2.Repositories
{
    public interface IImageRepository
    {
        Image Upload(Image image);

        List<Image> GetAllInfoImages();

        (byte[], string, string) DownloadFile(int Id);
    }
}
