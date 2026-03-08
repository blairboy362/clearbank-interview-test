using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClearBank.DeveloperTest.Tests.Services;

// Some of these tests could be moved to AccountTest but I'll leave them here to illustrate
// what I tested before I refactored.
[TestClass]
[TestSubject(typeof(PaymentService))]
public class PaymentServiceTest
{
    private PaymentService _subject;
    private Mock<IAccountDataStore> _accountDataStoreMock;

    public PaymentServiceTest()
    {
        _accountDataStoreMock = new Mock<IAccountDataStore>();
        _subject = new PaymentService(_accountDataStoreMock.Object);
    }

    [TestMethod]
    public void MakePaymentRequest_ValidCombinationDeductsAmount()
    {
        var account = new Account
        {
            AccountNumber = "TestDebtorAccountNumber",
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000000,
            Status = AccountStatus.Live,
        };
        
        _accountDataStoreMock.Setup(store => store.GetAccount(account.AccountNumber)).Returns(account);
        
        var request = new MakePaymentRequest
        {
            Amount = 12345,
            CreditorAccountNumber = "TestCreditorAccountNumber",
            DebtorAccountNumber = "TestDebtorAccountNumber",
            PaymentDate = DateTime.UtcNow,
            PaymentScheme = PaymentScheme.Bacs,
        };
        
        var result = _subject.MakePayment(request);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(987655, account.Balance);
    }

    [TestMethod]
    public void MakePaymentRequest_UnsupportedPaymentMethodReturnsFalseAndBalanceRemainsUnchanged()
    {
        var account = new Account
        {
            AccountNumber = "TestDebtorAccountNumber",
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Balance = 1000000,
            Status = AccountStatus.Live,
        };
        
        _accountDataStoreMock.Setup(store => store.GetAccount(account.AccountNumber)).Returns(account);
        
        var request = new MakePaymentRequest
        {
            Amount = 12345,
            CreditorAccountNumber = "TestCreditorAccountNumber",
            DebtorAccountNumber = "TestDebtorAccountNumber",
            PaymentDate = DateTime.UtcNow,
            PaymentScheme = PaymentScheme.Chaps,
        };
        
        var result = _subject.MakePayment(request);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(1000000, account.Balance);
    }

    [TestMethod]
    public void MakePaymentRequest_InvalidDebtorAccountNumberReturnsFalse()
    {
        _accountDataStoreMock.Setup(store => store.GetAccount("NotARealAccountNumber")).Throws(new InvalidOperationException());

        var request = new MakePaymentRequest
        {
            Amount = 12345,
            CreditorAccountNumber = "TestCreditorAccountNumber",
            DebtorAccountNumber = "NotARealAccountNumber",
            PaymentDate = DateTime.UtcNow,
            PaymentScheme = PaymentScheme.Chaps,
        };

        var result = _subject.MakePayment(request);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Success);
    }
}