namespace KustoExample
{
    using AutoMapper;
    using AutoMapper.Data;
    using Azure.Core;
    using Azure.Identity;
    using KustoExample.Kusto;
    using System.Data;

    internal class Program
    {
        static void Main(string[] args)
        {
            /////////////////////////////////////////////////////////////////////////////////////
            // You'll need to be logged into Azure for this to work
            TokenCredential tokenCredential = new DefaultAzureCredential();

            /////////////////////////////////////////////////////////////////////////////////////
            // Settings, you need your cluster endpoint
            KustoSettings settings = new KustoSettings(
                "https://ingest-[_YOUR_ADX_].eastus.kusto.windows.net",
                 "https://[_YOUR_ADX_].eastus.kusto.windows.net",
                "TestADX",
                "EventLog"
                );
            
            /////////////////////////////////////////////////////////////////////////////////////
            // Need to map the data we will read from the cluster, all of them.
            AutoMapper.IConfigurationProvider configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddDataReaderMapping();
                cfg.CreateMap<IDataRecord, EventLogItemDTO>();
            });
            AutoMapper.IMapper mapper = new Mapper(configuration);

            /////////////////////////////////////////////////////////////////////////////////////
            // Using on ingestor, intentional because if we need it >1 it should have the client 
            // loaded
            using (KustoIngest ingestor = new KustoIngest(tokenCredential, settings))
            {
                KustoReader reader = new KustoReader(tokenCredential, mapper, settings.ReadEndpoint, settings.Database);

                // Now let's generate some messages
                Console.WriteLine("Writing 10 events to Kusto");
                List<IKustoRecord> logEvents = new List<IKustoRecord>();
                for (int i = 0; i < 10; i++)
                {
                    logEvents.Add(new EventLogItemDTO() { Message = $"Message {i}" });
                }

                // Load them into kusto, the first time this is used, connections are 
                // made and it takes time. 
                ingestor.StreamRecords(logEvents);

                List<EventLogItemDTO> eventRecords = reader.ReadData<EventLogItemDTO>(settings.Table);
                Console.WriteLine($"There are currently {eventRecords.Count} records in Kusto");
            }
        }
    }
}
