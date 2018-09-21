using System;

namespace RegionOrebroLan.Transforming.UnitTests
{
	public abstract class BasicTest
	{
		#region Methods

		protected internal virtual void ValidateArgumentException<T>(Action action, string parameterName) where T : ArgumentException
		{
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

		protected internal virtual void ValidateDestinationParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "destination");
		}

		protected internal virtual void ValidateSourceParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "source");
		}

		protected internal virtual void ValidateTransformationParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "transformation");
		}

		#endregion
	}
}