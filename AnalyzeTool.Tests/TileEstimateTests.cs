using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace AnalyzeTool
{

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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(oreTypes.Length + resourceTypes.Length));

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Unknown));
            detected.Add(new Detected(TileType.Flint, Quality.Unknown));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(2));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Flint, Quality.Unknown)), "Expected estimates to contain flint");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Marble, Quality.Good));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(0));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(2));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Marble, Quality.Unknown)), "Expected estimates to contain marble");

            detected.Clear();
            detected.Add(new Detected(TileType.Marble, Quality.Good));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates to contain good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.VeryGood)), "Expected estimates to contain very good iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Utmost)), "Expected estimates to contain utmost iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(2));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(0));
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates not to contain iron");

            detected.Clear();
            detected.Add(new Detected(TileType.Iron, Quality.Good));
            detected.Add(new Detected(TileType.Iron, Quality.VeryGood));
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(0));
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

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Poor)), "Expected estimates to contain poor iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates not to contain goor iron");

            detected.Reverse();
            estimate = new TileEstimate();
            estimate.Add(detected);

            Assert.That(estimate.Estimates.Count, Is.EqualTo(1));
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Unknown)), "Expected estimates to contain iron");
            Assert.True(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Poor)), "Expected estimates to contain poor iron");
            Assert.False(estimate.Estimates.Contains(new Detected(TileType.Iron, Quality.Good)), "Expected estimates not to contain goor iron");
        }

    }

}
