namespace MiningRatio.Tests;

public class MessageParserLoaderTest
{
    [SetUp]
    public void Setup()
    {
    }

    string code = @"
        using System;
        public class TestMessageParser : IMessageParser
        {
            public bool isActionStart(String message)
            {
                return message == ""start"";
            }
            public bool isActionEnd(String message)
            {
                return message == ""end"";
            }
            public bool isSkillGain(String message)
            {
                return message == ""gain"";
            }
            public String getName()
            {
                return ""Test"";
            }
        }
    ";

    string code2 = @"
        using System.Text.RegularExpressions;

        public class ProspectingMessageParser : IMessageParser
        {
            private readonly Regex reStart = new Regex(""You start to gather fragments of the rock\\.|You start to analyse"");
            private readonly Regex reEnd = new Regex(""You find only rock\\.|You would mine (.* ore|.* shards) here\\.|You finish analysing"");
            private readonly Regex reSkill = new Regex(""Prospecting increased"");

            public bool isActionStart(string message)
            {
                return reStart.IsMatch(message);
            }
            public bool isActionEnd(string message)
            {
                return reEnd.IsMatch(message);
            }
            public bool isSkillGain(string message)
            {
                return reSkill.IsMatch(message);
            }
            public string getName()
            {
                return ""Prospecting"";
            }
        }
    ";

    [Test]
    public void TestLoadCode()
    {
        var loader = new MessageParserLoader();
        var parser = loader.LoadCode(code);
        Assert.That(parser, Is.InstanceOf(typeof(IMessageParser)));
    }

    [Test]
    public void TestLoadCode2()
    {
        var loader = new MessageParserLoader();
        var parser = loader.LoadCode(code2);
        Assert.That(parser, Is.InstanceOf(typeof(IMessageParser)));
    }

    [Test]
    public void TestLoadParsers() {
        string parsersDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "parsers");
        Assert.That(parsersDir, Does.Exist.IgnoreFiles);
        var loader = new MessageParserLoader(parsersDir);
        Assert.Multiple(() =>
        {
            foreach (string file in Directory.GetFiles(parsersDir, "*.cs"))
            {
                Assert.That(file, Does.Exist.IgnoreDirectories);
                Assert.DoesNotThrow(() => Assert.That(loader.LoadFile(file), Is.InstanceOf(typeof(IMessageParser))), file);
            }
        });
    }

}

