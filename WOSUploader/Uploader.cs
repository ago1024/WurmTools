using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace WOSUploader
{
    class Uploader
    {
        String username;
        String password;
        String referurl = "http://wurm.riceri.se/";
        String skillsurl = "http://wurm.riceri.se/post_skills.php";
        String loginurl = "http://wurm.riceri.se/post_login.php";
        CookieContainer cookies;

        public Uploader(String username, String password)
        {
            this.username = username;
            this.password = password;
        }

        public bool Login()
        {
            cookies = new CookieContainer();

            // load start page. required to get the PHPSESSION setup
            HttpGet(referurl);

            // login. this logs out any other of your sessions
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("username", username);
            nvc.Add("password", password);
            HttpPost(loginurl, null, null, null, nvc);

            // copy cookies. for some reason all cookies are restricted to the login url
            foreach (Cookie cookie in cookies.GetCookies(new Uri(loginurl)))
                cookies.Add(new Cookie(cookie.Name, cookie.Value, "/", cookie.Domain));

            foreach (Cookie cookie in cookies.GetCookies(new Uri(skillsurl)))
                if (cookie.Name == "Username")
                    return true;
            return false;
        }

        public void Upload(String filename)
        {
            // log in to the page
            if (cookies == null || cookies.Count == 0)
                Login();
            HttpPost(skillsurl, filename, "skillfile", "text/plain", new NameValueCollection());
        }

        public void HttpPost(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Uploading {0} to {1}", file == null ? "data" : file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.CookieContainer = cookies;
            wr.Referer = referurl;
            wr.AllowAutoRedirect = true;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            if (file != null)
            {
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, file, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            HttpWebResponse wresp = (HttpWebResponse)wr.GetResponse();
            System.Diagnostics.Debug.WriteLine(string.Format("Uploaded, server response is: {0}", wresp.StatusDescription));
            wresp.Close();
        }

        public void HttpGet(string url)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Retrieving {0}", url));

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "GET";
            wr.KeepAlive = true;
            wr.CookieContainer = cookies;
            wr.AllowAutoRedirect = true;

            HttpWebResponse wresp = (HttpWebResponse)wr.GetResponse();
            System.Diagnostics.Debug.WriteLine(string.Format("Server response is: {0}", wresp.StatusDescription));
            wresp.Close();
        }
    }
}
