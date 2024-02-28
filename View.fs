module View

open Bolero
open Bolero.Html
open Messages
open Model
open Money
open Payment
open MarketOption
open Radzen.Blazor
open System.Collections.Generic
open Trades

type Templates = Template<"wwwroot/templates.html">
type Main = Template<"wwwroot/main.html">

let keyValueMapDisplay msg name (model: Map<string,string>) dispatch =
    let configRow (kvp : KeyValuePair<string,string>) =
        Templates.ConfigRow()
            .Key(kvp.Key)
            .Value(kvp.Value,fun v -> dispatch <| msg (kvp.Key,v))
            .Elt()
    Templates.KeyValueMapDisplay()
        .Title(text name)
        .Rows(forEach model configRow)
        .Elt()

let configDisplay = keyValueMapDisplay ConfigChange "Configuration"
        
let marketDataDisplay = keyValueMapDisplay MarketDataChange "Market Data"
        

let summary (model: Model) dispatch =
    let groupedByCCy =
        model.trades
        |> Map.values
        |> Seq.choose (fun x ->
            match x.trade with
            | Payment p -> p.Value
            | Option (o: OptionRecord) -> o.AnalyticalValue)
        |> Seq.groupBy (fun m -> m.Currency)
    let summaryRow (ccy,values : Money seq) =
        let sum = values |> Seq.sumBy (fun v -> v.Value)
        Templates.SummaryRow()
            .CCY(text ccy)
            .Value(text <| sprintf "%.2f" sum)
            .Elt()
    Templates.Summary()
        .Rows(forEach groupedByCCy summaryRow)
        .Elt()

let paymentRow dispatch (tradeId, p : PaymentRecord) =
    let value = p.Value |> Option.map (string) |> Option.defaultValue "" 
    let tradeChange msg s = dispatch <| TradeChange (msg (tradeId,s))
    Templates.PaymentsRow()
        .Name(p.TradeName,tradeChange NewName)
        .Expiry(p.Expiry.ToString("yyyy-MM-dd"), tradeChange NewExpiry)
        .Currency(p.Currency, tradeChange NewCurrency)
        .Principal(sprintf "%i" p.Principal, tradeChange NewPrincipal)
        .Value(value)
        .Delete(fun e -> dispatch (RemoveTrade tradeId))
        .Elt()

let optionRow dispatch (optionId, o : OptionRecord) =
    let AnalyticalValue = o.AnalyticalValue |> Option.map (string) |> Option.defaultValue "" 
    let NumericalValue = o.NumericalValue |> Option.map (string) |> Option.defaultValue "" 
    let Delta = o.Delta |> Option.map (string) |> Option.defaultValue "" 
    let optionChange msg s = dispatch <| TradeChange (msg (optionId,s))
    Templates.OptionsRow()
        .Name(o.OptionName, optionChange NewName)
        .OptionType(o.OptionType.ToString(), optionChange NewOptionType)
        .AssetPrice(o.AssetPrice.ToString(), optionChange NewAssetPrice)
        .Strike(o.Strike.ToString(), optionChange NewStrike)
        .InterestRate(o.InterestRate.ToString(), optionChange NewInterestRate)
        .Volatility(o.Volatility.ToString(), optionChange NewVolatility)
        .Currency(o.Currency, optionChange NewCurrency)
        .Maturity(o.Maturity.ToString("yyyy-MM-dd"), optionChange NewExpiry)
        .AnalyticalValue(AnalyticalValue)
        .NumericalValue(NumericalValue)
        .Delta(Delta)
        .Delete(fun e -> dispatch (RemoveTrade optionId))
        .Elt()

let homePage (model: Model) dispatch =

    let payments = onlyPayments model.trades
    let options = onlyOptions model.trades
    let trades = 

        Templates.Trades()
            .AddPayment(fun _ -> dispatch AddPayment)
            .RecalculateAll(fun _ -> dispatch RecalculateAll)
            .PaymentRows(forEach payments (paymentRow dispatch))
            .Elt()

    let optionsAll =
        Templates.OptionsAll_()
            .AddOption(fun _ -> dispatch AddOption)
            .RecalculateAll(fun _ -> dispatch RecalculateAll)
            .OptionRows(forEach options (optionRow dispatch))
            .Elt()

    Templates.Home()
        .SummaryPlaceholder(summary model dispatch)
        .TradesPlaceholder(trades)
        .OptionsPlaceholder(optionsAll)
        .MarketDataPlaceholder(marketDataDisplay model.marketData dispatch)
        .Elt()

let menuItem (model: Model) (router :Router<_,_,_>) (page: Page) (text: string) =
    let activeFlag = "rz-button rz-secondary"
    Main.MenuItem()
        .Active(if model.page = page then activeFlag else "")
        .Url(router.Link page)
        .Text(text)
        .Elt()

let view router model dispatch =
    Main()
        .Menu(concat {
            menuItem model router Home "Home"
            menuItem model router Config "Config"
        })
        .Body(
            cond model.page <| function
            | Home -> homePage model dispatch
            | Config -> configDisplay model.configuration dispatch
        )
        .Error(
            cond model.error <| function
            | None -> empty()
            | Some err ->
                Templates.ErrorNotification()
                    .Text(err)
                    .Hide(fun _ -> dispatch ClearError)
                    .Elt()
        )
        .Elt()
