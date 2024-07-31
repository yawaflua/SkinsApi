using SkinsApi.Sources;

namespace SkinsApi.Interfaces.Services
{
    public interface ISkinService
    {
        Task<Skin> GetSkinStreamAsync(string data);
    }
}
