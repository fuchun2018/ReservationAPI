using ReservationAPI.Ports.Entities;

namespace ReservationAPI.Ports.In;

public interface IReservationManagement
{
    Task<IEnumerable<Doctor>> GetDoctors();
    Task<IEnumerable<DateTime>> GetAvailability(string doctorEmail);
}