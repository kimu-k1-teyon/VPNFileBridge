using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Common.Log
{
    public interface ILogService
    {
        string CurrentFilePath { get; }
        Task WriteAsync(string content, CancellationToken cancellationToken = default);
        void Write(string content);
    }
}
