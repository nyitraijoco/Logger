namespace LoggerLibrary
{
    class CallLogger
    {
        static void Main(string[] args)
        {
            Log();
            Console.ReadLine();
        }
        private static async Task Log() {
            Logger logger = new(Logger.LoggerTypeEnum.console);
            await logger.CreateLogAsync(Logger.LogLevelEnum.debug, "debug logger");
            await logger.CreateLogAsync(Logger.LogLevelEnum.info, "info logger");
            await logger.CreateLogAsync(Logger.LogLevelEnum.error, "error logger");
        }
    }
}