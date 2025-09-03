using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Events;

namespace Alphadigi_migration.Domain.EntitiesNew;

public class Alphadigi : EntityBase, IAggregateRoot
{
    // Propriedades
    public string Ip { get; private set; }
    public string Nome { get; private set; }
    public int AreaId { get; private set; }
    public Area Area { get; private set; }

    public bool Sentido { get; private set; }
    public string Estado { get; private set; }
    public Guid? UltimoId { get; private set; }
    public string UltimaPlaca { get; private set; }
    public DateTime? UltimaHora { get; private set; }
    public int LinhasDisplay { get; private set; }
    public bool Enviado { get; private set; }
    public bool FotoEvento { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    

    // Construtores
    protected Alphadigi() { } // Para ORM

    public Alphadigi(
        string ip,
        string nome,
        int areaId,
        bool sentido,
        int linhasDisplay = 2)
    {
        ValidarIp(ip);
        ValidarNome(nome);
        ValidarLinhasDisplay(linhasDisplay);

        Ip = ip;
        Nome = nome;
        AreaId = areaId;
        Sentido = sentido;
        Estado = "ACTIVE";
        LinhasDisplay = linhasDisplay;
        Enviado = false;
        FotoEvento = false;
        DataCriacao = DateTime.UtcNow;

        AddDomainEvent(new AlphadigiCreatedEvent(Id, Nome, Ip, AreaId));
    }

    // Métodos de Domínio

    public void AtualizarLinhasDisplay(int novaLinhasDisplay)
    {
        ValidarLinhasDisplay(novaLinhasDisplay);

        LinhasDisplay = novaLinhasDisplay;
        DataAtualizacao = DateTime.UtcNow;

         AddDomainEvent(new AlphadigiLinhasDisplayAtualizadasEvent(Id, Nome, novaLinhasDisplay));
    }
    public void AtualizarArea(Area area)
    {
        if (area == null)
            throw new Exception("Área não pode ser nula");

        Area = area;
       // AreaId = area.Id;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AlphadigiAreaAtualizadaEvent(Id, Nome, area.Nome));
    }

    public void AtualizarDadosBasicos(int areaId, bool sentido)
    {
        AreaId = areaId;
        Sentido = sentido;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AlphadigiDadosAtualizadosEvent(Id, areaId, sentido));
    }
    public void AtualizarUltimaPlaca(string placa, DateTime timestamp)
    {
        UltimaPlaca = placa;
        UltimaHora = timestamp;
        
    }
    public void AtualizarUltimoId(Guid? novoUltimoId)
    {
        if (novoUltimoId == Guid.Empty )
            throw new Exception("Último ID não pode ser negativo");

        UltimoId = novoUltimoId;
        DataAtualizacao = DateTime.UtcNow;

       // AddDomainEvent(new AlphadigiUltimoIdAtualizadoEvent(Id, novoUltimoId));
    }
    public void MarcarComoEnviado()
    {
        Enviado = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void MarcarComoNaoEnviado()
    {
        Enviado = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarEstado(string novoEstado)
    {
        if (string.IsNullOrWhiteSpace(novoEstado))
            throw new Exception("Estado não pode ser vazio");

        Estado = novoEstado;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AlphadigiEstadoAtualizadoEvent(Id, novoEstado));
    }

    public void AtualizarInformacoes(string ip, string nome, int areaId, bool sentido, Area area = null)
    {
        ValidarIp(ip);
        ValidarNome(nome);

        Ip = ip;
        Nome = nome;
        AreaId = areaId;
        Area = area;
        Sentido = sentido;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AlphadigiUpdatedEvent(Id, "Informações básicas atualizadas"));
    }

    // ... outros métodos permanecem iguais ...

    // Métodos de Validação (usando DomainException)

    private void ValidarLinhasDisplay(int linhasDisplay)
    {
        if (linhasDisplay < 0 || linhasDisplay > 4)
            throw new Exception("Linhas de display devem estar entre 0 e 4");
    }
    private void ValidarIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            throw new Exception("IP não pode ser vazio");

        if (ip.Length > 15)
            throw new     Exception("IP não pode exceder 15 caracteres");

        if (!System.Net.IPAddress.TryParse(ip, out _))
            throw new Exception("Formato de IP inválido");
    }

    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome não pode ser vazio");

        if (nome.Length > 100)
            throw new Exception("Nome não pode exceder 100 caracteres");
    }

    
}