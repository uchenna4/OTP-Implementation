using Microsoft.EntityFrameworkCore;
using OTPService.Models;

namespace OTPService.Data
{
    public class OTPSessionDbContext: DbContext
    {
        public OTPSessionDbContext(DbContextOptions<OTPSessionDbContext> options) : base(options)
        {
            
        }
        public DbSet <OTPSession> OTPSessions { get; set; }
    }
}
