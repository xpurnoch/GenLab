using BioLabManager.Models;
using BioLabManager.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public static class AuthService
{
	public static User CurrentUser { get; private set; }

	public static async Task<bool> RegisterAsync(string username, string password, string labName)
	{
        if (new[] { username, password, labName }.Any(string.IsNullOrWhiteSpace))
            return false;

        await using var db = new BioLabDbContext();
		if (await db.Users.AnyAsync(u => u.Username == username))
			return false;

        var lab = await LabService.GetOrCreateLabAsync(db, labName);
        var user = new User
        {
            Username     = username,
            PasswordHash = HashPassword(password),
            Lab          = lab,
            Role         = "User"
        };

        db.Users.Add(user);
		await db.SaveChangesAsync();
		return true;
	}

	public static async Task<bool> LoginAsync(string username, string password)
	{
		await using var db = new BioLabDbContext();
		var user = await db.Users
			.Include(u => u.Lab)
			.FirstOrDefaultAsync(u => u.Username == username);

		if (user == null || !SecureEquals(user.PasswordHash, HashPassword(password)))
            return false;

        CurrentUser = user;
        return true;
    }

    private static bool SecureEquals(string a, string b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }

    private static string HashPassword(string password)
	{
		using var sha256 = SHA256.Create();
		var bytes = Encoding.UTF8.GetBytes(password);
		var hash = sha256.ComputeHash(bytes);
		return Convert.ToBase64String(hash);
	}

	public static void Logout() => CurrentUser = null;
}
