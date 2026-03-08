using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        
        public PaymentService(IAccountDataStore accountDataStore)
        {
            this._accountDataStore = accountDataStore;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult();

            try
            {
                var account = _accountDataStore.GetAccount(request.DebtorAccountNumber);
                account.MakePayment(request.Amount, request.PaymentScheme, request.PaymentDate);
                _accountDataStore.UpdateAccount(account);
                result.Success = true;
            }
            catch (InvalidOperationException ex)
            {
                result.Success = false;
            }

            return result;
        }
    }
}
