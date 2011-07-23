using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;


namespace WurmUtils
{
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
                return playerDir + @"\dumps";
            }
        }

        private static String RegistryKey = @"HKEY_CURRENT_USER\Software\JavaSoft\Prefs\com\wurmonline\client";

        public static String getWurmUser()
        {
            Object wurmUser = Registry.GetValue(RegistryKey, "wurm_user", null);
            return wurmUser as String;
        }

        public static String getWurmDir()
        {
            Object wurmDir = Registry.GetValue(RegistryKey, "wurm_dir", null);
            String str = wurmDir as String;
            if (str == null)
                return null;

            return str.Replace("//", @"\");
        }
    }
}
