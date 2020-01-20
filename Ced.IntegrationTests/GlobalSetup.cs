using Ced.BusinessEntities;
using Ced.Data.Models;
using ITE.Utility.Extensions;
using NUnit.Framework;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Ced.IntegrationTests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            MigrateDbToLatestVersion();
            Seed();
        }

        private static void MigrateDbToLatestVersion()
        {
            var configuration = new Data.Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }

        private void Seed()
        {
            var context = new CedContext();

            if (!context.Editions.Any())
            {
                context.Editions.Add(
                    new Edition
                    {
                        EditionName = "Edition1",
                        StartDate = DateTime.Today.AddDays(10),
                        EndDate = DateTime.Today.AddDays(12),
                        VisitStartTime = TimeSpan.FromHours(9),
                        VisitEndTime = TimeSpan.FromHours(20),
                        Status = (byte)EditionStatusType.Published.GetHashCode(),
                        EventTypeCode = EventType.Exhibition.GetDescription(),
                        AxEventId = 1001,
                        CoHostedEvent = false,
                        AllDayEvent = false,
                        DwEventID = 100001,
                        EditionNo = 1,
                        MatchedKenticoEventId = 0,
                        CreateTime = DateTime.Now,
                        CreateUser = 1,
                        UpdateUser = 1,
                        Event =
                            new Event
                            {
                                MasterName = "Event1",
                                EventType = EventType.Exhibition.ToString(),
                                EventTypeCode = EventType.Exhibition.GetDescription(),
                                CreateTime = DateTime.Now,
                                CreateUser = 1,
                                UpdateUser = 1
                            }
                    });

                context.SaveChanges();
            }
        }
    }
}
