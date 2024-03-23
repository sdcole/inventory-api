using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
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
                };
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
                };
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
 