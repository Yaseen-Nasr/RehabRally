using RehabRally.Core.Abstractions;
using RehabRally.Core.Models;
using RehabRally.Ef.Data;
using RehabRally.EF.Respositories;

namespace RehabRally.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBaseRespository<RegisterdMashine> RegisterdMashines { get; private set; }
        public IBaseRespository<Category> Categories { get; private set; }
        public IBaseRespository<Exercise> Exercises { get; private set; }
        public IBaseRespository<PatientExercise> PatientExercises { get; private set; }
        public IBaseRespository<PatientConclusion> PatientConclusions { get; private set; }
        public IBaseRespository<SystemNotification> SystemNotifications { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new BaseRespository<Category>(_context);
            Exercises = new BaseRespository<Exercise>(_context);
            PatientExercises = new BaseRespository<PatientExercise>(_context);
            PatientConclusions = new BaseRespository<PatientConclusion>(_context);
            SystemNotifications = new BaseRespository<SystemNotification>(_context);
            RegisterdMashines = new BaseRespository<RegisterdMashine>(_context);
        }

        public int Complete()
        {
            return _context.SaveChangesAsync().Result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}