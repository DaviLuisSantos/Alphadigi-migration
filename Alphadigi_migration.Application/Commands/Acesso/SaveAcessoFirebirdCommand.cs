// Alphadigi_migration.Application.Commands.Acesso.SaveAcessoFirebirdCommand.cs
using MediatR;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.DTOs.Veiculos;

namespace Alphadigi_migration.Application.Commands.Acesso;

public class SaveAcessoFirebirdCommand : IRequest<bool>
{
    public string Placa { get; set; }
    public string Acesso { get; set; } // Este será o "Local"
    public DateTime Timestamp { get; set; } // dataHora
    public string Unidade { get; set; }
    public string IpCamera { get; set; }

    // NOVAS PROPRIEDADES QUE O CONSTRUTOR DA ENTIDADE PRECISA
    public string DadosVeiculo { get; set; }
    public string GrupoNome { get; set; }
}