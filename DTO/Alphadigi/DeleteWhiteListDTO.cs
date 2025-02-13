namespace Alphadigi_migration.DTO.Alphadigi;

public class DeleteWhiteListAllDTO
{
    public int DeleteWhiteListAll { get; set; }
}
public class DeleteWhiteListDTO
{
    public DeleteWhiteList DeleteWhiteList { get; set; }
}

public class DeleteWhiteList
{
    public List<DelData> DelData { get; set; }
}

public class DelData
{
    public string Carnum { get; set; }
}