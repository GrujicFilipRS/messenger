using System.Security.Cryptography;
using System.Text;
namespace Messenger.Models;

public class UserModel
{
    private static List<UserModel> users = new List<UserModel>();

    private int id;
    private string username;
    private string hashedPassword;

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

    /// <summary>
    /// Sets the username of the user to a new value
    /// </summary>
    /// <param name="newUsername">The new username</param>
    /// <param name="passwordConfirmation">Confirmation of the password, not encrypted</param>
    /// <returns>
    /// Status code of the operation 
    /// (Status code 0: success, 
    /// Status code 1: password incorrect, 
    /// Status code 2: username hasn't changed)
    /// </returns>
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
}