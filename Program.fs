﻿open System
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

// Final Result Display
let showFinalResults() =
    optionsPanel.Visible <- false
    answerBox.Visible <- false
    timerLabel.Visible <- false
    timer.Stop()
    resultLabel.Visible <- true
    if score >= (quizQuestions.Count / 2) then
        questionLabel.ForeColor <- Color.Green
        questionLabel.Text <- "Congratulations👏! You passed the quiz!"
    else
        questionLabel.ForeColor <- Color.Red
        questionLabel.Text <- "Sorry, you failed the quiz!"
    resultLabel.Text <- sprintf "Your score: %d/%d" score quizQuestions.Count
    submitButton.Visible <- false
    submitButton.Enabled <- false

// 3- Function to load the next question
let loadNextQuestion() =
    if quizQuestions.ContainsKey(currentQuestionIndex) then
        let question = quizQuestions.[currentQuestionIndex]
        questionLabel.Text <- question.Text
        optionsPanel.Controls.Clear()
        answerBox.Clear()
        resultLabel.Text <- ""
        remainingTime <- timePerQuestion
        timerLabel.Text <- sprintf "Time: %d" remainingTime
        timer.Start()

        match question.Choices with
        | Some choices ->
            answerBox.Visible <- false
            optionsPanel.Visible <- true
            choices
            |> List.iteri (fun i choice ->
                let optionButton = new RadioButton(Text = choice, Location = Point(10, i * 30), Size = Size(400, 30))
                optionsPanel.Controls.Add(optionButton))
        | None ->
            answerBox.Visible <- true
            optionsPanel.Visible <- false
    else
        showFinalResults()



timer.Tick.Add(fun _ ->
    remainingTime <- remainingTime - 1
    timerLabel.Text <- sprintf "Time: %d" remainingTime

    if remainingTime <= 0 then
        timer.Stop()
        resultLabel.Text <- "Time's up!"
        currentQuestionIndex <- currentQuestionIndex + 1
        loadNextQuestion()
)



// Submit button click event
submitButton.Click.Add(fun _ ->
    if quizQuestions.ContainsKey(currentQuestionIndex) then
        let question = quizQuestions.[currentQuestionIndex]
        let userAnswer =
            match question.Choices with
            | Some _ ->
                optionsPanel.Controls
                |> Seq.cast<RadioButton>
                |> Seq.tryFind (fun rb -> rb.Checked)
                |> Option.map (fun rb -> rb.Text)
            | None ->
                Some answerBox.Text

        match userAnswer with
        | None -> 
            ignore (MessageBox.Show("Please select or enter an answer before Submit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning))
        | Some answer when answer = question.CorrectAnswer ->
            score <- score + 1
            resultLabel.Text <- "Correct!"
            timer.Stop()
            currentQuestionIndex <- currentQuestionIndex + 1
            loadNextQuestion()
        | Some _ ->
            resultLabel.Text <- "Incorrect!"
            timer.Stop()
            currentQuestionIndex <- currentQuestionIndex + 1
            loadNextQuestion()
)



// Start the application
loadNextQuestion()
Application.Run(form)
