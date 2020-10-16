using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class CheatMenu : MonoBehaviour
{
    public static CheatMenu instance;
    
    private bool showConsole = false;
    private float previousTimeScale = 1f;

    private string input = "";

    [SerializeField] CheatCommand[] commands;

    private const int InputBoxHeight = 25;

    private void Awake()
    {
        instance = this;
    }
    
    public void ConfirmInput()
    {
        HandleInput(input);
        input = "";
    }

    private void OnGUI()
    {
        if (showConsole)
        {
            const string inputBox = "inputBox";
            
            Rect cheatMenuBox = new Rect(0,0, Screen.width, InputBoxHeight);
            GUI.Box(cheatMenuBox, "");
            
            GUI.SetNextControlName(inputBox);
            input = GUI.TextField(cheatMenuBox, input);
            GUI.FocusControl(inputBox);
            DisplayPreviewCommands();
        }
    }

    public void TurnOnOff()
    {
        showConsole = !showConsole;

        if (showConsole)
        {
            //freeze time
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;

            input = "";
        }
        else
        {
            //unfreeze
            Time.timeScale = previousTimeScale;
        }
    }

    private void HandleInput(string savedInput)
    {
        savedInput = savedInput.Trim().ToLower();
        foreach (CheatCommand command in commands)
        {
            if (savedInput.StartsWith(command.commandName.ToLower()))
            {
                string withoutCommand = savedInput.Substring(command.commandName.Length).Trim();
                float value;
                if (float.TryParse(withoutCommand, out value))
                {
                    command.Activate(value);
                    return;
                }
            }
        }
    }

    public void DisplayPreviewCommands()
    {
        if (input.Length > 0)
        {
            string fixedInput = input.Trim().ToLower();
            List<CheatCommand> matchingCommands = new List<CheatCommand>();
            StringBuilder commandsPreviewText = new StringBuilder();

            foreach (CheatCommand command in commands)
            {
                if (command.commandName.ToLower().StartsWith(fixedInput))
                {
                    matchingCommands.Add(command);
                    commandsPreviewText.Append(command.commandName);
                    commandsPreviewText.Append("\t");
                    commandsPreviewText.Append(command.description);
                    commandsPreviewText.Append("\n");
                }
            }
            if(matchingCommands.Count > 0)
            GUI.Box(new Rect(0f, InputBoxHeight, Screen.width, InputBoxHeight * matchingCommands.Count), commandsPreviewText.ToString());
        }
        else if(showConsole)
        {
            StringBuilder commandsPreviewText = new StringBuilder();
            foreach (CheatCommand command in commands)
            {
                    commandsPreviewText.Append(command.commandName);
                    commandsPreviewText.Append("\t");
                    commandsPreviewText.Append(command.description);
                    commandsPreviewText.Append("\n");
            }
            GUI.Box(new Rect(0f, InputBoxHeight, Screen.width, InputBoxHeight * commands.Length), commandsPreviewText.ToString());
        }
    }
}
