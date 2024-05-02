using System.Reflection.Emit;

namespace Labb2ProgTemplate.CostumerLevels;

public class SilverLevel : Customer
{
    public SilverLevel(string name, string password) : base(name, password)
    {
        Level = CostumerLevels.Silver;
    }
}