using Nest;
using Newtonsoft.Json;
using SearchProject.Infrastructure;
using SearchProject.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SearchProject.Service
{
    public class JsonFileImportService : IJsonFileImportService
    {
        public const string indexName = "news";
        public JsonFileImportService()
        {
        }
        public async Task<int> ImportJsonData(string fileName,IElasticClient elasticClient)
        {
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                List<object> items = JsonConvert.DeserializeObject<List<object>>(json);

                foreach (var item in items)
                {
                    var strItem = JsonConvert.SerializeObject(item);

                    List<News> doc1 = JsonConvert.DeserializeObject<List<News>>(strItem);

                    BulkInsertData data = new BulkInsertData(elasticClient);
                    await data.StoreBulk<News>(doc1, doc1.Count, indexName);
                }
                return items.Count;
            }
        }
    }
}
