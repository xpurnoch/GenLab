using BioLabManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BioLabManager.Services
{
    internal class LabService
    {
        public static async Task<Lab> GetOrCreateLabAsync(BioLabDbContext db, string labName)
        {
            if (string.IsNullOrWhiteSpace(labName))
                throw new ArgumentException("Lab name cannot be empty.", nameof(labName));

            var formattedName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labName.Trim().ToLower());
            var lab = await db.Labs.FirstOrDefaultAsync(l => l.Name == formattedName);

            if (lab == null)
            {
                lab = new Lab { Name = formattedName };
                db.Labs.Add(lab);
                await db.SaveChangesAsync();
            }
            return lab;
        }
    }
}
