using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;


namespace Alphadigi_migration.Domain.EntitiesNew;

public class Area : EntityBase, IAggregateRoot
{
    // Propriedades
    public string Nome { get; private set; }
    public bool ControlaVaga { get; private set; }
    public bool ExibeNaoCadastrado { get; private set; }
    public bool EntradaVisita { get; private set; }
    public bool SaidaVisita { get; private set; }
    public bool SaidaSempreAbre { get; private set; }
    public bool ExibeNaoCadastradoSoEntrada { get; private set; }
    public TimeSpan? TempoAntipassback { get; private set; }
    public DateTime DataCriacao { get; private set; }
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
        TimeSpan? tempoAntipassback = null)
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

    public Guid ObterId() => Id;
    public void AtualizarInformacoes(
        string nome,
        bool controlaVaga,
        bool exibeNaoCadastrado,
        bool entradaVisita,
        bool saidaVisita,
        bool saidaSempreAbre,
        bool exibeNaoCadastradoSoEntrada,
        TimeSpan? tempoAntipassback)
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

    public void ConfigurarTempoAntipassback(TimeSpan tempoAntipassback)
    {
        if (tempoAntipassback <= TimeSpan.Zero)
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
    public bool TemControleAntipassback() => TempoAntipassback.HasValue;
    public bool DeveExibirNaoCadastrados() => ExibeNaoCadastrado;
    public bool SaidaSempreLiberada() => SaidaSempreAbre;

    public override string ToString()
    {
        return $"{Nome} (Vagas: {(ControlaVaga ? "Controladas" : "Livre")})";
    }
}