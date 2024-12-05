open System
open System.Windows.Forms
open System.Drawing

[<EntryPoint>]
let main _ =
    // Create a new form
    let form = new Form(Text = "Hello World Form", Width = 300, Height = 200)

    // Create a label and add it to the form
    let label = new Label(Text = "Hello, World! From Quiz App", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter)
    form.Controls.Add(label)

    // Run the application
    Application.Run(form)
    0
