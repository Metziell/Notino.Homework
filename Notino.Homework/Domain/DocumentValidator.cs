using FluentValidation;

namespace Notino.Homework.Domain;

public class DocumentValidator : AbstractValidator<Document>
{
	public DocumentValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
	}
}
