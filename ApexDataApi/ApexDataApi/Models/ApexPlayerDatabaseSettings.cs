namespace ApexDataApi.Models
{
    public class ApexPlayerDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string PlayersCollectionName { get; set; } = null!;
        public string CharactersCollectionName { get; set; } = null!;
    }
}
