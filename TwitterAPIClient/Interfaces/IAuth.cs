using System.Threading.Tasks;
using Tweetinvi;

namespace TwitterAPIClient.Interfaces
{
    public interface IAuth
    {
        Task<TwitterClient> Authenticate();
    }
}