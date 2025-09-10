using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Alphadigi_migration.Domain.EntitiesNew;

[Table("UNIDADE")]
public class Unidade : EntityBase, IAggregateRoot
{
    [Key]
    [Column("IDUNIDADE")]
    public override int Id { get; protected set; }

    [Column("NUMVAGAS")]
    public int NumeroVagas { get; private set; }

    [Column("UNIDADE")]
    public string Nome { get; private set; }

   

    // Navegação para o condomínio (se existir)
    //public virtual Condominio Condominio { get; private set; }
    //public int? CondominioId { get; private set; }

    // Construtor protegido para ORM
    protected Unidade() { }

    // Construtor principal
    public Unidade(string nome, int numeroVagas, int? condominioId = null)
    {
        ValidateUnidade(nome, numeroVagas);

        Nome = nome;
        NumeroVagas = numeroVagas;
       // CondominioId = condominioId;

        AddDomainEvent(new UnidadeCriadaEvent(Id, Nome, NumeroVagas));
    }

    // Métodos de domínio
    public void AlterarNumeroVagas(int novoNumeroVagas)
    {
        if (novoNumeroVagas < 0)
            throw new Exception("Número de vagas não pode ser negativo");

        if (novoNumeroVagas == NumeroVagas)
            return;

        NumeroVagas = novoNumeroVagas;
       // AddDomainEvent(new UnidadeVagasAlteradasEvent(Id, Nome, NumeroVagas));
    }

    //public bool EstaAtiva()
    //{
    //    return Ativa == true; // retorna false se Ativa for null ou false
    //}
    public void AlterarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new Exception("Nome da unidade não pode ser vazio");

        if (novoNome.Length > 100)
            throw new Exception("Nome da unidade não pode exceder 100 caracteres");

        if (novoNome == Nome)
            return;

        Nome = novoNome.Trim();
       // AddDomainEvent(new UnidadeNomeAlteradoEvent(Id, Nome));
    }

    public void AtribuirAoCondominio(int condominioId)
    {
        if (condominioId <= 0)
            throw new Exception("ID do condomínio deve ser maior que zero");

      //  CondominioId = condominioId;
      //  AddDomainEvent(new UnidadeCondominioAlteradoEvent(Id, CondominioId.Value));
    }

    public void RemoverDoCondominio()
    {
       // CondominioId = null;
      //  AddDomainEvent(new UnidadeCondominioRemovidoEvent(Id));
    }

    public bool TemVagasDisponiveis(int vagasOcupadas)
    {
        return vagasOcupadas < NumeroVagas;
    }

    public int VagasDisponiveis(int vagasOcupadas)
    {
        return Math.Max(0, NumeroVagas - vagasOcupadas);
    }

    // Validações
    private void ValidateUnidade(string nome, int numeroVagas)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome da unidade não pode ser vazio");

        if (nome.Length > 100)
            throw new Exception("Nome da unidade não pode exceder 100 caracteres");

        if (numeroVagas < 0)
            throw new Exception("Número de vagas não pode ser negativo");

        if (numeroVagas > 100)
            throw new Exception("Número de vagas não pode exceder 100");
    }
}