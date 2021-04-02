using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchProject.Infrastructure
{
    public class BulkInsertData
    {
        private readonly IElasticClient _elasticClient;
        public BulkInsertData(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public async Task StoreBulk<T>(IList<T> documents, int size, string indexName = "") where T : class
        {
            //string index = string.IsNullOrEmpty(indexName) ? GenerateIndexName<T>() : indexName;
            //await CreateIndex<T>(index);
            var bulkAllObservable = _elasticClient.BulkAll(documents, b => b
            .Index("news")
            // how long to wait between retries
            .BackOffTime("30s")
            // how many retries are attempted if a failure occurs
            .BackOffRetries(2)
            // refresh the index once the bulk operation completes
            .RefreshOnCompleted()
            // how many concurrent bulk requests to make
            .MaxDegreeOfParallelism(Environment.ProcessorCount)
            // number of items per bulk request
            .Size(size)
             )
                // Perform the indexing, waiting up to 15 minutes. 
                // Whilst the BulkAll calls are asynchronous this is a blocking operation
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    // do something on each response e.g. write number of batches indexed to console
                });


        }
    }
}
