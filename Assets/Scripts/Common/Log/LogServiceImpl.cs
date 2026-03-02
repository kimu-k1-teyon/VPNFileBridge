using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Scripts.Common.Log
{
    public class LogServiceImpl : ILogService
    {
        [Inject] LogModel _model;

        readonly object _initializeLock = new object();
        bool _isInitialized;

        public string CurrentFilePath => _model.FilePath;

        public void Write(string content)
        {
            string line = $"[{DateTime.Now:HHmmss}]{content ?? string.Empty}{Environment.NewLine}";
            File.AppendAllText(_model.FilePath, line, Encoding.UTF8);
        }

        public async Task WriteAsync(string content, CancellationToken cancellationToken = default)
        {
            InitializeIfNeeded();

            string line = $"[{DateTime.Now:HHmmss}]{content ?? string.Empty}{Environment.NewLine}";

            await _model.WriteLock.WaitAsync(cancellationToken);
            try
            {
                await File.AppendAllTextAsync(_model.FilePath, line, Encoding.UTF8, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Log write failed: {ex}");
            }
            finally
            {
                _model.WriteLock.Release();
            }
        }

        void InitializeIfNeeded()
        {
            if (_isInitialized)
            {
                return;
            }

            lock (_initializeLock)
            {
                if (_isInitialized)
                {
                    return;
                }

                Directory.CreateDirectory(_model.DirectoryPath);

                if (!File.Exists(_model.FilePath))
                {
                    using var stream = File.Create(_model.FilePath);
                }

                _isInitialized = true;
            }
        }
    }
}
