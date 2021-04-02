using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchProject.Infrastructure;
using SearchProject.Model;
using SearchProject.Model.Command;
using SearchProject.Model.Query;
using SearchProject.ReadModel;
using SearchProject.Service;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SearchProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly IJsonFileImportService _jsonFileImportService;
        public const string indexName = "news";
        public string fileName = "newsArticles.json";
        public NewsController(IElasticClient elasticClient, IJsonFileImportService jsonFileImportService)
        {
            _elasticClient = elasticClient;
            _jsonFileImportService = jsonFileImportService;
        }


        [HttpGet]
        [Route("ImportJsonFile")]
        public async Task<IActionResult> JsonFileAsync(CancellationToken cancellationToken)
        {
            var result = await _jsonFileImportService.ImportJsonData(fileName, _elasticClient);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] News model, CancellationToken cancellationToken)
        {
            if (model is null) return BadRequest();
            var response = await _elasticClient.DocumentExistsAsync<News>(model.NId.ToString(), ct: cancellationToken);
            if (response.Exists) return BadRequest();
            var entity = await _elasticClient.IndexDocumentAsync(model, cancellationToken);

            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(int id, [FromBody] NewsUpdateRequest model, CancellationToken cancellationToken)
        {
            if (Equals(id, default(int))) return BadRequest("Identifier is not valid");
            var response = await _elasticClient.DocumentExistsAsync<NewsUpdateRequest>(id.ToString(), ct: cancellationToken);
            if (!response.Exists) return BadRequest();
            try
            {
                var entity = await _elasticClient.UpdateAsync<NewsUpdateRequest>(id, descriptor => descriptor.Doc(model), cancellationToken);

                if (String.Equals(entity.Result.ToString(), "Updated")) return Ok(model);
            }
            catch (Exception ex)
            {

            }
            return BadRequest();
        }
        [HttpGet]
        [Route("search")]
        public async Task<IPagedResponse<NewsReadModel>> SearchAsync([FromQuery] NewsSearchQuery searchQuery, CancellationToken cancellationToken)
        {
            IPagedResponse<NewsReadModel> response = null;
            await _elasticClient.Indices.FlushAsync(indexName, d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken);
            await _elasticClient.Indices.RefreshAsync(indexName, d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken);

            SearchRequest searchRequest = new SearchRequest<NewsReadModel>(indexName)
            {
                Query = new BoolQuery()
                {
                    Should = new QueryContainer[]
                    {
                        new MatchQuery()
                        {
                            Field = Infer.Field<NewsReadModel>(f => f.Title),
                            Query = searchQuery.Title,
                            Boost=1.1
                        },
                        new MatchQuery()
                        {
                            Field = Infer.Field<NewsReadModel>(f => f.SubTitle),
                            Query = searchQuery.SubTitle,
                        }
                    },
                    Filter = new QueryContainer[]
                    {
                        new TermQuery()
                        {
                            Field = Infer.Field<NewsReadModel>(f => f.NCategory),
                            Value=searchQuery.Category,
                        },
                        new TermQuery()
                        {
                             Field = Infer.Field<NewsReadModel>(f => f.Country),
                             Value=searchQuery.Country,
                        },
                         new MatchQuery()
                        {
                             Field = Infer.Field<NewsReadModel>(f => f.PortalName),
                             Query=searchQuery.Portal,
                        }
                    }
                }

            };

            var record = await _elasticClient.SearchAsync<NewsReadModel>(searchRequest, cancellationToken);

            if (record?.Documents?.Count > 0)
            {
                response = new PagedResponse<NewsReadModel>()
                {
                    Records = record.Documents.ToList(),
                    TotalCount = record.Total
                };
            }

            return response;
        }
    }
}
