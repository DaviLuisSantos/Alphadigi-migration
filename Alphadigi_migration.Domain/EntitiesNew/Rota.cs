

using Alphadigi_migration.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.EntitiesNew;
[Table("LPR_MT_ROTAS_CAM")]
public class Rota : EntityBase, IAggregateRoot
{
    public int RotaId { get; private set; }
    public int CameraId { get; private set; }

    // Navegação para a entidade Camera (opcional, se necessário)
    public virtual Camera Camera { get; private set; }

    // Construtor protegido para ORM
    protected Rota() { }

    // Construtor principal
    public Rota(int rotaId, int cameraId)
    {
        ValidateRota(rotaId, cameraId);

        RotaId = rotaId;
        CameraId = cameraId;
    }

    // Métodos de domínio
    public void UpdateCamera(int novaCameraId)
    {
        if (novaCameraId <= 0)
            throw new Exception("ID da câmera deve ser maior que zero");

        CameraId = novaCameraId;
        // Poderia adicionar um domain event aqui se necessário
        // AddDomainEvent(new RotaCameraAtualizadaEvent(Id, CameraId, novaCameraId));
    }

    public void UpdateRota(int novaRotaId)
    {
        if (novaRotaId <= 0)
            throw new Exception("ID da rota deve ser maior que zero");

        RotaId = novaRotaId;
    }

    // Validações
    private void ValidateRota(int rotaId, int cameraId)
    {
        if (rotaId <= 0)
            throw new Exception("ID da rota deve ser maior que zero");

        if (cameraId <= 0)
            throw new Exception("ID da câmera deve ser maior que zero");
    }
}