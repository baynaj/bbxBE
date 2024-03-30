using bbxBE.Application.BLL;
using bbxBE.Common.Consts;
using FluentMigrator;
using System;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00092, "v00.02.57-set PROD users")]
    public class InitialTables_00092 : Migration
    {
        public override void Down()
        {
        }
        public override void Up()
        {

            var envdn = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_DOTNET_ENVIRONMENT);
            var envasp = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_ASPNETCORE_ENVIRONMENT);
            if (envdn == "Prod" || envasp == "Prod")
            {


                var salt = Environment.GetEnvironmentVariable(bbxBEConsts.ENV_PWDSALT);
                Execute.Sql(string.Format(@"truncate table Users"));


                //Moderátor
                var pwdHash = BllAuth.GetPwdHash("modrel0311", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MOD'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Moderátor', '','mod', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                pwdHash));

                //Bera Zsolt
                pwdHash = BllAuth.GetPwdHash("bzs0710", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'BZS'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Bera Zsolt', '','bzs', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                pwdHash));

                //Czakó István
                pwdHash = BllAuth.GetPwdHash("ci0311", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'CI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Czakó István', '','ci', '{0}', 'Automatikusan létrehozva',1, 'LEVEL1')
                end",
                 pwdHash));

                //Dóczi Alajos
                pwdHash = BllAuth.GetPwdHash("da0630", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'DA'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Dóczi Alajos', '','da', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Göcző Attila
                pwdHash = BllAuth.GetPwdHash("ga1108", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'GA'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Göcző Attila', '','ga', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Gyöngyössy Sándor Szilveszter
                pwdHash = BllAuth.GetPwdHash("gysz0506", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'GS'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Gyöngyössy Sándor Szilveszter', '','gs', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Három László
                pwdHash = BllAuth.GetPwdHash("hl0723", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'HL'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Három László', '','hl', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Hegedűs Szabó Alexandra
                pwdHash = BllAuth.GetPwdHash("sza1227", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'HSA'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Hegedűs Szabó Alexandra', '','hsa', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Ifj. Mezei József
                pwdHash = BllAuth.GetPwdHash("mjdk928", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MJ'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Ifj. Mezei József', 'mezeirelaxvill@gmail.com','mj', '{0}', 'Automatikusan létrehozva',1, 'LEVEL1')
                end",
                 pwdHash));

                //Kormos Krisztián
                pwdHash = BllAuth.GetPwdHash("kk0816", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'KK'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Kormos Krisztián', 'vevoszolgalat@relaxvill.hu','kk', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Máté-Tóth Péter
                pwdHash = BllAuth.GetPwdHash("mp0814", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MTP'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Máté-Tóth Péter', 'ajanlat@relaxvill.hu','mtp', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Mezei József
                pwdHash = BllAuth.GetPwdHash("mj0711", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'MJ2'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Mezei József', '','mj2', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Nagy László
                pwdHash = BllAuth.GetPwdHash("nl1022", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'NL'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Nagy László', '','nl', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Ökrös Erika Juliánna
                pwdHash = BllAuth.GetPwdHash("öe1029", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'ÖE'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Ökrös Erika Juliánna', '','öe', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Pécsi Albertné
                pwdHash = BllAuth.GetPwdHash("pa0312", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'PA'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Pécsi Albertné', '','pa', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Rékási István
                pwdHash = BllAuth.GetPwdHash("ri1207", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'RI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Rékási István', 'rekasi@relaxvill.hu','ri', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Sokvári Csaba
                pwdHash = BllAuth.GetPwdHash("scs0321", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'SC'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Sokvári Csaba', '','sc', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Szekeres Zoltán
                pwdHash = BllAuth.GetPwdHash("szz0402", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'SZZ'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Szekeres Zoltán', '','szz', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Szijártó Viktor
                pwdHash = BllAuth.GetPwdHash("szv0819", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'SZV'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Szijártó Viktor', '','szv', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));


                //Török István
                pwdHash = BllAuth.GetPwdHash("ti0629", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'TI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Török István', 'kecskemet@relaxvill.hu','ti', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Trenovszki Csaba
                pwdHash = BllAuth.GetPwdHash("tcs1127", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'TCS'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Trenovszki Csaba', '','tcs', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Varga István
                pwdHash = BllAuth.GetPwdHash("vi0109", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'VI'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Varga István', '','vi', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //Vári Attila György
                pwdHash = BllAuth.GetPwdHash("vagy0211", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'VA'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('Vári Attila György', '','va', '{0}', 'Automatikusan létrehozva',1, 'LEVEL2')
                end",
                 pwdHash));

                //bbxAdmin
                pwdHash = BllAuth.GetPwdHash("bbx1", salt);
                Execute.Sql(string.Format(@"
                if not exists (select 1 from Users where upper(LoginName) = 'BBX'  )
                begin
                    INSERT INTO [dbo].[Users]  ([Name],[Email],[LoginName],[PasswordHash],[Comment],[Active], [UserLevel])
                     VALUES
                        ('bbxAdmin', 'bbxsoftware@gmail.com','bbx', '{0}', 'Automatikusan létrehozva',1, 'LEVEL1')
                end",
                 pwdHash));



            }
        }

    }
}

