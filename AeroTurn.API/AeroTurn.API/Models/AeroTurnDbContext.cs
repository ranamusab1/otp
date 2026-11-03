using Microsoft.EntityFrameworkCore;

namespace AeroTurn.API.Models

{
    public class AeroTurnDbContext : DbContext
    {
        public AeroTurnDbContext(DbContextOptions<AeroTurnDbContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<TurnaroundEvent> TurnaroundEvents { get; set; }
    }
}