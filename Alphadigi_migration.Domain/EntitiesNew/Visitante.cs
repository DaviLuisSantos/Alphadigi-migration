using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.EntitiesNew
{
    [Table("VISITANTES")]
    public class Visitante : EntityBase, IAggregateRoot
    {
        [Key]
        [Column("IDVISITA")]
        public override int Id { get; protected set; }

        [Column("ID_CAD_VISITA")]
        public int? IdCadVisita { get; private set; }

        [Column("UNIDADEDESTINO")]
        [MaxLength(70)]
        public string? UnidadeDestino { get; private set; }

        [Column("PLACAVISITANTE")]
        public PlacaVeiculo Placa { get; private set; }

        [Column("MARCACARROVISITA")]
        [MaxLength(30)]
        public string? Marca { get; private set; }

        [Column("MODELOCARROVISITA")]
        [MaxLength(20)]
        public string? Modelo { get; private set; }

        [Column("CORCARROVISITA")]
        [MaxLength(12)]
        public string? Cor { get; private set; }

        [Column("TIPOVISITANTE")]
        [MaxLength(40)]
        public string? TipoVisitante { get; private set; }

        [Column("NOMEVISITANTE")]
        [MaxLength(60)]
        public string? Nome { get; private set; }

        [Column("DOCUMENTOVISITA")]
        [MaxLength(20)]
        public string? Documento { get; private set; }

        [Column("CPF")]
        [MaxLength(20)]
        public string? Cpf { get; private set; }

        [Column("EMPRESAVISITANTE")]
        [MaxLength(50)]
        public string? EmpresaVisitante { get; private set; }

        [Column("TELEFONE")]
        [MaxLength(40)]
        public string? Telefone { get; private set; }

        [Column("EMAIL")]
        [MaxLength(40)]
        public string? Email { get; private set; }

        [Column("DATAHORAENTRADA")]
        public DateTime? DataHoraEntrada { get; private set; }

        [Column("DATAHORAPREVISAOSAIDA")]
        public DateTime? DataHoraPrevisaoSaida { get; private set; }

        [Column("DATAHORASAIDA")]
        public DateTime? DataHoraSaida { get; private set; }

        [Column("DATA_VISITA_AGENDADA")]
        public DateTime? DataVisitaAgendada { get; private set; }

        [Column("HORA_VISITA_AGENDADA")]
        public TimeSpan? HoraVisitaAgendada { get; private set; }

        [Column("VAGA_OCUPADA")]
        [MaxLength(2)]
        public bool? VagaOcupada { get; private set; }

        [Column("AUTORIZADOPOR")]
        [MaxLength(60)]
        public string? AutorizadoPor { get; private set; }

        [Column("AGENDADO_POR")]
        [MaxLength(60)]
        public string? AgendadoPor { get; private set; }

        // Propriedades computadas para lógica de negócio
        public bool EstaDentroDoCondominio => DataHoraEntrada.HasValue && !DataHoraSaida.HasValue;

        public bool EstaAutorizadoParaHoje
        {
            get
            {
                var hoje = DateTime.Today;
                return DataVisitaAgendada?.Date == hoje &&
                       !DataHoraSaida.HasValue; // Não saiu ainda
            }
        }

        public TimeSpan? TempoPermanencia
        {
            get
            {
                if (DataHoraEntrada.HasValue)
                {
                    var saida = DataHoraSaida ?? DateTime.Now;
                    return saida - DataHoraEntrada.Value;
                }
                return null;
            }
        }

        // Construtor
        protected Visitante() { }

        // Novo construtor que aceita PlacaLida
        public Visitante(
            string nome,
            string placa,
            string unidadeDestino,
            DateTime? dataVisitaAgendada,
            string? tipoVisitante = null,
            string? marca = null,
            string? modelo = null,
            string? cor = null,
            string? documento = null,
            string? cpf = null,
            string? telefone = null,
            string? email = null)
        {
            ValidarNome(nome);
            ValidarPlaca(placa);
            ValidarUnidade(unidadeDestino);

            Nome = nome;
            Placa = new PlacaVeiculo(placa);
            UnidadeDestino = unidadeDestino;
            DataVisitaAgendada = dataVisitaAgendada;
            TipoVisitante = tipoVisitante;
            Marca = marca;
            Modelo = modelo;
            Cor = cor;
            Documento = documento;
            Cpf = cpf;
            Telefone = telefone;
            Email = email;
           // DataCadastro = DateTime.Now;
        }

        // Métodos de Domínio
        public void RegistrarEntrada(string porteiroEntrada = null)
        {
            if (DataHoraEntrada.HasValue)
                throw new Exception("Visitante já possui entrada registrada");

            DataHoraEntrada = DateTime.Now;

            // CORREÇÃO: Agora Placa é do tipo PlacaLida, precisamos acessar a propriedade correta
            // AddDomainEvent(new VisitanteEntradaRegistradaEvent(Id, Placa.Numero, UnidadeDestino));
        }

        public void RegistrarSaida(string porteiroSaida = null)
        {
            if (!DataHoraEntrada.HasValue)
                throw new Exception("Visitante não possui entrada registrada");

            if (DataHoraSaida.HasValue)
                throw new Exception("Visitante já possui saída registrada");

            DataHoraSaida = DateTime.Now;

            // CORREÇÃO: Agora Placa é do tipo PlacaLida, precisamos acessar a propriedade correta
            // AddDomainEvent(new VisitanteSaidaRegistradaEvent(Id, Placa.Numero, UnidadeDestino));
        }

        public bool ValidarAcessoPorPlaca(string placa)
        {
            if (Placa == null || string.IsNullOrEmpty(placa))
                return false;

            // CORREÇÃO: Agora Placa é do tipo PlacaLida, acessamos a propriedade Numero
            var placaVisitante = NormalizarPlaca(Placa.Numero);
            var placaInformada = NormalizarPlaca(placa);

            return placaVisitante == placaInformada &&
                   EstaAutorizadoParaHoje &&
                   !DataHoraSaida.HasValue;
        }

        // Adicione este método na classe Visitante
        public void RegistrarSaidaEExcluir(string porteiroSaida = null)
        {
            if (!DataHoraEntrada.HasValue)
                throw new Exception("Visitante não possui entrada registrada");

            if (DataHoraSaida.HasValue)
                throw new Exception("Visitante já possui saída registrada");

            DataHoraSaida = DateTime.Now;

            if (!string.IsNullOrEmpty(porteiroSaida))
            {
                // PorteiroSaida = porteiroSaida; // Se tiver essa coluna
            }

            // Marca para exclusão (soft delete) ou prepara para exclusão física
            //AddDomainEvent(new VisitanteSaiuEExcluirEvent(Id, Placa?.Numero, UnidadeDestino));
        }

        public void AtualizarDadosVeiculo(string marca, string modelo, string cor)
        {
            Marca = marca;
            Modelo = modelo;
            Cor = cor;
        }

        public void AtualizarInformacoesContato(string telefone, string email)
        {
            Telefone = telefone;
            Email = email;
        }

        public void AtualizarPlaca(PlacaVeiculo novaPlaca)
        {
            ValidarPlaca(novaPlaca);
            Placa = novaPlaca;
        }

        // Métodos de Validação
        private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("Nome do visitante não pode ser vazio");

            if (nome.Length > 60)
                throw new Exception("Nome não pode exceder 60 caracteres");
        }

        private void ValidarPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new Exception("Placa não pode ser vazia");
        }

        private void ValidarUnidade(string unidade)
        {
            if (string.IsNullOrWhiteSpace(unidade))
                throw new Exception("Unidade não pode ser vazia");

            if (unidade.Length > 70)
                throw new Exception("Unidade não pode exceder 70 caracteres");
        }

        private string NormalizarPlaca(string placa)
        {
            return placa?.Replace("-", "").Replace(" ", "").ToUpper();
        }

        public override string ToString()
        {
            // CORREÇÃO: Agora Placa é do tipo PlacaLida, acessamos a propriedade Numero
            return $"{Nome} - {Placa?.Numero} - {UnidadeDestino} - {DataVisitaAgendada:dd/MM/yyyy}";
        }
    }
}