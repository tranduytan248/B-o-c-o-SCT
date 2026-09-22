using System.ComponentModel.DataAnnotations;

namespace TSFramework.App.OAuth
{
    public class Audience
    {
        [Key] [MaxLength(32)] public string ClientId { get; set; }

        [MaxLength(80)] [Required] public string Base64Secret { get; set; }

        [MaxLength(100)] [Required] public string Name { get; set; }
    }

    public class AudienceModel
    {
        [MaxLength(250)] [Required] public string Name { get; set; }
    }
}