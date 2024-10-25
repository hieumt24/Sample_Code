using Newtonsoft.Json;

namespace MatchFinder.Domain.Models
{
    public class GeneralBoolResponse
    {
        public bool success { get; set; } = true;
        public string message { get; set; } = string.Empty;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}