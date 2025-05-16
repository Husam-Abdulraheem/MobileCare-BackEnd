using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileCare.DTOs;
using MobileCare.Models;
using MySql.Data.MySqlClient;

namespace MobileCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CustomerController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("track-order/{trackCode}")]
        public async Task<IActionResult> TrackOrder(string trackCode)
        {
            try
            {
                var connection = _db.Database.GetDbConnection();

                await using (connection)
                {
                    await connection.OpenAsync();

                    await using var command = connection.CreateCommand();
                    command.CommandText = "TrackRepairOrderByCode";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var param = command.CreateParameter();
                    param.ParameterName = "@p_TrackCode";
                    param.Value = trackCode;
                    command.Parameters.Add(param);

                    await using var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows)
                        return NotFound(new { message = "No order found with this tracking code." });

                    await reader.ReadAsync();

                    var result = new RepairOrderTrackingDto
                    {
                        RepairOrderId = reader.GetInt32(reader.GetOrdinal("RepairOrderId")),
                        CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                        Brand = reader.GetString(reader.GetOrdinal("Brand")),
                        Model = reader.GetString(reader.GetOrdinal("Model")),
                        IMEI = reader.GetString(reader.GetOrdinal("IMEI")),
                        ProblemDescription = reader.GetString(reader.GetOrdinal("ProblemDescription")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        LastUpdatedAt = reader.GetDateTime(reader.GetOrdinal("LastUpdatedAt"))
                    };

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



    }
}
