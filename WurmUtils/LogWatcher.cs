using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Timers;

namespace WurmUtils
{
    public class LogWatcher
    {
        IList<FileSystemWatcher> watchers;
        IDictionary<String, long> fileSizes;
        Timer timer;
        Object timerLock = new Object();        

        public delegate void NotificationEventHandler(Object sender, String message);
        public event NotificationEventHandler Notify;

        public delegate void FileNotificationEventHandler(Object sender, String filename, String message);
        public event FileNotificationEventHandler FileNotify;

        public LogWatcher()
        {
            watchers = new List<FileSystemWatcher>();
            fileSizes = new Dictionary<String, long>();
            timer = new Timer();
        }

        public double PollInterval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        public void Add(String path, String filter)
        {
            timer.Enabled = false;

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);

            watcher.Error += new ErrorEventHandler(OnError);

            watcher.EnableRaisingEvents = true;

            lock (timerLock)
            {
                watchers.Add(watcher);

                if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major > 5)
                {
                    timer.AutoReset = true;
                    timer.Interval = 1000;
                    timer.Elapsed += new ElapsedEventHandler(OnTimer);
                    timer.Enabled = true;
                }
            }
        }

        // Work around a stupid bug, sideeffect or whatever crap they call it that was introduced with Vista
        // If a log file is kept open in another process the filesystem watcher will not fire right away
        // So we still have to rely on polling the size and even worse open the file for reading to pass the 
        // changes to the FSW. 
        void OnTimer(object sender, ElapsedEventArgs e)
        {
            lock (timerLock)
            {
                foreach (FileSystemWatcher watcher in watchers)
                {
                    String[] files = Directory.GetFiles(watcher.Path, watcher.Filter, watcher.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    foreach (String file in files)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (!fileSizes.ContainsKey(file))
                                fileSizes[file] = fileInfo.Length;
                            else if (fileSizes[file] != fileInfo.Length)
                                fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite).Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        void OnError(object sender, ErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.GetException().Message);
        }

        void OnDeleted(object sender, FileSystemEventArgs e)
        {
            fileSizes.Remove(e.FullPath);
        }

        void OnRenamed(object sender, RenamedEventArgs e)
        {
            long value = fileSizes[e.OldFullPath];
            fileSizes.Remove(e.OldFullPath);
            fileSizes[e.FullPath] = value;
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            NotifyChanges(e.FullPath);
        }

        void OnCreated(object sender, FileSystemEventArgs e)
        {
            fileSizes[e.FullPath] = 0;
            NotifyChanges(e.FullPath);
        }

        void NotifyChanges(String fullPath)
        {
            String message = "";
            if (!fileSizes.ContainsKey(fullPath))
            {
                FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    message = reader.ReadLine();
                }
                fileSizes[fullPath] = fileStream.Position;
                reader.Close();
                fileStream.Close();
            }
            else
            {
                long oldSize = fileSizes[fullPath];
                FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileStream.Seek(oldSize, SeekOrigin.Begin);

                StreamReader reader = new StreamReader(fileStream);
                message = reader.ReadToEnd();
                fileSizes[fullPath] = fileStream.Position;
                reader.Close();
                fileStream.Close();
            }

            message = message.Replace("\r\n", "\n");

            if (message.Length > 0)
            {
                if (FileNotify != null)
                    FileNotify(this, fullPath, message);
                if (Notify != null)
                    Notify(this, message);
            }
        }

        public void Close()
        {
            timer.Enabled = false;
            lock (timerLock)
            {
                foreach (FileSystemWatcher watcher in watchers)
                {
                    watcher.EnableRaisingEvents = false;
                }
                watchers.Clear();
            }
        }
    }
}
