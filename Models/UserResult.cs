using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("UserResults")]
public class UserResult
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; }

    [Required, StringLength(50)]
    public string UserId { get; set; }

    [Required, StringLength(50)]
    public string ContestId { get; set; }

    public int NumberCorrect { get; set; }

    public double TotalTime { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [StringLength(200)]
    public string DeviceInfo { get; set; }

    [StringLength(50)]
    public string IpAddress { get; set; }

    [Required, StringLength(100)]
    public string SubmissionToken { get; set; }

    public bool IsFlagged { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [ForeignKey("ContestId")]
    public virtual Contest Contest { get; set; }
}
