using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNext.Net.Cluster.Consensus.Raft.Http
{
    internal interface IHttpMessageReader<TContent>
    {
        Task<TContent> ParseResponse(HttpResponseMessage response);
    }

    internal interface IHttpMessageWriter<in TContent>
    {
        Task SaveResponse(HttpResponse response, TContent result);
    }
}