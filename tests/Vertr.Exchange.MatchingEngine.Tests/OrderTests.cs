
using Vertr.Exchange.MatchingEngine.Tests.Stubs;

namespace Vertr.Exchange.MatchingEngine.Tests;


[TestFixture(Category = "Unit")]
public class OrderTests
{
    [TestCase(-243.99)]
    public void CannotCreateOrderWithInvalidPrice(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var ord = OrderStub.CreateAskOrder(price, 0L);
        });
    }

    [TestCase(-12)]
    [TestCase(0L)]
    public void CannotCreateOrderWithInvalidPrice(long size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var ord = OrderStub.CreateAskOrder(45.98m, size);
        });
    }

    [TestCase(-456)]
    public void CannotCreateOrderWithInvalidFilled(long filled)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var ord = OrderStub.CreateAskOrder(45.98m, 8, filled);
        });
    }

    [TestCase(12, 20)]
    [TestCase(13, 14)]
    public void CannotCreateOrderWithInvalidSizeFilledCombination(long size, long filled)
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var ord = OrderStub.CreateAskOrder(45.98m, size, filled);
        });
    }

    [TestCase(12, 8)]
    public void CanReduceSize(long size, long reduce)
    {
        var ord = OrderStub.CreateBidOrder(78.99m, size);

        ord.ReduceSize(reduce);

        Assert.That(ord.Size, Is.EqualTo(size - reduce));
    }

    [Test]
    public void ReduceNegativeThrows()
    {
        var ord = OrderStub.CreateBidOrder(78.99m, 17);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ord.ReduceSize(-9);
        });
    }

    [TestCase(15, 0, 17)]
    [TestCase(15, 11, 6)]
    public void CannotReduceMoreThanSize(long size, long filled, long reduce)
    {
        var ord = OrderStub.CreateBidOrder(78.99m, size, filled);

        Assert.Throws<InvalidOperationException>(() =>
        {
            ord.ReduceSize(reduce);
        });
    }

    [Test]
    public void CanSetPrice()
    {
        var ord = OrderStub.CreateBidOrder(78.99m, 17);
        var newPrice = 7996.21M;

        ord.SetPrice(newPrice);

        Assert.That(ord.Price, Is.EqualTo(newPrice));
    }

    [Test]
    public void CannotSetNegativePrice()
    {
        var ord = OrderStub.CreateBidOrder(0.99999m, 179);
        var newPrice = -664.190M;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ord.SetPrice(newPrice);
        });
    }

    [TestCase(178, 0, 78)]
    [TestCase(93, 17, 0)]
    [TestCase(88, 40, 40)]
    public void CanFillOrder(long size, long initialFill, long fill)
    {
        var ord = OrderStub.CreateBidOrder(78.99m, size, initialFill);

        ord.Fill(fill);

        Assert.That(ord.Remaining, Is.EqualTo(size - (initialFill + fill)));
    }

    [Test]
    public void CannotFillNegativeValue()
    {
        var ord = OrderStub.CreateBidOrder(78.99m, 789);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ord.Fill(-88);
        });
    }

    [TestCase(178, 0, 678)]
    [TestCase(93, 17, 93)]
    [TestCase(88, 40, 49)]
    public void InvalidFillValueThrows(long size, long initialFill, long fill)
    {
        var ord = OrderStub.CreateBidOrder(78.99m, size, initialFill);

        Assert.Throws<InvalidOperationException>(() =>
        {
            ord.Fill(fill);
        });
    }
}
