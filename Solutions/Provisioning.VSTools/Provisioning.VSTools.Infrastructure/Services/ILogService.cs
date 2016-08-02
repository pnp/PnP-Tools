using System;
namespace Provisioning.VSTools.Services
{
    public interface ILogService
    {
        void Warn(string message);
        void Warn(string message, params object[] args);
        void Error(string message);
        void Error(string message, params object[] args);
        void Info(string message);
        void Info(string message, params object[] args);
        void Exception(string message, Exception ex);
    }
}
