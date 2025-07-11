using System.Security.Cryptography;
using System.Text;
namespace Messenger.Models;

public class UserModel
{
    private static List<UserModel> users = new List<UserModel>();

    public int id { get; private set; }
    public string username { get; private set; }
    public string hashedPassword { get; private set; }

    public UserModel() { }

    public UserModel(int id, string username, string hashedPassword)
    {
        this.id = id;
        this.username = username;
        this.hashedPassword = hashedPassword;
    }

    public UserModel(string username, string password)
    {
        id = users.Count;
        this.username = username;

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            hashedPassword = builder.ToString();
        }

        users.Add(this);
    }

    public int GetId() => id;
    public string GetUsername() => username;
    public string GetHashedPassword() => hashedPassword;

    public int SetUsername(string newUsername, string passwordConfirmation)
    {
        if (newUsername == username) return 2;

        string passwordEncrypted;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordConfirmation));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            passwordEncrypted = builder.ToString();
        }

        if (passwordEncrypted != hashedPassword) return 1;

        username = newUsername;
        return 0;
    }

    public static int? GetUserId(string username, string password)
    {
        string passwordEncrypted;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            passwordEncrypted = builder.ToString();
        }

        foreach (UserModel user in users)
        {
            if (user.GetUsername() == username && user.GetHashedPassword() == passwordEncrypted) return user.id;
        }

        return null;
    }

    public static bool UserWithNameExists(string username)
    {
        return users.Any(x => x.username == username);
    }

    public static bool ValidPassword(string password)
    {
        char[] uppercaseLetters = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();
        char[] lowercaseLetters = Enumerable.Range('a', 26).Select(x => (char)x).ToArray();

        bool correctLength = password.Length >= 8;
        bool oneUppercase = password.Any(x => uppercaseLetters.Contains(x));
        bool oneLowercase = password.Any(x => lowercaseLetters.Contains(x));
        bool noSpaces = !password.Any(x => x == ' ');

        return correctLength && oneUppercase && oneLowercase && noSpaces;
    }

    public static bool ValidUsername(string username)
    {
        char[] uppercaseLetters = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();
        char[] lowercaseLetters = Enumerable.Range('a', 26).Select(x => (char)x).ToArray();
        char[] numbersChars = Enumerable.Range('0', 10).Select(x => (char)x).ToArray();
        char[] specialChars = { '_', '-' };

        char[] validCharacters = uppercaseLetters.Concat(lowercaseLetters).Concat(numbersChars).Concat(specialChars).ToArray();

        bool minimalLength = username.Length >= 6;
        bool correctChars = username.All(x => validCharacters.Contains(x));

        return minimalLength && correctChars;
    }
}