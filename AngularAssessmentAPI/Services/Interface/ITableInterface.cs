using AngularAssessmentAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AngularAssessmentAPI.Services.Interface
{
    public interface ITableInterface
    {
        public Task<AoTable> GetAllTablesById(Guid id);
        public Task<AoTable> AddTable(AoTable aoTable);
        public Task DeleteTable(Guid id);
        public Task<bool> UpdateTable(AoTable aoTable);
        public Task<bool> IsExists(Guid id);

        public Task<ICollection<AoTable>> GetAllTables();

    }
}
