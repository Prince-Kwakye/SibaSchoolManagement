using Microsoft.EntityFrameworkCore;
using SibaSchoolManagementApi.Data;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Models;

namespace SibaSchoolManagementApi.Services
{
    public class TimetableService(SchoolDbContext context) : ITimetableService
    {
        private readonly SchoolDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<IEnumerable<TimetableDto>> GetAllTimetableSlotsAsync(bool? current = null)
        {
            var query = _context.Timetable.Include(t => t.Course).AsQueryable();

            if (current == true)
            {
                var currentYear = $"{DateTime.UtcNow.Year}/{DateTime.UtcNow.Year + 1}";
                query = query.Where(t => t.AcademicYear == currentYear);
            }

            return await query
                .Select(t => new TimetableDto
                {
                    Id = t.Id,
                    CourseId = t.CourseId,
                    CourseName = t.Course!.Name ?? string.Empty,
                    DayOfWeek = t.DayOfWeek,
                    DayName = GetDayName(t.DayOfWeek),
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    RoomNumber = t.RoomNumber,
                    AcademicYear = t.AcademicYear ?? string.Empty,
                    Semester = t.Semester ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<TimetableDto?> GetTimetableSlotByIdAsync(int id)
        {
            var slot = await _context.Timetable
                .Include(t => t.Course)
                .FirstOrDefaultAsync(t => t.Id == id);

            return slot is null ? null : MapToDto(slot);
        }

        public async Task<TimetableDto> CreateTimetableSlotAsync(CreateTimetableDto timetableDto)
        {
            ArgumentNullException.ThrowIfNull(timetableDto);

            var slot = new Timetable
            {
                CourseId = timetableDto.CourseId,
                DayOfWeek = timetableDto.DayOfWeek,
                StartTime = timetableDto.StartTime,
                EndTime = timetableDto.EndTime,
                RoomNumber = timetableDto.RoomNumber,
                AcademicYear = timetableDto.AcademicYear ?? throw new ArgumentException("Academic year is required"),
                Semester = timetableDto.Semester ?? throw new ArgumentException("Semester is required")
            };

            _context.Timetable.Add(slot);
            await _context.SaveChangesAsync();

            return await GetTimetableSlotByIdAsync(slot.Id)
                ?? throw new Exception("Failed to retrieve created timetable slot");
        }

        public async Task UpdateTimetableSlotAsync(int id, UpdateTimetableDto timetableDto)
        {
            ArgumentNullException.ThrowIfNull(timetableDto);

            var slot = await _context.Timetable.FindAsync(id)
                ?? throw new ArgumentException($"Timetable slot with ID {id} not found");

            slot.CourseId = timetableDto.CourseId;
            slot.DayOfWeek = timetableDto.DayOfWeek;
            slot.StartTime = timetableDto.StartTime;
            slot.EndTime = timetableDto.EndTime;
            slot.RoomNumber = timetableDto.RoomNumber;
            slot.AcademicYear = timetableDto.AcademicYear ?? slot.AcademicYear;
            slot.Semester = timetableDto.Semester ?? slot.Semester;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimetableSlotAsync(int id)
        {
            var slot = await _context.Timetable.FindAsync(id)
                ?? throw new ArgumentException($"Timetable slot with ID {id} not found");

            _context.Timetable.Remove(slot);
            await _context.SaveChangesAsync();
        }

        private static TimetableDto MapToDto(Timetable slot) => new()
        {
            Id = slot.Id,
            CourseId = slot.CourseId,
            CourseName = slot.Course?.Name ?? string.Empty,
            DayOfWeek = slot.DayOfWeek,
            DayName = GetDayName(slot.DayOfWeek),
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            RoomNumber = slot.RoomNumber,
            AcademicYear = slot.AcademicYear ?? string.Empty,
            Semester = slot.Semester ?? string.Empty
        };

        private static string GetDayName(int dayOfWeek) => dayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            7 => "Sunday",
            _ => "Unknown"
        };
    }
}
