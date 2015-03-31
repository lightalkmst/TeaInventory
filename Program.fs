open TeaInventory

open System
open System.Collections

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<EntryPoint>]
let main argv = 
    use g = new Game1()
    g.Run()
    0 // return an integer exit code