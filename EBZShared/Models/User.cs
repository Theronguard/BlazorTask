using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EBZShared.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        [EnumMember(Value = "male")]
        Male,
        [EnumMember(Value = "female")]
        Female,
        [EnumMember(Value = "unspecified")]
        Unspecified
    }

    public class User
    {
        [Key]
        [NotNull]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required!")]
        [MinLength(4, ErrorMessage = "Username has to be at least 4 characters long!")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can contain letters and numbers only!")]
        [NotNull]
        public string Username { get; set; }

        [AllowNull]
        public string? Description { get; set; }

        [AllowNull]
        public string? Skills { get; set; }

        [AllowNull]
        public string? Interests { get; set; }

        [Required(ErrorMessage = "City is required!")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "City can contain letters only!")]
        [NotNull]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required!")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Country can contain letters only!")]
        [NotNull]
        public string Country { get; set; }

        [NotNull]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required!")]
        [NotNull]
        public DateOnly DateOfBirth { get; set; }

        [NotNull]
        public DateTime Created { get; set; }

        [NotNull]
        public DateTime LastActive { get; set; }

        //empty constructor for ORM's
        public User() { }

    }

    
}
