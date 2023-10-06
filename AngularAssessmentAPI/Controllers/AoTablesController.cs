using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularAssessmentAPI.Data;
using AngularAssessmentAPI.Models;
using AngularAssessmentAPI.Services.Interface;

namespace AngularAssessmentAPI.Controllers
{
    [Route("Tables/")]
    [ApiController]
    public class AoTablesController : ControllerBase
    {
        private readonly ITableInterface TableInterface;

        public AoTablesController(ITableInterface TableInterface)
        {
            this.TableInterface = TableInterface;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTables()
        {
            try
            {
                var tables = await TableInterface.GetAllTables();
                if (tables == null || !tables.Any())
                {
                    return BadRequest("No Table Found");
                }
                else
                {
                    return Ok(tables);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllTablesById([FromRoute] Guid id)
        {
            try
            {
                var table = await TableInterface.GetAllTablesById(id);
                if (table == null)
                {
                    return BadRequest("Table Not Found");
                }
                else
                {
                    return Ok(table);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTable([FromBody] AoTable aoTable)
        {
            try
            {

                var isTrue = await TableInterface.IsExists(aoTable.Id);

                if (isTrue)
                {
                    var success = await TableInterface.UpdateTable(aoTable);

                    if (success)
                    {
                        var data = new { status = "Success" };
                        return Ok(data);
                    }
                    else
                    {
                        return BadRequest("Update failed");
                    }
                }
                else
                {
                    return BadRequest("Update Failed! Table not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] AoTable aoTable)
        {
            try
            {
                if (aoTable != null)
                {

                    var newForm = await TableInterface.AddTable(aoTable);

                    return Ok(newForm);

                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable([FromRoute] Guid id)
        {
            try
            {
                var isExists = await TableInterface.IsExists(id);

                if (isExists)
                {
                    await TableInterface.DeleteTable(id);

                    var data = new { status = "Deleted" };
                    return Ok(data);
                }
                else
                {
                    return BadRequest("Delete Failed! Table not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
