using SkinsApi.Interfaces.SkinService;

namespace SkinsApi.Models.SkinService
{
    public class MojangProfile : IProfile
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
