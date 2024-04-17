using InventoryAPI.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {

        private readonly ILogger<CategoriesController> _logger;
        private readonly IConfiguration _configuration;
        public CategoriesController(ILogger<CategoriesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /***
         * GetCategories
         * Function calls DB and returns a list of categories.
         * 
         ***/
        [HttpGet("GetCategories")]
        public ActionResult GetCategories()
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"CATEGORIES\";", conn);

                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Category> categories = new List<Category>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the categories list creating a new category using returned SQL data.
                        categories.Add(new Category(
                            (int)reader["CATEGORY_ID"],
                            reader["CATEGORY_NAME"].ToString(),
                            reader["CATEGORY_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of categories.
                    return Ok(categories);
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
         * GetCategoriesByID
         * Function calls DB and returns a single Category based off an ID.
         * 
         ***/
        [HttpGet("GetCategoriesByID")]
        public ActionResult GetCategoriesByID(int categoryID)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"CATEGORIES\" WHERE \"CATEGORY_ID\" = @categoryID;", conn);
                    cmd.Parameters.AddWithValue("@categoryID", categoryID);
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Category> categories = new List<Category>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the categories list creating a new category using returned SQL data.
                        categories.Add(new Category(
                            (int)reader["CATEGORY_ID"],
                            reader["CATEGORY_NAME"].ToString(),
                            reader["CATEGORY_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of categories.
                    return Ok(categories);
                }
            }
            //If issues occur log the error and send a bad HTTP response (500 is a bad response code).
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("GetCategoriesByName")]
        public ActionResult GetCategoriesByName(string categoryName)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"CATEGORIES\" WHERE UPPER(\"CATEGORY_NAME\") LIKE @categoryName;", conn);
                    cmd.Parameters.AddWithValue("@categoryName", "%" + categoryName.ToUpper() + "%");
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Category> categories = new List<Category>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the categories list creating a new category using returned SQL data.
                        categories.Add(new Category(
                            (int)reader["CATEGORY_ID"],
                            reader["CATEGORY_NAME"].ToString(),
                            reader["CATEGORY_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of categories.
                    return Ok(categories);
                }
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
