using System.ComponentModel.DataAnnotations;

namespace Demo.Function.Models;

public class Person : ModelMap
{
    [MappedTo("Id"), JsonProperty("id")]
    [Required(ErrorMessage = "Id is required")]
    public string Id { get; set; }

    [JsonProperty("addedOn")]
    public DateTime AddedOn { get; set; }

    [JsonProperty("modifiedOn")]
    public DateTime ModifiedOn { get; set; }

    [MappedTo("UName"), JsonProperty("userName")]
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(30, ErrorMessage = "Username cannot be longer than 30 characters.")]
    public string Username { get; set; }

    [MappedTo("FName"), JsonProperty("firstName")]
    [MaxLength(80, ErrorMessage = "Name cannot be longer than 80 characters.")]
    public string FirstName { get; set; }

    [MappedTo("LName"), JsonProperty("lastName")]
    [MaxLength(80, ErrorMessage = "Last name cannot be longer than 80 characters.")]
    public string LastName { get; set; }

    [MappedTo("FullName"), JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    [MappedTo("Address"), JsonProperty("address")]
    [MaxLength(250, ErrorMessage = "Description must be a string with a maximum length of '250'.")]
    public string Address { get; set; }

    [MappedTo("PhoneNumber"), JsonProperty("phoneNumber")]
    [RegularExpression(@"^(09|\+639)\d{9}$", ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }

    [MappedTo("EmailAddress"), JsonProperty("email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    public bool IsValid()
    {
        ValidationContext validationContext = new ValidationContext(this);
        IList<ValidationResult> validationErrors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(this, validationContext, validationErrors, true))
        {
            string errorMessages = string.Join("\r\n", validationErrors.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)).Select(e => e.ErrorMessage));
            throw new Exception(errorMessages ?? "An error occurred.",
                new Exception($"Error validating the following fields: {string.Join(",", validationErrors?.FirstOrDefault()?.MemberNames ?? new List<string>())}"));
        }
        else return !validationErrors.Any();
    }
}