module Trades

open Payment
open MarketOption

type Trade = 
    | Payment of PaymentRecord
    | Option of OptionRecord

type TradeID = System.Guid

let newTradeID () : TradeID= System.Guid.NewGuid()
