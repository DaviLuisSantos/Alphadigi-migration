using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using Alphadigi_migration.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Alphadigi_migration.Domain.EntitiesNew;

[Table("LPR_MT_ACESSO")]

public class Acesso : EntityBase, IAggregateRoot
{
    [Key]
    [Column("ID")]
    public override int Id { get; protected set; }

    [Column("LOCAL")]
    public string Local { get; private set; }

    [Column("DATA_HORA")]
    public DateTime DataHora { get; private set; }

    [Column("UNIDADE")]
    public string Unidade { get; private set; }

    [Column("PLACA_LPR")]
    public PlacaVeiculo Placa { get; private set; }

    [Column("DADOS_VEICULO")]
    public string DadosVeiculo { get; private set; }

    [Column("GRUPO_NOME")]
    public string GrupoNome { get; private set; }

    [Column("FOTO")]
    public string Foto { get; private set; }
    //public bool AcessoPermitido { get; private set; }
    //public string Motivo { get; private set; }
    //public string IpCamera { get; private set; }

    // Construtores
    protected Acesso() { } // Para ORM

    public Acesso(
        string local,
        DateTime dataHora,
        string unidade,
        string placa,
        string dadosVeiculo,
        string grupoNome,
        string ipCamera,
        string foto = null,
        bool acessoPermitido = false,
        string motivo = "")
        
    {
        ValidarLocal(local);
        ValidarUnidade(unidade);
        ValidarPlaca(placa);
        ValidarDadosVeiculo(dadosVeiculo);
        ValidarGrupoNome(grupoNome);
        ValidarIpCamera(ipCamera);

        Local = local;
        Unidade = unidade;
        Placa = new PlacaVeiculo(placa);
        DadosVeiculo = dadosVeiculo;
        GrupoNome = grupoNome;
        Foto = foto;
        //AcessoPermitido = acessoPermitido;
        //Motivo = motivo;
        //IpCamera = ipCamera;
        DataHora = dataHora;

        AddDomainEvent(new AcessoRegistradoEvent(Id, Placa.Numero, Local));
    }

    // Métodos de Domínio
    //public void PermitirAcesso(string motivo = "Liberado pelo sistema")
    //{
    //    if (AcessoPermitido) return;

    //    AcessoPermitido = true;
    //    Motivo = motivo;

    //    AddDomainEvent(new AcessoPermitidoEvent(Id, Placa.Numero, Local, motivo));
    //}

    //public void NegarAcesso(string motivo)
    //{
    //    if (!AcessoPermitido && !string.IsNullOrEmpty(Motivo)) return;

    //    AcessoPermitido = false;
    //    Motivo = motivo ?? "Acesso negado";

    //    AddDomainEvent(new AcessoNegadoEvent(Id, Placa.Numero, Local, motivo));
    //}

    public void AtualizarFoto(string fotoUrl)
    {
        if (string.IsNullOrWhiteSpace(fotoUrl))
            throw new Exception("URL da foto não pode ser vazia");

        Foto = fotoUrl;

        AddDomainEvent(new AcessoFotoAtualizadaEvent(Id, Placa.Numero, fotoUrl));
    }

    public void AtualizarDadosVeiculo(string dadosVeiculo)
    {
        ValidarDadosVeiculo(dadosVeiculo);

        DadosVeiculo = dadosVeiculo;

        AddDomainEvent(new AcessoDadosVeiculoAtualizadosEvent(Id, Placa.Numero, dadosVeiculo));
    }

    // Métodos de Validação
    private void ValidarLocal(string local)
    {
        if (string.IsNullOrWhiteSpace(local))
            throw new Exception("Local não pode ser vazio");

        if (local.Length > 100)
            throw new Exception("Local não pode exceder 100 caracteres");
    }

    private void ValidarUnidade(string unidade)
    {
        if (string.IsNullOrWhiteSpace(unidade))
            throw new Exception("Unidade não pode ser vazia");

        if (unidade.Length > 50)
            throw new Exception("Unidade não pode exceder 50 caracteres");
    }

    private void ValidarPlaca(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new Exception("Placa não pode ser vazia");
    }

    private void ValidarDadosVeiculo(string dadosVeiculo)
    {
        if (string.IsNullOrWhiteSpace(dadosVeiculo))
            throw new Exception("Dados do veículo não podem ser vazios");

        if (dadosVeiculo.Length > 200)
            throw new Exception("Dados do veículo não podem exceder 200 caracteres");
    }

    private void ValidarGrupoNome(string grupoNome)
    {
        if (string.IsNullOrWhiteSpace(grupoNome))
            throw new Exception("Nome do grupo não pode ser vazio");

        if (grupoNome.Length > 100)
            throw new Exception("Nome do grupo não pode exceder 100 caracteres");
    }

    private void ValidarIpCamera(string ipCamera)
    {
        if (string.IsNullOrWhiteSpace(ipCamera))
            throw new Exception("IP da câmera não pode ser vazio");

        if (!System.Net.IPAddress.TryParse(ipCamera, out _))
            throw new Exception("Formato de IP inválido");
    }

    public override string ToString()
    {
        return $"{DataHora:dd/MM/yyyy HH:mm} - {Placa} - {Local} ";
    }
}