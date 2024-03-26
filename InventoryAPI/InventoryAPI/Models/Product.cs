namespace InventoryAPI.Models
{
    public class Product
    {
        public int? productID {  get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public decimal productPrice { get; set; }
        public int productQuantity { get; set; }
        public int? categoryID { get; set; }


        public Product(int? productID, string productName, string productDescription, decimal productPrice, int productQuantity, int? categoryID)
        {
            this.productID = productID;
            this.productName = productName;
            this.productDescription = productDescription;
            this.productPrice = productPrice;
            this.productQuantity = productQuantity;
            this.categoryID = categoryID;

        }
    }
}
