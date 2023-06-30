module MarketOption
open System
open Configuration
open Money
open FSharp.Stats.Distributions


type OptionRecord =
    {
        OptionName          : string
        OptionType          : string
        AssetPrice          : int64 
        Strike              : int64 
        InterestRate        : float
        Volatility          : float
        Currency            : string
        Maturity            : DateTime
        AnalyticalValue     : Money option
        NumericalValue      : Money option
        Delta               : float option
    }

    static member sysRandom = Random()

    static member Random() = 
        {
            OptionName = sprintf "Option%04d" (OptionRecord.sysRandom.Next(9999))
            OptionType = [|"call"; "put"|][OptionRecord.sysRandom.Next(2)]
            
            AssetPrice = int64 (OptionRecord.sysRandom.Next())
            Strike = int64 (OptionRecord.sysRandom.Next())
            InterestRate = 0.05 
            Volatility = 0.1
            Currency = "USD"
            Maturity = (DateTime.Now.AddMonths (OptionRecord.sysRandom.Next(1, 6))).Date
            
            AnalyticalValue = None
            NumericalValue = None
            Delta = None
        }

type OptionValuationInputs = 
    {
        Options : OptionRecord
        Data : Configuration
        MarketData: MarketData
    }

type Pricer() = 
    let normal = ContinuousDistribution.normal 0.0 1.0

    member this.EuropeanCallOptionPrice S0 K r T vol =
        let d1 = (log(S0 / K) + (r + vol*vol / 2.0) * T) / (vol * sqrt T)
        S0 * normal.CDF(d1) - K * exp(-r * T) * normal.CDF(d1 - vol * sqrt T)

    member this.EuropeanPutOptionPrice S0 K r T vol = 
        let d1 = (log(S0 / K) + (r + vol*vol / 2.0) * T) / (vol * sqrt T)
        K * exp(-r * T) * normal.CDF(vol * sqrt T - d1) - S0 * normal.CDF(-d1)

    member this.EuropeanCallOptionPriceMC MC S0 K r T vol =
        [|1 .. MC|] 
        |> Array.map (fun _ -> S0 * exp((r - vol*vol * 0.5) * T  + vol * sqrt (T) * normal.Sample()) - K |> max 0.0) 
        |> Array.average |> fun x -> x * exp(-r*T)

    member this.EuropeanPutOptionPriceMC MC S0 K r T vol = 
        [|1 .. MC|] 
        |> Array.map (fun _ -> (K - S0 * exp((r - vol*vol * 0.5) * T  + vol * sqrt (T) * normal.Sample())) |> max 0.0) 
        |> Array.average |> fun x -> x * exp(-r*T)

    member this.callDelta S0 K r T vol = 
        (log(S0 / K) + (r + vol*vol * 0.5) * T) / (vol * sqrt T) 
        |> fun x -> normal.CDF(x)   

    member this.putDelta S0 K r T vol = 
        (log(S0 / K) + (r + vol*vol * 0.5) * T) / (vol * sqrt T) 
        |> fun x -> normal.CDF(x) - 1.0   


type OptionValuationModel(inputs: OptionValuationInputs) =
    let pricer = Pricer()

    member this.ConvertValues() = 
        let S0 = float inputs.Options.AssetPrice
        let K = float inputs.Options.Strike

        let tradeCcy = inputs.Options.Currency

        let targetCcy = match inputs.MarketData.TryFind "valuation::baseCurrency" with
                         | Some ccy -> ccy
                         | None -> tradeCcy

        let fxRateKey = sprintf "FX::%s%s" targetCcy tradeCcy
        

        let fxRate = if inputs.Data.ContainsKey fxRateKey then float inputs.Data.[ fxRateKey ] else 1.0 // lookup FX rate
        let finalCcy = if inputs.Data.ContainsKey fxRateKey then targetCcy else tradeCcy
        
        (S0 / fxRate, K / fxRate, finalCcy)
           

    member this.CalculateAnalytically() =
        let (S0, K, currency) =  this.ConvertValues()
        let r = inputs.Options.InterestRate
        let vol = inputs.Options.Volatility
        let T = (float (inputs.Options.Maturity - DateTime.Now).Days) / 365.0 
        let position_ = inputs.Options.OptionType |> String.map Char.ToLower

        let newValue = match position_ with
                                | "call" -> pricer.EuropeanCallOptionPrice S0 K r T vol
                                | _ -> pricer.EuropeanPutOptionPrice S0 K r T vol
 
        { Value = newValue; Currency = currency}

    member this.CalculateNumerically() = 
        let (S0, K, currency) =  this.ConvertValues()
        let r = inputs.Options.InterestRate
        let vol = inputs.Options.Volatility
        let T = (float (inputs.Options.Maturity - DateTime.Now).Days) / 365.0 
        let option = inputs.Options.OptionType |> String.map Char.ToLower
        
        let MonteCarloRuns = match inputs.MarketData.TryFind "monteCarlo::runs" with 
                                | Some MC -> int MC
                                | None -> 1000

        let newValue = match option with
                                | "call" -> pricer.EuropeanCallOptionPriceMC MonteCarloRuns S0 K r T vol
                                | _ -> pricer.EuropeanPutOptionPriceMC MonteCarloRuns S0 K r T vol
 
        { Value = newValue; Currency = currency}
            
    member this.CalculateDelta() = 
        let (S0, K, _) =  this.ConvertValues()
        let r = inputs.Options.InterestRate
        let vol = inputs.Options.Volatility
        let T = (float (inputs.Options.Maturity - DateTime.Now).Days) / 365.0 
        let option = inputs.Options.OptionType |> String.map Char.ToLower
        match option with
            | "call" -> (pricer.callDelta S0 K r T vol, 3) |> Math.Round 
            | _ -> (pricer.putDelta S0 K r T vol, 3) |> Math.Round
 



