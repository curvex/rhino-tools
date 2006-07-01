using System;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Abstract class that holds common information for 
	/// expectations.
	/// </summary>
	public abstract class AbstractExpectation : IExpectation
	{
		#region Variables

		/// <summary>
		/// Number of actuall calls made that passed this expectation
		/// </summary>
		private int actualCalls;

		/// <summary>
		/// Range of expected calls that should pass this expectation.
		/// </summary>
		private Range expected;

		/// <summary>
		/// The return value for a method matching this expectation
		/// </summary>
		private object returnValue;

		/// <summary>
		/// The exception to throw on a method matching this expectation.
		/// </summary>
		private Exception exceptionToThrow;

		/// <summary>
		/// The method this expectation is for.
		/// </summary>
		private MethodInfo method;

		/// <summary>
		/// The return value for this method was set
		/// </summary>
		private bool returnValueSet;

		/// <summary>
		/// Whether this method will repeat
		/// unlimited number of times.
		/// </summary>
		private RepeatableOption repeatableOption;

        /// <summary>
        /// A delegate that will be run when the 
        /// expectation is matched.
        /// </summary>
        private Delegate actionToExecute;

        /// <summary>
        /// The arguments that matched this expectation.
        /// </summary>
        private object[] matchingArgs;

	    private object[] outRefParams;

	    #endregion

		#region Properties

	    /// <summary>
	    /// Setter for the outpur / ref parameters for this expecataion.
	    /// Can only be set once.
	    /// </summary>
	    public object[] OutRefParams
	    {
	        set
	        {
                if (outRefParams != null)
                    throw new InvalidOperationException(
                        "Output and ref parameters has already been set for this expectation");
	            outRefParams = value;
	        }
	    }

	    /// <summary>
		/// Specify whatever this expectation has a return value set
		/// You can't check ReturnValue for this because a valid return value include null.
		/// </summary>
		public bool HasReturnValue
		{
			get { return returnValueSet; }
		}

		/// <summary>
		/// Gets the method this expectation is for.
		/// </summary>
		public MethodInfo Method
		{
			get { return method; }
		}

		/// <summary>
		/// Gets or sets what special condtions there are for this method
		/// </summary>
		public RepeatableOption RepeatableOption
		{
			get { return repeatableOption; }
			set { repeatableOption = value; }
		}

		/// <summary>
		/// Range of expected calls
		/// </summary>
		public Range Expected
		{
			get { return expected; }
			set { expected = value; }
		}

		/// <summary>
		/// Number of call actually made for this method
		/// </summary>
		public int ActualCalls
		{
			get { return actualCalls; }
		}

		/// <summary>
		/// If this expectation is still waiting for calls.
		/// </summary>
		public bool CanAcceptCalls
		{
			get
			{
				//I don't bother to check for RepeatableOption.Never because
				//this is handled the method recorder
				if (repeatableOption == RepeatableOption.Any)
					return true;
				return actualCalls < expected.Max;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this expectation was satisfied
		/// </summary>
		public bool ExpectationSatisfied
		{
			get
			{
				if (repeatableOption != RepeatableOption.Normal)
					return true;
				return expected.Min <= actualCalls && actualCalls <= expected.Max;
			}
		}


		/// <summary>
		/// The return value for a method matching this expectation
		/// </summary>
		public object ReturnValue
		{
			get { return returnValue; }
			set
			{
				ActionOnMethodNotSpesified();
				AssertTypesMatches(value);
				returnValueSet = true;
				returnValue = value;
			}
		}

        /// <summary>
        /// An action to execute when the method is matched.
        /// </summary>
        public Delegate ActionToExecute
        {
            get { return actionToExecute; }
            set 
            {
                ActionOnMethodNotSpesified();
                AssertReturnTypeMatch(value);
                AssertDelegateArgumentsMatchMethod(value);
                actionToExecute = value; 
            }
        }

		/// <summary>
		/// Gets or sets the exception to throw on a method matching this expectation.
		/// </summary>
		public Exception ExceptionToThrow
		{
			get { return exceptionToThrow; }
			set
			{
				ActionOnMethodNotSpesified();
				exceptionToThrow = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance's action is staisfied.
		/// A staisfied instance means that there are no more requirements from
		/// this method. A method with non void return value must register either
		/// a return value or an exception to throw or an action to execute.
		/// </summary>
		public bool ActionsSatisfied
		{
			get
			{
				if (method.ReturnType == typeof (void) ||
					exceptionToThrow != null ||
                    actionToExecute != null ||
					returnValueSet ||
					repeatableOption == RepeatableOption.OriginalCall ||
                    repeatableOption == RepeatableOption.PropertyBehavior)
					return true;
				return false;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Get the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Add an actual method call to this expectation
		/// </summary>
		public void AddActualCall()
		{
			actualCalls++;
		}

		/// <summary>
		/// Returns the return value or throw the exception and setup output / ref parameters
		/// </summary>
		public object ReturnOrThrow(object[] args)
		{
            SetupOutputAndRefParameters(args);
            if (actionToExecute != null)
                return ExecuteAction();
			if (exceptionToThrow != null)
				throw exceptionToThrow;
			return returnValue;
		}

	    /// <summary>
        /// Validate the arguments for the method on the child methods
        /// </summary>
        /// <param name="args">The arguments with which the method was called</param>
        public bool IsExpected(object[] args)
        {
            if (DoIsExpected(args))
            {
                matchingArgs = args;
                return true;
            }
            matchingArgs = null;
            return false;
        }


		#endregion

		#region C'tor

		/// <summary>
		/// Creates a new <see cref="AbstractExpectation"/> instance.
		/// </summary>
		/// <param name="method">Method.</param>
		protected AbstractExpectation(MethodInfo method)
		{
			Validate.IsNotNull(method, "method");
			this.method = method;
			this.expected = new Range(1, 1);
		}

		/// <summary>
		/// Creates a new <see cref="AbstractExpectation"/> instance.
		/// </summary>
		/// <param name="expectation">Expectation.</param>
		protected AbstractExpectation(IExpectation expectation) : this(expectation.Method)
		{
			returnValue = expectation.ReturnValue;
			returnValueSet = expectation.HasReturnValue;
			expected = expectation.Expected;
			actualCalls = expectation.ActualCalls;
			repeatableOption = expectation.RepeatableOption;
			exceptionToThrow = expectation.ExceptionToThrow;
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Validate the arguments for the method on the child methods
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected abstract bool DoIsExpected(object[] args);

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		public abstract string ErrorMessage { get; }

		/// <summary>
		/// Determines if this object equal to obj
		/// </summary>
		public abstract override bool Equals(object obj);

		#endregion

		#region Protected Methods

		/// <summary>
		/// The error message for these arguments
		/// </summary>
		protected string CreateErrorMessage(MethodInfo method, object[] args)
		{
			MethodCallUtil.FormatArgumnet format = new MethodCallUtil.FormatArgumnet(FormatArg);
			return MethodCallUtil.StringPresentation(format, method, args);
		}

		#endregion

		#region Implementation

        private void SetupOutputAndRefParameters(object[] args)
        {
            if(outRefParams==null)
                return;
            int argIndex=0, outputArgIndex =0;
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo info in parameters)
            {
                if(outputArgIndex>=outRefParams.Length)
                    return;

                if (info.IsOut || info.ParameterType.IsByRef)
                {
                    args[argIndex] = outRefParams[outputArgIndex];
                    outputArgIndex += 1;
                }
                argIndex++;
            }
        }

	    
		private static string FormatArg(Array args, int i)
		{
			if (args.Length <= i)
				return "missing parameter";
			object arg = args.GetValue(i);
			if (arg is Array)
			{
				Array arr = (Array) arg;
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (int j = 0; j < arr.Length; j++)
				{
					sb.Append(FormatArg( arr, j));
					if (j < arr.Length - 1)
						sb.Append(", ");
				}
				sb.Append("]");
				return sb.ToString();
			}
			else if (arg is string)
			{
				return '"' + arg.ToString() + '"';
			}
			else if (arg == null)
			{
				return "null";
			}
			else
			{
				return arg.ToString();
			}
		}

		private void ActionOnMethodNotSpesified()
		{
			if (returnValueSet == false && exceptionToThrow == null && actionToExecute == null)
				return;
			throw new InvalidOperationException("Can set only a single return value or exception to throw or delegate to throw on the same method call.");
		}

        private object ExecuteAction()
        {
            try
            {
                if (matchingArgs == null)
                    throw new InvalidOperationException("Trying to run a Do() delegate when no arguments were matched to the expectation.");
                try
                {
                    return actionToExecute.DynamicInvoke(matchingArgs);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            finally
            {
                matchingArgs = null;
            }
        }

		private void AssertTypesMatches(object value)
		{
			string type = null;
			if (value == null)
			{
				type = "null";
				if (method.ReturnType.IsValueType == false)
					return;
#if dotNet2
                if (method.ReturnType.IsGenericType &&
                    method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return;
#endif
			}
			else
			{
				if (method.ReturnType.IsInstanceOfType(value))
					return;
				type = value.GetType().FullName;
			}
			throw new InvalidOperationException("Type '" + type + "' doesn't match the return type '" + method.ReturnType.FullName + "' for method '" + MethodCallUtil.StringPresentation(method, new object[0]) + "'");
		}

        /// <summary>
        /// Asserts that the delegate has the same parameters as the expectation's method call
        /// </summary>
        protected void AssertDelegateArgumentsMatchMethod(Delegate callback)
        {
            ParameterInfo[] callbackParams = callback.Method.GetParameters(),
                         methodParams = method.GetParameters();
            string argsDontMatch = "Callback arguments didn't match the method arguments";
            if (callbackParams.Length != methodParams.Length)
                throw new InvalidOperationException(argsDontMatch);
            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].ParameterType != callbackParams[i].ParameterType)
                    throw new InvalidOperationException(argsDontMatch);
            }
        }
        private void AssertReturnTypeMatch(Delegate value)
        {
            if (method.ReturnType.IsAssignableFrom(value.Method.ReturnType) == false)
                throw new InvalidOperationException("The delegate return value should be assignable from " + method.ReturnType.FullName);
        }
		#endregion
	}
}