using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {

        private readonly ILogger<InventoryController> _logger;
        private readonly IConfiguration _configuration;
        public InventoryController(ILogger<InventoryController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /***
         * GetInventory
         * Function calls DB and returns a list of all entries in the inventory table.
         * 
         ***/
        [HttpGet("GetInventory")]
        public ActionResult GetInventory()
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"INVENTORY\";", conn);

                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Inventory> inventoryList = new List<Inventory>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the inventory list creating a new Inventory Object using returned SQL data.
                        inventoryList.Add(new Inventory(
                            (int)reader["INVENTORY_ID"],
                            (int)reader["PRODUCT_ID"],
                            (int)reader["ACTION_ID"],
                            (int)reader["QUANTITY_CHANGED"],
                            (DateTime)reader["TIMESTAMP"]
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of categories.
                    return Ok(inventoryList);
                };
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}