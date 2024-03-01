namespace KustoExample.Kusto
{
    internal class KustoSettings
    {
        public string IngestEndpoint { get; set; } = string.Empty;
        public string ReadEndpoint { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Table { get; set; } = string.Empty;

        public KustoSettings(string ingestEndpoint, string readEndpoint, string database, string table)
        {
            this.IngestEndpoint = ingestEndpoint;
            this.ReadEndpoint = readEndpoint;
            this.Database = database;
            this.Table = table;
        }
    }
}
