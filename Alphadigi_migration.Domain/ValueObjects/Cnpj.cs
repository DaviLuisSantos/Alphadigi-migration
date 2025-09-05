namespace Alphadigi_migration.Domain.ValueObjects;

public record Cnpj
{
    public string Numero { get; }

    private Cnpj() { }

    public Cnpj(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CNPJ não pode ser vazio");

        var cnpjLimpo = LimparCnpj(numero);

        if (!ValidarCnpj(cnpjLimpo))
            throw new ArgumentException("CNPJ inválido");

        Numero = cnpjLimpo;
    }
    public Cnpj(Cnpj original)
    {
        Numero = original.Numero;
    }

    private string LimparCnpj(string cnpj)
    {
        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    private bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        // Implementar validação completa do CNPJ aqui
        // (dígitos verificadores, etc.)
        return true;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Numero) || Numero.Length != 14)
            return Numero;

        return $"{Numero.Substring(0, 2)}.{Numero.Substring(2, 3)}.{Numero.Substring(5, 3)}/{Numero.Substring(8, 4)}-{Numero.Substring(12)}";
    }

    public static implicit operator string(Cnpj cnpj) => cnpj.ToString();
}