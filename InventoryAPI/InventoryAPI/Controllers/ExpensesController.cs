using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : ControllerBase
    {

        private readonly ILogger<ExpensesController> _logger;
        private readonly IConfiguration _configuration;
        public ExpensesController(ILogger<ExpensesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /***
         * GetExpenses
         * Function calls DB and returns a list of expenses.
         * 
         ***/
        [HttpGet("GetExpenses")]
        public ActionResult GetExpenses()
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"EXPENSES\";", conn);

                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Expense> expenses = new List<Expense>();

                    int? categoryID;
                    int? inventoryID;
                    int? marketplaceID;
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        categoryID = null;
                        inventoryID = null;
                        marketplaceID = null;

                        //This logic will check if the associations for the expense are avaiable (if so add them otherwise return null).
                        if (!reader.IsDBNull(4))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        if (!reader.IsDBNull(5))
                        {
                            inventoryID = (int?)reader["INVENTORY_ID"];
                        }
                        if (!reader.IsDBNull(6))
                        {
                            marketplaceID = (int?)reader["MARKETPLACE_ID"];
                        }
                        //Add to the expenses list creating a new expense using returned SQL data.
                        expenses.Add(new Expense(
                            (int)reader["EXPENSE_ID"],
                            reader["EXPENSE_DESCRIPTION"].ToString(),
                            (decimal)reader["EXPENSE_AMOUNT"],
                            (DateTime)reader["EXPENSE_DATE"],
                            categoryID,
                            inventoryID,
                            marketplaceID
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of expenses.
                    return Ok(expenses);
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
         * GetExpensesByID
         * Function calls DB and returns a single Expense based off an ID.
         * 
         ***/
        [HttpGet("GetExpensesByID")]
        public ActionResult GetExpensesByID(int expenseID)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"EXPENSES\" WHERE \"EXPENSE_ID\" = @expenseID;", conn);
                    cmd.Parameters.AddWithValue("@expenseID", expenseID);
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Expense> expenses = new List<Expense>();
                    int? categoryID;
                    int? inventoryID;
                    int? marketplaceID;
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        categoryID = null;
                        inventoryID = null;
                        marketplaceID = null;

                        //This logic will check if the associations for the expense are avaiable (if so add them otherwise return null).
                        if (!reader.IsDBNull(4))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        if (!reader.IsDBNull(5))
                        {
                            inventoryID = (int?)reader["INVENTORY_ID"];
                        }
                        if (!reader.IsDBNull(6))
                        {
                            marketplaceID = (int?)reader["MARKETPLACE_ID"];
                        }
                        //Add to the expenses list creating a new expense using returned SQL data.
                        expenses.Add(new Expense(
                            (int)reader["EXPENSE_ID"],
                            reader["EXPENSE_DESCRIPTION"].ToString(),
                            (decimal)reader["EXPENSE_AMOUNT"],
                            (DateTime)reader["EXPENSE_DATE"],
                            categoryID,
                            inventoryID,
                            marketplaceID
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of expenses.
                    return Ok(expenses);
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
         * GetExpensesByName
         * Returns a list of expenses that match a SQL Like statement of the input string.
         * 
         ***/
        [HttpGet("GetExpensesByName")]
        public ActionResult GetExpensesByName(string expenseDescription)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"EXPENSES\" WHERE UPPER(\"EXPENSE_DESCRIPTION\") LIKE @expenseDescription;", conn);
                    cmd.Parameters.AddWithValue("@expenseDescription", "%" + expenseDescription.ToUpper() + "%");
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Expense> expenses = new List<Expense>();
                    int? categoryID;
                    int? inventoryID;
                    int? marketplaceID;
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        categoryID = null;
                        inventoryID = null;
                        marketplaceID = null;

                        //This logic will check if the associations for the expense are avaiable (if so add them otherwise return null).
                        if (!reader.IsDBNull(4))
                        {
                            categoryID = (int?)reader["CATEGORY_ID"];
                        }
                        if (!reader.IsDBNull(5))
                        {
                            inventoryID = (int?)reader["INVENTORY_ID"];
                        }
                        if (!reader.IsDBNull(6))
                        {
                            marketplaceID = (int?)reader["MARKETPLACE_ID"];
                        }
                        //Add to the expensess list creating a new expense using returned SQL data.
                        expenses.Add(new Expense(
                            (int)reader["EXPENSE_ID"],
                            reader["EXPENSE_DESCRIPTION"].ToString(),
                            (decimal)reader["EXPENSE_AMOUNT"],
                            (DateTime)reader["EXPENSE_DATE"],
                            categoryID,
                            inventoryID,
                            marketplaceID
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of expenses.
                    return Ok(expenses);
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
         * AddExpense
         * 
         * Stores a passed expense to DB.
         ***/
        [HttpPost("AddExpense")]
        public ActionResult AddExpense([FromBody] Expense newExpense)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"EXPENSES\" (\"EXPENSE_DESCRIPTION\", \"EXPENSE_AMOUNT\", \"EXPENSE_DATE\", \"CATEGORY_ID\",  \"INVENTORY_ID\", \"MARKETPLACE_ID\") " +
                        "VALUES (@expenseDescription, @expenseAmount, @expenseDate, @categoryID, @inventoryID, @marketplaceID);", conn);
                    cmd.Parameters.AddWithValue("@expenseDescription", newExpense.expenseDescription);
                    cmd.Parameters.AddWithValue("@expenseAmount", newExpense.expenseAmount);
                    cmd.Parameters.AddWithValue("@expenseDate", newExpense.expenseDate);

                    // If no category gets assigned then send it as DBNULL.
                        if (newExpense.categoryID == null)
                    {
                        cmd.Parameters.AddWithValue("@categoryID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@categoryID", newExpense.categoryID);
                    }
                    //If no inventory gets assigned then send it as DBNULL.
                    if (newExpense.inventoryID == null)
                    {
                        cmd.Parameters.AddWithValue("@inventoryID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@categoryID", newExpense.inventoryID);
                    }
                    //If no marketplace gets assigned then send it as DBNULL.
                    if (newExpense.marketplaceID == null)
                    {
                        cmd.Parameters.AddWithValue("@marketplaceID", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@marketplaceID", newExpense.marketplaceID);
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
         * EditExpense
         * 
         * Edits a expense based off expenseID
         ***/
        [HttpPut("EditExpense")]
        public ActionResult EditExpense([FromBody] Expense newExpense, int expenseID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;


                    //Check if a expense with that ID exists.
                    cmd.CommandText = "SELECT * FROM \"EXPENSES\" WHERE \"EXPENSE_ID\" = @expenseID;";
                    cmd.Parameters.AddWithValue("@expenseID", expenseID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    //If it exists we update it.
                    if (reader.HasRows)
                    {
                        //Dispose of reader (cannot do this async since we are using the cmd).
                        reader.Dispose();
                        cmd.CommandText = "UPDATE \"EXPENSES\" SET \"EXPENSE_DESCRIPTION\" = @expenseDescription, \"EXPENSE_AMOUNT\" = @expenseAmount, \"EXPENSE_DATE\" = @expenseDate, \"CATEGORY_ID\" = @categoryID, \"INVENTORY_ID\" = @inventoryID, \"MARKETPLACE_ID\" = @marketplaceID WHERE \"EXPENSE_ID\" = @updateID;";
                        cmd.Parameters.AddWithValue("@expenseDescription", newExpense.expenseDescription);
                        cmd.Parameters.AddWithValue("@expenseAmount", newExpense.expenseAmount);
                        cmd.Parameters.AddWithValue("@expenseDate", newExpense.expenseDate);

                        //If no category gets assigned then send it as DBNULL.
                        if (newExpense.categoryID == null)
                        {
                            cmd.Parameters.AddWithValue("@categoryID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@categoryID", newExpense.categoryID);
                        }
                        //If no inventory gets assigned then send it as DBNULL.
                        if (newExpense.inventoryID == null)
                        {
                            cmd.Parameters.AddWithValue("@inventoryID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@categoryID", newExpense.inventoryID);
                        }
                        //If no marketplace gets assigned then send it as DBNULL.
                        if (newExpense.marketplaceID == null)
                        {
                            cmd.Parameters.AddWithValue("@marketplaceID", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@marketplaceID", newExpense.marketplaceID);
                        }
                        cmd.Parameters.AddWithValue("@updateID", expenseID);


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
    }

}
 