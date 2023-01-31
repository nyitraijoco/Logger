namespace LoggerLibrary
{
    class CallLogger
    {
        static void Main(string[] args)
        {
            Logger logger = new(Logger.LoggerTypeEnum.console);
            logger.CreateLog(Logger.LogLevelEnum.debug, "debug logger");
            logger.CreateLog(Logger.LogLevelEnum.info, "info logger");
            logger.CreateLog(Logger.LogLevelEnum.error, "error logger");
            Console.ReadLine();
        }
    }
}