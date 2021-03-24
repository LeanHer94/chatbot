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

        public BotRepository()
        {
            this.ConnectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
        }

        public int GetCount(string input)
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

        public void InsertRequest(string timezone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                var parameters = new { zone = timezone.GetLastRegion() };

                db.Execute(@"INSERT INTO requests (user_id,zone_id) SELECT 1, id FROM zones WHERE zone = @zone", parameters);
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
                return db.ExecuteScalar<int>("Select COUNT(1) From zones") > 0;
            }
        }

        public bool IsKnownZone(string zone)
        {
            using (IDbConnection db = new SqlConnection(this.ConnectionString))
            {
                return db.ExecuteScalar<int>("Select COUNT(1) From zones where zone = @zone", new { zone = zone }) > 0;
            }
        }
    }
}