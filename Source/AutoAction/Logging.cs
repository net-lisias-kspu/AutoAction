using System.Diagnostics;
using KSPe.Util.Log;

namespace AutoAction
{
	public static class Log
	{
		private static readonly Logger logger = Logger.CreateForType<AutoActionEditor>();

		[Conditional("DEBUG")]
		public static void Debug (string message, params object[] @params)
		{
			logger.dbg(message, @params);
		}
		[Conditional("DEBUG")]
		public static void Info(string message, params object[] @params)
		{
			logger.info(message, @params);
		}

		public static void Error(string message, params object[] @params)
		{
			logger.error(message, @params);
		}

		public static void Warn(string message, params object[] @params)
		{
			logger.warn(message, @params);
		}
	}
}

