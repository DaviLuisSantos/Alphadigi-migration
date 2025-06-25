using Newtonsoft.Json;

namespace Alphadigi_migration.DTO.Alphadigi;

public class AddWhiteListDTO
{
    [JsonProperty("addWhiteList")]
    public AddWhiteList AddWhiteList { get; set; }
}

public class AddWhiteList
{
    [JsonProperty("add_data")]
    public List<AddData> Add_data { get; set; }
}

public class AddData
{
    [JsonProperty("carnum")]
    public string Carnum { get; set; }
    [JsonProperty("startime")]
    public string Startime { get; set; }
    [JsonProperty("endtime")]
    public string Endtime { get; set; }
}
