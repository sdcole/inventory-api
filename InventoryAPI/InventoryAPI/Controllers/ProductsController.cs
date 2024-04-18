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

        /***
         * GetProductsByName
         * 
         * Returns a list of products based of a DB LIKE %name% statement.
         * 
         ***/
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

        /***
         * AddProduct
         * 
         * Stores a passed product to DB.
         ***/
        [HttpPost("AddProduct")]
        public ActionResult AddProduct([FromBody] Product newProduct) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"PRODUCTS\" (\"PRODUCT_NAME\", \"PRODUCT_DESCRIPTION\", \"PRODUCT_PRICE\", \"PRODUCT_QUANTITY\", \"CATEGORY_ID\") " +
                        "VALUES (@productName, @productDescription, @productPrice, @productQuantity, @categoryID);", conn);
                    cmd.Parameters.AddWithValue("@productName", newProduct.productName);
                    cmd.Parameters.AddWithValue("@productDescription", newProduct.productDescription);
                    cmd.Parameters.AddWithValue("@productPrice", newProduct.productPrice);
                    cmd.Parameters.AddWithValue("@productQuantity", newProduct.productQuantity);
                    //If no category gets assigned then send it as DBNULL.
                    if (newProduct.categoryID == null)
                    {
                        cmd.Parameters.AddWithValue("@categoryID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@categoryID", newProduct.categoryID);
                    }
                    

                    cmd.ExecuteNonQuery();
                    
                    //Return ok response and list of products.
                    
                }
                //Log error and return bad response
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest(ex.ToString());
                }

            }
            return Ok();
        }

        /***
         * EditProduct
         * 
         * Edits a product based off productID
         ***/
        [HttpPut("EditProduct")]
        public ActionResult EditProduct([FromBody] Product newProduct, int productID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;


                    //Check if a product with that ID exists.
                    cmd.CommandText = "SELECT * FROM \"PRODUCTS\" WHERE \"PRODUCT_ID\" = @productID;";
                    cmd.Parameters.AddWithValue("@productID", productID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    //If it exists we update it.
                    if (reader.HasRows)
                    {
                        //Dispose of reader (cannot do this async since we are using the cmd).
                        reader.Dispose();
                        cmd.CommandText = "UPDATE \"PRODUCTS\" SET \"PRODUCT_NAME\" = @productName, \"PRODUCT_DESCRIPTION\" = @productDescription, \"PRODUCT_PRICE\" = @productPrice, \"PRODUCT_QUANTITY\" = @productQuantity, \"CATEGORY_ID\" = @categoryID WHERE \"PRODUCT_ID\" = @updateID;";
                        cmd.Parameters.AddWithValue("@productName", newProduct.productName);
                        cmd.Parameters.AddWithValue("@productDescription", newProduct.productDescription);
                        cmd.Parameters.AddWithValue("@productPrice", newProduct.productPrice);
                        cmd.Parameters.AddWithValue("@productQuantity", newProduct.productQuantity);
                        //If no category gets assigned then send it as DBNULL.
                        if (newProduct.categoryID == null)
                        {
                            cmd.Parameters.AddWithValue("@categoryID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@categoryID", newProduct.categoryID);
                        }
                        cmd.Parameters.AddWithValue("@updateID", productID);


                        cmd.ExecuteNonQuery();

                        //Return ok response and list of products.
                    }
                    else
                    {
                        reader.DisposeAsync();
                        return BadRequest("Product_ID Not Valid");
                    }
                    return Ok();



                }
                //Log error and return bad response
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return BadRequest(ex.ToString());
                }

            }
        }

        /***
         * DeleteProduct
         * 
         * Deletes a product based off a given productID.
         ***/
        [HttpDelete("DeleteProduct")]
        public ActionResult DeleteProduct(int productID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;
                    //Check if productID is available.
                    cmd.CommandText = "SELECT * FROM \"PRODUCTS\" WHERE \"PRODUCT_ID\" = @productID;";
                    cmd.Parameters.AddWithValue("@productID", productID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    
                    if (reader.HasRows)
                    {
                        reader.Dispose();
                        cmd.CommandText = "DELETE FROM \"PRODUCTS\" WHERE \"PRODUCT_ID\" = @productID;";
                        cmd.Parameters.AddWithValue("@productID", productID);

                        
                        cmd.ExecuteNonQuery();

                        //Return ok response and stating that it was deleted from the database.
                    }
                    else
                    {
                        //If the ID wasn't found we know there was an error.
                        reader.DisposeAsync();
                        return BadRequest("Product_ID Not Valid");
                    }
                    return Ok();



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
