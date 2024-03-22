namespace InventoryAPI.Models
{
    public class Category
    {
        public int categoryID {  get; set; }
        public string categoryName { get; set; }
        public string categoryDescription { get; set; }


        public Category(int categoryID, string categoryName, string categoryDescription)
        {
            this.categoryID = categoryID;
            this.categoryName = categoryName;
            this.categoryDescription = categoryDescription;
        }
    }
}
