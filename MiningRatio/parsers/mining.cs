using System.Text.RegularExpressions;

public class MiningMessageParser : IMessageParser
{
    private readonly Regex reMineSome = new Regex("You mine some ore|You mine some (rock|marble|slate) shards|You mine some shards");
    private readonly Regex reStartMine = new Regex("You start to mine");
    private readonly Regex reMiningIncreased = new Regex("Mining increased");

    public bool isActionStart(string message)
    {
        return reStartMine.IsMatch(message);
    }
    public bool isActionEnd(string message)
    {
        return reMineSome.IsMatch(message);
    }
    public bool isSkillGain(string message)
    {
        return reMiningIncreased.IsMatch(message);
    }
    public string getName()
    {
        return "Mining";
    }
}

