using RPSLS.DotNetPlayer.API;
using System;
using System.Threading.Tasks;

namespace RPSLS.DotNetPlayer.Api.Services
{
    public interface IPredictorProxyService
    {
        Task<Choice> GetPickPredicted(string userName);
    }
}
