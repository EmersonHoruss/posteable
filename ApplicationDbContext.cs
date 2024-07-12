using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Postable
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
  }
}