using SibaSchoolManagementApi.DTOs;


namespace SibaSchoolManagementApi.Services
{
    public interface ITimetableService
    {
        Task<IEnumerable<TimetableDto>> GetAllTimetableSlotsAsync(bool? current = null);
        Task<TimetableDto?> GetTimetableSlotByIdAsync(int id);
        Task<TimetableDto> CreateTimetableSlotAsync(CreateTimetableDto timetableDto);
        Task UpdateTimetableSlotAsync(int id, UpdateTimetableDto timetableDto);
        Task DeleteTimetableSlotAsync(int id);
    }
}