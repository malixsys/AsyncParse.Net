using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncParse.Net.Model
{
    internal interface IAsyncClient
    {
        Task<HttpResponseMessage> Get(CancellationToken token);
        Task<HttpResponseMessage> Delete(CancellationToken token);
        Task<HttpResponseMessage> Post(CancellationToken token);
        Task<HttpResponseMessage> Put(object modifieds, CancellationToken token);
    }
}