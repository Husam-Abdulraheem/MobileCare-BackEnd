using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileCare.DTOs;
using MobileCare.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace MobileCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairOrdersController : ControllerBase
    {

        private readonly AppDbContext _db;

        public RepairOrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateViaStoredProcedure([FromBody] CreateRepairOrderDto dto)
        {
            try
            {
                var sql = @"CALL CreateRepairRequest(@p_CustomerName, @p_PhoneNumber, @p_Brand, 
                        @p_Model, @p_IMEI, @p_ProblemDescription, @p_DeviceCondition, @p_EstimatedCost, @p_UserId);";

                var parameters = new[]
                {
                new MySqlParameter("@p_CustomerName", dto.CustomerName),
                new MySqlParameter("@p_PhoneNumber", dto.PhoneNumber),
                new MySqlParameter("@p_Brand", dto.DeviceBrand),
                new MySqlParameter("@p_Model", dto.DeviceModel),
                new MySqlParameter("@p_IMEI", dto.IMEI),
                new MySqlParameter("@p_ProblemDescription", dto.ProblemDescription),
                new MySqlParameter("@p_DeviceCondition", dto.DeviceCondition),
                new MySqlParameter("@p_EstimatedCost", dto.EstimatedCost),
                new MySqlParameter("@p_UserId", dto.UserId),
            };

                await _db.Database.ExecuteSqlRawAsync(sql, parameters);

                return Ok(new { message = "Order Created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpGet("get-orders-by-user")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
            var orders = new List<RepairOrderDto>();

            using var connection = _db.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "CALL GetAllOrdersByUser(@param_UserId)";
            command.CommandType = CommandType.Text;

            var param = command.CreateParameter();
            param.ParameterName = "@param_UserId";
            param.Value = userId;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new RepairOrderDto
                {
                    RepairOrderId = reader.GetInt32(0),
                    CustomerFullName = reader.GetString(1),
                    PhoneNumber = reader.GetString(2),
                    Brand = reader.GetString(3),
                    Model = reader.GetString(4),
                    IMEI = reader.IsDBNull(5) ? null : reader.GetString(5),
                    ProblemDescription = reader.IsDBNull(6) ? null : reader.GetString(6),
                    DeviceCondition = reader.IsDBNull(7) ? null : reader.GetString(7),
                    UserFullName = reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9),
                    UpdatedAt = reader.GetDateTime(10),
                    Status = reader.GetString(11),
                    EstimatedCost = reader.IsDBNull(12) ? 0 : reader.GetDecimal(12),
                    TrackCode = reader.GetString(13)

                });
            }

            return Ok(orders);
        }





        [HttpPut("update-order/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateRepairOrderDto dto)
        {
            using var connection = _db.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "CALL UpdateRepairOrderById(@p_RepairOrderId, @p_CustomerFullName, @p_PhoneNumber, @p_Brand, @p_Model, @p_IMEI, @p_ProblemDescription, @p_DeviceCondition, @p_EstimatedCost, @p_Status)";

            command.Parameters.Add(new MySqlParameter("@p_RepairOrderId", id));
            command.Parameters.Add(new MySqlParameter("@p_CustomerFullName", (object?)dto.CustomerFullName ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_PhoneNumber", (object?)dto.PhoneNumber ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_Brand", (object?)dto.Brand ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_Model", (object?)dto.Model ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_IMEI", (object?)dto.IMEI ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_ProblemDescription", (object?)dto.ProblemDescription ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_DeviceCondition", (object?)dto.DeviceCondition ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_EstimatedCost", (object?)dto.EstimatedCost ?? DBNull.Value));
            command.Parameters.Add(new MySqlParameter("@p_Status", (object?)dto.Status ?? DBNull.Value));

            await command.ExecuteNonQueryAsync();
            return Ok("Order updated successfully");
        }




        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(int repairOrderId, string status)
        {
            try
            {
                var allowedStatuses = new[] { "Pending", "InProgress", "Ready", "Collected" };
                if (!allowedStatuses.Contains(status))
                {
                    return BadRequest(new { error = "Invalid status value." });
                }

                using var connection = _db.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "CALL UpdateRepairOrderStatus(@p_RepairOrderId, @p_Status)";
                command.CommandType = CommandType.Text;

                var paramId = command.CreateParameter();
                paramId.ParameterName = "@p_RepairOrderId";
                paramId.Value = repairOrderId;
                command.Parameters.Add(paramId);

                var paramStatus = command.CreateParameter();
                paramStatus.ParameterName = "@p_Status";
                paramStatus.Value = status;
                command.Parameters.Add(paramStatus);

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }





        [HttpDelete("delete-order/{id}")]
        public async Task<IActionResult> DeleteRepairOrder(int id)
        {
            try
            {
                var sql = "CALL DeleteRepairOrderById(@p_RepairOrderId);";
                var parameters = new[] {
            new MySqlParameter("@p_RepairOrderId", id)
        };

                await _db.Database.ExecuteSqlRawAsync(sql, parameters);

                return Ok(new { message = "Repair order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



        [HttpGet("user-stats/{userId}")]
        public async Task<IActionResult> GetUserRepairStats(int userId)
        {
            try
            {
                var connection = _db.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
            SELECT 
                CountRepairOrders(@userId) AS OrderCount,
                AverageRepairCost(@userId) AS AvgCost;
        ";
                command.CommandType = CommandType.Text;

                command.Parameters.Add(new MySqlParameter("@userId", userId));

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var result = new
                    {
                        UserId = userId,
                        OrderCount = reader["OrderCount"] != DBNull.Value ? Convert.ToInt32(reader["OrderCount"]) : 0,
                        AvgCost = reader["AvgCost"] != DBNull.Value ? Convert.ToDecimal(reader["AvgCost"]) : 0
                    };

                    return Ok(result);
                }

                return NotFound("No data found for the given user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




        [HttpGet("user-revenue/{userId}")]
        public async Task<IActionResult> GetTotalRevenueByUser(int userId)
        {
            try
            {
                var conn = _db.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT GetTotalRevenueByUser(@userId);";
                cmd.Parameters.Add(new MySqlParameter("@userId", userId));

                var result = await cmd.ExecuteScalarAsync();
                var revenue = Convert.ToDecimal(result);

                return Ok(new { userId = userId, totalRevenue = revenue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }





    }
}
