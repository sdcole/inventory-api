using InventoryAPI.Models;
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
        [HttpGet(Name = "GetCategories")]
        public ActionResult GetCategories()
        {
            try
            {

                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"CATEGORIES\";", conn);

                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Category> categories = new List<Category>();
                    while (reader.Read())
                    {
                            categories.Add(new Category(
                            (int)reader["CATEGORY_ID"],
                            reader["CATEGORY_NAME"].ToString(),
                            reader["CATEGORY_DESCRIPTION"].ToString()
                            ));

                    }

                    return Ok(categories);
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.ToString());
            }
            

            
        }
    }

}
