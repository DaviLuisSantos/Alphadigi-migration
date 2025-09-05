using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using Alphadigi_migration.Domain.ValueObjects;


namespace Alphadigi_migration.Domain.EntitiesNew
{
    public class MensagemDisplay : EntityBase, IAggregateRoot
    {
        // Propriedades
        public PlacaVeiculo Placa { get; private set; }
        public string Mensagem { get; private set; }
        public DateTime DataHora { get; private set; }
        public int AlphadigiId { get; private set; }

        public Alphadigi Alphadigi { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }
        public bool Exibida { get; private set; }
        public int Prioridade { get; private set; }

        // Construtores
        protected MensagemDisplay() { } // Para ORM

        public MensagemDisplay(
            string placa,
            string mensagem,
            int alphadigiId,
            DateTime dataHora,
            int prioridade = 1)
        {
            ValidarPlaca(placa);
            ValidarMensagem(mensagem);
            ValidarPrioridade(prioridade);

            Placa = new PlacaVeiculo(placa);
            Mensagem = mensagem;
            AlphadigiId = alphadigiId;
            DataHora = dataHora;
            Prioridade = prioridade;
            Exibida = false;
            DataCriacao = DateTime.UtcNow;

            AddDomainEvent(new MensagemDisplayCriadaEvent(Id, Placa.Numero, Mensagem, AlphadigiId));
        }

        // Métodos de Domínio
        public void AtualizarMensagem(string novaMensagem)
        {
            ValidarMensagem(novaMensagem);

            Mensagem = novaMensagem;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new MensagemDisplayAtualizadaEvent(Id, Placa.Numero, novaMensagem));
        }

        public void MarcarComoExibida()
        {
            if (Exibida) return;

            Exibida = true;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new MensagemDisplayExibidaEvent(Id, Placa.Numero, Mensagem));
        }

        public void AlterarPrioridade(int novaPrioridade)
        {
            ValidarPrioridade(novaPrioridade);

            Prioridade = novaPrioridade;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new MensagemDisplayPrioridadeAlteradaEvent(Id, Placa.Numero, novaPrioridade));
        }

        public void Reagendar(DateTime novaDataHora)
        {
            if (novaDataHora < DateTime.UtcNow)
                throw new Exception("Data/hora não pode ser no passado");

            DataHora = novaDataHora;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new MensagemDisplayReagendadaEvent(Id, Placa.Numero, novaDataHora));
        }

        // Métodos de Validação
        private void ValidarPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new Exception("Placa não pode ser vazia");
        }

        private void ValidarMensagem(string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
                throw new Exception("Mensagem não pode ser vazia");

            if (mensagem.Length > 200)
                throw new Exception("Mensagem não pode exceder 200 caracteres");
        }

        private void ValidarPrioridade(int prioridade)
        {
            if (prioridade < 1 || prioridade > 10)
                throw new Exception("Prioridade deve estar entre 1 e 10");
        }

        // Métodos de consulta
        public bool DeveSerExibida() => !Exibida && DataHora <= DateTime.UtcNow;
        public bool EhPrioritaria() => Prioridade >= 8;
        public bool ContemTermo(string termo) => Mensagem.Contains(termo, StringComparison.OrdinalIgnoreCase);

        public override string ToString()
        {
            return $"{Placa} - {DataHora:dd/MM/yyyy HH:mm} - {Mensagem}";
        }
    }
}