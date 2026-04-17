using FluentValidation;
using Models;

namespace api.Models.Validators;

public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        RuleFor(r => r.Start)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow)
                .WithMessage("Time travels have not been discovered... yet");

        RuleFor(r => r.End)
            .GreaterThanOrEqualTo(r => r.Start.AddDays(1))
                .WithMessage("Minimum reservation allowed is 1 night")
            .LessThanOrEqualTo(r => r.Start.AddDays(30))
                .WithMessage("Maximum reservation allowed is 30 nights")
            .When(r => r.Start != default);

        RuleFor(r => r.GuestEmail)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
                .WithMessage("Email address is missing the domain")
            .Matches(@"^[^@]+@[^@]+\.[^@]+$")
                .WithMessage("The email domain is incomplete");

        RuleFor(r => r.RoomNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(r => r[..1] != "-")
                .WithMessage("Underground rooms are not allowed")
            .Must(r => r[..1] != "0")
                .WithMessage("Rooms must be placed on the 1st floor or above")
            .Matches(@"^\d{3}$")
                .WithMessage("Invalid room number. Plese enter a number between 101 and 999 avoiding 00s")
            .Must(r => r[1..] != "00")
                .WithMessage("Invalid door '00'");
    }
}
