namespace medibook_API.Extensions.Services
{
    public interface IPasswordHasherRepository
    {
       public string HashPassword(string password);
       public  bool VerifyPassword(string password, string hashedPassword);
    }
}
