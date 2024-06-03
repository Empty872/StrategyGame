public class Buff
{
    public CharacteristicType CharacteristicType { get; private set; }
    public int Value { get; private set; }
    public int CurrentCooldown { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Buff(CharacteristicType characteristicType, int value, int cooldown, string name, string description)
    {
        CharacteristicType = characteristicType;
        Value = value;
        CurrentCooldown = cooldown;
        Name = name;
        Description = description;
    }
    public Buff(CharacteristicType characteristicType, int value, int cooldown)
    {
        CharacteristicType = characteristicType;
        Value = value;
        CurrentCooldown = cooldown;
    }

    public void ReduceCooldown()
    {
        CurrentCooldown--;
    }
}