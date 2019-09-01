using System;
using FarmMachine.Domain.Commands.Exchange;
using Xunit;

namespace FarmMachine.Tests.Tests
{
  public class BuyAndSellCurrencyTest : FarmMachineTest
  {
    public BuyAndSellCurrencyTest()
    {
    }
    
    [Fact]
    public void BuyBtcTest()
    {
      BusControl.Publish<BuyCurrency>(new
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Amount = 5,
        Bid = 1
      }).GetAwaiter().GetResult();
      
      Assert.True(true);
    }
    
    [Fact]
    public void SellUsdTest()
    {
      BusControl.Publish<SellCurrency>(new
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Amount = 5,
        Bid = 1
      }).GetAwaiter().GetResult();
      
      Assert.True(true);
    }
  }
}

/*
 * 
    [Fact]
    public void Test1()
    {
      BusControl.Publish<BuyCurrency>(new
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Amount = 18,
        Bid = 1
      }).GetAwaiter().GetResult();
      Assert.True(true);
    }
    
    [Fact]
    public void Test2()
    {
      BusControl.Publish<SellCurrency>(new
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Amount = 18,
        Ask = 1
      }).GetAwaiter().GetResult();
      Assert.True(true);
    }
 */