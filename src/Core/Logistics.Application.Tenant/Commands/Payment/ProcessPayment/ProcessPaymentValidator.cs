﻿using FluentValidation;
using Logistics.Shared.Enums;

namespace Logistics.Application.Tenant.Commands;

internal sealed class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentValidator()
    {
        RuleFor(i => i.PaymentId).NotEmpty();
        RuleFor(i => i.BillingAddress).NotEmpty();
        
        When(i => i.PaymentMethod == PaymentMethod.CreditCard, () =>
        {
            RuleFor(i => i.CardholderName).NotEmpty();
            RuleFor(i => i.CardNumber).NotEmpty().Matches(RegexPatterns.CREDIT_CARD_NUMBER);
            RuleFor(i => i.CardExpirationDate).NotEmpty().Matches(RegexPatterns.CARD_EXPIRATION_DATE);
            RuleFor(i => i.CardCvv).NotEmpty().Matches(RegexPatterns.CARD_CVV);
        });
        
        When(i => i.PaymentMethod == PaymentMethod.BankAccount, () =>
        {
            RuleFor(i => i.BankName).NotEmpty();
            RuleFor(i => i.BankAccountNumber).NotEmpty();
            RuleFor(i => i.BankRoutingNumber).NotEmpty().Matches(RegexPatterns.BANK_ROUTING_NUMBER);
        });
    }
}
