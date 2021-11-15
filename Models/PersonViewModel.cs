namespace Demo.Function.Models;

public class PersonViewModel : ModelMap
{
    [MappedTo("Id")]
    public string Id { get; set; }

    [MappedTo("Username")]
    public string UName { get; set; }

    [MappedTo("FirstName")]
    public string FName { get; set; }

    [MappedTo("LastName")]
    public string LName { get; set; }

    public string FullName { get; set; }

    [MappedTo("Address")]
    public string Address { get; set; }

    [MappedTo("Phone")]
    public string PhoneNumber { get; set; }

    [MappedTo("Email")]
    public string EmailAddress { get; set; }
}