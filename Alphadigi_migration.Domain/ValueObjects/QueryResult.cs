

using Alphadigi_migration.Domain.Common;

namespace Alphadigi_migration.Domain.ValueObjects;

public class QueryResult : ValueObject
{
    public int NumVagas { get; private set; }
    public int? VagasOcupadasVisitantes { get; private set; }
    public int VagasOcupadasMoradores { get; private set; }

    public QueryResult(int numVagas, int? vagasOcupadasVisitantes, int vagasOcupadasMoradores)
    {
        NumVagas = numVagas;
        VagasOcupadasVisitantes = vagasOcupadasVisitantes;
        VagasOcupadasMoradores = vagasOcupadasMoradores;
    }

    public int VagasDisponiveis =>
        NumVagas - (VagasOcupadasMoradores + (VagasOcupadasVisitantes ?? 0));

    public decimal TaxaOcupacao =>
        NumVagas > 0 ? (decimal)(VagasOcupadasMoradores + (VagasOcupadasVisitantes ?? 0)) / NumVagas : 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NumVagas;
        yield return VagasOcupadasVisitantes ?? -1; 
        yield return VagasOcupadasMoradores;
    }
}