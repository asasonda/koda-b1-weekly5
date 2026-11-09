namespace Ewallet.Api.Models;

public sealed class Users
{
    public int Id {get;}
    public string Name {get; private set;}
    public string Email {get; private set;}
    public string Phone {get; private set;}

    public Users(int id, string name, string email, string phone)
    {
        if(id < 0)
        {
            throw new ArgumentException("Id tidak boleh negatif");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Phone is required", nameof(phone));
        }

        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
    }


    public Users(string name, string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required", nameof(phone));

        Name = name;
        Email = email;
        Phone = phone;
    }
}