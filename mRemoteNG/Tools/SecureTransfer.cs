using System;
using System.IO;
using mRemoteNG.App;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using static System.IO.FileMode;
using mRemoteNG.Resources.Language;
using System.Runtime.Versioning;

namespace mRemoteNG.Tools
{
    [SupportedOSPlatform("windows")]
    internal class SecureTransfer : IDisposable
    {
        private readonly string Host;
        private readonly string User;
        private readonly string Password;
        private readonly int Port;
        public readonly SSHTransferProtocol Protocol;
        public readonly SSHTransferDirection Direction;
        public string LocalPath;
        public string RemotePath;
        public ScpClient ScpClt;
        public SftpClient SftpClt;
        public SftpUploadAsyncResult asyncResult;
        public SftpDownloadAsyncResult asyncDownloadResult;
        public AsyncCallback asyncCallback;


        public SecureTransfer()
        {
        }

        public SecureTransfer(string host, string user, string pass, int port, SSHTransferProtocol protocol, SSHTransferDirection direction)
        {
            Host = host;
            User = user;
            Password = pass;
            Port = port;
            Protocol = protocol;
            Direction = direction;
        }

        public SecureTransfer(string host,
            string user,
            string pass,
            int port,
            SSHTransferProtocol protocol,
            SSHTransferDirection direction,
            string localPath,
            string remotePath)
        {
            Host = host;
            User = user;
            Password = pass;
            Port = port;
            Protocol = protocol;
            Direction = direction;
            LocalPath = localPath;
            RemotePath = remotePath;
        }

        public void Connect()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                ScpClt = new ScpClient(Host, Port, User, Password);
                ScpClt.Connect();
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                SftpClt = new SftpClient(Host, Port, User, Password);
                SftpClt.Connect();
            }
        }

        public void Disconnect()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                ScpClt.Disconnect();
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                SftpClt.Disconnect();
            }
        }


        public void Upload()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                if (!ScpClt.IsConnected)
                {
                    Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                        Language.SshTransferUploadFailed + Environment.NewLine +
                        "SCP Not Connected!");
                    return;
                }

                ScpClt.Upload(new FileInfo(LocalPath), $"{RemotePath}");
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                if (!SftpClt.IsConnected)
                {
                    Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                        Language.SshTransferUploadFailed + Environment.NewLine +
                        "SFTP Not Connected!");
                    return;
                }

                asyncResult =
                    (SftpUploadAsyncResult)SftpClt.BeginUploadFile(new FileStream(LocalPath, Open), $"{RemotePath}",
                        asyncCallback);
            }
        }

        public void Download()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                if (!ScpClt.IsConnected)
                {
                    Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                        Language.SshTransferDownloadFailed + Environment.NewLine +
                        "SCP Not Connected!");
                    return;
                }

                ScpClt.Download($"{RemotePath}", new FileInfo(LocalPath));
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                if (!SftpClt.IsConnected)
                {
                    Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                        Language.SshTransferDownloadFailed + Environment.NewLine +
                        "SFTP Not Connected!");
                    return;
                }

                asyncDownloadResult =
                    (SftpDownloadAsyncResult)SftpClt.BeginDownloadFile($"{RemotePath}", new FileStream(LocalPath, Create),
                        asyncCallback);
            }
        }

        public enum SSHTransferProtocol
        {
            SCP = 0,
            SFTP = 1
        }

        public enum SSHTransferDirection
        {
            Upload = 0,
            Download = 1
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (Protocol == SSHTransferProtocol.SCP)
            {
                ScpClt.Dispose();
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                SftpClt.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}