using Alphadigi_migration.Domain.Common;

public class AlphadigiAreaAtualizadaEvent : DomainEvent
{
    public override string EventType => "AlphadigiAreaAtualizada";
    public override Guid AggregateId => AlphadigiId;

    public Guid AlphadigiId { get; }
    public string NomeAlphadigi { get; }
    public int AreaId { get; }
    public string NomeArea { get; }

    public AlphadigiAreaAtualizadaEvent(Guid alphadigiId, 
                                        string nomeAlphadigi, 
                                      
                                        string nomeArea)
    {
        AlphadigiId = alphadigiId;
        NomeAlphadigi = nomeAlphadigi;
       
        NomeArea = nomeArea;
    }
}