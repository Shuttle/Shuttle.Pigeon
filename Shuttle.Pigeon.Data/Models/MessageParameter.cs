using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shuttle.Pigeon.Data.Models;

[PrimaryKey(nameof(MessageId), nameof(Name))]
public class MessageParameter
{
    public Guid MessageId { get; set; }
    [Required]
    [StringLength(130)]
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}