using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Infrastructure.Repositories;

public class VisitaSaiuSemControleRepository : IVisitaSaiuSemControleRepository
{
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<VisitaSaiuSemControleRepository> _logger;

    public VisitaSaiuSemControleRepository(
        AppDbContextFirebird contextFirebird,
        ILogger<VisitaSaiuSemControleRepository> logger)
    {
        _contextFirebird = contextFirebird;
        _logger = logger;
    }

    public async Task<bool> SalvarRegistroSaidaAsync(VisitaSaiuSemControle registro)
    {
        _logger.LogInformation("Salvando registro de saída sem controle para visitante: {Nome}", registro.Nome);

        try
        {
           
            var registroParaSalvar = new VisitaSaiuSemControle
            {
               
                IdCadVisita = registro.IdCadVisita,
                UnidadeDestino = registro.UnidadeDestino,
                Cartao = registro.Cartao,
                Placa = registro.Placa,
                Marca = registro.Marca,
                Modelo = registro.Modelo,
                Cor = registro.Cor,
                TipoVisitante = registro.TipoVisitante,
                Foto1 = registro.Foto1,
                Foto2 = registro.Foto2,
                Foto3 = registro.Foto3,
                Foto4 = registro.Foto4,
                PorteiroEntrada = registro.PorteiroEntrada,
                PorteiroProrrogou = registro.PorteiroProrrogou,
                PorteiroSaida = registro.PorteiroSaida,
                Nome = registro.Nome,
                Documento = registro.Documento,
                EmpresaVisitante = registro.EmpresaVisitante,
                DataHoraPrevisaoSaida = registro.DataHoraPrevisaoSaida,
                DataHoraSaida = registro.DataHoraSaida,
                DataHoraEntrada = registro.DataHoraEntrada,
                TempoPermanencia = registro.TempoPermanencia,
                TempoProrrogacao = registro.TempoProrrogacao,
                AutorizadoPor = registro.AutorizadoPor,
                Obs = registro.Obs,
                AgendadoPor = registro.AgendadoPor,
                DataCadAgendamento = registro.DataCadAgendamento,
                DataVisitaAgendada = registro.DataVisitaAgendada,
                AnuncioVisitaAgendada = registro.AnuncioVisitaAgendada,
                HoraVisitaAgendada = registro.HoraVisitaAgendada,
                Telefone = registro.Telefone,
                Email = registro.Email,
                VagaOcupada = registro.VagaOcupada,
                Cpf = registro.Cpf,
              
            };

            _contextFirebird.VisitaSaiuSemControle.Add(registroParaSalvar);
            int rowsAffected = await _contextFirebird.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("✅ Registro de saída salvo com sucesso. ID: {Id}, Visitante: {Nome}",
                    registroParaSalvar.Id, registroParaSalvar.Nome);
                return true;
            }
            else
            {
                _logger.LogWarning("Nenhuma linha afetada ao salvar registro de saída para: {Nome}", registro.Nome);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao salvar registro de saída para visitante: {Nome}", registro.Nome);
            throw;
        }
    }
}