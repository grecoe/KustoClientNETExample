﻿namespace KustoExample.Kusto
{
    using Azure.Core;
    using System.Text;
    using global::Kusto.Data.Common;
    using global::Kusto.Data;
    using global::Kusto.Ingest;

    internal class KustoIngest : IDisposable
    {
        private readonly TokenCredential Credential;
        private readonly KustoConnectionStringBuilder KustoCSB;
        private bool Dispposed { get; set; } = false;
        private KustoSettings Settings { get; set; }
        private IKustoIngestClient KustoIngestClient { get; set; }
        private KustoIngestionProperties IngestProperties { get; set; }

        public KustoIngest(
            TokenCredential tokenCredential,
            KustoSettings ingestSettings
            )
        {
            this.Settings = ingestSettings;
            this.Credential = tokenCredential;

            this.KustoCSB = new KustoConnectionStringBuilder(
                this.Settings.IngestEndpoint,
                this.Settings.Database)
                .WithAadAzureTokenCredentialsAuthentication(this.Credential);

            this.KustoIngestClient = KustoIngestFactory.CreateStreamingIngestClient(this.KustoCSB);
            this.IngestProperties = new KustoIngestionProperties(
                this.Settings.Database,
                this.Settings.Table)
            {
                Format = DataSourceFormat.json,
                IngestionMapping = new IngestionMapping
                {
                    IngestionMappingKind = global::Kusto.Data.Ingestion.IngestionMappingKind.Json
                }
            };
        }

        public void StreamRecords(List<IKustoRecord> records)
        {
            using (Stream inputStream = CreateLogStream(records))
            {
                using (var ingestClient = KustoIngestFactory.CreateStreamingIngestClient(this.KustoCSB))
                {
                    this.KustoIngestClient.IngestFromStream(inputStream, this.IngestProperties);
                }
            }
        }

        private static Stream CreateLogStream(List<IKustoRecord> records)
        {
            var ms = new MemoryStream();
            using (var tw = new StreamWriter(ms, Encoding.UTF8, 4096, true))
            {
                foreach (IKustoRecord record in records)
                {
                    tw.WriteLine(record.GetEntity());
                }
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }


        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.Dispposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.KustoIngestClient != null)
                {
                    this.KustoIngestClient.Dispose();
                }
            }

            this.Dispposed = true;
        }

    }
}
