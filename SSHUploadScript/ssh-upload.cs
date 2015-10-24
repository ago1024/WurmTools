//css_reference Tamir.SharpSSH.dll
//css_reference DiffieHellman.dll
//css_reference Org.Mentalis.Security.dll
using System;
using System.IO;
using Tamir.SharpSsh;
using WurmUtils;

public class SSHUpload {
    public void HandleWurmTimeStamp(WurmDateTimeStamp stamp) 
	{
        String tempfile = Path.GetTempFileName();
        try
        {
            StreamWriter writer = new StreamWriter(tempfile);
            try
            {
                writer.Write(String.Format("var timestamp = {0}", stamp.toJSON()));
            }
            finally
            {
                writer.Close();
            }
            

            Sftp sftp = new Sftp("gotti.no-ip.org", "gotti-ftp", "");
            sftp.AddIdentityFile("id_dsa");
            sftp.Connect();
            sftp.Put(tempfile, "/srv/gotti.org/httpdocs/wurm/timestamp.js");
            sftp.Close();
        }
        finally
        {
            File.Delete(tempfile);
        }
	}
}
