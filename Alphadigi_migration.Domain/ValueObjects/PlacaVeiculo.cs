namespace Alphadigi_migration.Domain.ValueObjects
{
    public record PlacaVeiculo
    {
        public string Numero { get; }

        private PlacaVeiculo() { }
        public PlacaVeiculo(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("Placa não pode ser vazia");

            if (numero.Length > 8)
                throw new ArgumentException("Placa não pode exceder 8 caracteres");

            // Validação de formato de placa (Mercosul ou antigo)
            if (!IsValidPlacaFormat(numero))
                throw new ArgumentException("Formato de placa inválido");

            Numero = numero.ToUpper().Replace(" ", "").Replace("-", "");
        }
        public PlacaVeiculo(PlacaVeiculo original)
        {
            Numero = original.Numero;
        }
        private bool IsValidPlacaFormat(string placa)
        {
            // Formato Mercosul: AAA0A00
            // Formato antigo: AAA0000
            var mercosulPattern = @"^[A-Z]{3}[0-9][A-Z][0-9]{2}$";
            var oldPattern = @"^[A-Z]{3}[0-9]{4}$";

            var normalized = placa.ToUpper().Replace(" ", "").Replace("-", "");

            return System.Text.RegularExpressions.Regex.IsMatch(normalized, mercosulPattern) ||
                   System.Text.RegularExpressions.Regex.IsMatch(normalized, oldPattern);
        }

        public override string ToString()
        {
            if (Numero.Length == 7)
            {
                // Formatar para exibição: AAA-0A00 ou AAA-0000
                return $"{Numero.Substring(0, 3)}-{Numero.Substring(3)}";
            }
            return Numero;
        }

        public static implicit operator string(PlacaVeiculo placa) => placa.ToString();
    }
}