using bbxBE.Common.Consts;
using FluentMigrator;
using System;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(0094, "v01.00.03-CashOnDelivery counters")]
    public class InitialTables_00094 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            var envdn = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_DOTNET_ENVIRONMENT);
            var envasp = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_ASPNETCORE_ENVIRONMENT);
            var isProd = envdn == bbxBEConsts.ENV_PROD || envasp == bbxBEConsts.ENV_PROD;




            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "U_001",
               1, "U_001", "Szolnok utánvét", "U", (isProd ? 200000 : 0), 6, "/", ""));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "U_002",
               2, "U_002", "Kecskemét utánvét", "U", (isProd ? 500000 : 0), 6, "/", ""));

            Execute.Sql(string.Format(@"
                if not exists (select * from Counter where CounterCode='{0}')
                begin
                    insert into Counter ([WarehouseID],[CounterCode],[CounterDescription],[Prefix],[CurrentNumber],[NumbepartLength],[Suffix],[CounterPool])
                    select ID, '{2}','{3}', '{4}', {5}, {6}, '{7}', '{8}' from Warehouse where WarehouseCode='{1}'
               end",
               "U_003",
               3, "U_003", "Nagykőrös utánvét", "U", (isProd ? 800000 : 0), 6, "/", ""));
        }
    }
}
