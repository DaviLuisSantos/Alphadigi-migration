namespace Alphadigi_migration.DTO.Alphadigi;

public class AddWhiteListDTO
{
    public AddWhiteList AddWhiteList { get; set; }
}

public class AddWhiteList
{
    public List<AddData> add_data { get; set; }
}

public class AddData
{
    public string Carnum { get; set; }
    public string Startime { get; set; }
    public string Endtime { get; set; }
}
