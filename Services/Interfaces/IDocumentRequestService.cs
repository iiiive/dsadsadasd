using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IDocumentRequestService
    {
        Task<IEnumerable<DocumentRequestDto>> GetAllAsync();
        Task<IEnumerable<DocumentRequestDto>> GetByUserIdAsync(int userId);

        Task<DocumentRequestDto?> GetByIdAsync(int id);
        Task<DocumentRequestDto> AddAsync(DocumentRequestDto dto);
        Task<DocumentRequestDto?> UpdateAsync(int id, DocumentRequestDto dto);

        // ✅ REQUIRED for /status endpoint
        Task<DocumentRequestDto?> UpdateStatusAsync(int id, string status);

        Task DeleteAsync(int id);
    }
}
