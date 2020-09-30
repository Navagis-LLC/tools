using System.Text.Json.Serialization;

namespace RegisterProject_Spice.Pages.Models
{
    internal class Repository
    {
        //public string name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; internal set; }
    }
}