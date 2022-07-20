using System;
using System.IO;
using System.Threading;
using FolderMonitoringService.Constants;

namespace FolderMonitoringService
{
    public class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;

        public Logger()
        {
            watcher = new FileSystemWatcher(ServiceConstants.PathOfMonitoringFolder);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = LogMessagesConstants.RenamedTo + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = LogMessagesConstants.Modified;
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = LogMessagesConstants.Created;
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = LogMessagesConstants.Removed;
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(ServiceConstants.PathForSavedLogs, true))
                {
                    writer.WriteLine(LogMessagesConstants.Action, DateTime.Now.ToString(LogMessagesConstants.DateFormat), filePath, fileEvent);
                    writer.Flush();
                }
            }
        }
    }
}
