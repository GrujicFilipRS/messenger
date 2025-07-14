using System.Security.Cryptography;
using System.Text;

namespace Messenger.Models;

public class MessageModel
{
    public static List<MessageModel> messages = new List<MessageModel>();

    public int id { get; private set; }
    public int fromUserId { get; private set; }
    public int toUserId { get; private set; }
    public string messageText { get; private set; }
    public DateTime messageSentTime { get; private set; }

    public MessageModel() { }

    public MessageModel(int fromUserId, string toUserUsername, string message)
    {
        this.fromUserId = fromUserId;
        toUserId = UserModel.users.Find(x => x.username == toUserUsername)!.id;
        messageText = HashMessage(message, fromUserId, toUserId);
        messageSentTime = DateTime.Now;
    }

    public static string HashMessage(string message, int senderId, int receiverId)
    {
        string combinedKey = $"{Math.Min(senderId, receiverId)}-{Math.Max(senderId, receiverId)}";
        using SHA256 sha256 = SHA256.Create();
        byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedKey));
        byte[] aesKey = new byte[16];
        Array.Copy(keyBytes, aesKey, 16);

        using Aes aes = Aes.Create();
        aes.Key = aesKey;
        aes.GenerateIV();

        ICryptoTransform encryptor = aes.CreateEncryptor();
        using MemoryStream ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (StreamWriter sw = new StreamWriter(cs))
        {
            sw.Write(message);
        }

        byte[] encrypted = ms.ToArray();
        return Convert.ToBase64String(encrypted);
    }

    public static string UnhashMessage(string encryptedMessage, int senderId, int receiverId)
    {
        string combinedKey = $"{Math.Min(senderId, receiverId)}-{Math.Max(senderId, receiverId)}";
        using SHA256 sha256 = SHA256.Create();
        byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedKey));
        byte[] aesKey = new byte[16];
        Array.Copy(keyBytes, aesKey, 16);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
        using Aes aes = Aes.Create();
        aes.Key = aesKey;

        byte[] iv = new byte[16];
        Array.Copy(encryptedBytes, iv, 16);
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor();
        using MemoryStream ms = new MemoryStream(encryptedBytes, 16, encryptedBytes.Length - 16);
        using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}