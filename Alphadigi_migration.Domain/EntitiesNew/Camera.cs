using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;


namespace Alphadigi_migration.Domain.EntitiesNew;

[Table("LPR_MT_CAMERAS")]
public class Camera : EntityBase, IAggregateRoot
{
    // Propriedades
    public string Nome { get; private set; }
    public string Ip { get; private set; }
    public string Modelo { get; private set; }
    public string Direcao { get; private set; }
    public int IdArea { get; private set; }
    public Area Area { get; private set; }
    public bool FotoEvento { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public bool Ativa { get; private set; }

    // Construtores
    protected Camera() { } // Para ORM

    public Camera(
        string nome,
        string ip,
        int idArea,
        string modelo = null,
        string direcao = null,
        bool fotoEvento = false)
    {
        ValidarNome(nome);
        ValidarIp(ip);
        ValidarModelo(modelo);
        ValidarDirecao(direcao);

        Nome = nome;
        Ip = ip;
        IdArea = idArea;
        Modelo = modelo;
        Direcao = direcao;
        FotoEvento = fotoEvento;
        Ativa = true;
        DataCriacao = DateTime.UtcNow;

        AddDomainEvent(new CameraCriadaEvent(Id, Nome, Ip, IdArea));
    }

    // Métodos de Domínio
    public void AtualizarInformacoes(string nome, string ip, string modelo, string direcao)
    {
        ValidarNome(nome);
        ValidarIp(ip);
        ValidarModelo(modelo);
        ValidarDirecao(direcao);

        Nome = nome;
        Ip = ip;
        Modelo = modelo;
        Direcao = direcao;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraAtualizadaEvent(Id, Nome, "Informações básicas atualizadas"));
    }

    public void AtualizarArea(int  novaAreaId)
    {
        if (novaAreaId == null)
            throw new Exception("ID da área não encontrado");

        IdArea = novaAreaId;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraAreaAtualizadaEvent(Id, Nome, novaAreaId));
    }

    public void ConfigurarFotoEvento(bool habilitar)
    {
        FotoEvento = habilitar;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraFotoEventoConfiguradaEvent(Id, Nome, habilitar));
    }

    public void Ativar()
    {
        if (Ativa) return;

        Ativa = true;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraAtivadaEvent(Id, Nome));
    }

    public void Desativar(string motivo = "Manutenção")
    {
        if (!Ativa) return;

        Ativa = false;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraDesativadaEvent(Id, Nome, motivo));
    }

    public void AssociarArea(Domain.EntitiesNew.Area area)
    {
        if (area == null)
            throw new Exception("Área não pode ser nula");

        Area = area;
        IdArea = area.Id;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CameraAreaAssociadaEvent(Id, Nome, area.Id, area.Nome));
    }

    // Métodos de Validação
    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome da câmera não pode ser vazio");

        if (nome.Length > 100)
            throw new Exception("Nome da câmera não pode exceder 100 caracteres");
    }

    private void ValidarIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            throw new Exception("IP da câmera não pode ser vazio");

        if (ip.Length > 15)
            throw new Exception("IP da câmera não pode exceder 15 caracteres");

        if (!System.Net.IPAddress.TryParse(ip, out _))
            throw new Exception("Formato de IP inválido");
    }

    private void ValidarModelo(string modelo)
    {
        if (modelo != null && modelo.Length > 50)
            throw new Exception("Modelo da câmera não pode exceder 50 caracteres");
    }

    private void ValidarDirecao(string direcao)
    {
        if (direcao != null && direcao.Length > 50)
            throw new Exception("Direção da câmera não pode exceder 50 caracteres");
    }

    // Métodos de consulta
    public bool EstaAtiva() => Ativa;
    public bool CapturaFotoEvento() => FotoEvento;
    public string ObterLocalizacao() => Area?.Nome ?? "Área não especificada";

    public override string ToString()
    {
        return $"{Nome} ({Ip}) - {Area?.Nome} - {(Ativa ? "Ativa" : "Inativa")}";
    }
}