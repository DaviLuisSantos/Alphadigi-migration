using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using Alphadigi_migration.Domain.ValueObjects;

namespace Alphadigi_migration.Domain.EntitiesNew
{
    public class PlacaLida : EntityBase, IAggregateRoot
    {
        
        public PlacaVeiculo Placa { get; private set; }
        public string CarroImg { get; private set; }
        public string PlacaImg { get; private set; }
        public bool Liberado { get; private set; }
        public int AlphadigiId { get; private set; }
        public int AreaId { get; private set; }
        public Area Area { get; private set; }
        public Alphadigi Alphadigi { get; private set; }
        public DateTime DataHora { get; private set; }
        public bool Real { get; private set; }
        public bool Cadastrado { get; private set; }
        public bool Processado { get; private set; }
        public string Acesso { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

       
        protected PlacaLida() { } // Para ORM

        public PlacaLida(
            string placa,
            int alphadigiId,
            int areaId,
            DateTime dataHora,
            string carroImg = null,
            string placaImg = null,
            bool real = true,
            bool cadastrado = false,
            bool processado = false,
            string acesso = null)
        {
            ValidarPlaca(placa);
            ValidarImagens(carroImg, placaImg);

            Placa = new PlacaVeiculo(placa);
            AlphadigiId = alphadigiId;
            AreaId = areaId;
            DataHora = dataHora;
            CarroImg = carroImg;
            PlacaImg = placaImg;
            Real = real;
            Cadastrado = cadastrado;
            Processado = processado;
            Acesso = acesso;
            Liberado = false;
            DataCriacao = DateTime.UtcNow;

            AddDomainEvent(new PlacaLidaRegistradaEvent(Id, Placa.Numero, alphadigiId, dataHora));
        }

        
        public void MarcarComoProcessado(bool liberado, string acesso)
        {
            Processado = true;
            Liberado = liberado;
            Acesso = acesso;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new PlacaLidaProcessadaEvent(Id, Placa.Numero, liberado, acesso));
        }

        public void AtualizarCadastro(bool cadastrado)
        {
            Cadastrado = cadastrado;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new PlacaLidaCadastroAtualizadoEvent(Id, Placa.Numero, cadastrado));
        }

        public void AtualizarImagens(string carroImg, string placaImg)
        {
            ValidarImagens(carroImg, placaImg);

            CarroImg = carroImg;
            PlacaImg = placaImg;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new PlacaLidaImagensAtualizadasEvent(Id, Placa.Numero));
        }

        public void MarcarComoNaoReal()
        {
            Real = false;
            DataAtualizacao = DateTime.UtcNow;

            AddDomainEvent(new PlacaLidaMarcadaComoNaoRealEvent(Id, Placa.Numero));
        }

        // Métodos de Validação
        private void ValidarPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new Exception("Placa não pode ser vazia");
        }

        private void ValidarImagens(string carroImg, string placaImg)
        {
            if (carroImg != null && carroImg.Length > 500)
                throw new Exception("URL da imagem do carro muito longa");

            if (placaImg != null && placaImg.Length > 500)
                throw new Exception("URL da imagem da placa muito longa");
        }

        // Métodos de consulta
        public bool FoiProcessada() => Processado;
        public bool FoiLiberada() => Liberado;
        public bool VeiculoCadastrado() => Cadastrado;
        public bool LeituraReal() => Real;

        public override string ToString()
        {
            return $"{Placa} - {DataHora:dd/MM/yyyy HH:mm} - {(Liberado ? "LIBERADO" : "NEGADO")}";
        }
    }
}