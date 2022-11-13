
namespace webapp.model.filter
{
    public class ServiceFilterDTO
    {
        public string? name { get; set; } = "";
        public string[]? tags { get; set; } = { };
        public bool? union { get; set; } = false;
    }
}