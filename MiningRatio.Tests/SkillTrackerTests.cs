namespace MiningRatio.Tests;

using MiningRatio;

public class SkillTrackerTests {

    [Test]
    public void TestMultipleSkillUpsPerAction()
    {
        string Log = @"
            [13:06:10] You start to analyse the shard.
            [13:06:12] Prospecting increased by 0,0013 to 84,9970
            [13:06:15] Prospecting increased by 0,0013 to 84,9982
            [13:06:22] Prospecting increased by 0,0013 to 84,9995
            [13:06:25] Prospecting increased by 0,0013 to 85,0008
            [13:06:25] You finish analysing the shard.
            ";

        var tracker = new SkillTracker(new ProspectingMessageParser());
        tracker.ProcessLog(Log);
        Assert.Multiple(() =>
        {
            Assert.That(tracker.LastActionDuration, Is.EqualTo(TimeSpan.FromSeconds(15)), "LastActionDuration");
            Assert.That(tracker.LastActionSkill, Is.EqualTo(0.0013 * 4), "LastActionSkill");
            Assert.That(tracker.TotalTime, Is.EqualTo(TimeSpan.FromSeconds(15)), "TotalTime");
            Assert.That(tracker.SkillTime, Is.EqualTo(TimeSpan.FromSeconds(15)), "SkillTime");
            Assert.That(tracker.Ticks, Is.EqualTo(4), "Ticks");
            Assert.That(tracker.Actions, Is.EqualTo(1), "Actions");
            Assert.That(tracker.LastActionRate, Is.EqualTo(0.0013 * 4 / 15), "LastActionRate");
            Assert.That(tracker.SkillRate, Is.EqualTo(0.0013 * 4 / 15), "SkillRate");
            Assert.That(tracker.TotalRate, Is.EqualTo(0.0013 * 4 / 15), "TotalRate");
        });
    }

    [Test]
    public void TestMultipleActions()
    {
        string Log = @"
            [13:06:10] You start to analyse the shard.
            [13:06:12] Prospecting increased by 0,0013 to 84,9970
            [13:06:25] You finish analysing the shard.
            [13:06:25] You start to analyse the shard.
            [13:06:35] You finish analysing the shard.
            [13:06:35] You start to analyse the shard.
            [13:06:37] Prospecting increased by 0,0013 to 84,9970
            [13:06:39] Prospecting increased by 0,0013 to 84,9970
            [13:06:39] You finish analysing the shard.
            ";

        var tracker = new SkillTracker(new ProspectingMessageParser());
        tracker.ProcessLog(Log);
        Assert.Multiple(() =>
        {
            Assert.That(tracker.LastActionDuration, Is.EqualTo(TimeSpan.FromSeconds(4)), "LastActionDuration");
            Assert.That(tracker.LastActionSkill, Is.EqualTo(0.0013 * 2), "LastActionSkill");
            Assert.That(tracker.TotalTime, Is.EqualTo(TimeSpan.FromSeconds(29)), "TotalTime");
            Assert.That(tracker.SkillTime, Is.EqualTo(TimeSpan.FromSeconds(19)), "SkillTime");
            Assert.That(tracker.Ticks, Is.EqualTo(3), "Ticks");
            Assert.That(tracker.Actions, Is.EqualTo(3), "Actions");
            Assert.That(tracker.LastActionRate, Is.EqualTo(0.0013 * 2 / 4), "LastActionRate");
            Assert.That(tracker.SkillRate, Is.EqualTo(0.0013 * 3 / 19), "SkillRate");
            Assert.That(tracker.TotalRate, Is.EqualTo(0.0013 * 3 / 29), "TotalRate");
        });
    }

    [Test]
    public void TestLastActionNoSkillGain()
    {
        string Log = @"
            [13:06:10] You start to analyse the shard.
            [13:06:12] Prospecting increased by 0,0013 to 84,9970
            [13:06:25] You finish analysing the shard.
            [13:06:25] You start to analyse the shard.
            [13:06:35] You finish analysing the shard.
            ";

        var tracker = new SkillTracker(new ProspectingMessageParser());
        tracker.ProcessLog(Log);
        Assert.Multiple(() =>
        {
            Assert.That(tracker.LastActionDuration, Is.EqualTo(TimeSpan.FromSeconds(10)), "LastActionDuration");
            Assert.That(tracker.LastActionSkill, Is.EqualTo(0), "LastActionSkill");
            Assert.That(tracker.TotalTime, Is.EqualTo(TimeSpan.FromSeconds(25)), "TotalTime");
            Assert.That(tracker.SkillTime, Is.EqualTo(TimeSpan.FromSeconds(15)), "SkillTime");
            Assert.That(tracker.Ticks, Is.EqualTo(1), "Ticks");
            Assert.That(tracker.Actions, Is.EqualTo(2), "Actions");
            Assert.That(tracker.LastActionRate, Is.EqualTo(0.0), "LastActionRate");
            Assert.That(tracker.SkillRate, Is.EqualTo(0.0013 * 1 / 15), "SkillRate");
            Assert.That(tracker.TotalRate, Is.EqualTo(0.0013 * 1 / 25), "TotalRate");
        });
    }


}