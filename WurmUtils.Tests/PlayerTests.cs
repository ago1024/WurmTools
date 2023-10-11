using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using NUnit.Framework;

namespace WurmUtils
{
    [TestFixture]
    public class PlayerTests
    {
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
                Assert.AreEqual(test[1], Player.unescape(test[0]));
            }
        }
    }
}
