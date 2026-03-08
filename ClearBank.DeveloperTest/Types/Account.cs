using System;

namespace ClearBank.DeveloperTest.Types
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }

        // Depending on where the complexities lie, I could also see the creation of a Payment
        // class that handles this.
        public void MakePayment(decimal amount, PaymentScheme scheme, DateTime paymentDate)
        {
            var requestedScheme = FromPaymentScheme(scheme);

            if (!AllowedPaymentSchemes.HasFlag(requestedScheme))
            {
                throw new InvalidOperationException(
                    $"Requested payment scheme '{scheme}' is not supported on account '{AccountNumber}'.");
            }

            if (Status != AccountStatus.Live)
            {
                throw new InvalidOperationException($"Account '{AccountNumber}' is in state '{Status}'.");
            }

            if (Balance < amount)
            {
                throw new InvalidOperationException($"Insufficient balance '{Balance}' in account '{AccountNumber}' to deduct '{amount}'.");
            }

            if (paymentDate < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException($"Requested payment date '{paymentDate}' is in the past.");
            }
            
            Balance -= amount;
        }

        // We can argue forever about where to put this method.
        private static AllowedPaymentSchemes FromPaymentScheme(PaymentScheme scheme)
        {
            switch (scheme)
            {
                case PaymentScheme.FasterPayments:
                    return AllowedPaymentSchemes.FasterPayments;
                case PaymentScheme.Bacs:
                    return AllowedPaymentSchemes.Bacs;
                case PaymentScheme.Chaps:
                    return AllowedPaymentSchemes.Chaps;
            }
            
            throw new ArgumentException($"Requested payment scheme '{scheme}' is not supported.");
        }
    }
}
