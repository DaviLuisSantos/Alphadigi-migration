using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.EntitiesNew;


namespace Alphadigi_migration.Domain.Interfaces;

    public interface IMensagemDisplayService
    {
        Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem);
        Task<MensagemDisplay> FindLastMensagemAsync(FindLastMessage termo);
        Task<MensagemDisplay> FindLastCamMensagemAsync(Guid alphadigiId);
    }

