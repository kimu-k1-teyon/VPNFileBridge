using System.IO;
using System.Threading;
using UnityEngine;

namespace Scripts.Common.Log
{
    public class LogModel
    {
        static readonly SemaphoreSlim SharedWriteLock = new SemaphoreSlim(1, 1);

        public string DirectoryPath { get; } = Path.Combine(Application.persistentDataPath, "File", "Logs");
        public string FilePath { get; } = Path.Combine(Application.persistentDataPath, "File", "Logs", $"{LogStartupContext.StartupTimestamp}.log");
        public SemaphoreSlim WriteLock => SharedWriteLock;
    }
}
