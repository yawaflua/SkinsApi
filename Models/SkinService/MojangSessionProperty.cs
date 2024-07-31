namespace SkinsApi.Models.SkinService
{
    public class Property
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class MojangSessionProperty
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Property> properties { get; set; }
        public List<object> profileActions { get; set; }
    }
}
