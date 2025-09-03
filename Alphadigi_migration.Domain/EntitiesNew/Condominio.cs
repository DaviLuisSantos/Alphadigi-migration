using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using Alphadigi_migration.Domain.ValueObjects;

namespace Alphadigi_migration.Domain.EntitiesNew;

    public class Condominio : EntityBase, IAggregateRoot
    {
        // Propriedades
        public string Nome { get; private set; }
        public Cnpj Cnpj { get; private set; }
        public string Fantasia { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public bool Ativo { get; private set; }

        // Construtores
        protected Condominio() { } // Para ORM

        public Condominio(string nome, string cnpj, string fantasia = null)
        {
            ValidarNome(nome);
            ValidarFantasia(fantasia);

            Nome = nome;
            Cnpj = new Cnpj(cnpj);
            Fantasia = fantasia;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;

            AddDomainEvent(new CondominioCriadoEvent(Id, Nome, Cnpj.Numero));
        }

        // Métodos de Domínio
        public void AtualizarInformacoes(string nome, string fantasia)
        {
            ValidarNome(nome);
            ValidarFantasia(fantasia);

            Nome = nome;
            Fantasia = fantasia;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new CondominioAtualizadoEvent(Id, Nome));
        }

        public void AtualizarCnpj(string novoCnpj)
        {
            var cnpj = new Cnpj(novoCnpj);

            Cnpj = cnpj;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new CondominioCnpjAtualizadoEvent(Id, Nome, cnpj.Numero));
        }

        public void Ativar()
        {
            if (Ativo) return;

            Ativo = true;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new CondominioAtivadoEvent(Id, Nome));
        }

        public void Desativar(string motivo = "Desativado pelo sistema")
        {
            if (!Ativo) return;

            Ativo = false;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new CondominioDesativadoEvent(Id, Nome, motivo));
        }
    public void AtualizarInformacoesCompleto(string nome, string cnpj, string fantasia)
    {
        ValidarNome(nome);
        ValidarFantasia(fantasia);

        Nome = nome;
        Cnpj = new Cnpj(cnpj); // Usa o Value Object Cnpj
        Fantasia = fantasia;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CondominioAtualizadoEvent(Id, Nome));
    }

    // Método para atualização parcial (se necessário)
    public void AtualizarInformacoesBasicas(string nome, string fantasia)
    {
        ValidarNome(nome);
        ValidarFantasia(fantasia);

        Nome = nome;
        Fantasia = fantasia;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new CondominioAtualizadoEvent(Id, Nome));
    }

    // Métodos de Validação
    private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("Nome do condomínio não pode ser vazio");

            if (nome.Length > 100)
                throw new Exception("Nome do condomínio não pode exceder 100 caracteres");
        }

        private void ValidarFantasia(string fantasia)
        {
            if (fantasia != null && fantasia.Length > 100)
                throw new Exception("Nome fantasia não pode exceder 100 caracteres");
        }

        public override string ToString()
        {
            return $"{Nome} ({Fantasia}) - CNPJ: {Cnpj}";
        }
    }
