namespace Labb2ProgTemplate.CostumerLevels;

public class GoldLevel : Customer
{
    public GoldLevel(string name, string password) : base(name, password)
    {
        Level = CostumerLevels.Gold;
    }
}