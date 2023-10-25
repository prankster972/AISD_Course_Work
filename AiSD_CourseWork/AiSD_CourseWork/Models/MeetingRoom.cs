using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AiSD_CourseWork.Models;

[Table("MeetingRoom")]
public partial class MeetingRoom
{
    [Key]
    public int MeetingId { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Enter Meeting Name")]
    [DisplayName(displayName: "Meeting Name")]
    public string MeetingName { get; set; } = null!;

    [Column(TypeName = "date")]
    [Required(ErrorMessage = "Enter Meeting Date")]
    [DataType(DataType.Date)]
    [DisplayName(displayName: "Meeting Date")]
    public DateTime MeetingDate { get; set; }

    [Precision(0)]
    [Required(ErrorMessage = "Enter Start time")]
    [DataType(DataType.Time)]
    [DisplayName(displayName: "Start time")]
    public TimeSpan TimeFrom { get; set; }

    [Precision(0)]
    [Required(ErrorMessage = "Enter End time")]
    [DataType(DataType.Time)]
    [DisplayName(displayName: "End time")]
    public TimeSpan TimeTo { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Enter members")]
    public string Members { get; set; } = null!;

    public int Id { get; set; }

    [ForeignKey("Id")]
    [InverseProperty("MeetingRooms")]
    public virtual User IdNavigation { get; set; } = null!;
}
