namespace StatisticsService.Models
{
    public interface IReplaysStatisticsProvider
    {
        ReplaysStatistics Get();
    }

    public class ReplaysStatisticsProvider : IReplaysStatisticsProvider
    {
        private static ReplaysStatistics replaysStatistics;

        private readonly IReplayStatisticsRepo replayStatisticsRepo;
        private readonly IReplaysStatisticsConverter replaysStatisticsConverter;

        public ReplaysStatisticsProvider()
        {
            replayStatisticsRepo = new ReplayStatisticsRepo();
            replaysStatisticsConverter = new ReplaysStatisticsConverter();
        }

        public ReplaysStatistics Get()
        {
            return Build();
        }

        private ReplaysStatistics Build()
        {
            var healthStatistics = replayStatisticsRepo.GetReportsStatistics();

            return replaysStatisticsConverter.Build(healthStatistics);
        }
    }
}