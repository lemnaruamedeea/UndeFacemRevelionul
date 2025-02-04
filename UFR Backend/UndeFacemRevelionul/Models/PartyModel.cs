using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UndeFacemRevelionul.Models;

public class PartyModel
{
    
    public int Id { get; set; }
    public string Name { get; set; }

    public int? LocationId { get; set; } // Nullable foreign key
    public int? FoodMenuId { get; set; } // Nullable foreign key
    public virtual LocationModel? Location { get; set; }
    public virtual FoodMenuModel? FoodMenu { get; set; }

    [Range(0, float.MaxValue, ErrorMessage = "Budgetul nu poate fi negativ.")]
    public float TotalBudget { get; set; }

    [Range(0, float.MaxValue, ErrorMessage = "Budgetul ramas nu poate fi negativ.")]
    public float RemainingBudget { get; set; }
    public DateTime Date { get; set; }
    public int TotalPoints { get; set; }

    public ICollection<PartyPartierModel>? PartyUsers { get; set; }
    public PlaylistModel? Playlist { get; set; }
    public ICollection<TaskModel> Tasks { get; set; }
    public ICollection<SuperstitionModel>? Superstitions { get; set; }

}
