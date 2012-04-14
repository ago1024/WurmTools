using System;
using System.IO;
using System.Net;
using System.Text;
using WurmUtils;

public class DAVUpload {
    public void HandleWurmTimeStamp(WurmDateTimeStamp stamp) 
	{
        string URL = @"http://gotti.dnsalias.org/wurm/timestamp.js";
        String message = String.Format("var timestamp = {0}", stamp.toJSON());
        byte[] bytes = Encoding.UTF8.GetBytes(message);

        CredentialCache cache = new CredentialCache();
        cache.Add(new Uri(URL), "Digest", new NetworkCredential("user", "password"));

        HttpWebRequest PUTRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
        PUTRequest.Method = "PUT";
        PUTRequest.Credentials = cache;
        PUTRequest.ContentLength = bytes.Length;

        Stream requestStream = PUTRequest.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)PUTRequest.GetResponse();
        response.Close();
	}
}
