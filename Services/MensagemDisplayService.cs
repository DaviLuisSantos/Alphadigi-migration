using Alphadigi_migration.DTO.Display;
using Alphadigi_migration.Models;
using Alphadigi_migration.Repositories;

namespace Alphadigi_migration.Services;

public class MensagemDisplayService
{
    private readonly MensagemDisplayRepository _mensagemDisplayRepository;
    public MensagemDisplayService(MensagemDisplayRepository mensagemDisplayRepository)
    {
        _mensagemDisplayRepository = mensagemDisplayRepository;
    }
    public async Task<bool> SaveMensagemDisplay(MensagemDisplay mensagem)
    {
        return await _mensagemDisplayRepository.SaveMensagemDisplay(mensagem);
    }
    public async Task<MensagemDisplay> FindLastMensagem(FindLastMessage termo)
    {
        return await _mensagemDisplayRepository.FindLastMensagem(termo);
    }
    public async Task<MensagemDisplay> FindLastCamMensagem(int alphadigiId)
    {
        return await _mensagemDisplayRepository.FindLastCamMensagem(alphadigiId);
    }
}
