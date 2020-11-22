using System.Threading.Tasks;

namespace Bakhtawar.Apps.GatewayApp.Contracts
{
    public interface IPasswordValidator
    {
        Task<bool> ValidateCredentialsAsync(string userName, string password);
    }
}