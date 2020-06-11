using System.Threading.Tasks;

namespace Dinokin.ScanlationTools.Interfaces
{
    public interface IAuthenticatedRipper : IRipper
    {
        public Task Authenticate(string user, string password, string? twoFactorCode = null);
    }
}