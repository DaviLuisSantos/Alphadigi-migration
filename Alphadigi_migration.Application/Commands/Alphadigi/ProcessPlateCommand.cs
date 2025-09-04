using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class ProcessPlateCommand : IRequest<Object>
{
    public string Plate { get; set; }
    public string Ip { get; set; }
    public string PlateImage { get; set; }
    public string CarImage { get; set; }
    public string Modelo { get; set; }
    public bool IsCad { get; set; }
    public bool IsRealPlate { get; set; }
    public ProcessPlateCommand(
        string ip,
        string plate,
        bool isRealPlate,
        bool isCad,
        string carImage,
        string plateImage,
        string modelo)
    {
        Ip = ip;
        Plate = plate;
        IsRealPlate = isRealPlate;
        IsCad = isCad;
        CarImage = carImage;
        PlateImage = plateImage;
        Modelo = modelo;
    }

}
