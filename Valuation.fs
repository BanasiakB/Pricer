module Valuation
open Trades
open Payment
open MarketOption

let valuateTrade config marketData (trade : Trade) : Trade =
  match trade with
  | Payment p -> 
      let inputs: PaymentValuationInputs = 
        { Trade = p
          Data = config
          MarketData = marketData
        }
      let vm = PaymentValuationModel(inputs)
      Payment { p with Value = Some <| vm.Calculate()}
  
  | Option o -> 
    let inputs: OptionValuationInputs = 
        { Options = o
          Data = config
          MarketData = marketData
        }
    let vm = OptionValuationModel(inputs)
    Option {o with AnalyticalValue = Some <| vm.CalculateAnalytically(); NumericalValue = Some <| vm.CalculateNumerically(); Delta = Some <| vm.CalculateDelta()}


