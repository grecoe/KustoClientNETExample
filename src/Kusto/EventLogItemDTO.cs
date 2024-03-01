namespace KustoExample.Kusto
{
    internal class EventLogItemDTO : IKustoRecord
    {
        // Fields must be named the same as your columns
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;

        public string GetEntity()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
