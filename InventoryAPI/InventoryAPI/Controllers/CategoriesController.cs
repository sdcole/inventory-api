using InventoryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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

        /***
         * AddCategory
         * 
         * Stores a passed category to DB.
         ***/
        [HttpPost("AddCategory")]
        public ActionResult AddCategory([FromBody] Category newCategory)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO \"CATEGORIES\" (\"CATEGORY_NAME\", \"CATEGORY_DESCRIPTION\") " +
                        "VALUES (@categoryName, @categoryDescription);", conn);
                    cmd.Parameters.AddWithValue("@categoryName", newCategory.categoryName);
                    cmd.Parameters.AddWithValue("@categoryDescription", newCategory.categoryDescription);


                    cmd.ExecuteNonQuery();

                    //Return ok response and list of categorys.

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
         * EditCategory
         * 
         * Edits a category based off categoryID
         ***/
        [HttpPut("EditCategory")]
        public ActionResult EditCategory([FromBody] Category newCategory, int categoryID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;


                    //Check if a category with that ID exists.
                    cmd.CommandText = "SELECT * FROM \"CATEGORIES\" WHERE \"CATEGORY_ID\" = @categoryID;";
                    cmd.Parameters.AddWithValue("@categoryID", categoryID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    //If it exists we update it.
                    if (reader.HasRows)
                    {
                        //Dispose of reader (cannot do this async since we are using the cmd).
                        reader.Dispose();
                        cmd.CommandText = "UPDATE \"CATEGORIES\" SET \"CATEGORY_NAME\" = @categoryName, \"CATEGORY_DESCRIPTION\" = @categoryDescription WHERE \"CATEGORY_ID\" = @categoryID;";
                        cmd.Parameters.AddWithValue("@categoryName", newCategory.categoryName);
                        cmd.Parameters.AddWithValue("@categoryDescription", newCategory.categoryDescription);
                        cmd.Parameters.AddWithValue("@updateID", categoryID);


                        cmd.ExecuteNonQuery();

                        //Return ok response and list of categories.
                    }
                    else
                    {
                        reader.DisposeAsync();
                        return BadRequest("Category_ID Not Valid");
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
         * DeleteCategory
         * 
         * Deletes a category based off a given categoryID.
         ***/
        [HttpDelete("DeleteCategory")]
        public ActionResult DeleteCategory(int categoryID)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
            {
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = conn;
                    //Check if categoryID is available.
                    cmd.CommandText = "SELECT * FROM \"CATEGORIES\" WHERE \"CATEGORY_ID\" = @categoryID;";
                    cmd.Parameters.AddWithValue("@categoryID", categoryID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();


                    if (reader.HasRows)
                    {
                        reader.Dispose();
                        cmd.CommandText = "DELETE FROM \"CATEGORIES\" WHERE \"CATEGORY_ID\" = @categoryID;";
                        cmd.Parameters.AddWithValue("@categoryID", categoryID);


                        cmd.ExecuteNonQuery();

                        //Return ok response and stating that it was deleted from the database.
                    }
                    else
                    {
                        //If the ID wasn't found we know there was an error.
                        reader.DisposeAsync();
                        return BadRequest("Category_ID Not Valid");
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
