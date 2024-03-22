using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly ILogger<ProductsController> _logger;
        private readonly IConfiguration _configuration;
        public ProductsController(ILogger<ProductsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        
        [HttpGet(Name = "GetProducts")]
        public ActionResult GetProducts()
        {
            NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value);

            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"PRODUCTS\";", conn);

            NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Product> products = new List<Product>();
            while (reader.Read())
            {
                if (reader.IsDBNull(5))
                {
                    products.Add(new Product(
                    (int)reader["PRODUCT_ID"],
                    reader["PRODUCT_NAME"].ToString(),
                    reader["PRODUCT_DESCRIPTION"].ToString(),
                    (decimal)reader["PRODUCT_PRICE"],
                    (int)reader["PRODUCT_QUANTITY"]
                    ));
                }
                else
                {
                    products.Add(new Product(
                    (int)reader["PRODUCT_ID"],
                    reader["PRODUCT_NAME"].ToString(),
                    reader["PRODUCT_DESCRIPTION"].ToString(),
                    (decimal)reader["PRODUCT_PRICE"],
                    (int)reader["PRODUCT_QUANTITY"],
                    (int)reader["CATEGORY_ID"]
                    ));
                }
                
            }

            return Ok(products);
        }
    }

}
