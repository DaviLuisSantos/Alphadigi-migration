using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

    public interface IMensagemDisplayService
    {
        Task<bool> SaveMensagemDisplayAsync(MensagemDisplay mensagem);
        Task<MensagemDisplay> FindLastMensagemAsync(FindLastMessage termo);
        Task<MensagemDisplay> FindLastCamMensagemAsync(int alphadigiId);
    }

