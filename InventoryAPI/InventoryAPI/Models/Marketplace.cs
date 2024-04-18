namespace InventoryAPI.Models
{
    public class Marketplace
    {
        public int marketplaceID { get; set; }
        public string marketplaceName { get; set; }
        public string  marketplaceDescription { get; set; }
    

        public Marketplace (int marketplaceID, string marketplaceName, string marketplaceDescription)
        {
            this.marketplaceID = marketplaceID;
            this.marketplaceName = marketplaceName; 
            this.marketplaceDescription = marketplaceDescription;
        }
    }
}
