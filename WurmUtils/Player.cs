using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
#if NUNIT
using NUnit.Framework;
#endif


namespace WurmUtils
{
#if NUNIT
    [TestFixture]
#endif
    public class Player
    {
        public Player() : this(null, null)
        {
        }

        public Player(String playerName) : this(playerName, null)
        {
        }

        public Player(String playerName, String wurmDir)
        {
            playerName_ = playerName == null ? getWurmUser() : playerName;
            wurmDir_ = wurmDir == null ? getWurmDir() : wurmDir;

            System.Diagnostics.Debug.WriteLine("PlayerName:" + PlayerName);
            System.Diagnostics.Debug.WriteLine("WurmDir:" + WurmDir);
            System.Diagnostics.Debug.WriteLine("LogDir:" + LogDir);
            System.Diagnostics.Debug.WriteLine("DumpsDir:" + DumpsDir);

        }

        private String playerName_;
        public String PlayerName
        {
            get 
            { 
                if (playerName_ == null ||playerName_.Length == 0) 
                    return getWurmUser(); 
                else 
                    return playerName_; 
            }
            set
            {
                playerName_ = value;
            }
        }

        private String wurmDir_;
        public String WurmDir
        {
            get
            {
                if (wurmDir_ == null || wurmDir_.Length == 0)
                    return getWurmDir();
                else
                    return wurmDir_;
            }
            set
            {
                wurmDir_ = value;
            }
        }

        private bool testServer_ = false;
        public bool TestServer
        {
            get
            {
                return testServer_;
            }
            set
            {
                testServer_ = value;
            }
        }

        public String PlayerDir
        {
            get
            {
                if (PlayerName == null || WurmDir == null)
                {
                    return null;
                }

                return WurmDir + @"\players\" + PlayerName;
            }     
        }

        public String LogDir
        {
            get
            {
                String playerDir = PlayerDir;
                if (playerDir == null)
                    return null;
                if (testServer_)
                    return playerDir + @"\test_logs";
                else
                    return playerDir + @"\logs";
            }
        }

        public String DumpsDir
        {
            get
            {
                String playerDir = PlayerDir;
                if (playerDir == null)
                    return null;
                if (testServer_)
                    return playerDir + @"\test_dumps";
                else
                    return playerDir + @"\dumps";
            }
        }

        private static String RegistryKey = @"HKEY_CURRENT_USER\Software\JavaSoft\Prefs\com\wurmonline\client";

        private static String unescape(String str)
        {
            if (str == null)
                return null;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '/' || str[i] == '\\')
                {
                    i++;

                    if (i + 4 < str.Length && str[i] == 'u')
                    {
                        String unicode = str.Substring(i + 1, 4);
                        builder.Append(char.ConvertFromUtf32(Convert.ToInt32(unicode, 16)));
                        i += 4;
                    } else if (i < str.Length)
                        builder.Append(str[i]);
                }
                else
                {
                    builder.Append(str[i]);
                }
            }
            return builder.ToString();
        }

        public static String getWurmUser()
        {
            Object wurmUser = Registry.GetValue(RegistryKey, "wurm_user", null);
            return unescape(wurmUser as String);
        }

        public static String getWurmDir()
        {
            Object wurmDir = Registry.GetValue(RegistryKey, "wurm_dir", null);
            String str = wurmDir as String;
            if (str == null)
                return null;            
            return unescape(str).Replace("/", "\\");
        }

#if NUNIT
        [Test]
        public void testUnescape()
        {
            String[][] teststrings = { 
                                       new String[] { "/C:///Users///u00c4garen//wurm", "C:/Users/Ägaren/wurm" },
                                       new String[] { "d://ago//wurm", "d:/ago/wurm" },
                                       new String[] { "/Jalina", "Jalina" },
                                     };

            foreach (String[] test in teststrings) 
            {
                Assert.AreEqual(test[1], unescape(test[0]));
            }
        }
#endif
    }
}
