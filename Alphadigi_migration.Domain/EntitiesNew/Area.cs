using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Alphadigi_migration.Domain.EntitiesNew;

[Table("LPR_MT_AREAS")]
public class Area : EntityBase, IAggregateRoot
{
    // Propriedades
    [Key]
    [Column("ID")]
    public override int Id { get; protected set; }

    [Column("DESCRICAO")]
    public string Nome { get; private set; }

    [Column("CONTROLA_VAGA")]
    public bool ControlaVaga { get; private set; }

    [Column("EXIBE_NAO_CAD")]
    public bool ExibeNaoCadastrado { get; private set; }

    [Column("ENTRADA_VISITA")]
    public bool EntradaVisita { get; private set; }

    [Column("SAIDA_VISITA")]
    public bool SaidaVisita { get; private set; }

    [Column("SAIDA_SEMPRE_ABRE")]
    public bool SaidaSempreAbre { get; private set; }
    
    [Column("EXIBE_NAO_CAD_SO_ENTRADA")]

    public bool ExibeNaoCadastradoSoEntrada { get; private set; }

    [Column("TEMPO_ANTIPASSBACK")]
    public string? TempoAntipassback { get; set; }

    [NotMapped]
    public TimeSpan? TempoAntipassbackTimeSpan { get; private set; }

    [NotMapped]
    public DateTime DataCriacao { get; private set; }

    [NotMapped]
    public DateTime? DataAtualizacao { get; private set; }

    // Construtores
    protected Area() { } // Para ORM

    public Area(
        string nome,
        bool controlaVaga = false,
        bool exibeNaoCadastrado = false,
        bool entradaVisita = false,
        bool saidaVisita = false,
        bool saidaSempreAbre = false,
        bool exibeNaoCadastradoSoEntrada = false,
        string? tempoAntipassback = null)
    {
        ValidarNome(nome);

        Nome = nome;
        ControlaVaga = controlaVaga;
        ExibeNaoCadastrado = exibeNaoCadastrado;
        EntradaVisita = entradaVisita;
        SaidaVisita = saidaVisita;
        SaidaSempreAbre = saidaSempreAbre;
        ExibeNaoCadastradoSoEntrada = exibeNaoCadastradoSoEntrada;
        TempoAntipassback = tempoAntipassback;
        DataCriacao = DateTime.UtcNow;

        AddDomainEvent(new AreaCriadaEvent(Id, Nome));
    }

    // Métodos de Domínio

    public int ObterId() => Id;
    public void AtualizarInformacoes(
        string nome,
        bool controlaVaga,
        bool exibeNaoCadastrado,
        bool entradaVisita,
        bool saidaVisita,
        bool saidaSempreAbre,
        bool exibeNaoCadastradoSoEntrada,
        string? tempoAntipassback)
    {
        ValidarNome(nome);

        Nome = nome;
        ControlaVaga = controlaVaga;
        ExibeNaoCadastrado = exibeNaoCadastrado;
        EntradaVisita = entradaVisita;
        SaidaVisita = saidaVisita;
        SaidaSempreAbre = saidaSempreAbre;
        ExibeNaoCadastradoSoEntrada = exibeNaoCadastradoSoEntrada;
        TempoAntipassback = tempoAntipassback;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaAtualizadaEvent(Id, Nome));
    }

    public void AtivarControleVaga()
    {
        if (ControlaVaga) return;

        ControlaVaga = true;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaControleVagaAtivadoEvent(Id, Nome));
    }
    public void AtualizarNome(string novoNome)
    {
        ValidarNome(novoNome);
        Nome = novoNome;
        DataAtualizacao = DateTime.UtcNow;

      //  AddDomainEvent(new AreaNomeAtualizadoEvent(Id, Nome, novoNome));
    }


    public void DesativarControleVaga()
    {
        if (!ControlaVaga) return;

        ControlaVaga = false;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaControleVagaDesativadoEvent(Id, Nome));
    }

    public void ConfigurarTempoAntipassback(string tempoAntipassback)
    {
        if (tempoAntipassback == "")
            throw new Exception("Tempo de antipassback deve ser maior que zero");

        TempoAntipassback = tempoAntipassback;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaTempoAntipassbackConfiguradoEvent(Id, Nome, tempoAntipassback));
    }

    public void RemoverTempoAntipassback()
    {
        TempoAntipassback = null;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaTempoAntipassbackRemovidoEvent(Id, Nome));
    }

    public void HabilitarAcessoVisitantes(bool entrada, bool saida)
    {
        EntradaVisita = entrada;
        SaidaVisita = saida;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new AreaAcessoVisitantesConfiguradoEvent(Id, Nome, entrada, saida));
    }


    // Métodos de Validação
    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome da área não pode ser vazio");

        if (nome.Length > 100)
            throw new Exception("Nome da área não pode exceder 100 caracteres");
    }

    // Métodos de consulta
    public bool PermiteAcessoVisitantesEntrada() => EntradaVisita;
    public bool PermiteAcessoVisitantesSaida() => SaidaVisita;
    public string TemControleAntipassback() => TempoAntipassback;
    public bool DeveExibirNaoCadastrados() => ExibeNaoCadastrado;
    public bool SaidaSempreLiberada() => SaidaSempreAbre;

    public override string ToString()
    {
        return $"{Nome} (Vagas: {(ControlaVaga ? "Controladas" : "Livre")})";
    }
}