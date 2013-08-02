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
                Assert.IsFalse(Matches(type, TileType.Nothing), "{0} matches {1}", type, TileType.Nothing);
                Assert.IsFalse(Matches(TileType.Nothing, type), "{0} matches {1}", TileType.Nothing, type);
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
                Assert.IsFalse(Matches(type, TileType.Nothing), "{0} matches {1}", type, TileType.Nothing);
                Assert.IsFalse(Matches(TileType.Nothing, type), "{0} matches {1}", TileType.Nothing, type);
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

        [Test]
        public void testAdd()
        {
            // Setting a list of detected ores should set the estimates of the tile to exactly this list
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Unknown), new Detected(TileType.Silver, Quality.Unknown) }));

            Assert.IsNotNull(map[tile].Estimates);
            Assert.AreEqual(2, map[tile].Estimates.Count);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[0].Type);
            Assert.AreEqual(TileType.Silver, map[tile].Estimates[1].Type);
        }

        [Test]
        public void testAddUnknown()
        {
            // If nothing has been detected on the tile mark this as result. Estimates is empty and found is set
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Nothing, Quality.Unknown) }));

            Assert.IsNull(map[tile].Estimates);
            Assert.IsNotNull(map[tile].Found);
            Assert.AreEqual(TileType.Nothing, map[tile].Found.Type);
        }

        [Test]
        public void testMerge()
        {
            // Setting a list of detected ores and adding another list should leave the union of the two lists
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Unknown), new Detected(TileType.Silver, Quality.Unknown)}));
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Good) }));

            Assert.IsNotNull(map[tile].Estimates);
            Assert.AreEqual(1, map[tile].Estimates.Count);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[0].Type);
            Assert.AreEqual(Quality.Good, map[tile].Estimates[0].Quality);
        }

        [Test]
        public void testMergeUnknown()
        {
            // Setting Nothing and adding another detected ore should leave the tile at Nothing.
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Nothing, Quality.Unknown) }));
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Good) }));

            Assert.IsNull(map[tile].Estimates);
            Assert.IsNotNull(map[tile].Found);
            Assert.AreEqual(TileType.Nothing, map[tile].Found.Type);
        }

        [Test]
        public void testMergeSomething()
        {
            // Setting Iron and Something (eg iron and silver) and adding Iron later on should leave only Iron 
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Unknown), new Detected(TileType.Something, Quality.Unknown) }));
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Good) }));

            Assert.IsNotNull(map[tile].Estimates);
            Assert.AreEqual(1, map[tile].Estimates.Count);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[0].Type);
            Assert.AreEqual(Quality.Good, map[tile].Estimates[0].Quality);
        }

        [Test]
        public void testMergeComplex()
        {
            // Complex test
            // First "Something" is detected
            // Getting closer we detect "Iron, Zinc and something else
            // A 3rd analyze detects 2 iron of different quality and a zinc
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Something, Quality.Unknown) }));
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Unknown), new Detected(TileType.Zinc, Quality.Unknown), new Detected(TileType.Something, Quality.Unknown) }));

            Assert.IsNotNull(map[tile].Estimates);
            Assert.AreEqual(3, map[tile].Estimates.Count);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[0].Type);
            Assert.AreEqual(TileType.Zinc, map[tile].Estimates[1].Type);
            Assert.AreEqual(TileType.Something, map[tile].Estimates[2].Type);          
            
            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Good), new Detected(TileType.Iron, Quality.Acceptable), new Detected(TileType.Zinc, Quality.Unknown) }));

            Assert.IsNotNull(map[tile].Estimates);
            Assert.AreEqual(3, map[tile].Estimates.Count);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[0].Type);
            Assert.AreEqual(TileType.Iron, map[tile].Estimates[1].Type);
            Assert.AreEqual(TileType.Zinc, map[tile].Estimates[2].Type);
            Assert.AreEqual(Quality.Good, map[tile].Estimates[0].Quality);
            Assert.AreEqual(Quality.Acceptable, map[tile].Estimates[1].Quality);
            Assert.AreEqual(Quality.Unknown, map[tile].Estimates[2].Quality);
        }

        [Test]
        public void testMergeUpdate()
        {
            // Setting Iron and Something (eg iron and silver) and adding Iron later on should leave only Iron 
            AnalyzeMap map = new AnalyzeMap(1, 1);
            Tile tile = new Tile(0, 0);
            map[tile].Set(new Detected(TileType.Zinc, Quality.Unknown));

            Assert.IsNull(map[tile].Estimates);
            Assert.IsNotNull(map[tile].Found);
            Assert.AreEqual(TileType.Zinc, map[tile].Found.Type);
            Assert.AreEqual(Quality.Unknown, map[tile].Found.Quality);

            map[tile].Add(new List<Detected>(new Detected[] { new Detected(TileType.Iron, Quality.Good), new Detected(TileType.Zinc, Quality.Good) }));

            Assert.IsNull(map[tile].Estimates);
            Assert.IsNotNull(map[tile].Found);
            Assert.AreEqual(TileType.Zinc, map[tile].Found.Type);
            Assert.AreEqual(Quality.Good, map[tile].Found.Quality);
        }
    }
}
