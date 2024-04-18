namespace InventoryAPI.Models
{
    public class Inventory
    {
        public int inventoryID {  get; set; }
        public int productID { get; set; }
        public int actionID { get; set; }
        public int quantityChanged { get; set; }
        public DateTime timestamp { get; set; }

        public Inventory(int inventoryID, int productID, int actionID, int quantityChanged, DateTime timestamp)
        {
            this.inventoryID = inventoryID;
            this.productID = productID;
            this.actionID = actionID;
            this.quantityChanged = quantityChanged;
            this.timestamp = timestamp;

        }
    }
}
