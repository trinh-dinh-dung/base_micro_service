using Application.IServices;
using System.Threading.Tasks;

namespace Application.Services
{
    public class HomeService : IHomeService
    {
        public Task<int> DemoDynamic() => Task.FromResult(1);
    }
}
