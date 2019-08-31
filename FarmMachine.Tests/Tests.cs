using System;
using FarmMachine.Domain.Commands.Exchange;
using FarmMachine.Domain.Services;
using Xunit;

namespace FarmMachine.Tests
{
  public class Tests : FarmMachineTest
  {
    [Fact]
    public void Test1()
    {
      BusControl.Publish<SellCurrency>(new
      {
        Id = Guid.NewGuid(),
        Created = DateTime.Now,
        Amount = 18,
        Bid = 18
      }).GetAwaiter().GetResult();
      Assert.True(true);
    }
  }
}