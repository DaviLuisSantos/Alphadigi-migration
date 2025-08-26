using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Alphadigi.Domain.Common;

public abstract class EntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; } = true;

    // Para comparar entidades por valor (equality)
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(EntityBase? a, EntityBase? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(EntityBase? a, EntityBase? b)
    {
        return !(a == b);
    }

    // Métodos para alterar estado
    public void SetCreated(DateTime dateTime)
    {
        CreatedAt = dateTime;
    }

    public void SetUpdated(DateTime dateTime)
    {
        UpdatedAt = dateTime;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}