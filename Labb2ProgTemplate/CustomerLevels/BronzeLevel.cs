namespace Labb2ProgTemplate.CostumerLevels;

public class BronzeLevel : Customer
{
    public BronzeLevel(string name, string password) : base(name, password)
    {
        Level = CostumerLevels.Bronze;
    }

}