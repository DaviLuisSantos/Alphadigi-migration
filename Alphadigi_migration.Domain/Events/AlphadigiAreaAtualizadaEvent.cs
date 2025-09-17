using Alphadigi_migration.Domain.Common;

public class AlphadigiAreaAtualizadaEvent : DomainEvent
{
    public override string EventType => "AlphadigiAreaAtualizada";
    public override int AggregateId => AlphadigiId;

    public int AlphadigiId { get; }
    public string NomeAlphadigi { get; }
    public int AreaId { get; }
    public string NomeArea { get; }

    public AlphadigiAreaAtualizadaEvent(int alphadigiId, 
                                        string nomeAlphadigi, 
                                      
                                        string nomeArea)
    {
        AlphadigiId = alphadigiId;
        NomeAlphadigi = nomeAlphadigi;
       
        NomeArea = nomeArea;
    }
}