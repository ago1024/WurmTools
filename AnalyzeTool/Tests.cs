using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AnalyzeTool
{
    [TestFixture]
    public class DetectedTest
    {
        TileType[] oreTypes = {
                                    TileType.Iron,
                                    TileType.Copper,
                                    TileType.Gold,
                                    TileType.Lead,
                                    TileType.Marble,
                                    TileType.Silver,
                                    TileType.Slate,
                                    TileType.Tin,
                                    TileType.Zinc
                                };
        TileType[] plainTypes = {
                                    TileType.Rock,
                                    TileType.Reinforced,
                                    TileType.Tunnel
                                };
        TileType[] specialTypes = {
                                        TileType.Unknown,
                                        TileType.Nothing,
                                        TileType.Something
                                    };

        TileType[] allTypes = (TileType[])Enum.GetValues(typeof(TileType));



        private bool Matches(TileType t1, TileType t2)
        {
            return new Detected(t1, Quality.Unknown).Matches(new Detected(t2, Quality.Unknown));
        }

        [Test]
        public void verifySubsets()
        {
            foreach (TileType type in allTypes)
            {
                Assert.IsTrue(oreTypes.Contains(type) || plainTypes.Contains(type) || specialTypes.Contains(type), "{0} is not listed", type);
            }
        }

        [Test]
        public void testMatches()
        {
            foreach (TileType type in oreTypes)
            {
                Assert.IsTrue(Matches(type, type), "{0} matches {1}", type, type);
                Assert.IsTrue(Matches(type, TileType.Something), "{0} matches {1}", type, TileType.Something);
                Assert.IsTrue(Matches(TileType.Something, type), "{0} matches {1}", TileType.Something, type);
                Assert.IsFalse(Matches(type, TileType.Unknown), "{0} matches {1}", type, TileType.Unknown);
                Assert.IsFalse(Matches(TileType.Unknown, type), "{0} matches {1}", TileType.Unknown, type);
                Assert.IsTrue(Detected.IsOreType(type));

                foreach (TileType type2 in oreTypes)
                {
                    Assert.IsTrue(Matches(type, type2) == (type == type2));
                }

                foreach (TileType type2 in plainTypes)
                {
                    Assert.IsFalse(Matches(type, type2));
                }
            }

            foreach (TileType type in plainTypes)
            {
                Assert.IsTrue(Matches(type, type), "{0} matches {1}", type, type);
                Assert.IsFalse(Matches(type, TileType.Something), "{0} matches {1}", type, TileType.Something);
                Assert.IsFalse(Matches(TileType.Something, type), "{0} matches {1}", TileType.Something, type);
                Assert.IsFalse(Matches(type, TileType.Unknown), "{0} matches {1}", type, TileType.Unknown);
                Assert.IsFalse(Matches(TileType.Unknown, type), "{0} matches {1}", TileType.Unknown, type);
                Assert.IsFalse(Detected.IsOreType(type));

                foreach (TileType type2 in oreTypes)
                {
                    Assert.IsFalse(Matches(type, type2));
                }

                foreach (TileType type2 in plainTypes)
                {
                    Assert.IsTrue(Matches(type, type2) == (type == type2));
                }
            }
        }
    }
}
