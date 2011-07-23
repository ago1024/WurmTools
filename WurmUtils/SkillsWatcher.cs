using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WurmUtils
{
    public class SkillsWatcher : FileSystemWatcher
    {
        private Player player_;
        public SkillsWatcher(Player player)
        {
            player_ = player;
            Path = player.DumpsDir;
            Filter = "skills.*.txt";
        }

        public Player Player
        {
            get { return player_; }
        }
    
    }
}
