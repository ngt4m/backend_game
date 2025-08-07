using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Contests")]
public class Contest
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MaxAttempts { get; set; }

    [StringLength(100)]
    public string AntiCheatToken { get; set; }

    public virtual ICollection<UserResult> UserResults { get; set; }

    public Contest()
    {
        UserResults = new HashSet<UserResult>();
    }
}
