using FluentValidation;

namespace HospitalSaaS.Application.Appointments.Queries.SearchAppointments;

public sealed class SearchAppointmentsQueryValidator : AbstractValidator<SearchAppointmentsQuery>
{
    public SearchAppointmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
