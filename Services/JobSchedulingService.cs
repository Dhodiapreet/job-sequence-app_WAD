using JobSequenceApp.Data;
using JobSequenceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Services
{
    public class JobSchedulingService : IJobSchedulingService
    {
        private readonly ApplicationDbContext _context;

        public JobSchedulingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanScheduleJobAsync(int workerProfileId, DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                return false;

            var overlappingJobs = await _context.JobSequences
                .Where(j => j.WorkerProfileId == workerProfileId && j.Status != "Cancelled")
                .AnyAsync(j => startTime < j.EndTime && endTime > j.StartTime);

            return !overlappingJobs;
        }

        public async Task<JobSequence?> ScheduleJobAsync(int workerProfileId, int bookingId, DateTime startTime, DateTime endTime)
        {
            if (!await CanScheduleJobAsync(workerProfileId, startTime, endTime))
            {
                return null;
            }

            var jobSequence = new JobSequence
            {
                WorkerProfileId = workerProfileId,
                BookingId = bookingId,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Scheduled"
            };

            _context.JobSequences.Add(jobSequence);
            await _context.SaveChangesAsync();

            return jobSequence;
        }
    }
}
