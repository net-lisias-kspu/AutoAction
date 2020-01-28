using System;
using System.Diagnostics;
using KSPe.Util.Log;

namespace AutoAction
{
	public static class Log
	{
		private static readonly Logger logger = Logger.CreateForType<AutoActionEditor>();

		internal static void Init()
		{
			logger.level =
#if DEBUG
				Level.TRACE
#else
                Level.INFO
#endif
				;
		}

		internal static void Force(string message, params object[] @params)
		{
			logger.force(message, @params);
		}

		[Conditional("DEBUG")]
		public static void Debug (string message, params object[] @params)
		{
			logger.dbg(message, @params);
		}

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

