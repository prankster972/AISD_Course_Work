using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AiSD_CourseWork.Models;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Введите имя пользователя")]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = null!;

    [InverseProperty("IdNavigation")]
    public virtual ICollection<MeetingRoom> MeetingRooms { get; set; } = new List<MeetingRoom>();
}
