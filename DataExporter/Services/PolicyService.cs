using DataExporter.Dtos;
using Microsoft.EntityFrameworkCore;
using DataExporter.Model;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto policy)
        {
            var newPolicy = MapPolicyFromCreatePolicyDto(policy);
            _dbContext.Policies.Add(newPolicy);
            await _dbContext.SaveChangesAsync();

            var createdPolicy = await _dbContext.Policies.FindAsync(newPolicy.Id);
            if (createdPolicy != null)
            {
                var readPolicyDto = MapPolicyToReadPolicyDto(createdPolicy);
                return readPolicyDto;
            }

            return null;
        }

        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <returns>Returns a list of ReadPoliciesDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
        {
            var policies = await _dbContext.Policies.ToListAsync();
            var dtos = policies.Select(p => MapPolicyToReadPolicyDto(p)).ToList();

            return dtos;
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ReadPolicyDto.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id)
        {
            var policy = await _dbContext.Policies.SingleAsync(p => p.Id == id);
            if (policy == null)
            {
                return null;
            }

            return MapPolicyToReadPolicyDto(policy);
        }

        /// <summary>
        /// Retrives all policies with notes
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns a list of ExportDto.</returns>
        public async Task<IList<ExportDto>> ExportDataAsync(DateTime startDate, DateTime endDate)
        {
            var policies = await _dbContext.Policies
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
                .Include(p => p.Notes)
                .ToListAsync();

            var exportDtos = policies.Select(policy => MapPolicyToExportDto(policy)).ToList();

            return exportDtos;
        }

        private ReadPolicyDto MapPolicyToReadPolicyDto(Policy policy)
        {
            return new ReadPolicyDto()
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };
        }

        private ExportDto MapPolicyToExportDto(Policy policy)
        {
            return new ExportDto()
            {
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate,
                Notes = policy.Notes == null ? new List<string>() : policy.Notes.Select(n => n.Text).ToList()
            };
        }

        private Policy MapPolicyFromCreatePolicyDto(CreatePolicyDto dto)
        {
            return new Policy()
            {
                PolicyNumber = dto.PolicyNumber,
                Premium = dto.Premium,
                StartDate = dto.StartDate
            };
        }
    }
}
