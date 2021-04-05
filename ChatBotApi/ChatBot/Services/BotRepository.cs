using ChatBot.ExtensionMethods;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ChatBot.Services
{
    public class BotRepository
    {
        private string ConnectionString { get; }

        private const int CACHE_TIME = 1;

        public BotRepository()
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
        }

        public int GetCount(string input)
        {
            if (input != null)
            {
                using (IDbConnection db = new SqlConnection(this.ConnectionString))
                {
                    var parameters = new { lookup = input.GetLastRegion() };

                    return db.ExecuteScalar<int>(@"
                            SELECT count(1) FROM requests h
                            WHERE EXISTS(
                                SELECT 1 FROM
                                    (SELECT id FROM zones WHERE zone = @lookup AND available = 1 union
                                    SELECT id FROM zones WHERE parent = @lookup AND available = 1 union
                                    SELECT id FROM zones z1 WHERE
                                                  EXISTS(SELECT zone FROM zones z2
                                                          WHERE z2.parent = @lookup
                                                          AND z2.available = 0
                                                          AND z2.zone = z1.parent)) as ids
                                WHERE ids.id = h.zone_id)",
                                parameters);
                }
            }

            return 0;
        }

        public string GetValidRegionPath(string timezone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { lookup = timezone.GetLastRegion() };

                return String.Concat(db.ExecuteScalar<string>(@"
                            SELECT CONCAT(z3.zone, '/', z2.zone, '/', z1.zone)
                            FROM [ChatBotDatabase].[dbo].[zones] z1
                            FULL JOIN [ChatBotDatabase].[dbo].[zones] z2 ON z1.parent = z2.zone
                            FULL JOIN [ChatBotDatabase].[dbo].[zones] z3 ON z2.parent = z3.zone
                            WHERE z1.zone = @lookup",
                            parameters)
                            .SkipWhile(x => x == '/'));
            }
        }

        public void InsertRequest(string timezone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { zone = timezone.GetLastRegion() };

                db.Execute(@"INSERT INTO requests (user_id,zone_id) SELECT 1, id FROM zones WHERE zone = @zone", parameters);
            }
        }

        public void InsertLog(string description, Exception ex)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { description, exception = ex?.Message };

                db.Execute(@"INSERT INTO logs (description, exception) VALUES (@description, @exception)", parameters);
            }
        }

        public void CacheTimeZone(string timezone, DateTime timeAtTimezone, DateTime requestTime)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { timezone, timeAtTimezone, requestTime };

                db.Execute(@"IF NOT EXISTS (SELECT 1 FROM requestsCache WHERE timezone = @timezone)
                            BEGIN
                               INSERT INTO requestsCache (timezone, time_at_timezone, time_request) 
                               VALUES (@timezone, @timeAtTimezone, @requestTime)
                            END ELSE
                               UPDATE requestsCache SET time_at_timezone = @timeAtTimezone, time_request = @requestTime
                               WHERE timezone = @timezone", 
                               parameters);
            }
        }

        public bool ShouldUpdateCache(string timezone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { timezone };

                return db.ExecuteScalar<int>("SELECT DATEDIFF(MINUTE, time_request, GETDATE()) from requestsCache WHERE timezone = @timezone", parameters) > CACHE_TIME;
            }
        }

        public DateTime GetCachedTimeZone(string timezone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { timezone };

                return db.ExecuteScalar<DateTime>("SELECT time_at_timezone from requestsCache WHERE timezone = @timezone", parameters);
            }
        }

        public void TryInsert(IEnumerable<string> timezones)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                foreach (var timezone in timezones)
                {
                    var regions = timezone.GetRegions();

                    var last = regions.Last();
                    string parent = null;

                    foreach (var region in regions)
                    {
                        var parameters = new { zone = region, parent = parent, available = region == last };

                        db.Execute(@"
                           IF NOT EXISTS (SELECT 1 FROM zones WHERE zone = @zone)
                           BEGIN
                               INSERT INTO zones (zone, parent, available)
                               VALUES (@zone, @parent, @available)
                           END",
                           parameters);

                        parent = region;
                    }
                }                        
            }
        }
        
        public bool IsZonesTablePopulated()
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                return db.ExecuteScalar<int>("SELECT COUNT(1) FROM zones") > 0;
            }
        }

        public bool IsKnownZone(string zone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                return db.ExecuteScalar<int>("SELECT COUNT(1) FROM zones WHERE zone = @zone AND available = 1", new { zone }) > 0;
            }
        }
    }
}