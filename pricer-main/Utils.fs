module Utils
open System

let ofBool = function
  | true,a -> Some a
  | false,_ -> None
