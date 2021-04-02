using Nest;
using System.Threading.Tasks;

namespace SearchProject.Service
{
    public interface IJsonFileImportService
    {
        public Task<int> ImportJsonData(string fileName, IElasticClient elasticClient);
    }
}
