namespace InventoryAPI.Models
{
    public class Expense
    {
        public int expenseID {  get; set; }
        public string expenseDescription { get; set; }
        public decimal expenseAmount { get; set; }
        public DateTime expenseDate { get; set; }
        public int? categoryID { get; set; }
        public int? inventoryID { get; set; }
        public int? marketplaceID { get; set; }


        public Expense (int expenseID, string expenseDescription, decimal expenseAmount, DateTime expenseDate, int? categoryID, int? inventoryID, int? marketplaceID)
        {
            this.expenseID = expenseID;
            this.expenseDescription = expenseDescription;
            this.expenseAmount = expenseAmount;
            this.categoryID = categoryID;
            this.inventoryID = inventoryID;
            this.marketplaceID = marketplaceID;
        }



    }
}
