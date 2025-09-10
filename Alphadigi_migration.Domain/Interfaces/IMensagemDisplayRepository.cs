using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.EntitiesNew;
using System.Security.Cryptography;


namespace Alphadigi_migration.Domain.Interfaces;

public interface IMensagemDisplayRepository
{
    Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem);
    Task<MensagemDisplay?> FindLastMensagemAsync(string placa, string mensagem, int alphadigiId);
    Task<MensagemDisplay?> FindLastCamMensagemAsync(int alphadigiId);
}
