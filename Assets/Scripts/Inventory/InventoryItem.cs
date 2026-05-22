public class InventoryItem
{
    public ItemDefinition ItemDefinition { get; private set; }
    public int Quantity { get; private set; }

    public InventoryItem(ItemDefinition itemDefinition, int quantity)
    {
        ItemDefinition = itemDefinition;
        Quantity = quantity;
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        Quantity -= amount;

        if (Quantity < 0)
        {
            Quantity = 0;
        }
    }
}