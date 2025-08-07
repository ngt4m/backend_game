using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class User
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; }

    [Required, StringLength(20)]
    public string PhoneNumber { get; set; }

    [Required, StringLength(100)]
    public string Email { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public int Status { get; set; }

    [StringLength(50)]
    public string IpAddress { get; set; }

    [StringLength(200)]
    public string DeviceInfo { get; set; }

    public virtual ICollection<UserResult> UserResults { get; set; }

    public User()
    {
        UserResults = new HashSet<UserResult>();
    }
}
