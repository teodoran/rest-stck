namespace CoreStck

open System
open System.IO
open System.Text.RegularExpressions

module Interpreter =

    let standardLibrary = [
        ("2dup", ["over"; "over"]);
        ("rem", ["dup"; "rot"; "swap"; "2dup"; "divi"; "mult"; "sub"; "1000000"; "mult"; "swap"; "divi"]);
        ("div", ["2dup"; "divi"; "rot"; "rot"; "rem"]);
        ("empty", ["len"; "0"; "eq"]);
        ("clear", ["empty"; "if"; "else"; "drop"; "clear"; "end"]);
        ("max", ["len"; "1"; "eq"; "not"; "if"; "2dup"; "gt"; "if"; "swap"; "drop"; "else"; "drop"; "end"; "max"; "else"; "end"]);
        ("min", ["len"; "1"; "eq"; "not"; "if"; "2dup"; "lt"; "if"; "swap"; "drop"; "else"; "drop"; "end"; "min"; "else"; "end"])
    ]

    let mutable error = null

    let printerr op = error <- sprintf "Cannot %s on the stack" op

    let push e stack =
        e :: stack

    let drop stack =
        match stack with
        | tos :: rest -> rest
        | _ ->
            printerr "drop"
            stack

    let swap stack =
        match stack with
        | a :: b :: rest -> b :: a :: rest
        | _ ->
            printerr "swap"
            stack

    let dup stack =
        match stack with
        | tos :: rest -> tos :: tos :: rest
        | _ ->
            printerr "dup"
            stack

    let over stack =
        match stack with
        | a :: b :: rest -> b :: a :: b :: rest
        | _ ->
            printerr "over"
            stack

    let rot stack =
        match stack with
        | a :: b :: c :: rest -> c :: a :: b :: rest
        | _ ->
            printerr "rot"
            stack

    let len (stack:List<int>) =
        push stack.Length stack

    let isInt string = true

    let math op stack =
        match stack with
        | a :: b :: rest -> push (op a b) rest
        | _ ->
            printerr "do math"
            stack

    let add stack = math (fun a b -> b + a) stack

    let substract stack = math (fun a b -> b - a) stack

    let multiply stack = math (fun a b -> b * a) stack

    let divide stack = math (fun a b -> b / a) stack

    let modulo stack = math (fun a b -> b % a) stack

    let asInt b = if b then 1 else 0

    let equal stack = math (fun a b -> asInt(a = b)) stack

    let greater stack = math (fun a b -> asInt(a > b)) stack

    let less stack = math (fun a b -> asInt(a < b)) stack

    let not stack =
        match stack with
        | tos :: rest -> 
            if (tos <> 0) then
                0 :: rest
            else
                1 :: rest
        | _ ->
            printerr "not"
            stack

    let exec exp stack =
        match exp with
        | "drop" -> drop stack
        | "swap" -> swap stack
        | "dup" -> dup stack
        | "over" -> over stack
        | "rot" -> rot stack
        | "len" -> len stack
        | "add" -> add stack
        | "sub" -> substract stack
        | "mult" -> multiply stack
        | "divi" -> divide stack
        | "mod" -> modulo stack
        | "eq" -> equal stack
        | "gt" -> greater stack
        | "lt" -> less stack
        | "not" -> not stack
        | _ ->
            if isInt exp then
                push (int exp) stack
            else
                printerr exp
                stack

    let define s heap =
        s :: heap

    let rec find s heap =
        match heap with
        | [] -> []
        | head :: tail ->
            if fst head = s then
                snd head
            else 
                find s tail

    let tokens (s:string) =
        s.Split([|'_'|]) |> Array.toList

    let rec split delim n col exps =
        match exps with
        | [] -> (col |> List.rev, [])
        | head :: tail ->
            match head with
            | "if" -> split delim (n + 1) (head :: col) tail
            | d when d = delim ->
                match n with
                | 0 -> (col |> List.rev, tail)
                | _ -> split delim (n - 1) (head :: col) tail
            | _ -> split delim n (head :: col) tail
                
    let cond tos exps =
        let t = split "else" 0 [] exps
        let f = split "end" 0 [] (snd t)

        match tos <> 0 with
        | true -> (fst t) @ (snd f)
        | false -> (fst f) @ (snd f)

    let rec eval exps hs =
        let heap = fst hs
        let stack = snd hs

        match exps with
        | [] -> (heap, stack)
        | head :: tail ->
            match head with
            | "ignore" -> (heap, stack)
            | "def" ->
                match tail with
                | [] -> (heap, stack)
                | name :: definition -> (define (name, definition) heap, stack)
            | "if" ->
                match stack with
                | [] -> (heap, stack)
                | tos :: rest -> eval (cond tos tail) (heap, rest)
            | _ ->
                match find head heap with
                | [] -> eval tail (heap, exec head stack)
                | def -> eval (def @ tail) hs
    
    let evaluate hs line =
        let exps = tokens line

        hs
        |> eval exps