﻿using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarketplacesController : ControllerBase
    {

        private readonly ILogger<MarketplacesController> _logger;
        private readonly IConfiguration _configuration;
        public MarketplacesController(ILogger<MarketplacesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /***
         * GetMarketplaces
         * Function calls DB and returns a list of Marketplaces.
         * 
         ***/
        [HttpGet("GetMarketplaces")]
        public ActionResult GetMarketplaces()
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"MARKETPLACES\";", conn);

                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Marketplace> marketplaces = new List<Marketplace>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the Marketplaces list creating a new marketplace using returned SQL data.
                        marketplaces.Add(new Marketplace(
                            (int)reader["MARKETPLACE_ID"],
                            reader["MARKETPLACE_NAME"].ToString(),
                            reader["MARKETPLACE_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of Marketplaces.
                    return Ok(marketplaces);
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
        * GetMarketplaces
        * Function calls DB and returns a marketplace that matches a single ID.
        * 
        ***/
        [HttpGet("GetMarketplacesByID")]
        public ActionResult GetMarketplacesByID(int marketplaceID)
        {
            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"MARKETPLACES\" WHERE \"MARKETPLACE_ID\" = @marketplaceID;", conn);
                    cmd.Parameters.AddWithValue("@marketplaceID", marketplaceID);
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Marketplace> marketplaces = new List<Marketplace>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the Marketplaces list creating a new marketplace using returned SQL data.
                        marketplaces.Add(new Marketplace(
                            (int)reader["MARKETPLACE_ID"],
                            reader["MARKETPLACE_NAME"].ToString(),
                            reader["MARKETPLACE_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of Marketplaces.
                    return Ok(marketplaces);
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
        * GetMarketplaces
        * Function calls DB and returns a list of Marketplaces that match a like clause (like autocomplete).
        * 
        ***/
        [HttpGet("GetMarketplacesByName")]
        public ActionResult GetMarketplacesByName(string marketplaceName)
        {

            try
            {
                //Open database connection getting the config value from the appsettings.
                using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetSection("ConfigSettings").GetSection("DbConnection").Value))
                {
                    conn.Open();
                    //Create the command for the SQL to execute.
                    NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM \"MARKETPLACES\" WHERE UPPER(\"MARKETPLACE_NAME\") LIKE @marketplaceName;", conn);
                    cmd.Parameters.AddWithValue("@marketplaceName", "%" + marketplaceName.ToUpper() + "%");
                    //Execute the SQL Query.
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    List<Marketplace> marketplaces = new List<Marketplace>();
                    //Read through the returned items.
                    while (reader.Read())
                    {
                        //Add to the Marketplaces list creating a new marketplace using returned SQL data.
                        marketplaces.Add(new Marketplace(
                            (int)reader["MARKETPLACE_ID"],
                            reader["MARKETPLACE_NAME"].ToString(),
                            reader["MARKETPLACE_DESCRIPTION"].ToString()
                            ));

                    }

                    //Once completed send an ok HTTP response (200 is a successful response code) with the list of Marketplaces.
                    return Ok(marketplaces);
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