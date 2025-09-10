using MediatR;


namespace Alphadigi_migration.Application.Queries.QueryResult
{
    public class GetVagasQuery : IRequest<Domain.ValueObjects.QueryResult>
    {
        public int? CondominioId { get; set; }
        public int? AreaId { get; set; }
        public DateTime? DataReferencia { get; set; }

        public GetVagasQuery(int? condominioId = null, 
                             int? areaId = null, 
                             DateTime? dataReferencia = null)
        {
            CondominioId = condominioId;
            AreaId = areaId;
            DataReferencia = dataReferencia;
        }
    }
}
