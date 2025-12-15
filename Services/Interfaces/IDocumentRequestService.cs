using ApungLourdesWebApi.DTOs;

namespace ApungLourdesWebApi.Services.Interfaces
{
    public interface IDocumentRequestService
    {
        Task<DocumentRequestDto> AddAsync(DocumentRequestDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<DocumentRequestDto>> GetAllAsync();
        Task<DocumentRequestDto?> GetByIdAsync(int id);
        Task<DocumentRequestDto?> UpdateAsync(int id, DocumentRequestDto dto);
    }
}
