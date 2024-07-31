using SkinsApi.Interfaces.Services;
using SkinsApi.Interfaces.SkinService;
using SkinsApi.Models;
using SkinsApi.Models.SkinService;
using SkinsApi.Sources;
using System.Text;
using System.Text.Json;

namespace SkinsApi.Services
{
    public class SkinService(HttpClient client) : ISkinService
    {
        private async Task<IProfile> GetProfileByNicknameAsync(string nickname)
        {
            return await client.GetFromJsonAsync<MojangProfile>(Startup.PROFILE_MOJANG_API + nickname);
        }
        private async Task<string> GetUuidFromDeparsedData(string data)
        {
            if (data.Length < 25)
            {
                return (await GetProfileByNicknameAsync(data)).id;
            }
            else
            {
                return data;
            }
        }
        private async Task<DecodedSkinProperty> GetSkinProperty(string uuid)
        {
            var base64EncodedSkinProperty = await client.GetFromJsonAsync<MojangSessionProperty>(Startup.SESSION_MOJANG_API + uuid);
            var content = base64EncodedSkinProperty.properties.First().value;
            byte[] data = Convert.FromBase64String(content);
            string decodedString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<DecodedSkinProperty>(decodedString);
        }
        public async Task<Skin> GetSkinStreamAsync(string data)
        {
            var uuid = await GetUuidFromDeparsedData(data);
            var skinproperty = await GetSkinProperty(uuid);
            var rq = await client.GetAsync(skinproperty.Textures.SKIN.Url);
            return new Skin(rq.Content.ReadAsStream(), skinproperty.Textures.SKIN.Metadata?.Model == "slim");
        }

    }
}
