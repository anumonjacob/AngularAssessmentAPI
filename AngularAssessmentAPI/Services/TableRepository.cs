using AngularAssessmentAPI.Models;
using AngularAssessmentAPI.Data;
using AngularAssessmentAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace AngularAssessmentAPI.Services
{
    public class TableRepository : ITableInterface
    {
        private readonly AssessmentDbContext DbContext;
        public TableRepository(AssessmentDbContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<AoTable> AddTable(AoTable aoTable)
        {
            if (aoTable != null)
            {
                aoTable.Id = Guid.NewGuid();
                await DbContext.AoTables.AddAsync(aoTable);
                await DbContext.SaveChangesAsync();
                return aoTable;
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteTable(Guid id)
        {
            var tableToDelete = await DbContext.AoTables.FirstOrDefaultAsync(t => t.Id == id);

            if (tableToDelete != null)
            {
                DbContext.AoTables.Remove(tableToDelete);
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<AoTable>> GetAllTables()
        {
            var tables = await DbContext.AoTables.ToListAsync();
            return tables.Count > 0 ? tables : null;
        }
        public async Task<AoTable> GetAllTablesById(Guid id)
        {

            var table = await DbContext.AoTables.FirstOrDefaultAsync(t => t.Id == id);
            return table != null ? table : null;
        }

        public async Task<bool> IsExists(Guid id)
        {
            return await DbContext.AoTables.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> UpdateTable(Guid id, AoTable aoTable)
        {
            var existingTable = await DbContext.AoTables.FirstOrDefaultAsync(t => t.Id == id);

            if (existingTable == null)
            {
                return false;
            }
            DbContext.Entry(existingTable).CurrentValues.SetValues(aoTable);

            await DbContext.SaveChangesAsync();

            return true;
        }

    }
}
