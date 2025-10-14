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

        [Column("CARTAOVISITA")]
        [MaxLength(20)]
        public string? Cartao { get; set; }

        [Column("PLACAVISITANTE")]
        public PlacaVeiculo Placa { get; private set; }

        [Column("FOTO1")]
        [MaxLength(60)]
        public string? Foto1 { get; set; }

        [Column("FOTO2")]
        [MaxLength(60)]
        public string? Foto2 { get; set; }

        [Column("FOTO3")]
        [MaxLength(60)]
        public string? Foto3 { get; set; }

        [Column("FOTO4")]
        [MaxLength(60)]
        public string? Foto4 { get; set; }

        [Column("PORTEIROENTRADA")]
        [MaxLength(60)]
        public string? PorteiroEntrada { get; set; }

        [Column("PORTEIROPRORROGOU")]
        [MaxLength(60)]
        public string? PorteiroProrrogou { get; set; }

        [Column("PORTEIROSAIDA")]
        [MaxLength(60)]
        public string? PorteiroSaida { get; set; }

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

        [Column("TEMPOPERMANENCIA")]
        public int? TempoPermanencia { get; set; }

        [Column("TEMPO_PRORROGACAO")]
        public int? TempoProrrogacao { get; set; }

        [Column("DATAHORAPREVISAOSAIDA")]
        public DateTime? DataHoraPrevisaoSaida { get; private set; }

        [Column("DATAHORASAIDA")]
        public DateTime? DataHoraSaida { get; private set; }

        [Column("DATA_VISITA_AGENDADA")]
        public DateTime? DataVisitaAgendada { get; private set; }

        [Column("ANUNCIO_VISITA_AGENDADA")]
        [MaxLength(3)]
        public string? AnuncioVisitaAgendada { get; set; }

        [Column("HORA_VISITA_AGENDADA")]
        public TimeSpan? HoraVisitaAgendada { get; private set; }

        [Column("VAGA_OCUPADA")]
        [MaxLength(2)]
        public bool? VagaOcupada { get; private set; }

        [Column("AUTORIZADOPOR")]
        [MaxLength(60)]
        public string? AutorizadoPor { get; private set; }

        [Column("OBS")]
        [MaxLength(512)]
        public string? Obs { get; set; }

        [Column("AGENDADO_POR")]
        [MaxLength(60)]
        public string? AgendadoPor { get; private set; }

        [Column("DATA_CAD_AGENDAMENTO")]
        public DateTime? DataCadAgendamento { get; set; }


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

    

        // Construtor
        protected Visitante() { }

        // Novo construtor que aceita PlacaLida


       

        public Visitante(int id, 
            int? idCadVisita, 
            string? unidadeDestino, 
            string? cartao, 
            PlacaVeiculo placa, 
            string? foto1, 
            string? foto2, 
            string? foto3, 
            string? foto4, 
            string? porteiroEntrada, 
            string? porteiroProrrogou, 
            string? porteiroSaida, 
            string? marca, 
            string? modelo, 
            string? cor, 
            string? tipoVisitante, 
            string? nome, 
            string? documento, 
            string? cpf, 
            string? empresaVisitante, 
            string? telefone, 
            string? email, 
            DateTime? dataHoraEntrada, 
            int? tempoPermanencia, 
            int? tempoProrrogacao, 
            DateTime? dataHoraPrevisaoSaida, 
            DateTime? dataHoraSaida, 
            DateTime? dataVisitaAgendada, 
            string? anuncioVisitaAgendada, 
            TimeSpan? horaVisitaAgendada, 
            bool? vagaOcupada, 
            string? autorizadoPor, 
            string? obs, 
            string? agendadoPor, 
            DateTime? dataCadAgendamento)
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