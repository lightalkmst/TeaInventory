open TeaInventory

open System
open System.Collections

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<EntryPoint>]
let main argv = 
    try
        use g = new Game1()
        g.Run()
    with
    | e -> 
        let stream = new System.IO.StreamWriter("log.txt") in
        stream.WriteLine(e.ToString());
        stream.Flush();
        stream.Close()
    0 // return an integer exit code