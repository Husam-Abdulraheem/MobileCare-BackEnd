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
    public class RepairOrdersController : ControllerBase
    {

        private readonly AppDbContext _db;

        public RepairOrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create-via-stored-procedure")]
        public async Task<IActionResult> CreateViaStoredProcedure([FromBody] CreateRepairOrderDto dto)
        {
            try
            {
                var sql = @"CALL CreateRepairRequest(@p_CustomerName, @p_PhoneNumber, @p_Brand, 
                        @p_Model, @p_IMEI, @p_ProblemDescription, @p_DeviceCondition, @p_EstimatedCost, @p_UserId, @p_Notes);";

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
                new MySqlParameter("@p_Notes", dto.Notes),
            };

                await _db.Database.ExecuteSqlRawAsync(sql, parameters);

                return Ok(new { message = "Order Created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpGet("Get-all-orders")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _db.Repairorders
                .Include(r => r.Customer)
                .Include(r => r.Device)
                .Include(r => r.User)
                .ToListAsync();

            if (orders.Count == 0)
                return Ok("There is no Orders");

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _db.Repairorders
                .Include(r => r.Customer)
                .Include(r => r.Device)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RepairOrderId == id);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRepairOrderDto dto)
        {
            var order = await _db.Repairorders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found");

            order.Status = dto.Status ?? order.Status;
            order.Notes = dto.Notes ?? order.Notes;
            order.EstimatedCost = dto.EstimatedCost ?? order.EstimatedCost;
            order.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok("Order updated successfully");
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Repairorders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found");

            _db.Repairorders.Remove(order);
            await _db.SaveChangesAsync();

            return Ok("Deleted");
        }




    }
}
