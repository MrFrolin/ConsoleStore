namespace Labb2ProgTemplate.CostumerLevels;

public class RegularLevel : Customer
{
    public RegularLevel(string name, string password) : base(name, password)
    {
        Level = CostumerLevels.Regular;
    }
}