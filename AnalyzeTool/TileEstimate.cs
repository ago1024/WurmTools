using System;
using System.Collections.Generic;
using System.Text;
#if NUNIT
using NUnit.Framework;
#endif

namespace AnalyzeTool
{
    class TileEstimate
    {

        private HashSet<Detected> estimates = null;

        private static HashSet<Detected> emptySet = new HashSet<Detected>(comparer);
        private static IEqualityComparer<Detected> comparer = new DetectedEqualityComparer();
        private static List<Detected> somethingTypes = new List<Detected>();

        public HashSet<Detected> Estimates
        {
            get { if (estimates == null) return emptySet; else return estimates; }
        }

        static TileEstimate()
        {
            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                if (Detected.IsOreType(type) && type != TileType.Something)
                    somethingTypes.Add(new Detected(type, Quality.Unknown));
                else if (Detected.isSpecialType(type) && type != TileType.Something)
                    somethingTypes.Add(new Detected(type, Quality.Unknown));
            }
        }

        public void Add(List<Detected> detected)
        {
            if (estimates == null)
                estimates = this.MakeSet(detected);
            else if (estimates.Count > 0)
            {
                HashSet<Detected> set = this.MakeSet(detected);
                HashSet<Detected> newSet = new HashSet<Detected>(estimates, comparer);
                newSet.IntersectWith(set);

                foreach (Detected d in set)
                {
                    if (estimates.Contains(d) && d.Quality != Quality.Unknown)
                    {
                        newSet.Remove(d);
                        newSet.Add(d);
                    }
                }

                estimates = newSet;
            }

            if (estimates.Count == 0)
                estimates = emptySet;
        }

        private HashSet<Detected> MakeSet(List<Detected> detected)
        {
            HashSet<Detected> set = new HashSet<Detected>(comparer);

            foreach (Detected d in detected)
            {
                if (d.Type == TileType.Nothing)
                {
                    continue;
                }
                else if (d.Type == TileType.Something)
                {
                    set.UnionWith(somethingTypes);
                }
                else if (Detected.IsOreType(d.Type))
                {
                    if (!set.Contains(d) || d.Quality != Quality.Unknown)
                    {
                        set.Remove(d);
                        set.Add(d);
                    }
                }
                else if (Detected.isSpecialType(d.Type))
                {
                    if (!set.Contains(d))
                        set.Add(d);
                }
            }
            return set;
        }

        private String ToString(HashSet<Detected> set)
        {
            StringBuilder str = new StringBuilder();
            foreach (Detected d in set)
            {
                str.Append(d);
                str.Append(" ");
            }
            return str.ToString();
        }

        public override String ToString()
        {
            return ToString(Estimates);
        }
    }

    public class DetectedEqualityComparer : IEqualityComparer<Detected>
    {

        public bool Equals(Detected x, Detected y)
        {
            if (x.Type != y.Type)
                return false;
            if (x.Quality == Quality.Unknown || y.Quality == Quality.Unknown)
                return true;
            if (x.Quality == y.Quality)
                return true;
            return false;
        }

        public int GetHashCode(Detected obj)
        {
            return obj.Type.GetHashCode();
        }
    }

#if NUNIT
    [TestFixture]
    public class TileEstimateTests
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
                                    TileType.Zinc,
                                    TileType.RockSalt,
                                    TileType.SandStone,
                                };
        TileType[] resourceTypes = {
                                       TileType.Flint,
                                       TileType.Salt
                                };

        [Test]
        public void testSomething()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Something, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(oreTypes.Length + resourceTypes.Length, estimate.Estimates.Count, "Expected estimates");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            detected.Add(new Detected(TileType.Flint, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(2, estimate.Estimates.Count, "Expected 2 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Flint, Quality.Unknown)), "Expected estimates to contain flint");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates not to contain good iron");
        }

        [Test]
        public void testQuality()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates not to contain very good iron");
        }

        [Test]
        public void testExclusion()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Marble, Quality.Good));
            estimate.Add(detected);

            Assert.AreEqual(0, estimate.Estimates.Count, "Expected 0 estimates");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates not to contain iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.Unknown)), "Expected estimates not to contain marble");
        }

        [Test]
        public void testSelection()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            detected.Add(new Detected(TileType.Marble, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(2, estimate.Estimates.Count, "Expected 2 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.Unknown)), "Expected estimates to contain marble");

            detected.Clear();
            detected.Add(new Detected(TileType.Marble, Quality.Good));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates not to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.Unknown)), "Expected estimates to contain marble");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.Good)), "Expected estimates to contain good marble");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.VeryGood)), "Expected estimates not to contain very good marble");
        }

        [Test]
        public void testMulti()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Utmost)), "Expected estimates to contain utmost iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.AreEqual(2, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Utmost)), "Expected estimates not to contain utmost iron");
        }

        [Test]
        public void testNothing()
        {
            List<Detected> detected = new List<Detected>();

            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Nothing, Quality.Unknown));
            estimate.Add(detected);

            Assert.AreEqual(0, estimate.Estimates.Count, "Expected 0 estimates");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates not to contain iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.AreEqual(0, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates not to contain iron");
        }

        [Test]
        public void testSimilar()
        {
            List<Detected> detected = new List<Detected>();
            TileEstimate estimate = new TileEstimate();

            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            detected.Add(new Detected(TileType.Iron, Quality.Poor));
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Poor)), "Expected estimates to contain poor iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates not to contain goor iron");

            detected.Reverse();
            estimate = new TileEstimate();
            estimate.Add(detected);

            Assert.AreEqual(1, estimate.Estimates.Count, "Expected 1 estimates");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Poor)), "Expected estimates to contain poor iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates not to contain goor iron");
        }

    }
#endif

}
