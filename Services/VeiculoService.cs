using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;
using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services;

public class VeiculoService
{
    private readonly AppDbContextFirebird _context;
    public VeiculoService(AppDbContextFirebird context)
    {
        _context = context;
    }
    public async Task<List<Veiculo>> GetVeiculos()
    {
        return await _context.Veiculos.ToListAsync();
    }
    public async Task<Veiculo> GetVeiculo(int id)
    {
        return await _context.Veiculos.FindAsync(id);
    }
    public async Task<Veiculo> AddVeiculo(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();
        return veiculo;
    }
    public async Task<Veiculo> UpdateVeiculo(int id,Veiculo veiculo)
    {
        if (id != veiculo.Id)
        {
            throw new Exception("Id do veículo não confere");
        }
        _context.Entry(veiculo).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return veiculo;
    }
    public async Task DeleteVeiculo(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo == null)
        {
            throw new Exception("Veículo não encontrado");
        }
        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();
    }
}
