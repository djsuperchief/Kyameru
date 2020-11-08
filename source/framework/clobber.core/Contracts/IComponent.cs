using System;
namespace Kyameru.Core.Contracts
{
    public interface IComponent
    {
        void LogInformation(string info);

        void LogWarning(string warning);

        void LogError(string error);

        void LogCritical(string critical);

        void LogException(Exception ex);
    }
}
