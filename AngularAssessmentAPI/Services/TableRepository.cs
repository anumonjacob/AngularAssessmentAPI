using AngularAssessmentAPI.Models;
using AngularAssessmentAPI.Data;
using AngularAssessmentAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AngularAssessmentAPI.Services
{
    public class TableRepository : ITableInterface
    {
        private readonly TableDbContext DbContext;
        public TableRepository(TableDbContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public async Task<AoTable> AddTable(AoTable aoTable)
        {
            aoTable.Id = Guid.NewGuid();
            await DbContext.AoTables.AddAsync(aoTable);
            await DbContext.SaveChangesAsync();
            return aoTable;
            
        }

        public async Task DeleteTable(Guid id)
        {
            var tableToDelete = await DbContext.AoTables.FindAsync(id);
            if (tableToDelete != null)
            {
                DbContext.AoTables.Remove(tableToDelete);
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<AoTable>> GetAllTables()
        {
            var tables = await DbContext.AoTables.ToListAsync();
            if (tables != null)
                return tables;
            else
                throw new InvalidOperationException("No Tables found");
        }
        public async Task<AoTable> GetAllTablesById(Guid id)
        {

            var table = await DbContext.AoTables.FindAsync(id);
            if (table != null)
                return table;
            else
                throw new InvalidOperationException("Table not found");
        }

        public async Task<bool> IsExists(Guid id)
        {
            return await DbContext.AoTables.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> UpdateTable(AoTable aoTable)
        {
            var existingTable = await DbContext.AoTables.FindAsync(aoTable.Id);

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
