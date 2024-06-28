using InventoryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of inventory entries.
                    return Ok(inventoryList);
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        /***
         * GetInventoryByID
         * Function calls DB and returns a list of all entries in the inventory table.
         * 
         ***/
        [HttpGet("GetInventoryByID")]
        public ActionResult GetInventoryByID(int inventoryID)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"INVENTORY\" WHERE \"INVENTORY_ID\" = @inventoryID;", conn);
                    cmd.Parameters.AddWithValue("@inventoryID", inventoryID);
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

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of inventory entries.
                    return Ok(inventoryList);
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }


        /***
         * GetInventoryByDescription
         * Function calls DB and returns a list of entries in the inventory table based on description.
         * 
         ***/
        [HttpGet("GetInventoryByDescription")]
        public ActionResult GetInventoryByDescription(string inventoryDescription)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"INVENTORY\" WHERE \"INVENTORY_DESCRIPTION\" LIKE @inventoryDescription;", conn);
                    cmd.Parameters.AddWithValue("@inventoryDescription", "%" + inventoryDescription + "%");
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

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of inventory entries.
                    return Ok(inventoryList);
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }


        /***
        * AddInventory
        * Adds an inventory object to the database.
        * 
        ***/
        [HttpPost("AddInventory")]
        public ActionResult AddInventory([FromBody]Inventory newInventory)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"INVENTORY\" (\"PRODUCT_ID\", \"ACTION_ID\", \"QUANTITY_CHANGED\", \"TIMESTAMP\") VALUES" +
                        "(@productID, @actionID, @quantityChanged, @timestamp);", conn);
                    cmd.Parameters.AddWithValue("@productID", newInventory.productID);
                    cmd.Parameters.AddWithValue("@actionID", newInventory.actionID);
                    cmd.Parameters.AddWithValue("@quantityChanged", newInventory.quantityChanged);
                    cmd.Parameters.AddWithValue("@timestamp", newInventory.timestamp);

                    //Add inventory entry to database.
                    cmd.ExecuteNonQuery();
                    

                    //Once completed send an ok HTTP response (200 is a successful response code);
                    return Ok();
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }


        /***
        * EditInventory
        * Edits an inventory object to the database.
        * 
        ***/
        [HttpPut("EditInventory")]
        public ActionResult EditInventory([FromBody] Inventory newInventory, int inventoryID)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT * FROM \"INVENTORY\" WHERE \"INVENTORY_ID\" = @inventoryID";
                    cmd.Parameters.AddWithValue("@inventoryID", inventoryID);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Dispose();

                        cmd.CommandText = "UPDATE \"INVENTORY\" SET (\"PRODUCT_ID\" = @productID, \"ACTION_ID\" = @actionID, \"QUANTITY_CHANGED\" = @quantityChanged, \"TIMESTAMP\" = @timestamp) " +
                            "WHERE \"INVENTORY_ID\" = @inventoryID";
                        cmd.Parameters.AddWithValue("@productID", newInventory.productID);
                        cmd.Parameters.AddWithValue("@actionID", newInventory.actionID);
                        cmd.Parameters.AddWithValue("@quantityChanged", newInventory.quantityChanged);
                        cmd.Parameters.AddWithValue("@timestamp", newInventory.timestamp);

                        cmd.Parameters.AddWithValue("@inventoryID", inventoryID);

                        //Add inventory entry to database.
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.DisposeAsync();
                        return BadRequest("InventoryID Not Found");
                    }


                    //Once completed send an ok HTTP response (200 is a successful response code);
                    return Ok();
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
        /***
        * DeleteInventory
        * Deletes Inventory ID based on the Inventory ID
        * 
        ***/
        [HttpDelete("DeleteInventory")]
        public ActionResult DeleteInventory(int inventoryID)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT * FROM \"INVENTORY\" WHERE \"INVENTORY_ID\" = @inventoryID";
                    cmd.Parameters.AddWithValue("@inventoryID", inventoryID);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    //Check if there is an inventory with given ID
                    if (reader.HasRows)
                    {
                        reader.Dispose();

                        cmd.CommandText = "DELETE FROM \"INVENTORY\" WHERE \"INVENTORY_ID\" = @inventoryID";
                        cmd.Parameters.AddWithValue("@inventoryID", inventoryID);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.DisposeAsync();
                        return BadRequest("InventoryID Not Found");
                    }


                    return Ok();
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

    }
}