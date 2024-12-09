open System
open System.Drawing
open System.Windows.Forms
// Form UI for Quiz App
let form = new Form(Text = "Quiz Application", Size = Size(800, 600))
form.BackColor <- Color.DeepSkyBlue
form.StartPosition <- FormStartPosition.CenterScreen
let questionLabel = new Label(Text = "", Location = Point(10, 10), Size = Size(450, 50), ForeColor = Color.Brown, TextAlign = ContentAlignment.MiddleCenter)
questionLabel.BackColor <- Color.Beige
questionLabel.Location <- Point((form.ClientSize.Width - questionLabel.Width) / 2, (10))
let answerBox = new TextBox(Location = Point(160, 70), Size = Size(450, 50))
let optionsPanel = new Panel(Location = Point(160, 70), Size = Size(450, 120))
let timerLabel = new Label(Text = "Time: 10", Location = Point(160, 200), Size = Size(100, 30), ForeColor = Color.Red)
let submitButton = new Button(Text = "Submit", Location = Point(160, 230), Size = Size(150, 30))
submitButton.BackColor <- Color.SeaGreen
submitButton.ForeColor <- Color.WhiteSmoke
let resultLabel = new Label(Text = "", Location = Point(10, 270), Size = Size(450, 50),TextAlign = ContentAlignment.MiddleCenter)
resultLabel.Location <- Point((form.ClientSize.Width - resultLabel.Width) / 2, 
                              (form.ClientSize.Height - resultLabel.Height) / 2)
resultLabel.BackColor <- Color.Honeydew
resultLabel.Visible <- false
form.Controls.AddRange([| questionLabel; answerBox; optionsPanel; timerLabel; submitButton; resultLabel |])

Application.Run(form)

// 2- Questions using map
type Question =
    {
        Text: string // Question text
        Choices: string list option // List of choices (if any), otherwise None for open-ended questions
        CorrectAnswer: string // Correct answer for the question
    }

let quizQuestions : Map<int, Question> =
    Map.ofList [
        (1, { Text = "Which number comes next in the series? 2, 4, 8, 16, __"; 
              Choices = Some ["24"; "32"; "20"; "12"]; CorrectAnswer = "32" })
        (2, { Text = "What is the missing letter in this sequence? A, C, E, G, __"; 
              Choices = Some ["I"; "H"; "J"; "K"]; CorrectAnswer = "I" })
        (3, { Text = "Solve: 5 + 3"; 
              Choices = None; CorrectAnswer = "8" })
        (4, { Text = "What is the missing letter in this sequence? A, C, E, G, __"; 
              Choices = Some ["I"; "H"; "J"; "K"]; CorrectAnswer = "I" })
    ]

let mutable currentQuestionIndex = 1
let mutable score = 0
let timePerQuestion = 10
let mutable remainingTime = timePerQuestion

let timer = new Timer(Interval = 1000)

timer.Tick.Add(fun _ ->
    remainingTime <- remainingTime - 1
    timerLabel.Text <- sprintf "Time: %d" remainingTime

    if remainingTime <= 0 then
        timer.Stop()
        resultLabel.Text <- "Time's up!"
        currentQuestionIndex <- currentQuestionIndex + 1
        loadNextQuestion()
)