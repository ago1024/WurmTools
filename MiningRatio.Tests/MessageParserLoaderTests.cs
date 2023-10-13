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

    [Test]
    public void TestLoadCode()
    {
        var loader = new MessageParserLoader();
        var parser = loader.LoadCode(code);
        Assert.That(parser, Is.InstanceOf(typeof(IMessageParser)));
    }

}

