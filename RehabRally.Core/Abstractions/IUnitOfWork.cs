using RehabRally.Core.Models;

namespace RehabRally.Core.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        public IBaseRespository<RegisterdMashine> RegisterdMashines { get; }
        public IBaseRespository<Category> Categories { get; }
        public IBaseRespository<Exercise> Exercises { get; }
        public IBaseRespository<PatientExercise> PatientExercises { get; }
        public IBaseRespository<PatientConclusion> PatientConclusions { get; }
        public IBaseRespository<SystemNotification> SystemNotifications { get; }

        int Complete();

    }
}
