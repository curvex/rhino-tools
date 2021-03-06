using System.Collections;
using Castle.Components.Validator;
using NHibernate.Criterion;
using Rhino.Commons;
using Rhino.Security.Interfaces;
using Rhino.Security.Properties;

namespace Rhino.Security.Impl.Validatiors
{
	/// <summary>
	/// Validate that the given property is unique without using a separate transaction.
	/// </summary>
	/// <remarks>
	/// This is needed because we want to be able to run on in memory database, so we 
	/// have to reuse the same connection / transaction
	/// </remarks>
	public class ValidateIsUniqueInSameTransactionAttribute : AbstractValidationAttribute
	{
		private readonly IsUniqueInSameTransactionValidator validator = new IsUniqueInSameTransactionValidator();

		///<summary>
		///Constructs and configures an <see cref="T:Castle.Components.Validator.IValidator" />
		///instance based on the properties set on the attribute instance.
		///</summary>
		public override IValidator Build()
		{
			ConfigureValidatorMessage(validator);
			return validator;
		}

		/// <summary>
		/// Perform the actual validation
		/// </summary>
		public class IsUniqueInSameTransactionValidator : AbstractValidator
		{
			///<summary>
			///Implementors should perform the actual validation upon
			///the property value
			///</summary>
			///<param name="instance">The target type instance</param>
			///<param name="fieldValue">The property/field value. It can be null.</param>
			///<returns>
			///<c>true</c> if the value is accepted (has passed the validation test)
			///</returns>
			public override bool IsValid(object instance, object fieldValue)
			{
				IIDentifiable identifiable = (IIDentifiable)instance;
				IList list = UnitOfWork.CurrentSession.CreateCriteria(instance.GetType())
					.Add(Restrictions.Eq(Property.Name, fieldValue))
					.Add(Restrictions.Not(Restrictions.IdEq(identifiable.Id)))
					.SetMaxResults(1)
					.List();
				return list.Count == 0;
			}

			/// <summary>
			/// Builds the error message.
			/// </summary>
			/// <returns></returns>
			protected override string BuildErrorMessage()
			{
				return Resources.NonUniqueName;
			}

			///<summary>
			///Gets a value indicating whether this validator supports browser validation.
			///</summary>
			public override bool SupportsBrowserValidation
			{
				get { return false; }
			}
		}
	}
}