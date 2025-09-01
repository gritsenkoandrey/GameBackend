using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameBackend.Scripts.Data;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Money { get; set; }
}