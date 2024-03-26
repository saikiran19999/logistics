﻿namespace Logistics.Application.Tenant.Commands;

internal sealed class UpdatePaymentHandler : RequestHandler<UpdatePaymentCommand, ResponseResult>
{
    private readonly ITenantUnityOfWork _tenantUow;

    public UpdatePaymentHandler(ITenantUnityOfWork tenantUow)
    {
        _tenantUow = tenantUow;
    }

    protected override async Task<ResponseResult> HandleValidated(
        UpdatePaymentCommand req, CancellationToken cancellationToken)
    {
        var payment = await _tenantUow.Repository<Payment>().GetByIdAsync(req.Id);

        if (payment is null)
        {
            return ResponseResult.CreateError($"Could not find a payment with ID '{req.Id}'");
        }

        if (req.PaymentFor.HasValue && payment.PaymentFor != req.PaymentFor)
        {
            payment.PaymentFor = req.PaymentFor.Value;
        }
        if (req.Method.HasValue && payment.Method != req.Method)
        {
            payment.Method = req.Method.Value;
        }
        if (req.Status.HasValue && payment.Status != req.Status)
        {
            payment.SetStatus(req.Status.Value);
        }
        if (req.Amount.HasValue && payment.Amount != req.Amount)
        {
            payment.Amount = req.Amount.Value;
        }
        if (req.BillingAddress != null && payment.BillingAddress != req.BillingAddress)
        {
            payment.BillingAddress = req.BillingAddress;
        }
        if (!string.IsNullOrEmpty(req.Comment) && payment.Comment != req.Comment)
        {
            payment.Comment = req.Comment;
        }
        
        _tenantUow.Repository<Payment>().Update(payment);
        await _tenantUow.SaveChangesAsync();
        return ResponseResult.CreateSuccess();
    }
}
