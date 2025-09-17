

namespace Alphadigi_migration.Domain.Interfaces;

public interface IUpdateAlphadigiDTO
{
    int Id { get; set; }
    string Ip { get; set; }
    string Nome { get; set; }
    bool Sentido { get; set; }
    string? Estado { get; set; }
}
