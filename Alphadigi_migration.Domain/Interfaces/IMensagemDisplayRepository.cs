using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Models;


namespace Alphadigi_migration.Domain.Interfaces;

public interface IMensagemDisplayRepository
{
    Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem);
    Task<MensagemDisplay?> FindLastMensagemAsync(FindLastMessage termo);
    Task<MensagemDisplay?> FindLastCamMensagemAsync(int alphadigiId);
}
