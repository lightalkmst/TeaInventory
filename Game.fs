#nowarn "62"
#light "off"

namespace TeaInventory

open System
open System.Collections
open System.Collections.Generic
open System.IO

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1 () as x = class
    inherit Microsoft.Xna.Framework.Game()
    
    do x.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch: Microsoft.Xna.Framework.Graphics.SpriteBatch = null
    let mutable font = null

    let mutable teas: (string * (string * int * bool) array) array = [||]

    let mutable kind_scroll = 0
    let mutable single_scroll = 0
    let max_display = 10

    let mutable active_kind = ""
    let mutable active_single = ""
    let mutable randomed = false

    let type_delimiter = '|'
    let num_delimiter = ':'

    let mutable keys = [||]
    let mutable mouse = [| false; false |]
    let mutable input_string = ""
    let mutable solid_texture = null

    (* button drawing data *)
    let font_size = 14
    let active_font_color = Color.Green
    let default_font_color = Color.Black
    let inner_size = 14
    let active_inner_color = Color.LightBlue
    let default_inner_color = Color.White
    let outer_size = 4
    let outer_color = Color.Black

    (* putting the side effects inside the input state management is not something I would do in a multiple state application *)
    (* however I am doing it here because there is only one general state for this application *)
    let keyboard_update () =
        (* modify the input string directly since there's only one state *)
        (* i wouldn't do this in a proper application *)
        (* instead i would simply process the keyboard state into an intermediate keystate collection that is easier to consume 
            and pass it off to where it's needed
            *)
        let keys2 = Keyboard.GetState().GetPressedKeys() in
        let keys3 = Array.filter (fun h -> Array.forall (( <> ) h) keys) keys2 in
        keys3 |> Array.iter (fun h -> 
            (* prevent output being consumed as input *)
            if randomed then (
                randomed <- false;
                input_string <- ""
            );
            match h with
            | Keys.A -> input_string <- input_string + "A"
            | Keys.B -> input_string <- input_string + "B"
            | Keys.C -> input_string <- input_string + "C"
            | Keys.D -> input_string <- input_string + "D"
            | Keys.E -> input_string <- input_string + "E"
            | Keys.F -> input_string <- input_string + "F"
            | Keys.G -> input_string <- input_string + "G"
            | Keys.H -> input_string <- input_string + "H"
            | Keys.I -> input_string <- input_string + "I"
            | Keys.J -> input_string <- input_string + "J"
            | Keys.K -> input_string <- input_string + "K"
            | Keys.L -> input_string <- input_string + "L"
            | Keys.M -> input_string <- input_string + "M"
            | Keys.N -> input_string <- input_string + "N"
            | Keys.O -> input_string <- input_string + "O"
            | Keys.P -> input_string <- input_string + "P"
            | Keys.Q -> input_string <- input_string + "Q"
            | Keys.R -> input_string <- input_string + "R"
            | Keys.S -> input_string <- input_string + "S"
            | Keys.T -> input_string <- input_string + "T"
            | Keys.U -> input_string <- input_string + "U"
            | Keys.V -> input_string <- input_string + "V"
            | Keys.W -> input_string <- input_string + "W"
            | Keys.X -> input_string <- input_string + "X"
            | Keys.Y -> input_string <- input_string + "Y"
            | Keys.Z -> input_string <- input_string + "Z"
            | Keys.Space -> input_string <- input_string + " "
            | Keys.Back -> if input_string.Length > 0 then input_string <- input_string.Substring(0, input_string.Length - 1)
            | _ -> ()
        );
        input_string <- input_string.ToUpper();
        (* prevent name from being too long and colliding with other buttons and the side of the screen *)
        input_string <- input_string.Substring(0, Math.Min(input_string.Length, 38));
        keys <- keys2

    let mouse_update () =
        (* see keyboard_update for my notes on consuming input directly in the input function *)
        let mouse2 = Mouse.GetState() in
        let x = mouse2.X in
        let y = mouse2.Y in
        if mouse2.LeftButton = ButtonState.Pressed && not mouse.[0] then (
            let buttons: (Rectangle * (unit -> unit)) list = [
                (* default buttons *)
                (new Rectangle(50, 50, 246, 50), fun () -> 
                    (* select all teas *)
                    teas <- teas |> Array.map (fun (kind, h2) -> 
                        kind, (h2 |> Array.map (fun (name, number, _) ->
                                name, number, true
                            )
                        )
                    )
                );
                (new Rectangle(50, 100, 274, 50), fun () -> 
                    (* deselect all teas *)
                    teas <- teas |> Array.map (fun (kind, h2) -> 
                        kind, (h2 |> Array.map (fun (name, number, _) ->
                                name, number, false
                            )
                        )
                    )
                );
                (new Rectangle(50, 150, 148, 50), fun () -> 
                    (* add type *)
                    if randomed then (
                        randomed <- false;
                        input_string <- ""
                    )
                    elif input_string <> "" && (teas |> Array.forall (fun (kind, _) -> input_string <> kind)) then (
                        let _ = teas <- Array.append teas [| (input_string, [||]) |] in
                        let _ = teas <- Array.sortWith (fun (k1, _) (k2, _) -> k1.CompareTo(k2)) teas in
                        active_kind <- input_string
                    );
                    input_string <- ""
                );
                (new Rectangle(50, 200, 428, 50), fun () -> 
                    (* delete type *)
                    if active_kind <> "" then
                        teas <- teas |> Array.filter (fun (kind, _) -> kind <> active_kind);
                        active_kind <- "";
                        active_single <- ""
                );
                (new Rectangle(725, 50, 246, 50), fun () -> 
                    (* select all teas of type *)
                    if active_kind <> "" then
                        let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                        let kind, arr = teas.[i] in
                        teas.[i] <- kind, arr |> Array.map (fun (name, count, _) -> name, count, true)
                );
                (new Rectangle(725, 100, 274, 50), fun () -> 
                    (* deselect all teas of type *)
                    if active_kind <> "" then
                        let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                        let kind, arr = teas.[i] in
                        teas.[i] <- kind, arr |> Array.map (fun (name, count, _) -> name, count, false)
                );
                (new Rectangle(725, 150, 134, 50), fun () -> 
                    (* add tea *)
                    if randomed then (
                        randomed <- false;
                        input_string <- ""
                    )
                    elif teas.GetLength(0) > 0 then
                        let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                        let kind, arr = teas.[i] in
                        if input_string <> "" && (arr |> Array.forall (fun (name, _, _) -> input_string <> name)) then (
                            let _ = teas.[i] <- (kind, Array.append arr [| (input_string, 0, false) |]) in
                            active_single <- input_string
                        );
                        input_string <- ""
                );
                (new Rectangle(725, 200, 456, 50), fun () -> 
                    (* delete tea *)
                    if active_single <> "" then
                        let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                        let kind, arr = teas.[i] in
                        teas.[i] <- kind, arr |> Array.filter (fun (name, _, _) -> name <> active_single);
                        active_single <- ""
                );
                (new Rectangle(725, 250, 246, 50), fun () -> 
                    (* pick random tea *)
                    if teas.GetLength(0) > 0 then
                        let arr = 
                            teas |> Array.collect (fun (kind, arr) -> 
                                let arr = Array.filter (fun (_, _, check) -> check) arr in
                                Array.map (fun h -> (kind, h)) arr
                            )
                        in
                        if arr.GetLength(0) > 0 then
                            let kind, (name, count, _) = arr.[DateTime.Now.Millisecond % arr.GetLength(0)] in
                            input_string <- name + "  " + count.ToString();
                            active_kind <- kind;
                            active_single <- name;
                            (* pan to appropriate kind and single *)
                            let kind_index = Array.findIndex (fun (kind2, _) -> kind2 = kind) teas in
                            begin if kind_index < kind_scroll then
                                kind_scroll <- kind_index
                            elif kind_index > kind_scroll + max_display then
                                kind_scroll <- kind_index - max_display + 1
                            end;
                            let _, arr = teas.[kind_index] in
                            let single_index = Array.findIndex (fun (name2, _, _) -> name2 = name) arr in
                            begin if single_index < single_scroll then
                                single_scroll <- single_index
                            elif single_index > single_scroll + max_display then
                                single_scroll <- single_index - max_display + 1
                            end;
                            randomed <- true
                );
                (new Rectangle(1300, 150, 50, 50), fun () ->
                    (* right + *)
                    let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                    let kind, arr = teas.[i] in
                    teas.[i] <- kind, arr |> Array.map (fun (name, count, check) -> name, (if name = active_single then count + 1 else count), check);
                    input_string <- ""
                );
                (new Rectangle(1300, 200, 50, 50), fun () ->
                    (* right - *)
                    let i = Array.findIndex (fun (kind, _) -> kind = active_kind) teas in
                    let kind, arr = teas.[i] in
                    teas.[i] <- kind, arr |> Array.map (fun (name, count, check) -> name, (if name = active_single then Math.Max(count - 1, 0) else count), check);
                    input_string <- ""
                );
                (new Rectangle(611, 50, 64, 50), fun () ->
                    (* left up *)
                    kind_scroll <- Math.Max(kind_scroll - 1, 0)
                );
                (new Rectangle(583, 100, 92, 50), fun () ->
                    (* left down *)
                    kind_scroll <- Math.Min(kind_scroll + 1, teas.GetLength(0) - max_display)
                );
                (new Rectangle(1286, 50, 64, 50), fun () ->
                    (* left up *)
                    single_scroll <- Math.Max(single_scroll - 1, 0)
                );
                (new Rectangle(1258, 100, 92, 50), fun () ->
                    (* left down *)
                    if active_kind <> "" then
                        let _, arr = Array.find (fun (kind, _) -> kind = active_kind) teas in
                        single_scroll <- Math.Min(single_scroll + 1, arr.GetLength(0) - max_display)
                );
            ] in
            let _, buttons = 
                (* buttons for types *)
                teas |> ((0, buttons) |> Array.fold (fun (i, a) (kind, arr) ->
                    if i >= kind_scroll && i < kind_scroll + max_display then
                        let text_width = kind.Length * 14 + 36 in
                        i + 1, (new Rectangle(50, (i - kind_scroll) * 50 + 350, text_width, 50), fun () -> 
                            active_kind <- kind;
                            input_string <- ""
                        ) :: a
                    else
                        i + 1, a
                ))
            in
            let _, buttons = 
                (* add buttons for individual teas *)
                if active_kind <> "" then
                    let _, arr = Array.find (fun (kind, _) -> kind = active_kind) teas in
                    arr |> ((0, buttons) |> Array.fold (fun (i, a) (name, count, check) ->
                        if i >= single_scroll && i < single_scroll + max_display then
                            let text_width = name.Length * 14 + 35 + (int (Math.Log10(float count))) * 14 + 14 in
                            i + 1, (new Rectangle(725, (i - single_scroll) * 50 + 350, text_width, 50), fun () ->
                                arr.[i] <- name, count, not check;
                                active_single <- name;
                                input_string <- ""
                            ) :: a
                        else
                            i + 1, a
                    ))
                else
                    0, buttons
            in
            buttons |> List.iter (fun (rect, f) -> if x >= rect.X && x < rect.X + rect.Width && y >= rect.Y && y < rect.Y + rect.Height then f ())
        );
        mouse.[0] <- mouse2.LeftButton = ButtonState.Pressed;
        mouse.[1] <- mouse2.RightButton = ButtonState.Pressed

    override x.Initialize() =
        base.Initialize();
        x.Window.Title <- "Inventory";
        spriteBatch <- new SpriteBatch(graphics.GraphicsDevice);
        (* blank white solid texture that i overlay with other colors so i don't need to construct new textures for each color *)
        (* meaning i create exactly 1 texture 1 time to handle all of the colored panels in the program *)
        solid_texture <- new Texture2D(graphics.GraphicsDevice, 1, 1);
        solid_texture.SetData([| Color.White |]);
        (* vertical sync *)
        graphics.SynchronizeWithVerticalRetrace <- true;
        (* frame rate *)
        x.TargetElapsedTime <- TimeSpan.FromSeconds(1.0 / 30.0);
        (* resolution *)
        graphics.PreferredBackBufferWidth <- 1400;
        graphics.PreferredBackBufferHeight <- 900;
        ()
    
    override x.LoadContent() =
        (* load sprite font *)
        font <- Array.init 36 (fun _ -> new Texture2D(graphics.GraphicsDevice, 8, 8));
        let image = 
            let image = System.Drawing.Image.FromFile("font.png") in
            new System.Drawing.Bitmap(image)
        in
        [ 0 .. 35] |> List.iter (fun i ->
            let arr =
                Array.init 64 (fun i2 ->
                    let color = image.GetPixel(i * 8 + i2 % 8, i2 / 8) in
                    let a = color.A in
                    let r = color.R in
                    let b = color.B in
                    let g = color.G in
                    new Microsoft.Xna.Framework.Color((int) r, (int) b, (int) g, (int) a)
                )
            in
            font.[i].SetData(arr)
        );
        (* load inventory data *)
        if File.Exists("teas.txt")  then
            let stream = new StreamReader("teas.txt") in
            while not stream.EndOfStream do
                let line = stream.ReadLine() in
                if line.IndexOf(type_delimiter) > -1 then
                    let kind = line.Substring(0, line.IndexOf(type_delimiter)) in
                    let line = line.Substring(kind.Length + 1) in
                    let teas2 = line.Split(type_delimiter) in
                    let arr = Array.map (fun (h: string) ->
                        let pair = h.Split(num_delimiter) in
                        pair.[0], Int32.Parse(pair.[1]), false
                    ) teas2 in
                    teas <- Array.append teas [| (kind, arr) |]
                else
                    teas <- Array.append teas [| (line, [||]) |]
            done

    override x.Update (gameTime) =
        keyboard_update ();
        mouse_update ();
        ()
    
    override x.Draw (gameTime) =
        x.GraphicsDevice.Clear Color.LightBlue;
        let draw_text (s: string) x y size color =
            let draw_char i h =
                if h <> ' ' then
                    if h >= 'A' && h <= 'Z' then
                        spriteBatch.Draw(font.[(int) h % 65], new Rectangle(x + i * size, y, size, size), color)
                    else
                        spriteBatch.Draw(font.[(int) h % 48 + 26], new Rectangle(x + i * size, y, size, size), color)
            in
            Seq.iteri draw_char (s.ToUpper())
        in
        spriteBatch.Begin();
        let buttons = 
            (* text, x, y, font_color, inner_color *)
            let buttons = [
                (* default buttons *)
                ("Select All Teas", 50, 50, Color.Black, Color.White);
                ("Deselect All Teas", 50, 100, Color.Black, Color.White);
                ("Select All Teas Of This Kind", 725, 50, Color.Black, Color.White);
                ("Deselect All Teas Of This Kind", 725, 100, Color.Black, Color.White);
                ("Add Type", 50, 150, Color.Black, Color.White);
                ("Delete Type", 50, 200, Color.Black, Color.White);
                ("Add Tea", 725, 150, Color.Black, Color.White);
                ("Delete Tea", 725, 200, Color.Black, Color.White);
                ((if input_string = "" then "Input And Output" else input_string), 50, 250, Color.Black, Color.White);
                ("Pick Random Tea", 725, 250, Color.Black, Color.White);
                ("Up", 611, 50, Color.Black, Color.White);
                ("Down", 583, 100, Color.Black, Color.White);
                ("Up", 1286, 50, Color.Black, Color.White);
                ("Down", 1258, 100, Color.Black, Color.White);
                (* + button *)
                (" ", 1300, 150, Color.Black, Color.White);
                (* - button *)
                (" ", 1300, 200, Color.Black, Color.White);
            ] in
            let _, buttons =
                (* tea types *)
                teas |> ((0, buttons) |> Array.fold (fun (i, a) (kind, teas2) ->
                    if i >= kind_scroll && i < kind_scroll + max_display then
                        let check = teas2 |> Array.exists (fun (_, _, x) -> x) in
                        let color1 = if check then active_font_color else default_font_color in
                        let color2 = if kind = active_kind then active_inner_color else default_inner_color in
                        i + 1, (kind, 50, (i - kind_scroll) * 50 + 350, color1, color2) :: a
                    else
                        i + 1, a
                ))
            in
            let _, buttons =
                (* individual teas *)
                if active_kind <> "" then
                    let _, arr = Array.find (fun (kind, _) -> kind = active_kind) teas in
                    arr |> ((0, buttons) |> Array.fold (fun (i, a) (name, count, check) ->
                        if i >= single_scroll && i < single_scroll + max_display then
                            let color1 = if check then active_font_color else default_font_color in
                            let color2 = if name = active_single then active_inner_color else default_inner_color in
                            i + 1, (name + "  " + count.ToString(), 725, (i - single_scroll) * 50 + 350, color1, color2) :: a
                        else
                            i + 1, a
                    ))
                else
                    0, buttons
            in
            buttons
        in
        buttons |> List.iter (fun (text, x, y, font_color, inner_color) ->
            let text_width = text.Length * font_size in
            spriteBatch.Draw(solid_texture, new Rectangle(x, y, text_width + inner_size * 2 + outer_size * 2, font_size + inner_size * 2 + outer_size * 2), outer_color);
            spriteBatch.Draw(solid_texture, new Rectangle(x + outer_size, y + outer_size, text_width + inner_size * 2, font_size + inner_size * 2), inner_color);
            draw_text text (x + inner_size + outer_size) (y + inner_size + outer_size) font_size font_color;
            ()
        );

        (* finish drawing in the + and - buttons. i don't feel like making sprites for them *)
        (* 1300, 50 *)
        (* 1318, 68 *)
        (* right + *)
        spriteBatch.Draw(solid_texture, new Rectangle(1322, 168, 6, 14), Color.Black);
        spriteBatch.Draw(solid_texture, new Rectangle(1318, 172, 14, 6), Color.Black);
        (* right - *)
        spriteBatch.Draw(solid_texture, new Rectangle(1318, 222, 14, 6), Color.Black);

        (* draw mouse last *)
        (* change to mouse texture *)
        let mouse_state = Mouse.GetState() in
        (* too lazy to make mouse sprite *)
        spriteBatch.Draw(solid_texture, new Rectangle(mouse_state.X, mouse_state.Y, 5, 20), Color.Red);
        spriteBatch.Draw(solid_texture, new Rectangle(mouse_state.X, mouse_state.Y, 20, 6), Color.Red);
        spriteBatch.End()

    override x.OnExiting(sender: obj, args: EventArgs) : unit =
        (* save data on exit *)
        let stream = new StreamWriter("teas.txt") in
        teas |> Array.iter (fun (kind, arr) ->
            stream.Write(kind);
            arr |> Array.iter (fun (name, count, _) ->
                stream.Write(type_delimiter);
                stream.Write(name);
                stream.Write(num_delimiter);
                stream.Write(count.ToString())
            );
            stream.Write('\n');
        );
        stream.Flush();
        stream.Close()
end