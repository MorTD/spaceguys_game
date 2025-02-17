using UnityEngine;
 using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour
{
	//float height = 150f;
	//static private string text = "Unity Console v1.4.567\n";
	//Vector2 scrollPosition = new Vector2(0, 0);

	//void OnGUI()
	//{
	//    scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width/2), GUILayout.Height(height/2));
	//    GUILayout.TextArea(text, GUILayout.MinHeight(height));
	//    GUILayout.EndScrollView();
	//}

	//static public void Add(string line)
	//{
	//    text = text + line + "\n";
	//}
	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	/// <summary>
	/// The hotkey to show and hide the console window.
	/// </summary>
	public KeyCode toggleKey = KeyCode.BackQuote;

	List<Log> logs = new List<Log>();
	Vector2 scrollPosition;
	bool show;
	bool collapse;

	// Visual elements:

    private static Console myRef;

    public static Console getCurrentConsole()
    {
        return myRef;
    }

    public void Awake()
    {
        myRef = this;
    }

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	Rect windowRect = new Rect(0, Screen.height - Screen.height/4, Screen.width / 4, Screen.height / 4);
	Rect titleBarRect = new Rect(0, 0, 10000, 20);
	GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			show = !show;
		}
	}

	void OnGUI()
	{
		if (!show)
		{
			return;
		}

		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	void ConsoleWindow(int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		// Iterate through the recorded logs.
		for (int i = 0; i < logs.Count; i++)
		{
			var log = logs[i];

			// Combine identical messages if collapse option is chosen.
			if (collapse)
			{
				var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

				if (messageSameAsPrevious)
				{
					continue;
				}
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(log.message);
		}

		GUILayout.EndScrollView();

		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();

		if (GUILayout.Button(clearLabel))
		{
			logs.Clear();
		}

		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

		GUILayout.EndHorizontal();

		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(titleBarRect);
	}

	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	void HandleLog(string message, string stackTrace, LogType type)
	{
		logs.Add(new Log()
		{
			message = message,
			stackTrace = stackTrace,
			type = type,
		});
	}

    public void ShowWindow()
    {
        if (!show)
        {
            show = !show;
        }
    }
}