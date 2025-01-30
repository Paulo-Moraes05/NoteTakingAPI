using Microsoft.EntityFrameworkCore;
using NoteTakingAPI.Models;

namespace NoteTakingAPI.Data;

public class NoteDbContext : DbContext 
{
    // constructor
    public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options) {  }

    // used to perform CRUD operations
    public DbSet<Note> Notes { get; set; }
}