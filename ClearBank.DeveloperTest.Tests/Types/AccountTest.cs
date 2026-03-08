using System;
using ClearBank.DeveloperTest.Types;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClearBank.DeveloperTest.Tests.Types;

[TestClass]
[TestSubject(typeof(Account))]
public class AccountTest
{
    private readonly Account _subject;

    public AccountTest()
    {
        _subject = new Account
        {
            AccountNumber = "12345",
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs |
                                    AllowedPaymentSchemes.Chaps,
            Balance = 1000000,
            Status = AccountStatus.Live,
        };
    }

    [TestMethod]
    public void MakePayment_ValidCombinationUpdatesBalance()
    {
        _subject.MakePayment(100, PaymentScheme.Bacs, DateTime.UtcNow);
        Assert.AreEqual(999900, _subject.Balance);
    }

    [TestMethod]
    public void MakePayment_UnsupportedSchemeRaisesError()
    {
        _subject.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
        Assert.Throws<InvalidOperationException>(() => _subject.MakePayment(100, PaymentScheme.Bacs, DateTime.UtcNow));
    }

    [TestMethod]
    public void MakePayment_DisabledStatusRaisesError()
    {
        _subject.Status = AccountStatus.Disabled;
        Assert.Throws<InvalidOperationException>(() => _subject.MakePayment(100, PaymentScheme.Bacs, DateTime.UtcNow));
    }

    [TestMethod]
    public void MakePayment_InboundPaymentsOnlyStatusRaisesError()
    {
        _subject.Status = AccountStatus.InboundPaymentsOnly;
        Assert.Throws<InvalidOperationException>(() => _subject.MakePayment(100, PaymentScheme.Bacs, DateTime.UtcNow));
    }

    [TestMethod]
    public void MakePayment_NegativeResultingBalanceRaisesError()
    {
        Assert.Throws<InvalidOperationException>(() => _subject.MakePayment(1000001, PaymentScheme.Bacs, DateTime.UtcNow));
    }

    [TestMethod]
    public void MakePayment_PaymentDateInPastRaisesError()
    {
        Assert.Throws<InvalidOperationException>(() => _subject.MakePayment(100, PaymentScheme.Bacs, DateTime.UtcNow.AddDays(-1)));
    }
}