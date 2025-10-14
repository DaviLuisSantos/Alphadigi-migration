using Alphadigi_migration.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.EntitiesNew
{
    [Table("VISITA_SAIU_E_SEM_CONTROLE")]
    public class VisitaSaiuSemControle
    {
        [Key]
        [Column("IDVISITA")]
        public int Id { get; set; }

        [Column("ID_CAD_VISITA")]
        public int? IdCadVisita { get; set; }

        [Column("UNIDADEDESTINO")]
        [MaxLength(70)]
        public string? UnidadeDestino { get; set; }

        [Column("CARTAOVISITA")]
        [MaxLength(20)]
        public string? Cartao { get; set; }

        [Column("PLACAVISITANTE")]
        [MaxLength(8)]
        public PlacaVeiculo Placa { get; set; }

        [Column("MARCACARROVISITA")]
        [MaxLength(30)]
        public string? Marca { get; set; }

        [Column("MODELOCARROVISITA")]
        [MaxLength(20)]
        public string? Modelo { get; set; }

        [Column("CORCARROVISITA")]
        [MaxLength(12)]
        public string? Cor { get; set; }

        [Column("TIPOVISITANTE")]
        [MaxLength(40)]
        public string? TipoVisitante { get; set; }

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

        [Column("NOMEVISITANTE")]
        [MaxLength(60)]
        public string? Nome { get; set; }

        [Column("DOCUMENTOVISITA")]
        [MaxLength(20)]
        public string? Documento { get; set; }

        [Column("EMPRESAVISITANTE")]
        [MaxLength(50)]
        public string? EmpresaVisitante { get; set; }

        [Column("DATAHORAPREVISAOSAIDA")]
        public DateTime? DataHoraPrevisaoSaida { get; set; }

        [Column("DATAHORASAIDA")]
        public DateTime? DataHoraSaida { get; set; }

        [Column("DATAHORAENTRADA")]
        public DateTime? DataHoraEntrada { get; set; }

        [Column("TEMPOPERMANENCIA")]
        public int? TempoPermanencia { get; set; }

        [Column("TEMPO_PRORROGACAO")]
        public int? TempoProrrogacao { get; set; }

        [Column("AUTORIZADOPOR")]
        [MaxLength(60)]
        public string? AutorizadoPor { get; set; }

        [Column("OBS")]
        [MaxLength(512)]
        public string? Obs { get; set; }

        [Column("AGENDADO_POR")]
        [MaxLength(60)]
        public string? AgendadoPor { get; set; }

        [Column("DATA_CAD_AGENDAMENTO")]
        public DateTime? DataCadAgendamento { get; set; }

        [Column("DATA_VISITA_AGENDADA")]
        public DateTime? DataVisitaAgendada { get; set; }

        [Column("ANUNCIO_VISITA_AGENDADA")]
        [MaxLength(3)]
        public string? AnuncioVisitaAgendada { get; set; }

        [Column("HORA_VISITA_AGENDADA")]
        public TimeSpan? HoraVisitaAgendada { get; set; }

        [Column("TELEFONE")]
        [MaxLength(40)]
        public string? Telefone { get; set; }

        [Column("EMAIL")]
        [MaxLength(40)]
        public string? Email { get; set; }

        [Column("VAGA_OCUPADA")]
        [MaxLength(2)]
        public bool? VagaOcupada { get; set; }

        [Column("CPF")]
        [MaxLength(20)]
        public string? Cpf { get; set; }

    

        // Construtor padrão
        public VisitaSaiuSemControle() { }

        // Construtor para copiar dados do Visitante original
        public VisitaSaiuSemControle(Visitante visitante, string ipCameraSaida)
        {
            // Copia todos os dados do visitante original
            Id = visitante.Id;
            IdCadVisita = visitante.IdCadVisita;
            UnidadeDestino = visitante.UnidadeDestino;
            Cartao = visitante.Cartao;
            Placa = visitante.Placa;
            Marca = visitante.Marca;
            Modelo = visitante.Modelo;
            Cor = visitante.Cor;
            TipoVisitante = visitante.TipoVisitante;
            Foto1 = visitante.Foto1;
            Foto2 = visitante.Foto2;
            Foto3 = visitante.Foto3;
            Foto4 = visitante.Foto4;
            PorteiroEntrada = visitante.PorteiroEntrada;
            PorteiroProrrogou = visitante.PorteiroProrrogou;
            PorteiroSaida = visitante.PorteiroSaida;
            Nome = visitante.Nome;
            Documento = visitante.Documento;
            EmpresaVisitante = visitante.EmpresaVisitante;
            DataHoraPrevisaoSaida = visitante.DataHoraPrevisaoSaida;
            DataHoraSaida = visitante.DataHoraSaida;
            DataHoraEntrada = visitante.DataHoraEntrada;
            TempoPermanencia = visitante.TempoPermanencia;
            TempoProrrogacao = visitante.TempoProrrogacao;
            AutorizadoPor = visitante.AutorizadoPor;
            Obs = visitante.Obs;
            AgendadoPor = visitante.AgendadoPor;
            DataCadAgendamento = visitante.DataCadAgendamento;
            DataVisitaAgendada = visitante.DataVisitaAgendada;
            AnuncioVisitaAgendada = visitante.AnuncioVisitaAgendada;
            HoraVisitaAgendada = visitante.HoraVisitaAgendada;
            Telefone = visitante.Telefone;
            Email = visitante.Email;
            VagaOcupada = visitante.VagaOcupada;
            Cpf = visitante.Cpf;

          
        }

        // Método para calcular tempo de permanência real
        public TimeSpan? CalcularTempoPermanenciaReal()
        {
            if (DataHoraEntrada.HasValue)
            {
                var saida = DataHoraSaida ?? DateTime.Now;
                return saida - DataHoraEntrada.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return $"{Nome} - {Placa} - {UnidadeDestino} - Entrada: {DataHoraEntrada:dd/MM/yyyy HH:mm} - Saída: {DataHoraSaida:dd/MM/yyyy HH:mm}";
        }
    }
}