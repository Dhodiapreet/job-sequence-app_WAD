using JobSequenceApp.Models;

namespace JobSequenceApp.Services
{
    public interface IJobSchedulingService
    {
        Task<bool> CanScheduleJobAsync(int workerProfileId, DateTime startTime, DateTime endTime);
        Task<JobSequence?> ScheduleJobAsync(int workerProfileId, int bookingId, DateTime startTime, DateTime endTime);
    }
}
