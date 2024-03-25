namespace UnitTests.Helpers
{
	public static class ExceptionHelper
	{
		#region Methods

		public static void ValidateArgumentException<T>(Action action, string parameterName) where T : ArgumentException
		{
			if(action == null)
				throw new ArgumentNullException(nameof(action));

			try
			{
				action.Invoke();
			}
			catch(T argumentException)
			{
				if(string.Equals(argumentException.ParamName, parameterName, StringComparison.Ordinal))
					throw;
			}
		}

		public static void ValidateDestinationParameterException<T>(Action action) where T : ArgumentException
		{
			ValidateArgumentException<T>(action, "destination");
		}

		public static void ValidateSourceParameterException<T>(Action action) where T : ArgumentException
		{
			ValidateArgumentException<T>(action, "source");
		}

		public static void ValidateTransformationParameterException<T>(Action action) where T : ArgumentException
		{
			ValidateArgumentException<T>(action, "transformation");
		}

		#endregion
	}
}