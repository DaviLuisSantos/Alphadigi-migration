using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Application.Service;

public class MensagemDisplayService : IMensagemDisplayService
{
    private readonly IMensagemDisplayRepository _mensagemDisplayRepository;
    public MensagemDisplayService(IMensagemDisplayRepository mensagemDisplayRepository)
    {
        _mensagemDisplayRepository = mensagemDisplayRepository;
    }
    public async Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem)
    {
        return await _mensagemDisplayRepository.SaveMensagemDisplayAsync(mensagem);
    }
    public async Task<MensagemDisplay> FindLastMensagemAsync(FindLastMessage termo)
    {
        return await _mensagemDisplayRepository.FindLastMensagemAsync(termo);
    }
    public async Task<MensagemDisplay> FindLastCamMensagemAsync(int alphadigiId)
    {
        return await _mensagemDisplayRepository.FindLastCamMensagemAsync(alphadigiId);
    }
}
