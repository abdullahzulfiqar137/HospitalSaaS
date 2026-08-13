using FluentValidation;

namespace HospitalSaaS.Application.Doctors.Queries.SearchDoctors;

public sealed class SearchDoctorsQueryValidator : AbstractValidator<SearchDoctorsQuery>
{
    public SearchDoctorsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
