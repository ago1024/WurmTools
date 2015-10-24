using System;
using System.Collections.Generic;
using System.Text;
using WurmUtils;

namespace WOSUploader
{
    class WOSPlayer : WurmUtils.Player
    {
        public String WOSPassword
        {
            get;
            set;
        }

        public String WOSUser
        {
            get;
            set;
        }

    }
}
