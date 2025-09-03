using Alphadigi_migration.Domain.Common;
using Alphadigi_migration.Domain.Events;
using Alphadigi_migration.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.EntitiesNew;

public class Veiculo : EntityBase, IAggregateRoot
{
    // Propriedades
    public string Unidade { get; private set; }
    public PlacaVeiculo Placa { get; private set; }
    public Unidade UnidadeNavigation{ get; private set; }
    public Rota Rota { get; private set; }


    public string Marca { get; private set; }
    public string Modelo { get; private set; }
    public string Cor { get; private set; }
    public bool VeiculoDentro { get; private set; }
    public string IpCamUltAcesso { get; private set; }
    public DateTime? DataHoraUltAcesso { get; private set; }
    public int? IdRota { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Construtores
    protected Veiculo() { } // Para ORM

    public Veiculo(string placa, string unidade, string marca, string modelo, string cor)
    {
        ValidarPlaca(placa);
        ValidarUnidade(unidade);
        ValidarMarcaModelo(marca, modelo);
        ValidarCor(cor);

        Placa = new PlacaVeiculo(placa);
        Unidade = unidade;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        VeiculoDentro = false;
        DataCriacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoCriadoEvent(Id, Placa.Numero, Unidade));
    }
    public Veiculo(string placa)
    {
        Placa = new PlacaVeiculo(placa);
    }

    // Métodos de Domínio
    public void AtualizarInformacoes(string marca, string modelo, string cor, string unidade)
    {
        ValidarMarcaModelo(marca, modelo);
        ValidarCor(cor);
        ValidarUnidade(unidade);

        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        Unidade = unidade;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoAtualizadoEvent(Id, Placa.Numero));
    }

  

    public void RegistrarAcesso(string ipCamera, DateTime dataHoraAcesso)
    {
        ValidarIpCamera(ipCamera);

        IpCamUltAcesso = ipCamera;
        DataHoraUltAcesso = dataHoraAcesso;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoAcessoRegistradoEvent(Id, Placa.Numero, ipCamera, dataHoraAcesso));
    }

    public void EntrarCondominio(string ipCamera)
    {
        ValidarIpCamera(ipCamera);

        VeiculoDentro = true;
        IpCamUltAcesso = ipCamera;
        DataHoraUltAcesso = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoEntrouEvent(Id, Placa.Numero, ipCamera));
    }

    public void SairCondominio(string ipCamera)
    {
        ValidarIpCamera(ipCamera);

        VeiculoDentro = false;
        IpCamUltAcesso = ipCamera;
        DataHoraUltAcesso = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoSaiuEvent(Id, Placa.Numero, ipCamera));
    }

    public void AtualizarRota(int? idRota)
    {
        IdRota = idRota;
        DataAtualizacao = DateTime.UtcNow;

        AddDomainEvent(new VeiculoRotaAtualizadaEvent(Id, Placa.Numero, idRota));
    }

    // Métodos de Validação
    private void ValidarPlaca(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            throw new Exception("Placa não pode ser vazia");

        if (placa.Length > 8)
            throw new Exception("Placa não pode exceder 8 caracteres");
    }

    private void ValidarUnidade(string unidade)
    {
        if (string.IsNullOrWhiteSpace(unidade))
            throw new Exception("Unidade não pode ser vazia");
    }

    private void ValidarMarcaModelo(string marca, string modelo)
    {
        if (string.IsNullOrWhiteSpace(marca))
            throw new Exception("Marca não pode ser vazia");

        if (string.IsNullOrWhiteSpace(modelo))
            throw new Exception("Modelo não pode ser vazio");

        if (marca.Length > 50)
            throw new Exception("Marca não pode exceder 50 caracteres");

        if (modelo.Length > 50)
            throw new Exception("Modelo não pode exceder 50 caracteres");
    }

    private void ValidarCor(string cor)
    {
        if (string.IsNullOrWhiteSpace(cor))
            throw new Exception("Cor não pode ser vazia");

        if (cor.Length > 30)
            throw new Exception("Cor não pode exceder 30 caracteres");
    }

    private void ValidarIpCamera(string ipCamera)
    {
        if (string.IsNullOrWhiteSpace(ipCamera))
            throw new Exception("IP da câmera não pode ser vazio");

        if (!System.Net.IPAddress.TryParse(ipCamera, out _))
            throw new Exception("Formato de IP inválido");
    }
}
