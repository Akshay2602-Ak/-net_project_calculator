using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

public class IndexModel : PageModel
{
    public string Display { get; private set; } = "0";

    public string? ErrorMessage { get; private set; }

    [BindProperty]
    public string? Button { get; set; }

    private const string DisplayKey = "Calculator.Display";
    private const string FirstNumberKey = "Calculator.FirstNumber";
    private const string OperatorKey = "Calculator.Operator";
    private const string NewNumberKey = "Calculator.NewNumber";

    public void OnGet()
    {
        Display = "0";
    }

    public void OnPost()
    {
        Display = HttpContext.Session.GetString(DisplayKey) ?? "0";
        var firstNumberText = HttpContext.Session.GetString(FirstNumberKey);
        var operation = HttpContext.Session.GetString(OperatorKey);

        if (Button == "C")
        {
            ClearCalculator();
            return;
        }

        if (Button == "=")
        {
            Calculate(firstNumberText, operation);
            return;
        }

       if (Button is "+" or "-" or "*" or "/")
{
    if (decimal.TryParse(Display, out var currentNumber))
    {
        HttpContext.Session.SetString(
            FirstNumberKey,
            currentNumber.ToString(CultureInfo.InvariantCulture));

        HttpContext.Session.SetString(
            OperatorKey,
            Button);

        HttpContext.Session.SetString(
            NewNumberKey,
            "true");
    }

    return;
}

        if (Button == ".")
        {
            if (!Display.Contains("."))
            {
                Display += ".";
                SaveDisplay();
            }

            return;
        }

        if (int.TryParse(Button, out _))
{
    var newNumber = HttpContext.Session.GetString(NewNumberKey);

    if (newNumber == "true")
    {
        Display = Button!;
        HttpContext.Session.Remove(NewNumberKey);
    }
    else if (Display == "0")
    {
        Display = Button!;
    }
    else
    {
        Display += Button;
    }

    SaveDisplay();
}
    }

    private void Calculate(string? firstNumberText, string? operation)
    {
        if (!decimal.TryParse(
                firstNumberText,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var firstNumber) ||
            !decimal.TryParse(
                Display,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var secondNumber))
        {
            ErrorMessage = "Invalid calculation.";
            return;
        }

        decimal result;

        switch (operation)
        {
            case "+":
                result = firstNumber + secondNumber;
                break;

            case "-":
                result = firstNumber - secondNumber;
                break;

            case "*":
                result = firstNumber * secondNumber;
                break;

            case "/":
                if (secondNumber == 0)
                {
                    ErrorMessage = "Cannot divide by zero.";
                    return;
                }

                result = firstNumber / secondNumber;
                break;

            default:
                ErrorMessage = "Please select an operator.";
                return;
        }

        Display = result.ToString(CultureInfo.InvariantCulture);

        HttpContext.Session.Remove(FirstNumberKey);
        HttpContext.Session.Remove(OperatorKey);

        SaveDisplay();
    }

    private void ClearCalculator()
    {
        Display = "0";

        HttpContext.Session.Remove(DisplayKey);
        HttpContext.Session.Remove(FirstNumberKey);
        HttpContext.Session.Remove(OperatorKey);
    }

    private void SaveDisplay()
    {
        HttpContext.Session.SetString(DisplayKey, Display);
    }
}