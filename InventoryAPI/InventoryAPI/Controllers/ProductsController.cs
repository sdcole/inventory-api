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
        
        [HttpGet("GetProducts")]
        public ActionResult GetProducts()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"PRODUCTS\";", conn);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new List<Product>();
                    int? categoryID;
                    while (reader.Read())
                    {
                        categoryID = null;
                        if (!reader.IsDBNull(5))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        products.Add(new Product(
                            (int)reader["PRODUCT_ID"],
                            reader["PRODUCT_NAME"].ToString(),
                            reader["PRODUCT_DESCRIPTION"].ToString(),
                            (decimal)reader["PRODUCT_PRICE"],
                            (int)reader["PRODUCT_QUANTITY"],
                            categoryID
                            ));
                    }
                    //Return ok response and list of products.
                    return Ok(products);
                }
                //Log error and return bad response
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest(ex.ToString());
                }
                
            }

            
        }
        [HttpGet("GetProductsByID")]
        public ActionResult GetProductsByID(int productID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"PRODUCTS\" WHERE \"PRODUCT_ID\" = @productID;", conn);
                    cmd.Parameters.AddWithValue("@productID", productID);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new List<Product>();
                    int? categoryID;
                    while (reader.Read())
                    {
                        //This checks if Category ID is null if it is it passes a null value since it is optional.
                        categoryID = null;
                        if (!reader.IsDBNull(5))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        products.Add(new Product(
                        (int)reader["PRODUCT_ID"],
                        reader["PRODUCT_NAME"].ToString(),
                        reader["PRODUCT_DESCRIPTION"].ToString(),
                        (decimal)reader["PRODUCT_PRICE"],
                        (int)reader["PRODUCT_QUANTITY"],
                        categoryID
                        ));


                    }
                    //Return ok response and list of products.
                    return Ok(products);
                }
                //Log error and return bad response
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest(ex.ToString());
                }

            }


        }


        [HttpGet("GetProductsByName")]
        public ActionResult GetProductsByName(string productName)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"PRODUCTS\" WHERE UPPER(\"PRODUCT_NAME\") LIKE @productName;", conn);
                    cmd.Parameters.AddWithValue("@productName", "%" + productName.ToUpper() + "%");

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new List<Product>();
                    int? categoryID;
                    while (reader.Read())
                    {
                        categoryID = null;
                        if (!reader.IsDBNull(5))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        products.Add(new Product(
                            (int)reader["PRODUCT_ID"],
                            reader["PRODUCT_NAME"].ToString(),
                            reader["PRODUCT_DESCRIPTION"].ToString(),
                            (decimal)reader["PRODUCT_PRICE"],
                            (int)reader["PRODUCT_QUANTITY"],
                            categoryID
                            ));

                    }
                    //Return ok response and list of products.
                    return Ok(products);
                }
                //Log error and return bad response
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest(ex.ToString());
                }

            }


        }

    }

}
