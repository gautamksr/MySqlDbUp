namespace DbDeploy.MySql
{
    using DbUp;
    using DbUp.Engine;
    using DbUp.Helpers;
    using DbUp.ScriptProviders;
    using System;
    using System.Configuration;
    using System.IO;

    class Program
    {
       
        private static DatabaseUpgradeResult ExecuteDBScript(string connectionString, string dbScriptFileName)
        {            
            FileSystemScriptOptions fsso = new FileSystemScriptOptions();
            fsso.IncludeSubDirectories = true;
            fsso.Filter = (string s) => s.EndsWith(Path.GetFileName(dbScriptFileName));
            string directoryPath = Path.GetDirectoryName(dbScriptFileName);
            var upgrader =
               DeployChanges.To
                   .MySqlDatabase(connectionString)                   
                   .WithScriptsFromFileSystem(directoryPath, fsso)
                   .LogToConsole()                   
                   .JournalTo(new NullJournal())
                   .Build();            

            return upgrader.PerformUpgrade();            
        }
        static int Main(string[] args)
        {
            int returnValue = -1;

            if (!(string.IsNullOrEmpty(args[0])) || (string.IsNullOrEmpty(args[1])))
            {
                var dbConnectionString = args[0].Trim();
                var dbScriptPath = args[1].Trim();
                Console.ForegroundColor = ConsoleColor.Blue;
                dbConnectionString += "Allow User Variables=True;";
                Console.WriteLine("DB connection string : " + dbConnectionString);
                Console.WriteLine("DB script file path : " + dbScriptPath);
                Console.ResetColor();

                var result = ExecuteDBScript(dbConnectionString, dbScriptPath);

                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception : " + result.Error);
#if DEBUG
                Console.ReadLine();
#endif
                    returnValue = -1;
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    returnValue = 0;
                }
                Console.ResetColor();
            }
            return returnValue;
        }
    }
}
